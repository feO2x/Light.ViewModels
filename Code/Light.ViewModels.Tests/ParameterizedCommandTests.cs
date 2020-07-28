using System;
using System.Reflection;
using System.Windows.Input;
using FluentAssertions;
using Xunit;

namespace Light.ViewModels.Tests
{
    public sealed class ParameterizedCommandTests
    {
        private static readonly Action<object> DoNothing = _ => { };

        [Fact]
        public void ImplementICommand()
        {
            typeof(ParameterizedCommand).Should().Implement<ICommand>();
        }

        [Fact]
        public void NotSealed()
        {
            typeof(ParameterizedCommand).GetTypeInfo().IsSealed.Should().BeFalse();
        }

        [Fact]
        public void ExecuteCallsTheDelegate()
        {
            var receivedParameter = default(object);
            var testTarget = new ParameterizedCommand(parameter => receivedParameter = parameter);

            var givenParameter = new object();
            testTarget.Execute(givenParameter);

            receivedParameter.Should().BeSameAs(givenParameter);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CanExecuteCallsTheDelegate(bool canExecuteOutcome)
        {
            var receivedParameter = default(object);
            var testTarget = new ParameterizedCommand(DoNothing,
                                                      parameter =>
                                                      {
                                                          receivedParameter = parameter;
                                                          return canExecuteOutcome;
                                                      });

            var givenParameter = new object();
            var result = testTarget.CanExecute(givenParameter);

            receivedParameter.Should().BeSameAs(givenParameter);
            result.Should().Be(canExecuteOutcome);
        }

        [Fact]
        public void RaiseCanExecuteChangedRaisesTheEvent()
        {
            var testTarget = new ParameterizedCommand(DoNothing);
            var callCount = 0;
            testTarget.CanExecuteChanged += (s, a) => callCount++;

            testTarget.RaiseCanExecuteChanged();

            callCount.Should().Be(1);
        }

        [Fact]
        public void ActionNotNull()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new ParameterizedCommand(null);

            act.Should().Throw<ArgumentNullException>()
               .And.ParamName.Should().Be("execute");
        }

        [Fact]
        public void ExecuteMustThrowWhenCanExecuteReturnsFalse()
        {
            var testTarget = new ParameterizedCommand(DoNothing, _ => false);

            Action act = () => testTarget.Execute(null);

            act.Should().Throw<InvalidOperationException>()
               .And.Message.Should().Be("Execute must not be called when CanExecute returns false.");
        }

        [Fact]
        public void Inheritance()
        {
            var testTarget = new InheritanceMock(DoNothing);

            testTarget.Execute(null);
            testTarget.CanExecute(null);
            testTarget.RaiseCanExecuteChanged();

            testTarget.AllOverriddenMethodsShouldHaveBeenCalled();
        }

        public sealed class InheritanceMock : ParameterizedCommand
        {
            private int _canExecuteCallCount;
            private int _executeCallCount;
            private int _raiseCanExecuteChangedCallCount;

            public InheritanceMock(Action<object> execute, Func<object, bool> canExecute = null) : base(execute, canExecute) { }

            public override void Execute(object parameter)
            {
                base.Execute(parameter);
                _executeCallCount++;
            }

            public override bool CanExecute(object parameter)
            {
                _canExecuteCallCount++;
                return base.CanExecute(parameter);
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