using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;
using Autodesk.Revit.DB;
using RevitDBExplorer.Domain;
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
      


        public CommandAndControlVM()
        {
            
        }


        public async Task SetInput(GroupTreeItem groupTreeItemVM)
        {
            selectedGroup = groupTreeItemVM;
            ItemsCount = selectedGroup.GetAllSnoopableObjects().Select(x => x.Object).OfType<Element>().Count();
            var elements = selectedGroup.GetAllSnoopableObjects().Select(x => x.Object).OfType<Element>().Take(100).ToArray();
           
            
        }
    }
}