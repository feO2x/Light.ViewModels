namespace Light.ViewModels
{
    /// <summary>
    /// Represents a callback mechanism to raise error notifications on a target object.
    /// </summary>
    /// <remarks>
    /// Note to implementers: by default, we suggest that you implement this interface explicitly so that
    /// the public API of the target class is not poluted unnecessarily. The <see cref="ValidationManager{TError}" /> class
    /// uses this interface to call back into your target entity when the errors for a certain property have changed, but
    /// <see cref="OnErrorsChanged" /> should not be visible to your primary clients (e.g. views of your view model).
    /// </remarks>
    public interface IRaiseErrorsChanged
    {
        /// <summary>
        /// Raises the error notification mechanism of the target object.
        /// </summary>
        /// <param name="propertyName">The name of the property whose errors have changed.</param>
        void OnErrorsChanged(string propertyName);
    }
}