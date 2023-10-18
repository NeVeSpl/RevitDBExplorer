using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.Breadcrumbs
{
    internal class BreadcrumbsVM : BaseViewModel
    {
        public ObservableCollection<Breadcrumb> Crumbs { get; set; } = new ObservableCollection<Breadcrumb>();


        public void Set(string crumb)
        {
            Crumbs.Clear();
            Crumbs.Add(new Breadcrumb { Title = crumb});
        }
    }

    internal class Breadcrumb : BaseViewModel
    {
         public string Title { get; set; }




        public Breadcrumb()
        {
               
        }
    }

}