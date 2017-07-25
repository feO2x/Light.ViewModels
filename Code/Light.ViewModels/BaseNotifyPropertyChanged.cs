using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Light.ViewModels
{
    /// <summary>
    ///     Represents a base class that implements <see cref="INotifyPropertyChanged" /> and <see cref="IRaiseChangeNotification" />.
    ///     Use the extension methods of <see cref="ChangeNotificationExtensions" /> or the protected method <see cref="OnPropertyChanged" />
    ///     to easily raise property change notifications.
    /// </summary>
    public abstract class BaseNotifyPropertyChanged : INotifyPropertyChanged, IRaisePropertyChanged
    {
        /// <summary>
        ///     Represents the event that propagates property change notifications.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        void IRaisePropertyChanged.OnPropertyChanged(string memberName)
        {
            // ReSharper disable once ExplicitCallerInfoArgument
            OnPropertyChanged(memberName);
        }

        /// <summary>
        ///     Raises the <see cref="PropertyChanged" /> event when handlers are attached.
        /// </summary>
        /// <param name="memberName">
        ///     The name of the property that has changed. This value is automatically set to the name
        ///     of the property that called this method using the <see cref="CallerMemberNameAttribute" /> - so only set this parameter explicitly
        ///     if you change the value from a different member (we suggest you use the nameof operator in those scenarios).
        /// </param>
        protected virtual void OnPropertyChanged([CallerMemberName] string memberName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));
        }
    }
}
