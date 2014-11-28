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
using System.Diagnostics;
using System.Windows.Forms;

namespace BIM.STLExport
{
   /// <summary>
   /// Manager class for dialogs in the project.
   /// </summary>
   public class STLDialogManager
   {
      /// <summary>
      /// Pop up a standard SaveAs dialog.
      /// </summary>
      /// <returns>The filename return by SaveAs dialog.</returns>
      public static string SaveDialog()
      {
         // save file dialog options
         using (SaveFileDialog saveDialog = new SaveFileDialog())
         {
            saveDialog.OverwritePrompt = true;
            saveDialog.AddExtension = true;
            saveDialog.DefaultExt = STLExportResource.SAVE_DIALOG_DEFAULT_FILE_EXTEND;
            saveDialog.Filter = STLExportResource.SAVE_DIALOG_FILE_FILTER;

            if (System.Windows.Forms.DialogResult.OK != saveDialog.ShowDialog())
            {
               return String.Empty;
            }
            return saveDialog.FileName;
         }
      }

      /// <summary>
      /// Used to show error message when debug.
      /// </summary>
      /// <param name="exception">The exception message.</param>
      [Conditional("DEBUG")]
      public static void ShowDebug(string exception)
      {
         MessageBox.Show(exception, STLExportResource.MESSAGE_BOX_TITLE,
                        MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
      }
   }
}
