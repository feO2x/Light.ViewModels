using System;
using System.Reflection;
using System.Windows.Input;
using FluentAssertions;
using Xunit;

namespace Light.ViewModels.Tests
{
    public sealed class ParameterizedCommandOfTTests
    {
        [Fact]
        public void ImplementICommand()
        {
            typeof(ParameterizedCommand<>).Should().Implement<ICommand>();
        }

        [Fact]
        public void NotSealed()
        {
            typeof(ParameterizedCommand<>).GetTypeInfo().IsSealed.Should().BeFalse();
        }

        [Fact]
        public void ExecuteCallsTheDelegate()
        {
            var receivedParameter = default(string);
            var testTarget = new ParameterizedCommand<string>(parameter => receivedParameter = parameter);

            const string givenParameter = "Foo";
            testTarget.Execute(givenParameter);

            receivedParameter.Should().BeSameAs(givenParameter);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void CanExecuteCallsTheDelegate(bool canExecuteOutcome)
        {
            var receivedParameter = default(int);
            var testTarget = new ParameterizedCommand<int>(_ => { },
                                                           parameter =>
                                                           {
                                                               receivedParameter = parameter;
                                                               return canExecuteOutcome;
                                                           });

            const int givenParameter = 42;
            var result = testTarget.CanExecute(givenParameter);

            receivedParameter.Should().Be(givenParameter);
            result.Should().Be(canExecuteOutcome);
        }

        [Fact]
        public void RaiseCanExecuteChangedRaisesTheEvent()
        {
            var testTarget = new ParameterizedCommand<object>(_ => { });
            var callCount = 0;
            testTarget.CanExecuteChanged += (s, a) => callCount++;

            testTarget.RaiseCanExecuteChanged();

            callCount.Should().Be(1);
        }

        [Fact]
        public void ActionNotNull()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new ParameterizedCommand<object>(null);

            act.Should().Throw<ArgumentNullException>()
               .And.ParamName.Should().Be("execute");
        }

        [Fact]
        public void ExecuteMustThrowWhenCanExecuteReturnsFalse()
        {
            var testTarget = new ParameterizedCommand<string>(_ => { }, _ => false);

            Action act = () => testTarget.Execute(null);

            act.Should().Throw<InvalidOperationException>()
               .And.Message.Should().Be("Execute must not be called when CanExecute returns false.");
        }

        [Fact]
        public void Inheritance()
        {
            var testTarget = new InheritanceMock(_ => { });

            testTarget.Execute(null);
            testTarget.CanExecute(null);
            testTarget.RaiseCanExecuteChanged();

            testTarget.AllOverriddenMethodsShouldHaveBeenCalled();
        }

        public sealed class InheritanceMock : ParameterizedCommand<string>
        {
            private int _canExecuteCallCount;
            private int _executeCallCount;
            private int _raiseCanExecuteChangedCallCount;

            public InheritanceMock(Action<string> execute, Func<string, bool> canExecute = null) : base(execute, canExecute) { }

            public override void Execute(string parameter)
            {
                base.Execute(parameter);
                _executeCallCount++;
            }

            public override bool CanExecute(string parameter)
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