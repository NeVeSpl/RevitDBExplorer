using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer
{

    public partial class Assemblies : Window, INotifyPropertyChanged
    {
        public ObservableCollection<AssemblyViewModel> Items { get; set; } = new ObservableCollection<AssemblyViewModel>();



        public Assemblies()
        {
            InitializeComponent();

            var viewModels =  AppDomain.CurrentDomain.GetAssemblies().Select((x, i) => new AssemblyViewModel(i, x));
            Items = new ObservableCollection<AssemblyViewModel>(viewModels);

            this.DataContext = this;
        }





        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var lcv = (ListCollectionView)CollectionViewSource.GetDefaultView(Items);
            lcv.Filter = Filter;
        }

        private bool Filter(object item)
        {
            if (item is AssemblyViewModel assemblyViewModel)
            {
                return assemblyViewModel.Filter(cFilter.Text);
            }
            return true;
        }
    }



    public class AssemblyViewModel : BaseViewModel
    {
        public int No { get; set; }
        public string Name { get; set; }
        public string Version { get; private set; }
        public string Path { get; private set; }
        public string AssemblyLoadContext { get; private set; }


        public AssemblyViewModel(int no, Assembly asm)
        {
            No = no + 1;

            var asmName = asm.GetName();
            Name = asmName.Name;
            Version = asmName.Version?.ToString();
            Path = asm.IsDynamic ? string.Empty : asm.Location;
#if R2025_MIN
            var context = System.Runtime.Loader.AssemblyLoadContext.GetLoadContext(asm);
            AssemblyLoadContext = context.Name;
#endif

        }


        public bool Filter(string text)
        {
            if (string.IsNullOrEmpty(text)) return true;
            if (Name?.Contains(text, StringComparison.OrdinalIgnoreCase) == true) return true;
            if (Path?.Contains(text, StringComparison.OrdinalIgnoreCase) == true) return true;
            if (AssemblyLoadContext?.Contains(text, StringComparison.OrdinalIgnoreCase) == true) return true;
            if (Version?.Contains(text, StringComparison.OrdinalIgnoreCase) == true) return true;

            return false;
        }
    }
}