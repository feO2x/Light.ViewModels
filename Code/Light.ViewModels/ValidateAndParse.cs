namespace Light.ViewModels
{
    /// <summary>
    /// Represents a delegate that is used to validate and parse a value.
    /// </summary>
    /// <typeparam name="TInput">The type of the input value that should be validated.</typeparam>
    /// <typeparam name="TError">The type of the error object.</typeparam>
    /// <typeparam name="TParsed">The type that <paramref name="value"/> should be parsed to.</typeparam>
    /// <param name="value">The value to be valideted and parsed.</param>
    /// <param name="parsedValue">The value that was parsed from the input value.</param>
    /// <returns>The result indicating whether the validation and parsing process was successful.</returns>
    public delegate ValidationResult<TError> ValidateAndParse<in TInput, TError, TParsed>(TInput value, out TParsed parsedValue);
}