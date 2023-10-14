using System;
using System.Windows.Input;

namespace RevitDBExplorer.WPF
{
    internal class RelayCommand : ICommand
    {
        private readonly object execute;       
        private readonly Func<object, bool> canExecute;

      
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }
        public RelayCommand(Action execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }


        #region ICommand

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return canExecute?.Invoke(parameter) ?? true;
        }
        public void Execute(object parameter)
        {
            if (execute is Action<object> actionWithObject)
            {
                actionWithObject(parameter);
            }
            if (execute is Action action)
            {
                action();
            }
        }

        #endregion
    }
}