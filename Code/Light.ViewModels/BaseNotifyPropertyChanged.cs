using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Light.GuardClauses;

namespace Light.ViewModels
{
    /// <summary>
    ///     Represents a base class that implements <see cref="INotifyPropertyChanged" /> and <see cref="IRaisePropertyChanged" />.
    ///     Use the extension methods of <see cref="IRaisePropertyChanged" /> or the protected method <see cref="OnPropertyChanged" />
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

        /// <summary>
        ///     Raises the <see cref="PropertyChanged" /> event when handlers are attached.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="propertyExpression">The expression of the shape "() => Property" where the property name is extracted from.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="propertyExpression" /> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="propertyExpression"/> is not of the shape "() => Property".</exception>
        protected void OnPropertyChanged<TProperty>(Expression<Func<TProperty>> propertyExpression)
        {
            var memberExpression = propertyExpression.MustNotBeNull(nameof(propertyExpression)).Body as MemberExpression;
            if (!(memberExpression?.Member is PropertyInfo propertyInfo))
                throw new ArgumentException("The specified expression is not of the shape \"() => Property\".", nameof(propertyExpression));
            OnPropertyChanged(propertyInfo.Name);
        }
    }
}