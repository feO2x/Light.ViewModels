using System;
using Light.GuardClauses;
using Light.GuardClauses.Exceptions;
using Light.GuardClauses.FrameworkExtensions;

namespace Light.ViewModels
{
    /// <summary>
    ///     Represents a validation object that contains a message and a level.
    /// </summary>
    public class ValidationMessage : IEquatable<ValidationMessage>
    {
        private readonly string _level;
        private readonly string _message;

        /// <summary>
        ///     Initializes a new instance of <see cref="ValidationMessage" /> with a message and a level.
        /// </summary>
        /// <param name="message">The message that describes the validation error.</param>
        /// <param name="level">The level that is associated with the validation message.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="message" /> or <paramref name="level" /> is null.</exception>
        /// <exception cref="EmptyStringException">Thrown when <paramref name="message" /> or <paramref name="level" /> are empty.</exception>
        /// <exception cref="StringIsOnlyWhiteSpaceException">Thrown when <paramref name="message" /> or <paramref name="level" /> contains only white space.</exception>
        public ValidationMessage(string message, string level = ValidationMessageLevel.Error)
        {
            _message = message.MustNotBeNullOrWhiteSpace(nameof(message));
            _level = level.MustNotBeNullOrWhiteSpace(nameof(level));
        }

        /// <summary>
        ///     Gets the validation message.
        /// </summary>
        public string Message => _message;

        /// <summary>
        ///     Gets the level associated with the validation message.
        /// </summary>
        public string Level => _level;

        /// <summary>
        ///     Checks if the other validation message is equal to this instance.
        ///     This is true when the other instance points to the same reference as this one
        ///     or if both their <see cref="Message" /> and <see cref="Level" /> values are equal.
        /// </summary>
        public bool Equals(ValidationMessage other)
        {
            if (ReferenceEquals(other, this)) return true;
            if (ReferenceEquals(other, null)) return false;

            return _message == other._message &&
                   _level == other._level;
        }

        /// <summary>
        ///     Checks if the other object is equal to this instance.
        ///     This is true when the other instance is a <see cref="ValidationMessage" />, too, and if
        ///     both their <see cref="Message" /> and <see cref="Level" /> values are equal.
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as ValidationMessage);
        }

        /// <summary>
        ///     Gets the hash code of this validation message.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Equality.CreateHashCode(_message, _level);
        }

        /// <summary>
        ///     Checks if the two validation messages are equal.
        /// </summary>
        public static bool operator ==(ValidationMessage x, ValidationMessage y)
        {
            return ReferenceEquals(x, null) ? ReferenceEquals(y, null) : x.Equals(y);
        }

        /// <summary>
        ///     Checks if the two validation message are not equal.
        /// </summary>
        public static bool operator !=(ValidationMessage x, ValidationMessage y)
        {
            return !(x == y);
        }
    }
}