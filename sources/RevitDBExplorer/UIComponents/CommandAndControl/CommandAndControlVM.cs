using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain;
using RevitDBExplorer.Domain.RevitDatabaseView;
using RevitDBExplorer.UIComponents.Trees.Base.Items;
using RevitDBExplorer.WPF;
using Binding = System.Windows.Data.Binding;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.CommandAndControl
{
    internal class CommandAndControlVM : BaseViewModel
    {
        private GroupTreeItem selectedGroup;
        private int itemsCount;
        private ObservableCollection<DataGridColumn> columns;
        private ObservableCollection<Row> rows;

        public int ItemsCount
        {
            get
            {
                return itemsCount;
            }
            set
            {
                itemsCount = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<DataGridColumn> Columns
        {
            get
            {
                return columns;
            }
            set
            {
                columns = value;
                OnPropertyChanged();
            }
        }     
        public ObservableCollection<Row> Rows
        {
            get
            {
                return rows;
            }
            set
            {
                rows = value;
                OnPropertyChanged();
            }
        }


        public CommandAndControlVM()
        {
            
        }


        public async Task SetInput(GroupTreeItem groupTreeItemVM)
        {
            selectedGroup = groupTreeItemVM;
            ItemsCount = selectedGroup.GetAllSnoopableObjects().Select(x => x.Object).OfType<Element>().Count();
            var elements = selectedGroup.GetAllSnoopableObjects().Select(x => x.Object).OfType<Element>().Take(100).ToArray();
           
            var view =  await ExternalExecutor.ExecuteInRevitContextAsync(x => RevitDBExplorer.Domain.RevitDatabaseView.View.Create(x?.ActiveUIDocument?.Document, elements));

            var idColumns = new DataGridTextColumn[] { new DataGridTextColumn() { Header = "Name", Binding= new Binding("Name")}};
            var parameterColumns = view.Columns.Select((x, idx) => new DataGridTextColumn() { Header = x.Name, Binding = new Binding($"[{x.Id.Value()}].Value")});
            
            Columns = new ObservableCollection<DataGridColumn>(idColumns.Union(parameterColumns));
            Rows = new ObservableCollection<Row>(view.Rows);  
        }
    }
}