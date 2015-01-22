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
using System.IO;
using System.Text;
using System.Security;
using System.Windows.Forms;

namespace BIM.STLExport
{
    /// <summary>
    /// Base class providing interface to save data to STL file.
    /// </summary>
    public abstract class SaveData : IDisposable
    {
        // stl file name
        protected string m_FileName = string.Empty;
        // file format: Binary or ASCII
        protected SaveFormat m_SaveFormat = SaveFormat.Binary;
        // total triangular number in the model
        protected int m_TriangularNumber = 0;

        /// <summary>
        /// Number of triangulars.
        /// </summary>
        public int TriangularNumber
        {
            get { return m_TriangularNumber; }
            set { m_TriangularNumber = value; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="fileName">STL file name.</param>
        /// <param name="format">File format.</param>
        public SaveData(string fileName, SaveFormat format)
        {
            m_FileName = fileName;
            m_SaveFormat = format;
        }

        /// <summary>
        /// Close file here if user forget it.
        /// </summary>
        public virtual void Dispose()
        {
            CloseFile();
        }

        /// <summary>
        /// Interface to create file.
        /// </summary>
        /// <returns>True if succeeded, false if failed.</returns>
        public abstract bool CreateFile();

        /// <summary>
        /// Interface to write one section include normal and vertex.
        /// </summary>
        /// <param name="normal">Facet normal.</param>
        /// <param name="vertexArr">Vertex array.</param>
        /// <returns>True if succeeded, false if failed.</returns>
        public abstract bool WriteSection(Autodesk.Revit.DB.XYZ normal, double[] vertexArr);

        /// <summary>
        /// Add triangular number section.
        /// </summary>
        /// <returns>True if succeeded, false if failed.</returns>
        public abstract bool AddTriangularNumberSection();

        /// <summary>
        /// Close file handle.
        /// </summary>
        /// <returns>True if succeeded, false if failed.</returns>
        public abstract bool CloseFile();
    }

    /// <summary>
    /// Save date to binary stl file.
    /// </summary>   
    public class SaveDataAsBinary : SaveData
    {
        FileStream fileWriteStream = null;
        BinaryWriter binaryWriter = null;

        private Autodesk.Revit.DB.Color m_color = null;

        /// <summary>
        /// Color of trangle mesh to export in Binary format.
        /// </summary>
        public Autodesk.Revit.DB.Color Color
        {
            set { m_color = value; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fileName">STL file name.</param>
        /// <param name="format">File format.</param>
        public SaveDataAsBinary(string fileName, SaveFormat format)
            : base(fileName, format)
        {
        }

        /// <summary>
        /// Implement interface of creating a file.
        /// </summary>
        /// <returns>True if succeeded, false if failed.</returns>
        public override bool CreateFile()
        {
            bool succeed = true;
            try
            {
                FileAttributes fileAttribute = FileAttributes.Normal;

                if (File.Exists(m_FileName))
                {
                    fileAttribute = File.GetAttributes(m_FileName);
                    FileAttributes tempAtt = fileAttribute & FileAttributes.ReadOnly;
                    if (FileAttributes.ReadOnly == tempAtt)
                    {
                        MessageBox.Show(STLExportResource.ERR_FILE_READONLY, STLExportResource.MESSAGE_BOX_TITLE,
                              MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }
                    File.Delete(m_FileName);
                }

                fileWriteStream = new FileStream(m_FileName, FileMode.Create);
                fileAttribute = File.GetAttributes(m_FileName) | fileAttribute;
                File.SetAttributes(m_FileName, fileAttribute);
                binaryWriter = new BinaryWriter(fileWriteStream);

                binaryWriter.BaseStream.Seek(0, SeekOrigin.Begin);

                // write 80 bytes to STL file as the STL file entity name
                // and preserve 4 bytes space for Triangular Number Section
                byte[] entityName = new byte[84];
                entityName[0] = (byte)/*MSG0*/'n';
                entityName[1] = (byte)/*MSG0*/'a';
                entityName[2] = (byte)/*MSG0*/'m';
                entityName[3] = (byte)/*MSG0*/'e';
                for (int i = 4; i < 84; i++)
                {
                    entityName[i] = (byte)/*MSG0*/'\0';
                }
                binaryWriter.Write(entityName);
            }
            catch (SecurityException)
            {
                MessageBox.Show(STLExportResource.ERR_SECURITY_EXCEPTION, STLExportResource.MESSAGE_BOX_TITLE,
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                succeed = false;
            }
            catch (IOException)
            {
                MessageBox.Show(STLExportResource.ERR_IO_EXCEPTION, STLExportResource.MESSAGE_BOX_TITLE,
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                succeed = false;
            }
            catch (Exception)
            {
                MessageBox.Show(STLExportResource.ERR_EXCEPTION, STLExportResource.MESSAGE_BOX_TITLE,
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                succeed = false;
            }
            return succeed;
        }

        /// <summary>
        /// Implement interface of closing the file.
        /// </summary>
        /// <returns>True if succeeded, false if failed.</returns>
        public override bool CloseFile()
        {
            bool succeed = true;
            if (null != binaryWriter)
            {
                binaryWriter.Close();
                binaryWriter = null;
            }
            if (null != fileWriteStream)
            {
                fileWriteStream.Close();
                fileWriteStream = null;
            }
            return succeed;
        }

        /// <summary>
        /// Implement interface of writing one section include normal and vertex.
        /// </summary>
        /// <param name="normal">Facet normal.</param>
        /// <param name="vertexArr">Vertex array.</param>
        /// <returns>True if succeeded, false if failed.</returns>
        public override bool WriteSection(Autodesk.Revit.DB.XYZ normal, double[] vertexArr)
        {
            bool succeed = true;
            try
            {
                // write 3 float numbers to stl file using 12 bytes. 
                for (int j = 0; j < 3; j++)
                {
                    binaryWriter.Write((float)normal[j]);
                }

                for (int i = 0; i < 9; i++)
                {
                    binaryWriter.Write((float)vertexArr[i]);
                }

                // add color to stl file using two bytes.
                if(m_color!=null)
                    binaryWriter.Write((UInt16)(((m_color.Red) >>3) | (((m_color.Green) >>3)<<5) | (((m_color.Blue)>>3)<<10)));
                else
                {
                    // add two spaces to stl file using two bytes.
                    byte[] anotherSpace = new byte[2];
                    anotherSpace[0] = (byte)/*MSG0*/'\0';
                    anotherSpace[1] = (byte)/*MSG0*/'\0';
                    binaryWriter.Write(anotherSpace);
                }
            }
            catch (SecurityException)
            {
                MessageBox.Show(STLExportResource.ERR_SECURITY_EXCEPTION, STLExportResource.MESSAGE_BOX_TITLE,
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                succeed = false;
            }
            catch (IOException)
            {
                MessageBox.Show(STLExportResource.ERR_IO_EXCEPTION, STLExportResource.MESSAGE_BOX_TITLE,
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                succeed = false;
            }
            catch (Exception)
            {
                MessageBox.Show(STLExportResource.ERR_EXCEPTION, STLExportResource.MESSAGE_BOX_TITLE,
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                succeed = false;
            }
            return succeed;
        }

        /// <summary>
        /// Implement interface of adding triangular number section.
        /// </summary>
        /// <returns>True if succeeded, false if failed.</returns>
        public override bool AddTriangularNumberSection()
        {
            bool succeed = true;
            try
            {
                binaryWriter.BaseStream.Seek(80, SeekOrigin.Begin);

                //write the tringle number to the STL file using 4 bytes.
                binaryWriter.Write(m_TriangularNumber);
            }
            catch (SecurityException)
            {
                MessageBox.Show(STLExportResource.ERR_SECURITY_EXCEPTION, STLExportResource.MESSAGE_BOX_TITLE,
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                succeed = false;
            }
            catch (IOException)
            {
                MessageBox.Show(STLExportResource.ERR_IO_EXCEPTION, STLExportResource.MESSAGE_BOX_TITLE,
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                succeed = false;
            }
            catch (Exception)
            {
                MessageBox.Show(STLExportResource.ERR_EXCEPTION, STLExportResource.MESSAGE_BOX_TITLE,
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                succeed = false;
            }
            return succeed;
        }

    }

    /// <summary>
    /// Save data to ASCII stl file.
    /// </summary>
    public class SaveDataAsAscII : SaveData
    {
        StreamWriter stlFile = null;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fileName">STL file name.</param>
        /// <param name="format">File format.</param>
        public SaveDataAsAscII(string fileName, SaveFormat format)
            : base(fileName, format)
        {
        }

        /// <summary>
        /// Implement interface of creating a file.
        /// </summary>
        /// <returns>True if succeeded, false if failed.</returns>
        public override bool CreateFile()
        {
            bool succeed = true;
            try
            {
                FileAttributes fileAttribute = FileAttributes.Normal;

                if (File.Exists(m_FileName))
                {
                    fileAttribute = File.GetAttributes(m_FileName);
                    FileAttributes tempAtt = fileAttribute & FileAttributes.ReadOnly;
                    if (FileAttributes.ReadOnly == tempAtt)
                    {
                        MessageBox.Show(STLExportResource.ERR_FILE_READONLY, STLExportResource.MESSAGE_BOX_TITLE,
                              MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }
                    File.Delete(m_FileName);
                }

                stlFile = new StreamWriter(m_FileName);
                fileAttribute = File.GetAttributes(m_FileName) | fileAttribute;
                File.SetAttributes(m_FileName, fileAttribute);

                stlFile.WriteLine(/*MSG0*/"solid ascii"); //file header
            }
            catch (SecurityException)
            {
                MessageBox.Show(STLExportResource.ERR_SECURITY_EXCEPTION, STLExportResource.MESSAGE_BOX_TITLE,
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                succeed = false;
            }
            catch (IOException)
            {
                MessageBox.Show(STLExportResource.ERR_IO_EXCEPTION, STLExportResource.MESSAGE_BOX_TITLE,
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                succeed = false;
            }
            catch (Exception)
            {
                MessageBox.Show(STLExportResource.ERR_EXCEPTION, STLExportResource.MESSAGE_BOX_TITLE,
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                succeed = false;
            }
            return succeed;
        }

        /// <summary>
        /// Implement interface of closing the file.
        /// </summary>
        /// <returns>True if succeeded, false if failed.</returns>
        public override bool CloseFile()
        {
            bool succeed = true;
            if (null != stlFile)
            {
                stlFile.Close();
                stlFile = null;
            }
            return succeed;
        }

        /// <summary>
        /// Implement interface of writing one section include normal and vertex.
        /// </summary>
        /// <param name="normal">Facet normal.</param>
        /// <param name="vertexArr">Vertex array.</param>
        /// <returns>True if succeeded, false if failed.</returns>
        public override bool WriteSection(Autodesk.Revit.DB.XYZ normal, double[] vertexArr)
        {
            bool succeed = true;
            try
            {
                StringBuilder normalSb = new StringBuilder(/*MSG0*/"  facet normal ");

                for (int j = 0; j < 3; j++)
                {
                    normalSb.Append(normal[j]).Append(/*MSG0*/" ");
                }
                stlFile.WriteLine(normalSb);
                stlFile.WriteLine(/*MSG0*/"    outer loop");
                for (int i = 0; i < 3; i++)
                {
                    StringBuilder vertexSb = new StringBuilder(/*MSG0*/" vertex ");

                    for (int j = 0; j < 3; j++)
                    {
                        vertexSb.Append(vertexArr[i * 3 + j]).Append(/*MSG0*/" ");
                    }

                    stlFile.WriteLine(vertexSb);
                }
                stlFile.WriteLine(/*MSG0*/"   endloop");
                stlFile.WriteLine(/*MSG0*/"  endfacet");
            }
            catch (SecurityException)
            {
                MessageBox.Show(STLExportResource.ERR_SECURITY_EXCEPTION, STLExportResource.MESSAGE_BOX_TITLE,
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                succeed = false;
            }
            catch (IOException)
            {
                MessageBox.Show(STLExportResource.ERR_IO_EXCEPTION, STLExportResource.MESSAGE_BOX_TITLE,
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                succeed = false;
            }
            catch (Exception)
            {
                MessageBox.Show(STLExportResource.ERR_EXCEPTION, STLExportResource.MESSAGE_BOX_TITLE,
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                succeed = false;
            }
            return succeed;
        }

        /// <summary>
        /// ASCII doesn't need to add triangular number
        /// </summary>
        public override bool AddTriangularNumberSection()
        {
            // ASCII doesn't need to add triangular number
            throw new NotImplementedException("ASCII doesn't need to add triangular number");
        }
    }
}
