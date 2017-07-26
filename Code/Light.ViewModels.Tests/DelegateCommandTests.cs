using System;
using System.Reflection;
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
        public void NotSealed()
        {
            typeof(DelegateCommand).GetTypeInfo().IsSealed.Should().BeFalse();
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

        [Fact]
        public void Inheritance()
        {
            var testTarget = new InheritanceMock(DoNothing);

            testTarget.Execute();
            testTarget.CanExecute();
            testTarget.RaiseCanExecuteChanged();

            testTarget.AllOverriddenMethodsShouldHaveBeenCalled();
        }

        public sealed class InheritanceMock : DelegateCommand
        {
            private int _executeCallCount;
            private int _canExecuteCallCount;
            private int _raiseCanExecuteChangedCallCount;

            public InheritanceMock(Action execute, Func<bool> canExecute = null) : base(execute, canExecute) { }

            public override void Execute()
            {
                base.Execute();
                _executeCallCount++;
            }

            public override bool CanExecute()
            {
                _canExecuteCallCount++;
                return base.CanExecute();
            }

            public override void RaiseCanExecuteChanged()
            {
                base.RaiseCanExecuteChanged();
                _raiseCanExecuteChangedCallCount++;
            }

            public void AllOverriddenMethodsShouldHaveBeenCalled()
            {
                _executeCallCount.Should().Be(1);
                _canExecuteCallCount.Should().Be(2, "because Execute calls CanExecute, too, before invoking the delegate");
                _raiseCanExecuteChangedCallCount.Should().Be(1);
            }
        }
    }
}