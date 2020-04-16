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
        private SortedDictionary<string, ForgeTypeId> m_DisplayUnits = new SortedDictionary<string, ForgeTypeId>();
        private static ForgeTypeId m_SelectedDUT = new ForgeTypeId();

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

            foreach (Category category in m_CategoryList.Values)
            {
                TreeNode treeNode = GetChildNode(category,m_Revit.ActiveUIDocument.Document.ActiveView);
                if (treeNode != null)
                    tvCategories.Nodes.Add(treeNode);                               
            }
            
            string unitName = "Use Internal: Feet";
            m_DisplayUnits.Add(unitName, new ForgeTypeId());
            int selectedIndex = comboBox_DUT.Items.Add(unitName);
            if (m_SelectedDUT.Empty())
               comboBox_DUT.SelectedIndex = selectedIndex;

            Units currentUnits = m_Revit.ActiveUIDocument.Document.GetUnits();
            ForgeTypeId currentDut = currentUnits.GetFormatOptions(SpecTypeId.Length).GetUnitTypeId();
            unitName = "Use Current: " + LabelUtils.GetLabelForUnit(currentDut);
            m_DisplayUnits.Add(unitName, currentDut);
            selectedIndex = comboBox_DUT.Items.Add(unitName);
            if (m_SelectedDUT == currentDut)
               comboBox_DUT.SelectedIndex = selectedIndex;

            foreach (ForgeTypeId dut in UnitUtils.GetValidUnits(SpecTypeId.Length))
            {
               if (currentDut == dut)
                  continue;
               unitName = LabelUtils.GetLabelForUnit(dut);
               m_DisplayUnits.Add(unitName, dut);
               selectedIndex = comboBox_DUT.Items.Add(unitName);
               if (m_SelectedDUT == dut)
                  comboBox_DUT.SelectedIndex = selectedIndex;
            }

            // initialize the UI differently for Families
            if (revit.ActiveUIDocument.Document.IsFamilyDocument)
            {
                cbIncludeLinked.Enabled = false;
                tvCategories.Enabled = false;
                btnCheckAll.Enabled = false;
                btnCheckNone.Enabled = false;
            }
        }

        /// <summary>
        /// Get all subcategory.
        /// </summary>
        /// <param name="category"></param>
        /// <param name="view">Current view.</param>
        /// <returns></returns>
        private TreeNode GetChildNode(Category category,Autodesk.Revit.DB.View view)
        {
            if(category == null)
                return null;

            if (!category.get_AllowsVisibilityControl(view))
                return null;

            TreeNode treeNode = new TreeNode(category.Name);
            treeNode.Tag = category;
            treeNode.Checked = true;

            if(category.SubCategories.Size == 0)
            {                
                return treeNode;
            }

            foreach (Category subCategory in category.SubCategories)
            {
                TreeNode child = GetChildNode(subCategory,view);
                if(child !=null)
                    treeNode.Nodes.Add(child);
            }

            return treeNode;
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
                List<Category> selectedCategories = new List<Category>();

                // only for projects
                if (m_Revit.ActiveUIDocument.Document.IsFamilyDocument == false)
                {
                    foreach (TreeNode treeNode in tvCategories.Nodes)
                    {
                        AddSelectedTreeNode(treeNode, selectedCategories);
                    }
                }

                ForgeTypeId dup = m_DisplayUnits[comboBox_DUT.Text];
                m_SelectedDUT = dup;

                // create settings object to save setting information
                Settings aSetting = new Settings(saveFormat, exportRange, cbIncludeLinked.Checked,cbExportColor.Checked,cbExportSharedCoordinates.Checked, selectedCategories, dup);

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
        /// Add selected category into dictionary.
        /// </summary>
        /// <param name="treeNode"></param>
        /// <param name="selectedCategories"></param>
        private void AddSelectedTreeNode(TreeNode treeNode,List<Category> selectedCategories)
        {
            if (treeNode.Checked)
              selectedCategories.Add((Category)treeNode.Tag);

            if(treeNode.Nodes.Count != 0)
            {
                foreach(TreeNode child in treeNode.Nodes)
                {
                    AddSelectedTreeNode(child, selectedCategories);
                }
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
            foreach (TreeNode treeNode in tvCategories.Nodes)
            {
                SetCheckedforTreeNode(treeNode,true);
            }
        }

        /// <summary>
        /// Check none click event, unchecks all categories in the category list.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void btnCheckNone_Click(object sender, EventArgs e)
        {
            foreach (TreeNode treeNode in tvCategories.Nodes)
            {
                SetCheckedforTreeNode(treeNode,false);
            }
        }

        /// <summary>
        /// Set the checked property of treenode to true or false.
        /// </summary>
        /// <param name="treeNode">The tree node.</param>
        /// <param name="selected">Checked or not.</param>
        private void SetCheckedforTreeNode(TreeNode treeNode,bool selected)
        {
            treeNode.Checked = selected;
            if (treeNode.Nodes.Count != 0)
            {
                foreach (TreeNode child in treeNode.Nodes)
                {
                    SetCheckedforTreeNode(child, selected);
                }
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

        private void rbExportFormat_CheckedChanged(object sender, EventArgs e)
        {
            cbExportColor.Enabled = rbBinary.Checked;
        }

    }
}
