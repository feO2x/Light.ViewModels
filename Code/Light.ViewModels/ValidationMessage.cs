using System;
using Light.GuardClauses;
using Light.GuardClauses.Exceptions;
using Light.GuardClauses.FrameworkExtensions;

namespace Light.ViewModels
{
    /// <summary>
    /// Represents a validation object that contains a message and a level.
    /// </summary>
    public class ValidationMessage : IEquatable<ValidationMessage>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ValidationMessage" /> with a message and a level.
        /// </summary>
        /// <param name="message">The message that describes the validation error.</param>
        /// <param name="level">The level that is associated with the validation message.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="message" /> or <paramref name="level" /> is null.</exception>
        /// <exception cref="EmptyStringException">Thrown when <paramref name="message" /> or <paramref name="level" /> are empty.</exception>
        /// <exception cref="WhiteSpaceStringException">Thrown when <paramref name="message" /> or <paramref name="level" /> contains only white space.</exception>
        public ValidationMessage(string message, string level = ValidationMessageLevel.Error)
        {
            Message = message.MustNotBeNullOrWhiteSpace(nameof(message));
            Level = level.MustNotBeNullOrWhiteSpace(nameof(level));
        }

        /// <summary>
        /// Gets the validation message.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets the level associated with the validation message.
        /// </summary>
        public string Level { get; }

        /// <summary>
        /// Checks if the other validation message is equal to this instance.
        /// This is true when the other instance points to the same reference as this one
        /// or if both their <see cref="Message" /> and <see cref="Level" /> values are equal.
        /// </summary>
        public bool Equals(ValidationMessage? other)
        {
            if (ReferenceEquals(other, this)) return true;
            if (other is null) return false;

            return Message == other.Message &&
                   Level == other.Level;
        }

        /// <summary>
        /// Checks if the other object is equal to this instance.
        /// This is true when the other instance is a <see cref="ValidationMessage" />, too, and if
        /// both their <see cref="Message" /> and <see cref="Level" /> values are equal.
        /// </summary>
        public override bool Equals(object obj) => Equals(obj as ValidationMessage);

        /// <summary>
        /// Gets the hash code of this validation message.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => MultiplyAddHash.CreateHashCode(Message, Level);

        /// <summary>
        /// Checks if the two validation messages are equal.
        /// </summary>
        public static bool operator ==(ValidationMessage x, ValidationMessage y) => x?.Equals(y) ?? y is null;

        /// <summary>
        /// Checks if the two validation message are not equal.
        /// </summary>
        public static bool operator !=(ValidationMessage x, ValidationMessage y) => !(x == y);

        /// <summary>
        /// Returns the message this instance.
        /// </summary>
        public override string ToString() => Message;
    }
}