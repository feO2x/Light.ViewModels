using System;
using System.Collections.Generic;
using Light.GuardClauses;
using Light.GuardClauses.Exceptions;

namespace Light.ViewModels
{
    /// <summary>
    /// Represents a validation result indicating whether a validation process discovered errors.
    /// </summary>
    /// <typeparam name="TError">The type of the error messages.</typeparam>
    public struct ValidationResult<TError> : IEquatable<ValidationResult<TError>>
    {
        private readonly int _hashCode;
        private readonly List<TError> _errors;

        /// <summary>
        /// Creates a new instance of <see cref="ValidationResult{TError}" /> with a single error.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="singleError" /> is null.</exception>
        public ValidationResult(TError singleError)
        {
            singleError.MustNotBeNullReference(nameof(singleError));
            _errors = new List<TError>(1) { singleError };
            _hashCode = singleError.GetHashCode();
        }

        /// <summary>
        /// Creates a new instance of <see cref="ValidationResult{TError}" /> with a several errors.
        /// </summary>
        /// <param name="errors">The collection containing one or several errors.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="errors" /> is null.</exception>
        /// <exception cref="EmptyCollectionException">Thrown when <paramref name="errors" /> is empty.</exception>
        public ValidationResult(List<TError> errors)
        {
            _errors = errors.MustNotBeNullOrEmpty(nameof(errors));
            _hashCode = errors.Count.GetHashCode();
        }

        /// <summary>
        /// Gets the value indicating whether this validation result is valid or not. If it is valid,
        /// it contains no errors.
        /// </summary>
        public bool IsValid => _errors == null;

        /// <summary>
        /// Get the list of errors. This property is null when <see cref="IsValid" /> returns true.
        /// </summary>
        public IReadOnlyList<TError> Errors => _errors;

        /// <summary>
        /// Gets a valid instance of <see cref="ValidationResult{TError}" />.
        /// </summary>
        public static ValidationResult<TError> Valid => new ValidationResult<TError>();

        /// <summary>
        /// Checks if the <paramref name="other" /> validation result is equal to this instance.
        /// This is true when both are valid, or when they contain the same error messages in any order.
        /// </summary>
        public bool Equals(ValidationResult<TError> other)
        {
            if (other._errors == null)
                return _errors == null;

            if (_errors == null)
                return false;

            if (other._errors.Count != _errors.Count)
                return false;

            foreach (var error in _errors)
            {
                if (!other._errors.Contains(error))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Checks if the other object is equal to this validation result.
        /// This is true when the object is a <see cref="ValidationResult{TError}" /> instance and
        /// is valid or contains the same error messages in any order.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ValidationResult<TError> validationResult && Equals(validationResult);
        }

        /// <summary>
        /// Gets the hash code of this instance.
        /// </summary>
        public override int GetHashCode() => _hashCode;

        /// <summary>
        /// Checks if the two validation results are equal.
        /// </summary>
        public static bool operator ==(ValidationResult<TError> x, ValidationResult<TError> y) => x._hashCode == y._hashCode && x.Equals(y);

        /// <summary>
        /// Checks if the two validation results are not equal.
        /// </summary>
        public static bool operator !=(ValidationResult<TError> x, ValidationResult<TError> y) => !(x._hashCode == y._hashCode && x.Equals(y));

        /// <summary>
        /// Implicitely converts the specified error instance to a <see cref="ValidationResult{TError}" /> instance.
        /// </summary>
        public static implicit operator ValidationResult<TError>(TError error) => new ValidationResult<TError>(error);
    }
}