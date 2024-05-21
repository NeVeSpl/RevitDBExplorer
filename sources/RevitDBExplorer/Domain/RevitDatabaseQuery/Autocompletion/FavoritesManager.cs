using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Autocompletion.Internals;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.Domain.RevitDatabaseQuery.Autocompletion
{
    internal class FavoritesManager
    {
        private static readonly string FilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "_RDBE_\\FavoriteQueries.json");
        private static List<FavoriteQueryDTO> favoriteQueries = new List<FavoriteQueryDTO>()
        {
            new FavoriteQueryDTO() { Query = "t: ParameterElement", Description = "User defined parameters" }
        };


        public static List<FavoriteQueryDTO> FavoriteQueries
        {
            get
            {
                return favoriteQueries;
            }
            set
            {
                favoriteQueries = value;              
            }
        }


        static FavoritesManager()
        {            
            Load();
        }


        private static void Load()
        {
            if (File.Exists(FilePath))
            {
                string jsonString = File.ReadAllText(FilePath);
                favoriteQueries = JsonSerializer.Deserialize<List<FavoriteQueryDTO>>(jsonString);
            } 
        }
        public static void Save()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
            string jsonString = JsonSerializer.Serialize(favoriteQueries);
            File.WriteAllText(FilePath, jsonString);
        }
        public static void Add(string query)
        {
            favoriteQueries.Add(new FavoriteQueryDTO() { Query = query } );
            Save();
        }

        public static IEnumerable<IAutocompleteItem> GetFavorites()
        {
            return favoriteQueries.Select(x => new AutocompleteItem(x.Query, x.Query, x.Description, AutocompleteItemGroups.Favourite)).OrderBy(x => x.Description);
        }
    }

    internal class FavoriteQueryDTO : BaseViewModel
    {
        public string Query { get; set; }
        public string Description { get; set; }        
    }
}