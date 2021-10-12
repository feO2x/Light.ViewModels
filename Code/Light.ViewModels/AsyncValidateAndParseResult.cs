using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Light.GuardClauses;

namespace Light.ViewModels
{
    /// <summary>
    /// Represents an asynchronous validation result which consists of a
    /// parsed value and a validation result.
    /// </summary>
    /// <typeparam name="TError">The type that is used to indicate error messages.</typeparam>
    /// <typeparam name="TParsedValue">The potentially parsed value.</typeparam>
    public readonly struct AsyncValidateAndParseResult<TError, TParsedValue> : IEquatable<AsyncValidateAndParseResult<TError, TParsedValue>>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="AsyncValidateAndParseResult{TError,TParsedValue}" /> with an error.
        /// </summary>
        /// <param name="errorResult">The errors that occurred during validation.</param>
        public AsyncValidateAndParseResult(ValidationResult<TError> errorResult)
        {
            errorResult.IsValid.MustBe(false, nameof(errorResult));
            ValidationResult = errorResult;
            ParsedValue = default;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="AsyncValidateAndParseResult{TError,TParsedValue}" /> with a validly parsed value.
        /// </summary>
        /// <param name="parsedValue">The value that was parsed successfully.</param>
        public AsyncValidateAndParseResult(TParsedValue parsedValue)
        {
            ParsedValue = parsedValue;
            ValidationResult = ValidationResult<TError>.Valid;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="AsyncValidateAndParseResult{TError,TParsedValue}" /> with the specified values.
        /// </summary>
        /// <param name="validationResult">The validation result.</param>
        /// <param name="parsedValue">The value that has been parsed.</param>
        public AsyncValidateAndParseResult(ValidationResult<TError> validationResult, TParsedValue parsedValue)
        {
            ValidationResult = validationResult;
            ParsedValue = parsedValue;
        }

        /// <summary>
        /// Gets the validation result.
        /// </summary>
        public ValidationResult<TError> ValidationResult { get; }

        /// <summary>
        /// Gets the value that has been parsed.
        /// </summary>
        public TParsedValue? ParsedValue { get; }

        /// <summary>
        /// Tries to get the parsed value. This method will succeed when the specified value is not null.
        /// </summary>
        /// <param name="parsedValue">The value that was parsed.</param>
        /// <returns>True if the value is present, else false.</returns>
        public bool TryGetParsedValue([NotNullWhen(true)] out TParsedValue? parsedValue)
        {
            if (ParsedValue != null)
            {
                parsedValue = ParsedValue;
                return true;
            }

            parsedValue = default;
            return false;
        }

        /// <inheritdoc />
        public bool Equals(AsyncValidateAndParseResult<TError, TParsedValue> other) =>
            ValidationResult.Equals(other.ValidationResult) && EqualityComparer<TParsedValue?>.Default.Equals(ParsedValue, other.ParsedValue);

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is AsyncValidateAndParseResult<TError, TParsedValue> other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return (ValidationResult.GetHashCode() * 397) ^ EqualityComparer<TParsedValue?>.Default.GetHashCode(ParsedValue);
            }
        }

        /// <summary>
        /// Equality operator
        /// </summary>
        public static bool operator ==(AsyncValidateAndParseResult<TError, TParsedValue> x, AsyncValidateAndParseResult<TError, TParsedValue> y) => x.Equals(y);

        /// <summary>
        /// Inequality operator
        /// </summary>
        public static bool operator !=(AsyncValidateAndParseResult<TError, TParsedValue> x, AsyncValidateAndParseResult<TError, TParsedValue> y) => !x.Equals(y);
    }
}