using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SchoolMedicalSystem.WPF.ViewModels
{
    public class RelayCommand : ICommand
    {
        private readonly Func<object, Task> _executeAsync;
        private readonly Action<object> _executeSync;
        private readonly Predicate<object> _canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        // Constructor cho lệnh bất đồng bộ (async)
        public RelayCommand(Func<object, Task> execute, Predicate<object> canExecute = null)
        {
            _executeAsync = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        // Constructor cho lệnh đồng bộ (sync)
        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _executeSync = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);

        public async void Execute(object parameter)
        {
            if (_executeAsync != null)
            {
                await _executeAsync(parameter);
            }
            else
            {
                _executeSync(parameter);
            }
        }
    }
}