using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain
{
    internal class FavoriteMembersManager
    {
        private static readonly string FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "_RDBE_\\FavoriteMembers.json");
        private static List<FavoriteMemberDTO> favorites = new List<FavoriteMemberDTO>();
        private static HashSet<string> favoriteIds = new HashSet<string>()
        {
            "Element.get_Geometry(Options)",
            "Element.get_Parameters()",
            "Rebar.GetCenterlineCurves(Boolean,Boolean,Boolean,MultiplanarOption,Int32)",
            "FamilyInstance.get_Symbol()",
            "FamilyInstance.get_Location()",
            "Instance.GetTransform()",
            "Element.GetEntity(Schema)",
            "Document.get_ParameterBindings()",
            "Application.OpenSharedParameterFile()",
            "Rebar.GetRebarConstraintsManager()",
            "Element.GetTypeId()",
        };

        static FavoriteMembersManager()
        {
            Load();
        }

        private static void Load()
        {
            if (File.Exists(FilePath))
            {
                string jsonString = File.ReadAllText(FilePath);
                favorites = JsonSerializer.Deserialize<List<FavoriteMemberDTO>>(jsonString);
                favoriteIds = new HashSet<string>(favorites.Select(x => x.Id));
            }
        }
        private static void Save()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
            string jsonString = JsonSerializer.Serialize(favorites);
            File.WriteAllText(FilePath, jsonString);
        }


        public static bool IsFavorite(string id)
        {
            if (string.IsNullOrEmpty(id)) return false;
            return favoriteIds.Contains(id);
        }
        public static void AddFavorite(string id)
        {
            favorites.Add(new FavoriteMemberDTO { Id = id });   
            favoriteIds.Add(id);
            Save();
        }
        public static void RemoveFavorite(string id)
        {           
            favorites.RemoveAll(x => x.Id == id);
            favoriteIds.Remove(id);
            Save();
        }
    }


    internal class FavoriteMemberDTO :  IEquatable<FavoriteMemberDTO>
    {
        public string Id { get; set; }


        public bool Equals(FavoriteMemberDTO other)
        {
            return Id == other.Id;
        }
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}