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

namespace BIM.STLExport
{
    partial class STLExportForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(STLExportForm));
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnHelp = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tpGeneral = new System.Windows.Forms.TabPage();
            this.cbExportColor = new System.Windows.Forms.CheckBox();
            this.comboBox_DUT = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cbIncludeLinked = new System.Windows.Forms.CheckBox();
            this.gbSTLFormat = new System.Windows.Forms.GroupBox();
            this.rbAscii = new System.Windows.Forms.RadioButton();
            this.rbBinary = new System.Windows.Forms.RadioButton();
            this.tpCategories = new System.Windows.Forms.TabPage();
            this.btnCheckNone = new System.Windows.Forms.Button();
            this.btnCheckAll = new System.Windows.Forms.Button();
            this.cbExportSharedCoordinates = new System.Windows.Forms.CheckBox();
            this.tvCategories = new System.Windows.Forms.TreeView();
            this.tabControl1.SuspendLayout();
            this.tpGeneral.SuspendLayout();
            this.gbSTLFormat.SuspendLayout();
            this.tpCategories.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnHelp
            // 
            resources.ApplyResources(this.btnHelp, "btnHelp");
            this.btnHelp.Name = "btnHelp";
            this.btnHelp.UseVisualStyleBackColor = true;
            this.btnHelp.Click += new System.EventHandler(this.btnHelp_Click);
            // 
            // btnSave
            // 
            resources.ApplyResources(this.btnSave, "btnSave");
            this.btnSave.Name = "btnSave";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tpGeneral);
            this.tabControl1.Controls.Add(this.tpCategories);
            resources.ApplyResources(this.tabControl1, "tabControl1");
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            // 
            // tpGeneral
            // 
            this.tpGeneral.BackColor = System.Drawing.SystemColors.Control;
            this.tpGeneral.Controls.Add(this.cbExportSharedCoordinates);
            this.tpGeneral.Controls.Add(this.cbExportColor);
            this.tpGeneral.Controls.Add(this.comboBox_DUT);
            this.tpGeneral.Controls.Add(this.label1);
            this.tpGeneral.Controls.Add(this.cbIncludeLinked);
            this.tpGeneral.Controls.Add(this.gbSTLFormat);
            resources.ApplyResources(this.tpGeneral, "tpGeneral");
            this.tpGeneral.Name = "tpGeneral";
            // 
            // cbExportColor
            // 
            resources.ApplyResources(this.cbExportColor, "cbExportColor");
            this.cbExportColor.Name = "cbExportColor";
            this.cbExportColor.UseVisualStyleBackColor = true;
            // 
            // comboBox_DUT
            // 
            this.comboBox_DUT.FormattingEnabled = true;
            resources.ApplyResources(this.comboBox_DUT, "comboBox_DUT");
            this.comboBox_DUT.Name = "comboBox_DUT";
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // cbIncludeLinked
            // 
            resources.ApplyResources(this.cbIncludeLinked, "cbIncludeLinked");
            this.cbIncludeLinked.Name = "cbIncludeLinked";
            this.cbIncludeLinked.UseVisualStyleBackColor = true;
            this.cbIncludeLinked.CheckedChanged += new System.EventHandler(this.cbIncludeLinked_CheckedChanged);
            // 
            // gbSTLFormat
            // 
            this.gbSTLFormat.Controls.Add(this.rbAscii);
            this.gbSTLFormat.Controls.Add(this.rbBinary);
            resources.ApplyResources(this.gbSTLFormat, "gbSTLFormat");
            this.gbSTLFormat.Name = "gbSTLFormat";
            this.gbSTLFormat.TabStop = false;
            // 
            // rbAscii
            // 
            resources.ApplyResources(this.rbAscii, "rbAscii");
            this.rbAscii.Name = "rbAscii";
            this.rbAscii.UseVisualStyleBackColor = true;
            // 
            // rbBinary
            // 
            this.rbBinary.Checked = true;
            resources.ApplyResources(this.rbBinary, "rbBinary");
            this.rbBinary.Name = "rbBinary";
            this.rbBinary.TabStop = true;
            this.rbBinary.UseVisualStyleBackColor = true;
            this.rbBinary.CheckedChanged += new System.EventHandler(this.rbExportFormat_CheckedChanged);
            // 
            // tpCategories
            // 
            this.tpCategories.BackColor = System.Drawing.SystemColors.Control;
            this.tpCategories.Controls.Add(this.tvCategories);
            this.tpCategories.Controls.Add(this.btnCheckNone);
            this.tpCategories.Controls.Add(this.btnCheckAll);
            resources.ApplyResources(this.tpCategories, "tpCategories");
            this.tpCategories.Name = "tpCategories";
            // 
            // btnCheckNone
            // 
            resources.ApplyResources(this.btnCheckNone, "btnCheckNone");
            this.btnCheckNone.Name = "btnCheckNone";
            this.btnCheckNone.UseVisualStyleBackColor = true;
            this.btnCheckNone.Click += new System.EventHandler(this.btnCheckNone_Click);
            // 
            // btnCheckAll
            // 
            resources.ApplyResources(this.btnCheckAll, "btnCheckAll");
            this.btnCheckAll.Name = "btnCheckAll";
            this.btnCheckAll.UseVisualStyleBackColor = true;
            this.btnCheckAll.Click += new System.EventHandler(this.btnCheckAll_Click);
            // 
            // cbExportSharedCoordinates
            // 
            resources.ApplyResources(this.cbExportSharedCoordinates, "cbExportSharedCoordinates");
            this.cbExportSharedCoordinates.Name = "cbExportSharedCoordinates";
            this.cbExportSharedCoordinates.UseVisualStyleBackColor = true;
            // 
            // tvCategories
            // 
            this.tvCategories.CheckBoxes = true;
            resources.ApplyResources(this.tvCategories, "tvCategories");
            this.tvCategories.Name = "tvCategories";
            // 
            // STLExportForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnHelp);
            this.Controls.Add(this.btnCancel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "STLExportForm";
            this.tabControl1.ResumeLayout(false);
            this.tpGeneral.ResumeLayout(false);
            this.tpGeneral.PerformLayout();
            this.gbSTLFormat.ResumeLayout(false);
            this.tpCategories.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnHelp;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tpGeneral;
        private System.Windows.Forms.TabPage tpCategories;
        private System.Windows.Forms.CheckBox cbIncludeLinked;
        private System.Windows.Forms.GroupBox gbSTLFormat;
        private System.Windows.Forms.RadioButton rbAscii;
        private System.Windows.Forms.RadioButton rbBinary;
        private System.Windows.Forms.Button btnCheckNone;
        private System.Windows.Forms.Button btnCheckAll;
        private System.Windows.Forms.ComboBox comboBox_DUT;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbExportColor;
        private System.Windows.Forms.CheckBox cbExportSharedCoordinates;
        private System.Windows.Forms.TreeView tvCategories;
    }
}
