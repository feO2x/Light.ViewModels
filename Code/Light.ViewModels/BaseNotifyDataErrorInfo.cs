using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Light.ViewModels
{
    /// <summary>
    /// Represents a base class implementation of <see cref="INotifyDataErrorInfo" />, using a <see cref="ViewModels.ValidationManager" />
    /// internally. Also provides <see cref="INotifyPropertyChanged" /> functionality via <see cref="BaseNotifyPropertyChanged" />.
    /// </summary>
    public abstract class BaseNotifyDataErrorInfo<TError> : BaseNotifyPropertyChanged, INotifyDataErrorInfo, IRaiseErrorsChanged
    {
        private int _numberOfConcurrentRequests;

        /// <summary>
        /// Initializes a new instance of <see cref="BaseNotifyDataErrorInfo{TError}" /> with an optional <paramref name="validationManager" />.
        /// </summary>
        /// <param name="validationManager">The validation manager used by this instance (optional). If null is passed in, then a new instance of <see cref="ValidationManager" /> is created.</param>
        protected BaseNotifyDataErrorInfo(ValidationManager<TError>? validationManager = null)
        {
            ValidationManager = validationManager ?? new ValidationManager<TError>(this);
        }

        /// <summary>
        /// Gets the validation manager used to validate properties.
        /// </summary>
        protected ValidationManager<TError> ValidationManager { get; }

        /// <summary>
        /// Gets all errors that this entity currently has.
        /// </summary>
        public IReadOnlyDictionary<string, ValidationResult<TError>> AllErrors => ValidationManager.AllErrors;

        /// <summary>
        /// Gets the value indicating whether at least one async validation is currently active.
        /// </summary>
        public bool IsValidatingAsynchronously => _numberOfConcurrentRequests > 0;

        /// <summary>
        /// Gets the number of concurrent async requests that are currently active.
        /// </summary>
        protected int NumberOfConcurrentRequests
        {
            get => _numberOfConcurrentRequests;
            private set
            {
                var previousValue = _numberOfConcurrentRequests;
                _numberOfConcurrentRequests = value;
                if (previousValue == 0 && value > 0)
                    OnIsValidatingAsyncChangedInternal();
                else if (previousValue > 0 && value == 0)
                    OnIsValidatingAsyncChangedInternal();
            }
        }

        /// <inheritdoc />
        public IEnumerable? GetErrors(string propertyName) => ValidationManager.GetErrors(propertyName);

        /// <inheritdoc />
        public bool HasErrors => ValidationManager.HasErrors;

        /// <summary>
        /// Occurs when the validation errors have changed for a property or for the entire entity.
        /// </summary>
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

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
        protected ValidationResult<TError> Validate<T>(T value, Func<T, ValidationResult<TError>> validate, [CallerMemberName] string? propertyName = null) =>
            ValidationManager.Validate(value, validate, propertyName);

        /// <summary>
        /// Validates the value asynchronously using the specified <paramref name="validateAsync" /> delegate and calls
        /// <see cref="ErrorsChanged" /> when the errors have changed for the given property.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The value to be validated.</param>
        /// <param name="validateAsync">The method that performs the validation of the given value.</param>
        /// <param name="propertyName">The name of the property (optional). This value is automatically set using the <see cref="CallerMemberNameAttribute" />.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="validateAsync" /> or <paramref name="propertyName" /> is null.</exception>
        protected async Task<ValidationResult<TError>> ValidateAsync<T>(T value,
                                                                        Func<T, Task<ValidationResult<TError>>> validateAsync,
                                                                        [CallerMemberName] string? propertyName = null)
        {
            NumberOfConcurrentRequests++;
            var result = await ValidationManager.ValidateAsync(value, validateAsync, propertyName);
            NumberOfConcurrentRequests--;
            return result;
        }

        /// <summary>
        /// Validates and parses the <paramref name="value"/> with the specified <paramref name="validateAndParse"/> delegate and calls
        /// <see cref="ErrorsChanged"/> when the errors have changed for the given property.
        /// </summary>
        /// <typeparam name="TInput">The type of the value.</typeparam>
        /// <typeparam name="TParsed">The type that <paramref name="value" /> should be parsed to.</typeparam>
        /// <param name="value">The value to be validated and parsed.</param>
        /// <param name="validateAndParse">The delegate that performs the validation and parsing of the given value.</param>
        /// <param name="parsedValue">The value that was parsed from the input value.</param>
        /// <param name="propertyName">The name of the property (optional). This value is automatically set using the <see cref="CallerMemberNameAttribute" />.</param>
        /// <returns>The validation result of the <paramref name="validateAndParse" /> delegate.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="validateAndParse" /> or <paramref name="propertyName" /> is null.</exception>
        protected ValidationResult<TError> ValidateAndParse<TInput, TParsed>(TInput value,
                                                                             ValidateAndParse<TInput, TError, TParsed> validateAndParse,
                                                                             out TParsed parsedValue,
                                                                             [CallerMemberName] string? propertyName = null) =>
            ValidationManager.ValidateAndParse(value, validateAndParse, out parsedValue, propertyName);

        /// <summary>
        /// Validates and parses the <paramref name="value"/> asynchronously with the specified <paramref name="validateAndParseAsync"/> delegate and calls
        /// <see cref="ErrorsChanged"/> when the errors have changed for the given property.
        /// </summary>
        /// <typeparam name="TInput">The type of the value.</typeparam>
        /// <typeparam name="TParsed">The type that <paramref name="value" /> should be parsed to.</typeparam>
        /// <param name="value">The value to be validated and parsed.</param>
        /// <param name="validateAndParseAsync">The delegate that performs the validation and parsing of the given value.</param>
        /// <param name="propertyName">The name of the property (optional). This value is automatically set using the <see cref="CallerMemberNameAttribute" />.</param>
        /// <returns>The async validation result of the <paramref name="validateAndParseAsync" /> delegate.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="validateAndParseAsync" /> or <paramref name="propertyName" /> is null.</exception>
        protected async Task<AsyncValidateAndParseResult<TError, TParsed>> ValidateAndParseAsync<TInput, TParsed>(TInput value,
                                                                                                                  ValidateAndParseAsync<TInput, TError, TParsed> validateAndParseAsync,
                                                                                                                  [CallerMemberName] string? propertyName = null)
        {
            NumberOfConcurrentRequests++;
            var result = await ValidationManager.ValidateAndParseAsync(value, validateAndParseAsync, propertyName);
            NumberOfConcurrentRequests--;
            return result;
        }

        private void OnIsValidatingAsyncChangedInternal()
        {
            OnPropertyChanged(nameof(IsValidatingAsynchronously));
            OnIsValidatingAsynchronouslyChanged();
        }

        /// <summary>
        /// Override this method to get notified in your subclass when <see cref="IsValidatingAsynchronously" /> changes.
        /// You do not to include the <c>base.OnIsValidatingAsynchronouslyChanged</c> - this method is empty.
        /// </summary>
        protected virtual void OnIsValidatingAsynchronouslyChanged() { }
    }

    /// <summary>
    /// Represents a base class implementation of <see cref="INotifyDataErrorInfo" />, using a <see cref="ValidationManager{TError}" />
    /// internally. Also provides <see cref="INotifyPropertyChanged" /> functionality via <see cref="BaseNotifyPropertyChanged" />.
    /// </summary>
    public abstract class BaseNotifyDataErrorInfo : BaseNotifyDataErrorInfo<string>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="BaseNotifyDataErrorInfo" />.
        /// </summary>
        /// <param name="validationManager">The validation manager used by this instance (optional). If null is passed in, then a new instance of <see cref="ValidationManager" /> is created.</param>
        protected BaseNotifyDataErrorInfo(ValidationManager<string>? validationManager = null) : base(validationManager) { }
    }
}