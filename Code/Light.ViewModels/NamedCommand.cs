using System;
using Light.GuardClauses;

namespace Light.ViewModels
{
    /// <summary>
    /// Represents a <see cref="DelegateCommand" /> that has a name.
    /// </summary>
    public class NamedCommand : DelegateCommand
    {
        /// <summary>
        /// Initializes a new instance of <see cref="NamedCommand" />.
        /// </summary>
        /// <param name="name">The name of the command.</param>
        /// <param name="execute">The delegate that will be executed when <see cref="DelegateCommand.Execute" /> is called.</param>
        /// <param name="canExecute">The delegate that will be executed when <see cref="DelegateCommand.CanExecute" /> is called (optional).</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name" /> or <paramref name="execute" /> is null.</exception>
        public NamedCommand(string name, Action execute, Func<bool> canExecute = null) : base(execute, canExecute)
        {
            Name = name.MustNotBeNull(nameof(name));
        }

        /// <summary>
        /// Gets the name of the command.
        /// </summary>
        public string Name { get; }
    }
}