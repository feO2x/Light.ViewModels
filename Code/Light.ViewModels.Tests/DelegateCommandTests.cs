using System;
using System.Windows.Input;
using FluentAssertions;
using Xunit;

namespace Light.ViewModels.Tests
{
    public sealed class DelegateCommandTests
    {
        private static readonly Action DoNothing = () => { };

        [Fact]
        public void DelegateCommandImplementsICommand()
        {
            typeof(DelegateCommand).Should().Implement<ICommand>();
        }

        [Fact]
        public void CanExecuteReturnsTrueWhenNoFuncWasPassedIn()
        {
            var testTarget = new DelegateCommand(DoNothing);

            var result = testTarget.CanExecute();

            result.Should().BeTrue();
            testTarget.CanExecuteFunc.Should().BeNull();
        }

        [Fact]
        public void ExecuteMustCallTheAction()
        {
            var callCount = 0;
            var testTarget = new DelegateCommand(() => callCount++);

            testTarget.Execute();

            callCount.Should().Be(1);
        }

        [Fact]
        public void CanExecuteMustCallTheFunc()
        {
            var callCount = 0;
            var testTarget = new DelegateCommand(DoNothing,
                                                 () =>
                                                 {
                                                     callCount++;
                                                     return false;
                                                 });

            var result = testTarget.CanExecute();

            result.Should().BeFalse();
            callCount.Should().Be(1);
        }

        [Fact]
        public void ActionNotNull()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new DelegateCommand(null);

            act.ShouldThrow<ArgumentNullException>()
               .And.ParamName.Should().Be("execute");
        }

        [Fact]
        public void RaiseCanExecuteChangedMustRaiseEvent()
        {
            var testTarget = new DelegateCommand(DoNothing);
            var callCount = 0;
            testTarget.CanExecuteChanged += (sender, args) => callCount++;

            testTarget.RaiseCanExecuteChanged();

            callCount.Should().Be(1);
        }

        [Fact]
        public void ExecuteMustThrowIfCanExecuteReturnsFalse()
        {
            var testTarget = new DelegateCommand(DoNothing, () => false);

            Action act = () => testTarget.Execute();

            act.ShouldThrow<InvalidOperationException>()
               .And.Message.Should().Be("Execute must not be called when CanExecute returns false.");
        }
    }
}