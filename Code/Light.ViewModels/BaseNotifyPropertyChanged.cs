using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using Light.GuardClauses;

namespace Light.ViewModels
{
    /// <summary>
    /// Represents a base class that implements <see cref="INotifyPropertyChanged" /> and <see cref="IRaisePropertyChanged" />.
    /// Use the extension methods of <see cref="IRaisePropertyChanged" /> or the protected method <see cref="OnPropertyChanged" />
    /// to easily raise property change notifications.
    /// </summary>
    public abstract class BaseNotifyPropertyChanged : INotifyPropertyChanged, IRaisePropertyChanged
    {
        /// <summary>
        /// Represents the event that propagates property change notifications.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        void IRaisePropertyChanged.OnPropertyChanged(string memberName) => OnPropertyChanged(memberName);

        /// <summary>
        /// Sets the given <paramref name="value" /> on the target <paramref name="field" /> and raises the change notification mechanism.
        /// </summary>
        /// <typeparam name="T">The type of the target field and value.</typeparam>
        /// <param name="field">The field of the target object that will be changed.</param>
        /// <param name="value">The value that will be set on <paramref name="field" />.</param>
        /// <param name="memberName">
        /// The name of the member that has changed. This value is automatically set to the name
        /// of the property or function that called this method using the <see cref="CallerMemberNameAttribute" /> - so you only have to set this parameter explicitly
        /// if you change the value from a different member (we suggest you use the nameof operator in those scenarios).
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void Set<T>(out T field, T value, [CallerMemberName] string? memberName = null)
        {
            field = value;
            OnPropertyChanged(memberName);
        }

        /// <summary>
        /// Checks if the given <paramref name="value" /> is equal to <paramref name="field" /> using the default equality comparer.
        /// If they are not equal, <paramref name="value" /> is set on <paramref name="field" /> and PropertyChanged is raised.
        /// </summary>
        /// <typeparam name="T">The type of the target field and value.</typeparam>
        /// <param name="field">The field of the target object that will be changed if possible.</param>
        /// <param name="value">The value that will be set on <paramref name="field" /> if possible.</param>
        /// <param name="memberName">
        /// The name of the member that has changed. This value is automatically set to the name
        /// of the property or function that called this method using the <see cref="CallerMemberNameAttribute" /> - so only set this parameter explicitly
        /// if you change the value from a different member (we suggest you use the nameof operator in those scenarios).
        /// </param>
        /// <returns>True if <paramref name="value" /> was set on <paramref name="field" /> and the change notification mechanism was raised, else false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool SetIfDifferent<T>(ref T field, T value, [CallerMemberName] string? memberName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(memberName);
            return true;
        }

        /// <summary>
        /// Checks if the given <paramref name="value" /> is equal to <paramref name="field" /> using an equality comparer.
        /// If they are not equal, <paramref name="value" /> is set on <paramref name="field" /> and PropertyChanged is raised.
        /// </summary>
        /// <typeparam name="T">The type of the target field and value.</typeparam>
        /// <param name="field">The field of the target object that will be changed if possible.</param>
        /// <param name="value">The value that will be set on <paramref name="field" /> if possible.</param>
        /// <param name="comparer">The equality comparer that is used to check if <paramref name="field" /> and <paramref name="value" /> are equal.</param>
        /// <param name="memberName">
        /// The name of the member that has changed. This value is automatically set to the name
        /// of the property or function that called this method using the <see cref="CallerMemberNameAttribute" /> - so only set this parameter explicitly
        /// if you change the value from a different member (we suggest you use the nameof operator in those scenarios).
        /// </param>
        /// <returns>True if <paramref name="value" /> was set on <paramref name="field" /> and the change notification mechanism was raised, else false.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="comparer" /> is null.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool SetIfDifferent<T>(ref T field, T value, IEqualityComparer<T> comparer, [CallerMemberName] string? memberName = null)
        {
            comparer.MustNotBeNull(nameof(comparer));

            if (comparer.Equals(field, value))
                return false;

            field = value;
            OnPropertyChanged(memberName);
            return true;
        }

        /// <summary>
        /// Raises the <see cref="PropertyChanged" /> event when handlers are attached.
        /// </summary>
        /// <param name="memberName">
        /// The name of the property that has changed. This value is automatically set to the name
        /// of the property that called this method using the <see cref="CallerMemberNameAttribute" /> - so only set this parameter explicitly
        /// if you change the value from a different member (we suggest you use the nameof operator in those scenarios).
        /// </param>
        protected void OnPropertyChanged([CallerMemberName] string? memberName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(memberName));

        /// <summary>
        /// Raises the <see cref="PropertyChanged" /> event when handlers are attached.
        /// </summary>
        /// <typeparam name="TProperty">The type of the property.</typeparam>
        /// <param name="propertyExpression">The expression of the shape "() => Property" where the property name is extracted from.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="propertyExpression" /> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="propertyExpression" /> is not of the shape "() => Property".</exception>
        protected void OnPropertyChanged<TProperty>(Expression<Func<TProperty>> propertyExpression)
        {
            var memberExpression = propertyExpression.MustNotBeNull(nameof(propertyExpression)).Body as MemberExpression;
            if (!(memberExpression?.Member is PropertyInfo propertyInfo))
                throw new ArgumentException("The specified expression is not of the shape \"() => Property\".", nameof(propertyExpression));
            OnPropertyChanged(propertyInfo.Name);
        }
    }
}