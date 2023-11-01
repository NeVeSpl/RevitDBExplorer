using System;
using RevitDBExplorer.Domain.RevitDatabaseQuery.Autocompletion;
using RevitDBExplorer.UIComponents.List;
using RevitDBExplorer.WPF;
using RevitDBExplorer.WPF.Controls;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.QueryEditor
{
    internal class QueryEditorViewModel : BaseViewModel, IAmQueryExecutor
    {
        private readonly IAutocompleteItemProvider databaseQueryAutocompleteItemProvider = new AutocompleteItemProvider();
        private readonly Action<string> tryQueryDatabase;      
        private bool isPopupOpen;
        private string databaseQuery = string.Empty;
        private string databaseQueryToolTip = string.Empty;
       


        public bool IsPopupOpen
        {
            get
            {
                return isPopupOpen;
            }
            set
            {
                isPopupOpen = value;
                OnPropertyChanged();
            }
        }
        public string DatabaseQuery
        {
            get
            {
                return databaseQuery;
            }
            set
            {
                databaseQuery = value;
                if (IsPopupOpen == false)
                {
                    tryQueryDatabase(value);
                }
                OnPropertyChanged();
            }
        }
        public string DatabaseQueryToolTip
        {
            get
            {
                return databaseQueryToolTip;
            }
            set
            {
                databaseQueryToolTip = value;
                OnPropertyChanged();
            }
        }

        public IAutocompleteItemProvider DatabaseQueryAutocompleteItemProvider
        {
            get
            {
                return databaseQueryAutocompleteItemProvider;
            }
        }
        public RelayCommand OpenScriptingWithQueryCommand { get; }
        public RelayCommand SaveQueryAsFavoriteCommand { get; }




        public QueryEditorViewModel(Action<string> tryQueryDatabase, Action generateScriptForQueryAndOpenRDS)
        {
            this.tryQueryDatabase = tryQueryDatabase;           

            OpenScriptingWithQueryCommand = new RelayCommand(generateScriptForQueryAndOpenRDS);
            SaveQueryAsFavoriteCommand = new RelayCommand(SaveQueryAsFavorite, x => !string.IsNullOrEmpty(DatabaseQuery));
        }

        public void ResetDatabaseQuery()
        {
            databaseQuery = "";
            OnPropertyChanged(nameof(DatabaseQuery));
        }
        void IAmQueryExecutor.Query(string query)
        {
            DatabaseQuery = query;
        }



        private void SaveQueryAsFavorite()
        {
            FavoritesManager.Add(DatabaseQuery);
        }       
    }
}
