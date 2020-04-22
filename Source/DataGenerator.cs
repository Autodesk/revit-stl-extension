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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Windows.Forms;

using Autodesk.Revit;
using Autodesk.Revit.DB;

using RevitApplication = Autodesk.Revit.ApplicationServices.Application;
using GeometryElement = Autodesk.Revit.DB.GeometryElement;
using GeometryOptions = Autodesk.Revit.DB.Options;
using GeometryInstance = Autodesk.Revit.DB.GeometryInstance;
using RevitView = Autodesk.Revit.DB.View;

namespace BIM.STLExport
{
    /// <summary>
    /// Generate triangular data and save them in a temporary file.
    /// </summary>
    public class DataGenerator
    {
        public enum GeneratorStatus { SUCCESS, FAILURE, CANCEL };

        private SaveData m_Writer;
        private readonly RevitApplication m_RevitApp;
        private readonly Document m_ActiveDocument;
        private readonly RevitView m_ActiveView;
        private int m_TriangularNumber;
        private GeometryOptions m_ViewOptions;
        private SortedDictionary<string, Category> m_Categories;
        private Settings m_Settings;
        private STLExportCancelForm m_StlExportCancel = new STLExportCancelForm();

        /// <summary>
        /// Number of triangles in exported Revit document.
        /// </summary>
        public int TriangularNumber
        {
            get
            {
                return m_TriangularNumber;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="revit">
        /// The application object for the active instance of Autodesk Revit.
        /// </param>
        public DataGenerator(RevitApplication revitApp, Document doc, RevitView view)
        {
            //initialize the member variable
            if (revitApp != null)
            {
                m_RevitApp = revitApp;
                m_ActiveDocument = doc;
                m_ActiveView = view;

                m_ViewOptions = m_RevitApp.Create.NewGeometryOptions();
                m_ViewOptions.View = m_ActiveView;
            }
        }

        /// <summary>
        /// Save active Revit document as STL file according to customer's settings.
        /// </summary>
        /// <param name="fileName">The name of the STL file to be saved.</param>
        /// <param name="settings">Settings for save operation.</param>
        /// <returns>Successful or failed.</returns>      
        public GeneratorStatus SaveSTLFile(string fileName, Settings settings)
        {
            m_Settings = settings;

            try
            {

                m_StlExportCancel.Show();

                // save data in certain STL file
                if (SaveFormat.Binary == settings.SaveFormat)
                {
                    m_Writer = new SaveDataAsBinary(fileName, settings.SaveFormat);
                }
                else
                {
                    m_Writer = new SaveDataAsAscII(fileName, settings.SaveFormat);
                }

                m_Writer.CreateFile();
                ScanElement(settings.ExportRange);

                System.Windows.Forms.Application.DoEvents();

                if (m_StlExportCancel.CancelProcess == true)
                {
                    m_StlExportCancel.Close();
                    return GeneratorStatus.CANCEL;
                }

                if (0 == m_TriangularNumber)
                {
                    MessageBox.Show(STLExportResource.ERR_NOSOLID, STLExportResource.MESSAGE_BOX_TITLE,
                             MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                    m_StlExportCancel.Close();
                    return GeneratorStatus.FAILURE;
                }

                if (SaveFormat.Binary == settings.SaveFormat)
                {
                    // add triangular number to STL file
                    m_Writer.TriangularNumber = m_TriangularNumber;
                    m_Writer.AddTriangularNumberSection();
                }
                m_Writer.CloseFile();
            }
            catch (SecurityException)
            {
                MessageBox.Show(STLExportResource.ERR_SECURITY_EXCEPTION, STLExportResource.MESSAGE_BOX_TITLE,
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                m_StlExportCancel.Close();
                return GeneratorStatus.FAILURE;
            }
            catch (Exception)
            {
                MessageBox.Show(STLExportResource.ERR_EXCEPTION, STLExportResource.MESSAGE_BOX_TITLE,
                            MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

                m_StlExportCancel.Close();
                return GeneratorStatus.FAILURE;
            }

            m_StlExportCancel.Close();
            return GeneratorStatus.SUCCESS;
        }

        /// <summary>
        /// Scans all elements in the active document and creates a list of
        /// the categories of those elements.
        /// </summary>
        /// <returns>Sorted dictionary of categories.</returns>
        public SortedDictionary<string, Category> ScanCategories()
        {
            m_Categories = new SortedDictionary<string, Category>();

            // get all elements in the active document
            FilteredElementCollector filterCollector = new FilteredElementCollector(m_ActiveDocument);

            filterCollector.WhereElementIsNotElementType();

            FilteredElementIterator iterator = filterCollector.GetElementIterator();

            // create sorted dictionary of the categories of the elements
            while (iterator.MoveNext())
            {
                Element element = iterator.Current;

                if (element.Category != null)
                {
                    if (!m_Categories.ContainsKey(element.Category.Name))
                    {
                        m_Categories.Add(element.Category.Name, element.Category);
                    }
                }
            }

            return m_Categories;
        }

        /// <summary>
        /// Gets all categores in all open documents if allCategories is true
        /// or the categories of the elements in the active document if allCategories
        /// is set to false. 
        /// </summary>
        /// <param name="allCategories">True to get all categores in all open documents, 
        /// false to get the categories of the elements in the active document.</param>
        /// <returns>Sorted dictionary of categories.</returns>
        public SortedDictionary<string, Category> ScanCategories(bool allCategories)
        {
            if (!allCategories)
            {
                return ScanCategories();
            }
            else
            {
                // get and return all categories
                SortedDictionary<string, Category> sortedCategories = new SortedDictionary<string, Category>();

                // scan the active document for categories
                foreach (Category category in m_ActiveDocument.Settings.Categories)
                {
                    
                    if (!sortedCategories.ContainsKey(category.Name))
                        sortedCategories.Add(category.Name, category);
                }

                // if linked models exist scan for categories
                List<Document> linkedDocs = GetLinkedModels();

                foreach (Document linkedDoc in linkedDocs)
                {
                    foreach (Category category in linkedDoc.Settings.Categories)
                    {
                        if (!sortedCategories.ContainsKey(category.Name))
                            sortedCategories.Add(category.Name, category);
                    }
                }

                return sortedCategories;
            }
        }

        /// <summary>
        /// Get every Element in all open documents.
        /// </summary>
        /// <param name="exportRange">
        /// The range of elements to be exported.
        /// </param>
        private void ScanElement(ElementsExportRange exportRange)
        {
            List<Document> documents = new List<Document>();

            // active document should be the first docuemnt in the list
            documents.Add(m_ActiveDocument);

            // figure out if we need to get linked models
            if (m_Settings.IncludeLinkedModels)
            {
                List<Document> linkedDocList = GetLinkedModels();
                documents.AddRange(linkedDocList);
            }

            foreach (Document doc in documents)
            {
                FilteredElementCollector collector = null;

                if (ElementsExportRange.OnlyVisibleOnes == exportRange)
                {
                    // find the view having the same name of ActiveView.Name in active and linked model documents.
                    ElementId viewId = FindView(doc, m_ActiveView.Name);

                    if (viewId != ElementId.InvalidElementId)
                        collector = new FilteredElementCollector(doc, viewId);
                    else
                        collector = new FilteredElementCollector(doc);
                }
                else
                {
                    collector = new FilteredElementCollector(doc);
                }

                collector.WhereElementIsNotElementType();

                FilteredElementIterator iterator = collector.GetElementIterator();

                while (iterator.MoveNext())
                {
                    System.Windows.Forms.Application.DoEvents();

                    if (m_StlExportCancel.CancelProcess == true)
                        return;

                    Element element = iterator.Current;

                    // check if element's category is in the list, if it is continue.
                    // if there are no selected categories, take anything.
                    if (m_Settings.SelectedCategories.Count > 0)
                    {
                        if (element.Category == null)
                        {
                            continue;
                        }
                        else
                        {
                            IEnumerable<Category> cats = from cat in m_Settings.SelectedCategories
                                                         where cat.Id == element.Category.Id
                                                         select cat;

                            if (cats.Count() == 0)
                            {
                                continue;
                            }
                        }
                    }

                    // get the GeometryElement of the element
                    GeometryElement geometry = null;
                    geometry = element.get_Geometry(m_ViewOptions);

                    if (null == geometry)
                    {
                        continue;
                    }
                 
                    // get the solids in GeometryElement
                    ScanGeomElement(doc,geometry, null);
                }
            }
        }

        /// <summary>
        /// Get view by view name.
        /// </summary>
        /// <param name="doc">The document to find the view.</param>
        /// <param name="activeViewName">The view name.</param>
        /// <returns>The element id of the view found.</returns>
        private ElementId FindView(Document doc, string activeViewName)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(RevitView));

            IEnumerable<Element> selectedView = from view in collector.ToList<Element>()
                                                where view.Name == activeViewName
                                                select view;

            if (selectedView.Count() > 0)
            {
                return (selectedView.First() as RevitView).Id;
            }

            return ElementId.InvalidElementId;
        }

        /// <summary>
        /// Scan GeometryElement to collect triangles.
        /// </summary>
        /// <param name="geometry">The geometry element.</param>
        /// <param name="trf">The transformation.</param>
        private void ScanGeomElement(Document document, GeometryElement geometry, Transform transform)
        {
            //get all geometric primitives contained in the GeometryElement
            foreach (GeometryObject gObject in geometry)
            {
                // if the type of the geometric primitive is Solid
                Solid solid = gObject as Solid;
                if (null != solid)
                {
                    ScanSolid(document,solid, transform);
                    continue;
                }

                // if the type of the geometric primitive is instance
                GeometryInstance instance = gObject as GeometryInstance;
                if (null != instance)
                {
                    ScanGeometryInstance(document, instance, transform);
                    continue;
                }

                GeometryElement geomElement = gObject as GeometryElement;
                if (null != geomElement)
                {
                    ScanGeomElement(document,geomElement, transform);
                }
            }
        }

        /// <summary>
        /// Scan GeometryInstance to collect triangles.
        /// </summary>
        /// <param name="instance">The geometry instance.</param>
        /// <param name="trf">The transformation.</param>
        private void ScanGeometryInstance(Document document, GeometryInstance instance, Transform transform)
        {
            GeometryElement instanceGeometry = instance.SymbolGeometry;
            if (null == instanceGeometry)
            {
                return;
            }
            Transform newTransform;
            if (null == transform)
            {
                newTransform = instance.Transform;
            }
            else
            {
                newTransform = transform.Multiply(instance.Transform);	// get a transformation of the affine 3-space
            }

            // get all geometric primitives contained in the GeometryElement
            ScanGeomElement(document,instanceGeometry, newTransform);
        }

        /// <summary>
        /// Scan Solid to collect triangles.
        /// </summary>
        /// <param name="solid">The solid.</param>
        /// <param name="trf">The transformation.</param>
        private void ScanSolid(Document document, Solid solid, Transform transform)
        {
            GetTriangular(document, solid, transform);	// get triangles in the solid
        }

        /// <summary>
        /// Get triangles in a solid with transform.
        /// </summary>
        /// <param name="solid">The solid contains triangulars</param>
        /// <param name="transform">The transformation.</param>
        private void GetTriangular(Document document, Solid solid, Transform transform)
        {
            // a solid has many faces
            FaceArray faces = solid.Faces;
            bool hasTransform = (null != transform);
            if (0 == faces.Size)
            {
                return;
            }

            foreach (Face face in faces)
            {
                if (face.Visibility != Visibility.Visible)
                {
                    continue;
                }
                Mesh mesh = face.Triangulate();
                if (null == mesh)
                {
                    continue;
                }

                m_TriangularNumber += mesh.NumTriangles;

                PlanarFace planarFace = face as PlanarFace;

                // write face to stl file
                // a face has a mesh, all meshes are made of triangles
                for (int ii = 0; ii < mesh.NumTriangles; ii++)
                {
                    MeshTriangle triangular = mesh.get_Triangle(ii);
                    double[] xyz = new double[9];
                    Autodesk.Revit.DB.XYZ normal = new Autodesk.Revit.DB.XYZ();
                    try
                    {
                        Autodesk.Revit.DB.XYZ[] triPnts = new Autodesk.Revit.DB.XYZ[3];
                        for (int n = 0; n < 3; ++n)
                        {
                            double x, y, z;
                            Autodesk.Revit.DB.XYZ point = triangular.get_Vertex(n);
                            if (hasTransform)
                            {
                                point = transform.OfPoint(point);
                            }
                            if (m_Settings.ExportSharedCoordinates)
                            {
                                ProjectPosition ps = document.ActiveProjectLocation.GetProjectPosition(point);
                                x = ps.EastWest;
                                y = ps.NorthSouth;
                                z = ps.Elevation;
                            }
                            else
                            {
                                x = point.X;
                                y = point.Y;
                                z = point.Z;
                            }
                            if (!m_Settings.Units.Empty())
                            {
                                xyz[3 * n] = UnitUtils.ConvertFromInternalUnits(x, m_Settings.Units);
                                xyz[3 * n + 1] = UnitUtils.ConvertFromInternalUnits(y, m_Settings.Units);
                                xyz[3 * n + 2] = UnitUtils.ConvertFromInternalUnits(z, m_Settings.Units);
                            }
                            else
                            {
                                xyz[3 * n] = x;
                                xyz[3 * n + 1] = y;
                                xyz[3 * n + 2] = z;
                            }

                            var mypoint = new XYZ(xyz[3 * n], xyz[3 * n + 1], xyz[3 * n + 2]);
                            triPnts[n] = mypoint;
                        }

                        Autodesk.Revit.DB.XYZ pnt1 = triPnts[1] - triPnts[0];
                        normal = pnt1.CrossProduct(triPnts[2] - triPnts[1]);
                    }
                    catch (Exception ex)
                    {
                        m_TriangularNumber--;
                        STLDialogManager.ShowDebug(ex.Message);
                        continue;
                    }

                    if (m_Writer is SaveDataAsBinary && m_Settings.ExportColor)
                    {
                        Material material = document.GetElement(face.MaterialElementId) as Material;
                        if(material!=null)
                            ((SaveDataAsBinary)m_Writer).Color = material.Color;
                    }

                    m_Writer.WriteSection(normal, xyz);
                }
            }
        }

        /// <summary>
        /// Scans and returns the documents linked to the current model.
        /// </summary>
        /// <returns>List of linked documents.</returns>
        private List<Document> GetLinkedModels()
        {
            List<Document> linkedDocs = new List<Document>();

            try
            {
                // scan the current model looking for Revit links
                List<Element> linkedElements = FindLinkedModelElements();

                foreach (Element linkedElem in linkedElements)
                {
                    RevitLinkType linkType = linkedElem as RevitLinkType;

                    if (linkType != null)
                    {
                        // now look that up in the open documents
                        foreach (Document openedDoc in m_RevitApp.Documents)
                        {
                            if (Path.GetFileNameWithoutExtension(openedDoc.Title).ToUpper() == Path.GetFileNameWithoutExtension(linkType.Name).ToUpper())
                                linkedDocs.Add(openedDoc);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                STLDialogManager.ShowDebug(ex.Message);
            }

            return linkedDocs;

        }

        /// <summary>
        /// Scan model and return linked model elements.
        /// </summary>
        /// <returns>List of linked model elements.</returns>
        private List<Element> FindLinkedModelElements()
        {
            Document doc = m_ActiveDocument;

            FilteredElementCollector linksCollector = new FilteredElementCollector(doc);
            List<Element> linkElements = linksCollector.WherePasses(new ElementCategoryFilter(BuiltInCategory.OST_RvtLinks)).ToList<Element>();

            FilteredElementCollector familySymbolCollector = new FilteredElementCollector(doc);
            linkElements.AddRange(familySymbolCollector.OfClass(typeof(Autodesk.Revit.DB.FamilySymbol)).ToList<Element>());

            return linkElements;
        }

        /// <summary>
        /// Initializes the Cancel form.
        /// </summary>
        private void StartCancelForm()
        {
            STLExportCancelForm stlCancel = new STLExportCancelForm();
            stlCancel.Show();
        }
    }
}
