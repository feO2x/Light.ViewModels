using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Light.GuardClauses;

namespace Light.ViewModels
{
    public class ValidationManager<TError>
    {
        private readonly Dictionary<string, ValidationResult<TError>> _errors;

        public ValidationManager() : this(new Dictionary<string, ValidationResult<TError>>()) { }

        public ValidationManager(Dictionary<string, ValidationResult<TError>> errors)
        {
            _errors = errors.MustNotBeNull(nameof(errors));
        }

        public virtual bool HasErrors => _errors.Count > 0;


        public IEnumerable GetErrors(string propertyName)
        {
            return _errors.TryGetValue(propertyName, out var validationResult) ? validationResult.Errors : null;
        }

        public ValidationResult<TError> Validate<T>(T value, Func<T, ValidationResult<TError>> validate, IRaiseErrorsChanged raiseErrorsChanged, [CallerMemberName] string propertyName = null)
        {
            validate.MustNotBeNull(nameof(validate));
            raiseErrorsChanged.MustNotBeNull(nameof(raiseErrorsChanged));
            propertyName.MustNotBeNull(nameof(propertyName));

            var validationResult = validate(value);
            if (validationResult.IsValid)
            {
                if (_errors.ContainsKey(propertyName) == false)
                    return validationResult;

                _errors.Remove(propertyName);
                raiseErrorsChanged.OnErrorsChanged(propertyName);
                return validationResult;
            }

            if (_errors.TryGetValue(propertyName, out var existingErrors))
            {
                if (existingErrors == validationResult)
                    return validationResult;

                _errors[propertyName] = validationResult;
                raiseErrorsChanged.OnErrorsChanged(propertyName);
                return validationResult;
            }

            _errors.Add(propertyName, validationResult);
            raiseErrorsChanged.OnErrorsChanged(propertyName);
            return validationResult;
        }
    }
}