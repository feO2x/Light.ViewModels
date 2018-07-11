using System;
using System.Windows.Input;
using Light.GuardClauses;

namespace Light.ViewModels
{
    /// <summary>
    /// Represents an <see cref="ICommand" /> that calls parameterless delegates.
    /// </summary>
    public class DelegateCommand : ICommand
    {
        /// <summary>
        /// Gets the delegate that is executed when <see cref="CanExecute" /> is called. This value might be null.
        /// </summary>
        public readonly Func<bool> CanExecuteFunc;

        /// <summary>
        /// Gets the delegate that is executed when <see cref="Execute" /> is called.
        /// </summary>
        public readonly Action ExecuteAction;

        /// <summary>
        /// Initializes a new instance of <see cref="DelegateCommand" />.
        /// </summary>
        /// <param name="execute">The delegate that will be executed when <see cref="Execute" /> is called.</param>
        /// <param name="canExecute">The delegate that will be executed when <see cref="CanExecute" /> is called (optional).</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="execute" /> is null.</exception>
        public DelegateCommand(Action execute, Func<bool> canExecute = null)
        {
            ExecuteAction = execute.MustNotBeNull(nameof(execute));
            CanExecuteFunc = canExecute;
        }

        bool ICommand.CanExecute(object parameter) => CanExecute();

        void ICommand.Execute(object parameter) => Execute();

        /// <inheritdoc />
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when this method is called, but <see cref="CanExecute" /> returns false.</exception>
        public virtual void Execute()
        {
            if (CanExecute() == false)
                throw new InvalidOperationException("Execute must not be called when CanExecute returns false.");
            ExecuteAction();
        }

        /// <summary>
        /// Checks if the command can be executed.
        /// </summary>
        public virtual bool CanExecute() => CanExecuteFunc?.Invoke() ?? true;

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged" /> event.
        /// </summary>
        public virtual void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}