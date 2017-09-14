using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Light.GuardClauses;

namespace Light.ViewModels
{
    public class ValidationManager<TError>
    {
        protected readonly Dictionary<string, ValidationResult<TError>> Errors;

        public ValidationManager() : this(new Dictionary<string, ValidationResult<TError>>()) { }

        public ValidationManager(Dictionary<string, ValidationResult<TError>> errors)
        {
            Errors = errors.MustNotBeNull(nameof(errors));
        }

        public virtual bool HasErrors => Errors.Count > 0;


        public IEnumerable GetErrors(string propertyName)
        {
            return Errors.TryGetValue(propertyName, out var validationResult) ? validationResult.Errors : null;
        }

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

    public class ValidationManager : ValidationManager<ValidationMessage>
    {
        public ValidationManager() { }

        public ValidationManager(Dictionary<string, ValidationResult<ValidationMessage>> errors) : base(errors) { }

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