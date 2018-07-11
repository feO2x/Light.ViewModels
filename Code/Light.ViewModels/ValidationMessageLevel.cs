namespace Light.ViewModels
{
    /// <summary>
    /// Provides predefined levels for <see cref="ValidationMessage" />.
    /// You can easily derive from this class to add your own constants.
    /// </summary>
    public abstract class ValidationMessageLevel
    {
        /// <summary>
        /// Represents a warning validation message.
        /// </summary>
        public const string Warning = "Warning";

        /// <summary>
        /// Represents an error validation message.
        /// </summary>
        public const string Error = "Error";
    }
}