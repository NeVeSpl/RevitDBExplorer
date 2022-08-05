using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autodesk.Revit.UI;
using Autodesk.RevitAddIns;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain
{
    internal static class AddInManifestWizard
    {
        private static Dictionary<string, RevitAddInManifest> assemblyToManifestMap = new();


        /// <summary>
        /// The real name is `Update`, but it does not sound that cool as : 
        /// </summary>
        public static void WingardiumLeviosa(UIApplication application)
        {
            Dictionary<string, string> invalidFormatManifestDictionary = new Dictionary<string, string>();

            var allUsers = AddInManifestUtility.GetRevitAddInManifests(application.Application.AllUsersAddinsLocation, invalidFormatManifestDictionary);
            var currentUser = AddInManifestUtility.GetRevitAddInManifests(application.Application.CurrentUserAddinsLocation, invalidFormatManifestDictionary);

            assemblyToManifestMap = new();
            foreach (var manifest in allUsers.Concat(currentUser))
            {
                foreach (var app in manifest.AddInApplications)
                {
                    Add(app.Assembly, manifest);
                }
                foreach (var app in manifest.AddInDBApplications)
                {
                    Add(app.Assembly, manifest);
                }
                foreach (var cmd in manifest.AddInCommands)
                {
                    Add(cmd.Assembly, manifest);
                }
            }
        }

        private static void Add(string assembly, RevitAddInManifest manifest)
        {            
            if (!Path.IsPathRooted(assembly))
            {
                var root = Path.GetDirectoryName(manifest.FullName);
                assembly = Path.Combine(root, assembly);
            }
            var path = new Uri(assembly).LocalPath;
            assemblyToManifestMap[path] = manifest;
        }

        public static RevitAddInManifest Get(string assemblyPath)
        {
            assemblyToManifestMap.TryGetValue(assemblyPath, out RevitAddInManifest result);
            return result;
        }
    }
}