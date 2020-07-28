using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Light.GuardClauses;

namespace Light.ViewModels
{
    /// <summary>
    /// Provides extension methods for the <see cref="IRaisePropertyChanged" /> interface.
    /// Normally, these methods are called in property setters of classes implementing this
    /// interface (e.g. <c>this.SetIfDifferent(ref _myValue, value);</c>). Derive your view models
    /// from <see cref="BaseNotifyPropertyChanged" /> to implement <see cref="INotifyPropertyChanged" /> and
    /// <see cref="IRaisePropertyChanged" /> in your target class with no further code.
    /// </summary>
    public static class RaisePropertyChangedExtensions
    {
        /// <summary>
        /// Sets the given <paramref name="value" /> on the target <paramref name="field" /> and PropertyChanged.
        /// </summary>
        /// <typeparam name="T">The type of the target field and value.</typeparam>
        /// <param name="target">The target object that will raise the change notification mechanism.</param>
        /// <param name="field">The field of the target object that will be changed.</param>
        /// <param name="value">The value that will be set on <paramref name="field" />.</param>
        /// <param name="memberName">
        /// The name of the member that has changed. This value is automatically set to the name
        /// of the property or function that called this method using the <see cref="CallerMemberNameAttribute" /> - so only set this parameter explicitly
        /// if you change the value from a different member (we suggest you use the nameof operator in those scenarios).
        /// </param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="target" /> is null.</exception>
        public static void Set<T>(this IRaisePropertyChanged target, out T field, T value, [CallerMemberName] string? memberName = null)
        {
            target.MustNotBeNull(nameof(target));
            memberName.MustNotBeNull(nameof(memberName));

            field = value;
            target.OnPropertyChanged(memberName!);
        }

        /// <summary>
        /// Checks if the given <paramref name="value" /> is equal to <paramref name="field" /> using the default equality comparer.
        /// If they are not equal, <paramref name="value" /> is set on <paramref name="field" /> and PropertyChanged is raised.
        /// </summary>
        /// <typeparam name="T">The type of the target field and value.</typeparam>
        /// <param name="target">The target object that will raise the change notification mechanism.</param>
        /// <param name="field">The field of the target object that will be changed if possible.</param>
        /// <param name="value">The value that will be set on <paramref name="field" /> if possible.</param>
        /// <param name="memberName">
        /// The name of the member that has changed. This value is automatically set to the name
        /// of the property or function that called this method using the <see cref="CallerMemberNameAttribute" /> - so only set this parameter explicitly
        /// if you change the value from a different member (we suggest you use the nameof operator in those scenarios).
        /// </param>
        /// <returns>True if <paramref name="value" /> was set on <paramref name="field" /> and the change notification mechanism was raised, else false.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="target" /> is null.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetIfDifferent<T>(this IRaisePropertyChanged target, ref T field, T value, [CallerMemberName] string? memberName = null)
        {
            target.MustNotBeNull(nameof(target));
            memberName.MustNotBeNull(nameof(memberName));

            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;

            field = value;
            target.OnPropertyChanged(memberName!);
            return true;
        }

        /// <summary>
        /// Checks if the given <paramref name="value" /> is equal to <paramref name="field" /> using an equality comparer.
        /// If they are not equal, <paramref name="value" /> is set on <paramref name="field" /> and PropertyChanged is raised.
        /// </summary>
        /// <typeparam name="T">The type of the target field and value.</typeparam>
        /// <param name="target">The target object that will raise the change notification mechanism.</param>
        /// <param name="field">The field of the target object that will be changed if possible.</param>
        /// <param name="value">The value that will be set on <paramref name="field" /> if possible.</param>
        /// <param name="comparer">The equality comparer that is used to check if <paramref name="field" /> and <paramref name="value" /> are equal.</param>
        /// <param name="memberName">
        /// The name of the member that has changed. This value is automatically set to the name
        /// of the property or function that called this method using the <see cref="CallerMemberNameAttribute" /> - so only set this parameter explicitly
        /// if you change the value from a different member (we suggest you use the nameof operator in those scenarios).
        /// </param>
        /// <returns>True if <paramref name="value" /> was set on <paramref name="field" /> and the change notification mechanism was raised, else false.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="target" /> or <paramref name="comparer" /> is null.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool SetIfDifferent<T>(this IRaisePropertyChanged target, ref T field, T value, IEqualityComparer<T> comparer, [CallerMemberName] string? memberName = null)
        {
            target.MustNotBeNull(nameof(target));
            memberName.MustNotBeNull(nameof(memberName));

            if (comparer.MustNotBeNull(nameof(comparer)).Equals(field, value))
                return false;

            field = value;
            target.OnPropertyChanged(memberName!);
            return true;
        }
    }
}