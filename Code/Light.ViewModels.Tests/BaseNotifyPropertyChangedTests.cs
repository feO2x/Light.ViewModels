using System.ComponentModel;
using System.Reflection;
using FluentAssertions;
using Xunit;

namespace Light.ViewModels.Tests
{
    public sealed class BaseNotifyPropertyChangedTests
    {
        [Fact]
        public void ImplementsIRaiseChangeNotification()
        {
            typeof(BaseNotifyPropertyChanged).Should().Implement<IRaisePropertyChanged>();
        }

        [Fact]
        public void IsAbstractBaseClass()
        {
            typeof(BaseNotifyPropertyChanged).GetTypeInfo().IsAbstract.Should().BeTrue();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(42)]
        [InlineData(-100)]
        public void NotificationForOnPropertyChanged(int newValue)
        {
            var propertyChangedStub = new PropertyChangedStub();
            propertyChangedStub.MonitorEvents();

            propertyChangedStub.IntegerValue = newValue;

            propertyChangedStub.ShouldRaisePropertyChangeFor(s => s.IntegerValue);
        }

        [Fact]
        public void ImplementsINotifyPropertyChanged()
        {
            typeof(BaseNotifyPropertyChanged).Should().Implement<INotifyPropertyChanged>();
        }

        [Fact]
        public void PropertyChangedViaExpression()
        {
            var propertyChangedStub = new PropertyChangedStub();
            propertyChangedStub.MonitorEvents();

            propertyChangedStub.Increment();

            propertyChangedStub.ShouldRaisePropertyChangeFor(s => s.IntegerValue);
        }

        public class PropertyChangedStub : BaseNotifyPropertyChanged
        {
            private int _integerValue;

            public int IntegerValue
            {
                get => _integerValue;
                set
                {
                    _integerValue = value;
                    OnPropertyChanged();
                }
            }

            public void Increment()
            {
                _integerValue++;
                OnPropertyChanged(() => IntegerValue);
            }
        }
    }
}