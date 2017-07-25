using System;
using System.Windows.Input;
using Light.GuardClauses;

namespace Light.ViewModels
{
    public sealed class DelegateCommand : ICommand
    {
        public readonly Func<bool> CanExecuteFunc;
        public readonly Action ExecuteAction;

        public DelegateCommand(Action execute, Func<bool> canExecute = null)
        {
            ExecuteAction = execute.MustNotBeNull(nameof(execute));
            CanExecuteFunc = canExecute;
        }

        public void Execute()
        {
            ExecuteAction();
        }

        public bool CanExecute()
        {
            return CanExecuteFunc?.Invoke() ?? true;
        }

        bool ICommand.CanExecute(object parameter)
        {
            return CanExecute();
        }

        void ICommand.Execute(object parameter)
        {
            ExecuteAction();
        }

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}