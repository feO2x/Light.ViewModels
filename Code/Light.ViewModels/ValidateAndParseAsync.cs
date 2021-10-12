using System.Threading.Tasks;

namespace Light.ViewModels
{
    /// <summary>
    /// Represents a delegate that is used to validate and parse a value asynchronously
    /// </summary>
    /// <typeparam name="TInput">The type of the input value that should be validated.</typeparam>
    /// <typeparam name="TError">The type of the error object.</typeparam>
    /// <typeparam name="TParsed">The type that <paramref name="value" /> should be parsed to.</typeparam>
    /// <param name="value">The value to be validated and parsed.</param>
    public delegate Task<AsyncValidateAndParseResult<TError, TParsed>> ValidateAndParseAsync<in TInput, TError, TParsed>(TInput value);
}