using System;
using System.Windows.Input;
using Light.GuardClauses;

namespace Light.ViewModels
{
    /// <summary>
    /// Represents an <see cref="ICommand" /> that calls parameterized delegates.
    /// </summary>
    public class ParameterizedCommand : ICommand
    {
        /// <summary>
        /// Gets the delegate that is executed when <see cref="CanExecute" /> is called. This value might be null.
        /// </summary>
        public readonly Func<object, bool> CanExecuteFunc;

        /// <summary>
        /// Gets the delegate that is executed when <see cref="Execute" /> is called.
        /// </summary>
        public readonly Action<object> ExecuteAction;

        /// <summary>
        /// Initializes a new instance of <see cref="ParameterizedCommand" />
        /// </summary>
        /// <param name="execute">The delegate that will be executed when <see cref="Execute" /> is called.</param>
        /// <param name="canExecute">the delegate that will be executed when <see cref="CanExecute" /> is called (optional).</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="execute" /> is null.</exception>
        public ParameterizedCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            ExecuteAction = execute.MustNotBeNull(nameof(execute));
            CanExecuteFunc = canExecute;
        }

        /// <summary>
        /// Checks if the command can be executed.
        /// </summary>
        public virtual bool CanExecute(object parameter) => CanExecuteFunc?.Invoke(parameter) ?? true;

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when this method is called, but <see cref="CanExecute" /> returns false.</exception>
        public virtual void Execute(object parameter)
        {
            if (CanExecute(parameter) == false)
                throw new InvalidOperationException("Execute must not be called when CanExecute returns false.");
            ExecuteAction(parameter);
        }

        /// <inheritdoc />
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged" /> event.
        /// </summary>
        public virtual void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Represents an <see cref="ICommand" /> that calls delegates with a strongly typed parameter.
    /// </summary>
    /// <typeparam name="T">The type of the parameter that will be passed to the delegates.</typeparam>
    public class ParameterizedCommand<T> : ICommand
    {
        /// <summary>
        /// Gets the delegate that is executed when <see cref="CanExecute" /> is called. This value might be null.
        /// </summary>
        public readonly Func<T, bool> CanExecuteFunc;

        /// <summary>
        /// Gets the delegate that is executed when <see cref="Execute" /> is called.
        /// </summary>
        public readonly Action<T> ExecuteAction;

        /// <summary>
        /// Initializes a new instance of <see cref="ParameterizedCommand{T}" />
        /// </summary>
        /// <param name="execute">The delegate that will be executed when <see cref="Execute" /> is called.</param>
        /// <param name="canExecute">the delegate that will be executed when <see cref="CanExecute" /> is called (optional).</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="execute" /> is null.</exception>
        public ParameterizedCommand(Action<T> execute, Func<T, bool> canExecute = null)
        {
            ExecuteAction = execute.MustNotBeNull(nameof(execute));
            CanExecuteFunc = canExecute;
        }

        bool ICommand.CanExecute(object parameter) => CanExecute((T) parameter);

        void ICommand.Execute(object parameter) => Execute((T) parameter);

        /// <inheritdoc />
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when this method is called, but <see cref="CanExecute" /> returns false.</exception>
        public virtual void Execute(T parameter)
        {
            if (CanExecute(parameter) == false)
                throw new InvalidOperationException("Execute must not be called when CanExecute returns false.");
            ExecuteAction(parameter);
        }

        /// <summary>
        /// Checks if the command can be executed.
        /// </summary>
        public virtual bool CanExecute(T parameter) => CanExecuteFunc?.Invoke(parameter) ?? true;

        /// <summary>
        /// Raises the <see cref="CanExecuteChanged" /> event.
        /// </summary>
        public virtual void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
    }
}