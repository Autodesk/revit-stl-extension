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

using System.Collections.Generic;
using Category = Autodesk.Revit.DB.Category;
using Autodesk.Revit.DB;

namespace BIM.STLExport
{
    /// <summary>
    /// The file format of STL.
    /// </summary>
    public enum SaveFormat
    {
        Binary = 0,
        ASCII
    }

    /// <summary>
    /// The range of elements to be exported.
    /// </summary>
    public enum ElementsExportRange
    {
        All = 0,
        OnlyVisibleOnes
    }

    /// <summary>
    /// Settings made by user to export.
    /// </summary>
    public struct Settings
    {
        private SaveFormat m_SaveFormat;
        private ElementsExportRange m_ExportRange;
        private bool m_IncludeLinkedModels;
        private bool m_exportColor;
        private bool m_exportSharedCoordinates;
        private List<Category> m_SelectedCategories;
        private ForgeTypeId m_Units;

        /// <summary>
        /// Binary or ASCII STL file.
        /// </summary>
        public SaveFormat SaveFormat
        {
            get
            {
                return m_SaveFormat;
            }
        }

        /// <summary>
        /// The range of elements to be exported.
        /// </summary>
        public ElementsExportRange ExportRange
        {
            get
            {
                return m_ExportRange;
            }
        }

        /// <summary>
        /// Include linked models.
        /// </summary>
        public bool IncludeLinkedModels
        {
            get
            {
                return m_IncludeLinkedModels;
            }
        }

        /// <summary>
        /// Export Color.
        /// </summary>
        public bool ExportColor
        {
            get
            {
                return m_exportColor;
            }
        }

        /// <summary>
        /// Export point in shared coordinates.
        /// </summary>
        public bool ExportSharedCoordinates
        {
            get
            {
                return m_exportSharedCoordinates;
            }
        }

        /// <summary>
        /// Include selected categories.
        /// </summary>
        public List<Category> SelectedCategories
        {
            get
            {
                return m_SelectedCategories;
            }
        }

        public ForgeTypeId Units
        {
           get
              {
                 return m_Units;
              }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="saveFormat">The file format.</param>
        /// <param name="exportRange">The export range.</param>
        public Settings(SaveFormat saveFormat, ElementsExportRange exportRange)
        {
            m_SaveFormat = saveFormat;
            m_ExportRange = exportRange;
            m_IncludeLinkedModels = false;
            m_exportColor = false;
            m_exportSharedCoordinates = false;
            m_SelectedCategories = new List<Category>();
            m_Units = new ForgeTypeId();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="saveFormat">The file format.</param>
        /// <param name="exportRange">The export range.</param>
        /// <param name="includeLinkedModels">True to include linked models, false otherwise.</param>
        /// <param name="selectedCategories">The selected categories to be included.</param>
        public Settings(SaveFormat saveFormat, ElementsExportRange exportRange, bool includeLinkedModels,bool exportColor,bool exportSharedCoordinates,
            List<Category> selectedCategories, ForgeTypeId units)
        {
            m_SaveFormat = saveFormat;
            m_ExportRange = exportRange;
            m_IncludeLinkedModels = includeLinkedModels;
            m_exportColor = exportColor;
            m_exportSharedCoordinates = exportSharedCoordinates;
            m_SelectedCategories = selectedCategories;
            m_Units = units;
        }
    }
}
