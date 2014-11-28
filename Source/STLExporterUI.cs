using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Revit.UI;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Media.Imaging;

namespace BIM.STLExport
{
   public class STLExporterUI : IExternalApplication
   {
      // Fields
      private static string AddInPath;

      // Methods
      static STLExporterUI()
      {
         AddInPath = typeof(STLExporterUI).Assembly.Location;
      }

      Result IExternalApplication.OnShutdown(UIControlledApplication application)
      {
         return Result.Succeeded;
      }

      Result IExternalApplication.OnStartup(UIControlledApplication application)
      {
         try
         {
            string str = "STL Exporter";
            RibbonPanel panel = application.CreateRibbonPanel(str);
            string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            PushButtonData data = new PushButtonData("STL Exporter for Revit", "STL Exporter for Revit", directoryName + @"\STLExport.dll", "BIM.STLExport.STLExportCommand");
            PushButton button = panel.AddItem(data) as PushButton;
            button.LargeImage = LoadPNGImageFromResource("BIM.STLExport.Resources.STLExporter_32.png");
            button.ToolTip = "The STL Exporter for Revit is designed to produce a stereolithography file (STL) of your building model.";
            button.LongDescription = "The STL Exporter for the Autodesk Revit Platform is a proof-of-concept project designed to create an STL file from a 3D building information model, thereby enabling easier 3D printing.";
            ContextualHelp help = new ContextualHelp(ContextualHelpType.ChmFile, directoryName + @"\Resources\ADSKSTLExporterHelp.htm");
            button.SetContextualHelp(help);
            return Result.Succeeded;
         }
         catch (Exception exception)
         {
            MessageBox.Show(exception.ToString(), "STL Exporter for Revit");
            return Result.Failed;
         }

      }

      private static System.Windows.Media.ImageSource LoadPNGImageFromResource(string imageResourceName)
      {
         PngBitmapDecoder decoder = new PngBitmapDecoder(Assembly.GetExecutingAssembly().GetManifestResourceStream(imageResourceName), BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
         return decoder.Frames[0];
      }
   }
}
