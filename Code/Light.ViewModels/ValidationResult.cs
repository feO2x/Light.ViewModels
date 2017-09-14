using System;
using System.Collections.Generic;
using Light.GuardClauses;
using Light.GuardClauses.Exceptions;

namespace Light.ViewModels
{
    /// <summary>
    ///     Represents a validation result indicating whether a validation process discovered errors.
    /// </summary>
    /// <typeparam name="TError">The type of the error messages.</typeparam>
    public struct ValidationResult<TError> : IEquatable<ValidationResult<TError>>
    {
        private readonly IReadOnlyList<TError> _errors;
        private readonly int _hashCode;

        /// <summary>
        ///     Creates a new instance of <see cref="ValidationResult{TError}" /> with a single error.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="singleError" /> is null.</exception>
        public ValidationResult(TError singleError)
        {
            if (singleError == null)
                throw new ArgumentNullException(nameof(singleError));

            _errors = new[] { singleError };
            _hashCode = singleError.GetHashCode();
        }

        /// <summary>
        ///     Creates a new instance of <see cref="ValidationResult{TError}" /> with a several errors.
        /// </summary>
        /// <param name="errors">The collection containing one or several errors.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="errors" /> is null.</exception>
        /// <exception cref="EmptyCollectionException">Thrown when <paramref name="errors" /> is empty.</exception>
        public ValidationResult(IReadOnlyList<TError> errors)
        {
            errors.MustNotBeNullOrEmpty(nameof(errors));
            _errors = errors;

            _hashCode = errors.Count.GetHashCode();
        }

        /// <summary>
        ///     Gets the value indicating whether this validation result is valid or not. If it is valid,
        ///     it contains no errors.
        /// </summary>
        public bool IsValid => _errors == null;

        /// <summary>
        ///     Get the list of errors. This property is null when <see cref="IsValid" /> returns true.
        /// </summary>
        public IReadOnlyList<TError> Errors => _errors;

        /// <summary>
        ///     Gets a valid instance of <see cref="ValidationResult{TError}" />.
        /// </summary>
        public static ValidationResult<TError> Valid => new ValidationResult<TError>();

        /// <summary>
        ///     Checks if the <paramref name="other" /> validation result is equal to this instance.
        ///     This is true when both are valid, or when they contain the same error messages in any order.
        /// </summary>
        public bool Equals(ValidationResult<TError> other)
        {
            if (other._errors == null)
                return _errors == null;

            if (_errors == null)
                return false;

            return _errors.IsStructurallyEquivalentTo(other._errors);
        }

        /// <summary>
        ///     Checks if the other object is equal to this validation result.
        ///     This is true when the object is a <see cref="ValidationResult{TError}" /> instance and
        ///     is valid or contains the same error messages in any order.
        /// </summary>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is ValidationResult<TError> validationResult && Equals(validationResult);
        }

        /// <summary>
        ///     Gets the hash code of this instance.
        /// </summary>
        public override int GetHashCode()
        {
            return _hashCode;
        }

        public static bool operator ==(ValidationResult<TError> x, ValidationResult<TError> y)
        {
            return x._hashCode == y._hashCode && x.Equals(y);
        }

        public static bool operator !=(ValidationResult<TError> x, ValidationResult<TError> y)
        {
            return !(x._hashCode == y._hashCode && x.Equals(y));
        }

        public static implicit operator ValidationResult<TError>(TError error)
        {
            return new ValidationResult<TError>(error);
        }
    }
}