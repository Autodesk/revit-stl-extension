//
// STL exporter library: this library works with Autodesk(R) Revit(R) to export an STL file containing model geometry.
// Copyright (C) 2013  Autodesk, Inc.
// 
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Autodesk.Revit;
using Category = Autodesk.Revit.DB.Category;
using Autodesk.Revit.DB;

namespace BIM.STLExport
{
    public partial class STLExportForm : System.Windows.Forms.Form
    {
        private DataGenerator m_Generator = null;
        private SortedDictionary<string, Category> m_CategoryList = new SortedDictionary<string, Category>();
        private SortedDictionary<string, DisplayUnitType> m_DisplayUnits = new SortedDictionary<string, DisplayUnitType>();
        private static DisplayUnitType m_SelectedDUT = DisplayUnitType.DUT_UNDEFINED;

        readonly Autodesk.Revit.UI.UIApplication m_Revit = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="revit">The revit application.</param>
        public STLExportForm(Autodesk.Revit.UI.UIApplication revit)
        {
            InitializeComponent();
            m_Revit = revit;

            // get new data generator
            m_Generator = new DataGenerator(m_Revit.Application, m_Revit.ActiveUIDocument.Document, m_Revit.ActiveUIDocument.Document.ActiveView);

            // scan for categories to populate category list
            m_CategoryList = m_Generator.ScanCategories(true);

            foreach (KeyValuePair<string, Category> kvp in m_CategoryList)
            {
                lbCategories.Items.Add(kvp.Key.ToString(), true);
            }

            string unitName = "Use Internal: Feet";
            m_DisplayUnits.Add(unitName, DisplayUnitType.DUT_UNDEFINED);
            int selectedIndex = comboBox_DUT.Items.Add(unitName);
            if (m_SelectedDUT == DisplayUnitType.DUT_UNDEFINED)
               comboBox_DUT.SelectedIndex = selectedIndex;

            Units currentUnits = m_Revit.ActiveUIDocument.Document.GetUnits();
            DisplayUnitType currentDut = currentUnits.GetFormatOptions(UnitType.UT_Length).DisplayUnits;
            unitName = "Use Current: " + LabelUtils.GetLabelFor(currentDut);
            m_DisplayUnits.Add(unitName, currentDut);
            selectedIndex = comboBox_DUT.Items.Add(unitName);
            if (m_SelectedDUT == currentDut)
               comboBox_DUT.SelectedIndex = selectedIndex;

            foreach (DisplayUnitType dut in UnitUtils.GetValidDisplayUnits(UnitType.UT_Length))
            {
               if (currentDut == dut)
                  continue;
               unitName = LabelUtils.GetLabelFor(dut);
               m_DisplayUnits.Add(unitName, dut);
               selectedIndex = comboBox_DUT.Items.Add(unitName);
               if (m_SelectedDUT == dut)
                  comboBox_DUT.SelectedIndex = selectedIndex;
            }

            // initialize the UI differently for Families
            if (revit.ActiveUIDocument.Document.IsFamilyDocument)
            {
                cbIncludeLinked.Enabled = false;
                lbCategories.Enabled = false;
                btnCheckAll.Enabled = false;
                btnCheckNone.Enabled = false;
            }
        }

        /// <summary>
        /// Help button click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void btnHelp_Click(object sender, EventArgs e)
        {
            string helpfile = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            helpfile = System.IO.Path.Combine(helpfile, "STL_Export.chm");

            if (System.IO.File.Exists(helpfile) == false)
            {
                MessageBox.Show("Help File " + helpfile + " NOT FOUND!");
                return;
            }

            Help.ShowHelp(this, "file://" + helpfile);

        }

        /// <summary>
        /// Save button click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            string fileName = STLDialogManager.SaveDialog();

            if (!string.IsNullOrEmpty(fileName))
            {
                SaveFormat saveFormat;
                if (rbBinary.Checked)
                {
                    saveFormat = SaveFormat.Binary;
                }
                else
                {
                    saveFormat = SaveFormat.ASCII;
                }

                ElementsExportRange exportRange;

                exportRange = ElementsExportRange.OnlyVisibleOnes;

                // get selected categories from the category list
                Dictionary<string, Category> selectedCategories = new Dictionary<string, Category>();

                // only for projects
                if (m_Revit.ActiveUIDocument.Document.IsFamilyDocument == false)
                {
                    foreach (Object obj in lbCategories.CheckedItems)
                    {
                        Category selectedCat = null;
                        m_CategoryList.TryGetValue(obj.ToString(), out selectedCat);

                        selectedCategories.Add(obj.ToString(), selectedCat);
                    }
                }

                DisplayUnitType dup = m_DisplayUnits[comboBox_DUT.Text];
                m_SelectedDUT = dup;

                // create settings object to save setting information
                Settings aSetting = new Settings(saveFormat, exportRange, cbIncludeLinked.Checked, selectedCategories, dup);

                // save Revit document's triangular data in a temporary file
                m_Generator = new DataGenerator(m_Revit.Application, m_Revit.ActiveUIDocument.Document, m_Revit.ActiveUIDocument.Document.ActiveView);
                DataGenerator.GeneratorStatus succeed = m_Generator.SaveSTLFile(fileName, aSetting);

                if (succeed == DataGenerator.GeneratorStatus.FAILURE)
                {
                    this.DialogResult = DialogResult.Cancel;
                    MessageBox.Show(STLExportResource.ERR_SAVE_FILE_FAILED, STLExportResource.MESSAGE_BOX_TITLE,
                             MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else if (succeed == DataGenerator.GeneratorStatus.CANCEL)
                {
                    this.DialogResult = DialogResult.Cancel;
                    MessageBox.Show(STLExportResource.CANCEL_FILE_NOT_SAVED, STLExportResource.MESSAGE_BOX_TITLE,
                             MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

                this.DialogResult = DialogResult.OK;

                this.Close();
            }
            else
            {
                MessageBox.Show(STLExportResource.CANCEL_FILE_NOT_SAVED, STLExportResource.MESSAGE_BOX_TITLE,
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        /// <summary>
        /// Cancel button click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        /// <summary>
        /// Check all button click event, checks all categories in the category list.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void btnCheckAll_Click(object sender, EventArgs e)
        {
            for (int index = 0; index < lbCategories.Items.Count; index++)
            {
                lbCategories.SetItemChecked(index, true);
            }
        }

        /// <summary>
        /// Check none click event, unchecks all categories in the category list.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void btnCheckNone_Click(object sender, EventArgs e)
        {
            foreach (int index in lbCategories.CheckedIndices)
            {
                lbCategories.SetItemChecked(index, false);
            }
        }

        /// <summary>
        /// Included linked models click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void cbIncludeLinked_CheckedChanged(object sender, EventArgs e)
        {
            if (cbIncludeLinked.Checked == true)
            {
                MessageBox.Show(STLExportResource.WARN_PROJECT_POSITION, STLExportResource.MESSAGE_BOX_TITLE,
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
