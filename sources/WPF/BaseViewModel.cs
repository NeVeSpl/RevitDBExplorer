using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RevitDBExplorer.WPF
{
    internal class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        protected virtual void OnPropertyChanged([CallerMemberName] String propertyName = "")
        {           
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));            
        }
    }
}
