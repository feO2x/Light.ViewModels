using System.ComponentModel;

namespace Light.ViewModels
{
    /// <summary>
    /// Represents a callback mechanism to raise change notifications on a target object.
    /// </summary>
    /// <remarks>
    /// Note to implementers: by default, we suggest that you implement this interface explicitly so that
    /// the public API of the target class is not poluted unnecessarily. The extension methods of <see cref="RaisePropertyChangedExtensions" />
    /// use the <see cref="OnPropertyChanged" /> method to callback into your object to raise the change notification mechanism
    /// (e.g. <see cref="INotifyPropertyChanged.PropertyChanged" />), but <see cref="OnPropertyChanged" /> should not be visible to your primary
    /// clients (e.g. views of your view models).
    /// </remarks>
    public interface IRaisePropertyChanged
    {
        /// <summary>
        /// Raises the change notification mechanism of the target object.
        /// </summary>
        /// <param name="propertyName">The name of the property whose value has changed.</param>
        void OnPropertyChanged(string propertyName);
    }
}