using System;
using System.Collections;
using System.ComponentModel;

namespace Light.ViewModels
{
    /// <summary>
    ///     Represents a base class implementation of <see cref="INotifyDataErrorInfo" />, using a <see cref="ValidationManager" />
    ///     internally. Also provides <see cref="INotifyPropertyChanged" /> functionality via <see cref="BaseNotifyPropertyChanged" />.
    /// </summary>
    public abstract class BaseNotifyDataErrorInfo : BaseNotifyPropertyChanged, INotifyDataErrorInfo, IRaiseErrorsChanged
    {
        /// <summary>
        ///     Gets the validation manager used to validate properties.
        /// </summary>
        protected readonly ValidationManager ValidationManager;

        /// <summary>
        ///     Initializes a new instance of <see cref="BaseNotifyDataErrorInfo" /> with an optional <paramref name="validationManager" />.
        /// </summary>
        /// <param name="validationManager">The validation manager used by this instance (optional). If null is passed in, then a new instance of <see cref="ValidationManager" /> is created.</param>
        protected BaseNotifyDataErrorInfo(ValidationManager validationManager = null)
        {
            ValidationManager = validationManager ?? new ValidationManager();
        }

        /// <inheritdoc />
        public IEnumerable GetErrors(string propertyName) => ValidationManager.GetErrors(propertyName);

        /// <inheritdoc />
        public bool HasErrors => ValidationManager.HasErrors;

        /// <summary>
        ///     Occurs when the validation errors have changed for a property or for the entire entity.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        void IRaiseErrorsChanged.OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
    }
}