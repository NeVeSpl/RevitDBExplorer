using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Autodesk.Revit.DB;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain
{
    internal static class UpdaterInfoWizard
    {
        private static Dictionary<string, UpdaterId> nameToGuidMap = new();


        /// <summary>
        /// The real name is `Update`, but it does not sound that cool as : 
        /// </summary>
        public static void AvadaKedavra()
        {
            var iupdater = typeof(IUpdater);           

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {                
                if (assembly.GlobalAssemblyCache) continue;
                if (assembly.FullName.StartsWith("Microsoft") || assembly.FullName.StartsWith("System")) continue;
                try
                {
                    var updaterTypes = assembly.GetTypes().Where(x => iupdater.IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract).ToList();
                    foreach (var updaterType in updaterTypes)
                    {
                        try
                        {
                            var updater = FormatterServices.GetUninitializedObject(updaterType) as IUpdater;
                            var updaterId = updater.GetUpdaterId();
                            var updaterName = updater.GetUpdaterName();

                            var addinId = updaterId.GetAddInId();
                            var updaterGuid = updaterId.GetGUID();

                            var addinName = addinId.GetAddInName();
                            var addinGuid = addinId.GetGUID();
                          
                            nameToGuidMap[$"{addinName}.{updaterName}"] = updaterId;
                        }
                        catch
                        {

                        }
                    }
                }
                catch
                {

                }
            }
        }

        public static UpdaterId Get(string applicationName, string updaterName)
        {
            nameToGuidMap.TryGetValue($"{applicationName}.{updaterName}", out UpdaterId result);
            return result;            
        }
    }
}