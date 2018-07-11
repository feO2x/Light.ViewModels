using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Light.ViewModels
{
    /// <summary>
    /// Represents a base class implementation of <see cref="INotifyDataErrorInfo" />, using a <see cref="ViewModels.ValidationManager" />
    /// internally. Also provides <see cref="INotifyPropertyChanged" /> functionality via <see cref="BaseNotifyPropertyChanged" />.
    /// </summary>
    public abstract class BaseNotifyDataErrorInfo : BaseNotifyPropertyChanged, INotifyDataErrorInfo, IRaiseErrorsChanged
    {
        /// <summary>
        /// Gets the validation manager used to validate properties.
        /// </summary>
        protected readonly ValidationManager ValidationManager;

        /// <summary>
        /// Initializes a new instance of <see cref="BaseNotifyDataErrorInfo" /> with an optional <paramref name="validationManager" />.
        /// </summary>
        /// <param name="validationManager">The validation manager used by this instance (optional). If null is passed in, then a new instance of <see cref="ValidationManager" /> is created.</param>
        protected BaseNotifyDataErrorInfo(ValidationManager validationManager = null)
        {
            ValidationManager = validationManager ?? new ValidationManager(this);
        }

        /// <summary>
        /// Gets all errors that this entity currently has.
        /// </summary>
        public IReadOnlyDictionary<string, ValidationResult<ValidationMessage>> AllErrors => ValidationManager.AllErrors;

        /// <inheritdoc />
        public IEnumerable GetErrors(string propertyName) => ValidationManager.GetErrors(propertyName);

        /// <inheritdoc />
        public bool HasErrors => ValidationManager.HasErrors;

        /// <summary>
        /// Occurs when the validation errors have changed for a property or for the entire entity.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        void IRaiseErrorsChanged.OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Validates the value using the specified <paramref name="validate" /> delegate and calls <see cref="ErrorsChanged" />
        /// when the errors have changed for the given property.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The value to be validated.</param>
        /// <param name="validate">The method that performs the validation of the given value.</param>
        /// <param name="propertyName">The name of the property (optional). This value is automatically set using the <see cref="CallerMemberNameAttribute" />.</param>
        /// <returns>The validation result of the <paramref name="validate" /> delegate.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="validate" /> or <paramref name="propertyName" /> is null.</exception>
        protected ValidationResult<ValidationMessage> Validate<T>(T value, Func<T, ValidationResult<ValidationMessage>> validate, [CallerMemberName] string propertyName = null) =>
            ValidationManager.Validate(value, validate, propertyName);
    }
}