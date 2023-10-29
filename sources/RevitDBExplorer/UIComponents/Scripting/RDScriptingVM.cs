using System;
using System.Windows;
using RevitDBExplorer.WPF;

// (c) Revit Database Explorer https://github.com/NeVeSpl/RevitDBExplorer/blob/main/license.md

namespace RevitDBExplorer.UIComponents.Scripting
{
    internal class RDScriptingVM : BaseViewModel
    {       
        private bool isPanelOpen = false;
        private GridLength height;      
    

        public bool IsOpen
        {
            get
            {
                return isPanelOpen;
            }
            set
            {
                isPanelOpen = value;
                OnPropertyChanged();
            }
        }
        public GridLength Height
        {
            get
            {
                return height;
            }
            set
            {
                height = value;
                OnPropertyChanged();
            }
        }
   

        public RelayCommand CloseCommand { get; }
       
       
      

        public RDScriptingVM()
        {
            CloseCommand = new RelayCommand(Close);
                
        }


        public void Open()
        {           
            IsOpen = true;
            Height = new GridLength(Math.Max(Height.Value, 198));
          
            
        }
        private void Close(object parameter)
        {
            IsOpen = false;
            Height = new GridLength(0, GridUnitType.Auto);
        }
      
    }  
}