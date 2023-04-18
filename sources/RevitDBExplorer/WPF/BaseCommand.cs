using System;
using System.Windows.Input;

namespace RevitDBExplorer.WPF
{
    internal abstract class BaseCommand : ICommand
    {
        #region ICommand

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public abstract bool CanExecute(object parameter);
        public abstract void Execute(object parameter);

        #endregion
    }
}