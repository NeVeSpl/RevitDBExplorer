using System;
using System.Linq;

namespace RevitDBExplorer.API
{
    public class RevitDBExplorer
    {
        public static IRDBEController CreateController()
        {
            var rdbeAssembly = AppDomain.CurrentDomain.GetAssemblies().Where(e => e.FullName.StartsWith("RevitDBExplorer,")).FirstOrDefault();
           
            if (rdbeAssembly is null)
            {
                throw new Exception("Revit database explorer is not available.");
            }

            var rdbeVersion = rdbeAssembly.GetName().Version;
            var apiVersion = typeof(RevitDBExplorer).Assembly.GetName().Version;

            if (rdbeVersion < apiVersion)
            {
                throw new Exception("You are using the old version of Revit database explorer, please do update.");
            }

            var controllerIType = rdbeAssembly.GetType("RevitDBExplorer.APIAdapter");
            var controller = Activator.CreateInstance(controllerIType) as IRDBEController;
            return controller;
        }
    }
}