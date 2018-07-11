using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Light.GuardClauses;

namespace Light.ViewModels
{
    /// <summary>
    /// Represents an object that handles the current errors of an entity.
    /// </summary>
    /// <typeparam name="TError">The type of the error messages.</typeparam>
    public class ValidationManager<TError>
    {
        /// <summary>
        /// Gets the internal dictionary used to store erroneous validation results.
        /// </summary>
        protected readonly Dictionary<string, ValidationResult<TError>> Errors;

        /// <summary>
        /// Initializes a new instance of <see cref="ValidationManager{TError}" /> with an optional errors dictionary.
        /// </summary>
        /// <param name="errors">The dictionary used to store the validation results of erroneous properties (optional). A new dictionary will be automatically created if null is passed in.</param>
        public ValidationManager(Dictionary<string, ValidationResult<TError>> errors = null)
        {
            Errors = errors ?? new Dictionary<string, ValidationResult<TError>>();
        }

        /// <summary>
        /// Gets the value indicating whether the target entity contains errors.
        /// </summary>
        public virtual bool HasErrors => Errors.Count > 0;

        /// <summary>
        /// Gets the errors dictionary of the validation manager.
        /// </summary>
        public IReadOnlyDictionary<string, ValidationResult<TError>> AllErrors => Errors;

        /// <summary>
        /// Gets all error messages associated with the given property name.
        /// </summary>
        /// <param name="propertyName">The name of the property.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="propertyName" /> is null.</exception>
        public IEnumerable GetErrors(string propertyName)
        {
            propertyName.MustNotBeNull(nameof(propertyName));
            return Errors.TryGetValue(propertyName, out var validationResult) ? validationResult.Errors : null;
        }

        /// <summary>
        /// Validates the <paramref name="value" /> with the specified <paramref name="validate" /> delegate and calls <see cref="IRaiseErrorsChanged.OnErrorsChanged" />
        /// when the errors have changed for the given property.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The value to be validated.</param>
        /// <param name="validate">The method that performs the validation of the given value.</param>
        /// <param name="raiseErrorsChanged">The target entity which raise OnErrorsChanged.</param>
        /// <param name="propertyName">The name of the property (optional). This value is automatically set using the <see cref="CallerMemberNameAttribute" />.</param>
        /// <returns>The validation result of the <paramref name="validate" /> delegate.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="validate" />, <paramref name="raiseErrorsChanged" />, or <paramref name="propertyName" /> is null.</exception>
        public ValidationResult<TError> Validate<T>(T value, Func<T, ValidationResult<TError>> validate, IRaiseErrorsChanged raiseErrorsChanged, [CallerMemberName] string propertyName = null)
        {
            validate.MustNotBeNull(nameof(validate));
            raiseErrorsChanged.MustNotBeNull(nameof(raiseErrorsChanged));
            propertyName.MustNotBeNull(nameof(propertyName));

            var validationResult = validate(value);
            if (validationResult.IsValid)
            {
                if (Errors.ContainsKey(propertyName) == false)
                    return validationResult;

                Errors.Remove(propertyName);
                raiseErrorsChanged.OnErrorsChanged(propertyName);
                return validationResult;
            }

            if (Errors.TryGetValue(propertyName, out var existingErrors))
            {
                if (existingErrors == validationResult)
                    return validationResult;

                Errors[propertyName] = validationResult;
                raiseErrorsChanged.OnErrorsChanged(propertyName);
                return validationResult;
            }

            Errors.Add(propertyName, validationResult);
            raiseErrorsChanged.OnErrorsChanged(propertyName);
            return validationResult;
        }
    }

    /// <summary>
    /// Represents a <see cref="ValidationManager{TError}" /> that uses <see cref="ValidationMessage" /> instances as error objects.
    /// </summary>
    public class ValidationManager : ValidationManager<ValidationMessage>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ValidationManager" /> with an optional errors dictionary.
        /// </summary>
        /// <param name="errors">The dictionary used to store the validation results of erroneous properties (optional). A new dictionary will be automatically created if null is passed in.</param>
        public ValidationManager(Dictionary<string, ValidationResult<ValidationMessage>> errors = null) : base(errors) { }

        /// <summary>
        /// Gets the value indicating whether the target entity contains errors. Only <see cref="ValidationMessage" /> instances
        /// that contain level <see cref="ValidationMessageLevel.Error" /> are considered as actual errors.
        /// </summary>
        public override bool HasErrors
        {
            get
            {
                if (Errors.Count == 0) return false;

                var numberOfErrors = 0;
                foreach (var keyValuePair in Errors)
                {
                    for (var i = 0; i < keyValuePair.Value.Errors.Count; i++)
                    {
                        if (keyValuePair.Value.Errors[i].Level == ValidationMessageLevel.Error)
                            numberOfErrors++;
                    }
                }

                return numberOfErrors > 0;
            }
        }
    }
}