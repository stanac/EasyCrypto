using System;
using System.Windows.Input;

namespace SampleApp.ViewModels
{
    public class Command : ICommand
    {
        private Action _toExecute;

        public event EventHandler CanExecuteChanged;

        public Command(Action toExecute)
        {
            _toExecute = toExecute;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _toExecute?.Invoke();
        }
    }
}
