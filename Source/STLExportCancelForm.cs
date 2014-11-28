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

namespace BIM.STLExport
{
    public partial class STLExportCancelForm : Form
    {
        private bool m_CancelProcess = false;

        public bool CancelProcess
        {
            get { return m_CancelProcess; }
            set { m_CancelProcess = value; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public STLExportCancelForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Cancel button click event.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The event args.</param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            m_CancelProcess = !m_CancelProcess;

            if (!m_CancelProcess)
                this.Close();
        }
    }
}
