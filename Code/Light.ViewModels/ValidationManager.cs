﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Light.GuardClauses;

namespace Light.ViewModels
{
    /// <summary>
    /// Represents an object that handles the current errors of an object (usually a view model) that implements <see cref="System.ComponentModel.INotifyDataErrorInfo" />.
    /// </summary>
    /// <typeparam name="TError">The type of the error messages.</typeparam>
    public class ValidationManager<TError>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ValidationManager{TError}" /> with an optional errors dictionary.
        /// </summary>
        /// <param name="target">The object that can raise the errors-changed-mechanism.</param>
        /// <param name="errors">The dictionary used to store the validation results of erroneous properties (optional). A new dictionary will be automatically created if null is passed in.</param>
        public ValidationManager(IRaiseErrorsChanged target, Dictionary<string, ValidationResult<TError>>? errors = null)
        {
            Target = target.MustNotBeNull(nameof(target));
            Errors = errors ?? new Dictionary<string, ValidationResult<TError>>();
        }

        /// <summary>
        /// Gets the target that can raise the Errors-Changed event.
        /// </summary>
        protected IRaiseErrorsChanged Target { get; }

        /// <summary>
        /// Gets the internal dictionary used to store erroneous validation results.
        /// </summary>
        protected Dictionary<string, ValidationResult<TError>> Errors { get; }

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
        public IEnumerable? GetErrors(string propertyName)
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
        /// <param name="propertyName">The name of the property (optional). This value is automatically set using the <see cref="CallerMemberNameAttribute" />.</param>
        /// <returns>The validation result of the <paramref name="validate" /> delegate.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="validate" /> or <paramref name="propertyName" /> is null.</exception>
        public ValidationResult<TError> Validate<T>(T value, Func<T, ValidationResult<TError>> validate, [CallerMemberName] string? propertyName = null)
        {
            validate.MustNotBeNull(nameof(validate));
            propertyName.MustNotBeNull(nameof(propertyName));

            var validationResult = validate(value);
            ProcessValidationResult(propertyName!, validationResult);
            return validationResult;
        }

        /// <summary>
        /// Validates the specified value asynchronously.
        /// </summary>
        /// <typeparam name="T">The type of the value that is being validated.</typeparam>
        /// <param name="value">The value to be validated.</param>
        /// <param name="validateAsync"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public async Task<ValidationResult<TError>> ValidateAsync<T>(T value,
                                                                     Func<T, Task<ValidationResult<TError>>> validateAsync,
                                                                     [CallerMemberName] string? propertyName = null)
        {
            validateAsync.MustNotBeNull(nameof(validateAsync));
            propertyName.MustNotBeNull(nameof(propertyName));

            var validationResult = await validateAsync(value);
            ProcessValidationResult(propertyName!, validationResult);
            return validationResult;
        }

        /// <summary>
        /// Validates and parses the <paramref name="value" /> with the specified <paramref name="validateAndParse" /> delegate to <paramref name="parsedValue" /> and calls
        /// <see cref="IRaiseErrorsChanged.OnErrorsChanged" /> when the errors have changed for the given property.
        /// </summary>
        /// <typeparam name="TInput">The type of the value.</typeparam>
        /// <typeparam name="TParsed">The type that <paramref name="value" /> should be parsed to.</typeparam>
        /// <param name="value">The value to be validated and parsed.</param>
        /// <param name="validateAndParse">The delegate that performs the validation and parsing of the given value.</param>
        /// <param name="parsedValue">The value that was parsed from the input value.</param>
        /// <param name="propertyName">The name of the property (optional). This value is automatically set using the <see cref="CallerMemberNameAttribute" />.</param>
        /// <returns>The validation result of the <paramref name="validateAndParse" /> delegate.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="validateAndParse" /> or <paramref name="propertyName" /> is null.</exception>
        public ValidationResult<TError> ValidateAndParse<TInput, TParsed>(TInput value,
                                                                          ValidateAndParse<TInput, TError, TParsed> validateAndParse,
                                                                          out TParsed parsedValue,
                                                                          [CallerMemberName] string? propertyName = null)
        {
            validateAndParse.MustNotBeNull(nameof(validateAndParse));
            propertyName.MustNotBeNull(nameof(propertyName));

            var validationResult = validateAndParse(value, out parsedValue);
            ProcessValidationResult(propertyName!, validationResult);
            return validationResult;
        }

        /// <summary>
        /// Validates and parses the <paramref name="value" /> asynchronously with the specified <paramref name="validateAndParseAsync" /> delegate and calls
        /// <see cref="IRaiseErrorsChanged.OnErrorsChanged" /> when the errors have changed for the given property.
        /// </summary>
        /// <typeparam name="TInput">The type of the value.</typeparam>
        /// <typeparam name="TParsed">The type that <paramref name="value" /> should be parsed to.</typeparam>
        /// <param name="value">The value to be validated and parsed.</param>
        /// <param name="validateAndParseAsync">The delegate that performs the validation and parsing of the given value.</param>
        /// <param name="propertyName">The name of the property (optional). This value is automatically set using the <see cref="CallerMemberNameAttribute" />.</param>
        /// <returns>The async validation result of the <paramref name="validateAndParseAsync" /> delegate.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="validateAndParseAsync" /> or <paramref name="propertyName" /> is null.</exception>
        public async Task<AsyncValidateAndParseResult<TError, TParsed>> ValidateAndParseAsync<TInput, TParsed>(TInput value,
                                                                                                               ValidateAndParseAsync<TInput, TError, TParsed> validateAndParseAsync,
                                                                                                               [CallerMemberName] string? propertyName = null)
        {
            validateAndParseAsync.MustNotBeNull(nameof(validateAndParseAsync));
            propertyName.MustNotBeNull(nameof(propertyName));

            var result = await validateAndParseAsync(value);
            ProcessValidationResult(propertyName!, result.ValidationResult);
            return result;
        }

        /// <summary>
        /// Clears all errors if necessary and calls <see cref="IRaiseErrorsChanged.OnErrorsChanged" /> for each entry that was present in the errors dictionary.
        /// </summary>
        public void ClearErrors()
        {
            if (Errors.Count == 0)
                return;

            var keys = Errors.Keys.ToArray();
            Errors.Clear();
            for (var i = 0; i < keys.Length; i++)
            {
                Target.OnErrorsChanged(keys[i]);
            }
        }

        private void ProcessValidationResult(string propertyName, ValidationResult<TError> validationResult)
        {
            if (validationResult.IsValid)
            {
                if (!Errors.ContainsKey(propertyName))
                    return;

                Errors.Remove(propertyName);
                Target.OnErrorsChanged(propertyName);
                return;
            }

            if (Errors.TryGetValue(propertyName, out var existingErrors))
            {
                if (existingErrors == validationResult)
                    return;

                Errors[propertyName] = validationResult;
                Target.OnErrorsChanged(propertyName);
                return;
            }

            Errors.Add(propertyName, validationResult);
            Target.OnErrorsChanged(propertyName);
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
        /// <param name="target">The object that can raise the errors-changed-mechanism.</param>
        /// <param name="errors">The dictionary used to store the validation results of erroneous properties (optional). A new dictionary will be automatically created if null is passed in.</param>
        public ValidationManager(IRaiseErrorsChanged target, Dictionary<string, ValidationResult<ValidationMessage>>? errors = null) : base(target, errors) { }

        /// <summary>
        /// Gets the value indicating whether the target entity contains errors. Only <see cref="ValidationMessage" /> instances
        /// that contain level <see cref="ValidationMessageLevel.Error" /> are considered as actual errors.
        /// </summary>
        public override bool HasErrors
        {
            get
            {
                if (Errors.Count == 0)
                    return false;

                var numberOfErrors = 0;
                foreach (var keyValuePair in Errors)
                {
                    for (var i = 0; i < keyValuePair.Value.Errors!.Count; i++)
                    {
                        if (keyValuePair.Value.Errors![i].Level == ValidationMessageLevel.Error)
                            numberOfErrors++;
                    }
                }

                return numberOfErrors > 0;
            }
        }
    }
}