using System.Windows.Input;

namespace MesProject.ViewModels
{
    public class DelegateCommand : ICommand
    {
        public Action<object?>? ExecuteAction { get; set; }

        public Func<object?, bool>? CanExecuteFunc { get; set; } 

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            if (CanExecuteFunc == null)
                return true;
            return CanExecuteFunc(parameter);
        }

        public void Execute(object? parameter)
        {
            ExecuteAction?.Invoke(parameter);
        }
    }
}
