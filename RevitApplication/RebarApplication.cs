using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using System.Linq;
using System.Collections.Generic;

namespace RevitApplication
{
    [Transaction(TransactionMode.Manual)]
    public class RebarApplication : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            application.ControlledApplication.DocumentOpened += OnDocumentOpened;
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            application.ControlledApplication.DocumentOpened -= OnDocumentOpened;
            return Result.Succeeded;
        }

        private void OnDocumentOpened(object sender, Autodesk.Revit.DB.Events.DocumentOpenedEventArgs args)
        {
            Document doc = args.Document;

            // Step 1: Find all elements of category OST_Rebar
            FilteredElementCollector rebarCollector = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_Rebar)
                .WhereElementIsNotElementType();
            IList<Element> rebarElements = rebarCollector.ToList();

            if (rebarElements.Count == 0) return;

            // Step 2: Filter "fake" rebars
            List<Element> fakeRebars = new List<Element>();
            foreach (Element rebarElement in rebarElements)
            {
                Rebar rebar = rebarElement as Rebar;
                RebarInSystem rebarInSystem = rebarElement as RebarInSystem;

                if (rebar == null && rebarInSystem == null)
                {
                    fakeRebars.Add(rebarElement);
                }
            }

            if (fakeRebars.Count == 0) return;

            // Step 3: Start a transaction to modify parameters
            using (Transaction tx = new Transaction(doc, "Update comments of fake rebars"))
            {
                tx.Start();

                foreach (Element fakeRebar in fakeRebars)
                {
                    string diameterValue = "Failed to retrieve";
                    string lengthValue = "Failed to retrieve";

                    Parameter rebarDiameterParam = fakeRebar.LookupParameter("Rebar Diameter");
                    Parameter lengthParam = fakeRebar.LookupParameter("L");

                    if (rebarDiameterParam != null && rebarDiameterParam.HasValue)
                    {
                        diameterValue = rebarDiameterParam.AsValueString();
                        if (double.TryParse(diameterValue, out double diameterNum))
                        {
                            diameterValue = diameterNum.ToString(diameterNum % 1 == 0 ? "F0" : "F1");
                        }
                    }

                    if (lengthParam != null && lengthParam.HasValue)
                    {
                        lengthValue = lengthParam.AsValueString();
                        if (double.TryParse(lengthValue, out double lengthNum))
                        {
                            lengthValue = lengthNum.ToString(lengthNum % 1 == 0 ? "F0" : "F1");
                        }
                    }

                    if (diameterValue == "Failed to retrieve" || lengthValue == "Failed to retrieve") continue;

                    Parameter commentsParam = fakeRebar.LookupParameter("Comments");
                    if (commentsParam == null || commentsParam.IsReadOnly) continue;

                    string commentsValue = $"Ø{diameterValue}, Grade 60, L={lengthValue}";
                    commentsParam.Set(commentsValue);
                }

                tx.Commit();
            }
        }
    }
}