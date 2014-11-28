//STL exporter library: this library works with Autodesk(R) Revit(R) to export an STL file containing model geometry.
//Copyright (C) 2012  Autodesk, Inc.

//This library is free software; you can redistribute it and/or
//modify it under the terms of the GNU Lesser General Public
//License as published by the Free Software Foundation; either
//version 2.1 of the License, or (at your option) any later version.

//This library is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//Lesser General Public License for more details.

//You should have received a copy of the GNU Lesser General Public
//License along with this library; if not, write to the Free Software
//Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Modify_Addin_file
{
    class Program
    {
        static void Main(string[] args)
        {
            //A sample of arguments will be like:
            // /TARGETDIR="[TARGETDIR] " /AllUsersAddInFileName="[ALLUSERS_ADDINFOLDER]STLExport.addin" /CurrentUserAddInFileName="[CURRENTUSER_ADDINFOLDER]STLExport.addin"

            //get the parameters from command line.

            string targetDir = null;
            string allUsersAddInFileName = null;
            string currentUserAddInFileName = null;

            foreach (string arg in args)
            {
                int equalMarkIndex = arg.IndexOf("=");

                if (arg.StartsWith("/TARGETDIR", StringComparison.CurrentCultureIgnoreCase))
                {
                    targetDir = arg.Substring(equalMarkIndex + 1).Trim();
                }
                else if (arg.StartsWith("/AllUsersAddInFileName", StringComparison.CurrentCultureIgnoreCase))
                {
                    allUsersAddInFileName = arg.Substring(equalMarkIndex + 1).Trim();
                }
                else if (arg.StartsWith("/CurrentUserAddInFileName", StringComparison.CurrentCultureIgnoreCase))
                {
                    currentUserAddInFileName = arg.Substring(equalMarkIndex + 1).Trim();
                }
            }

            //Modify the AddInFile under All Users folder
            ReplaceStringInFile("[TARGETDIR]", targetDir, allUsersAddInFileName);

            //Modify the AddInFile under Current User folder
            ReplaceStringInFile("[TARGETDIR]", targetDir, currentUserAddInFileName);
        }

        /// <summary>
        /// Replace a string in a text file.
        /// </summary>
        /// <param name="oldString">the string in the text file to be replaced</param>
        /// <param name="newString">the new string to replace the old string</param>
        /// <param name="fileName">the text file name with path</param>
        static private void ReplaceStringInFile(string oldString, string newString, string fileName)
        {
            if (!File.Exists(fileName))
            {
                return;
            }

            //Read the file content.
            StreamReader reader = new StreamReader(fileName);
            string fileContent = reader.ReadToEnd();
            Encoding currentEncoding = reader.CurrentEncoding;
            reader.Close();

            //Replace string in the file content.
            fileContent = fileContent.Replace(oldString, newString);

            //Write back the modified string to the file.
            StreamWriter writer = new StreamWriter(fileName, false, currentEncoding);
            writer.Write(fileContent);
            writer.Flush();
            writer.Close();

            return;
        }
    }
}
