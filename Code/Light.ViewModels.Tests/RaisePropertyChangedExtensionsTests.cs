using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Light.ViewModels.Tests
{
    public sealed class RaisePropertyChangedExtensionsTests : BaseNotifyPropertyChanged
    {
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public void SetWithDifferentValue(int newValue)
        {
            var propertyChangedStub = new PropertyChangedStub();
            propertyChangedStub.MonitorEvents();

            propertyChangedStub.IntegerValue = newValue;

            propertyChangedStub.IntegerValue.Should().Be(newValue);
            propertyChangedStub.ShouldRaisePropertyChangeFor(s => s.IntegerValue);
        }

        [Theory]
        [InlineData("Foo")]
        [InlineData("Bar")]
        public void SetIfDifferent(string newValue)
        {
            var propertyChangedStub = new PropertyChangedStub();
            propertyChangedStub.TextValue.Should().NotBe(newValue);
            propertyChangedStub.MonitorEvents();

            propertyChangedStub.TextValue = newValue;

            propertyChangedStub.TextValue.Should().Be(newValue);
            propertyChangedStub.ShouldRaisePropertyChangeFor(s => s.TextValue);
        }

        [Theory]
        [InlineData("Baz")]
        [InlineData("Qux")]
        public void NoNotificationWhenValueIsSame(string value)
        {
            var propertyChangedStub = new PropertyChangedStub { TextValue = value };
            propertyChangedStub.MonitorEvents();

            propertyChangedStub.TextValue = value;

            propertyChangedStub.ShouldNotRaisePropertyChangeFor(s => s.TextValue);
        }

        [Fact]
        public void SetDifferentWithEqualityComparer()
        {
            var equalityComparer = new EqualityComparerMock();
            var propertyChangedStub = new PropertyChangedStub(equalityComparer)
                                      {
                                          IntegerValue2 = 42
                                      };

            propertyChangedStub.IntegerValue2 = 42;

            propertyChangedStub.IntegerValue2.Should().Be(42);
            equalityComparer.MustHaveBeenCalled();
        }

        [Theory]
        [InlineData("Foo", "Foo", false)]
        [InlineData("Foo", "Bar", true)]
        public void SetIfDifferentReturnValue(string oldValue, string newValue, bool expected)
        {
            var fieldStub = oldValue;

            var result = this.SetIfDifferent(ref fieldStub, newValue);

            result.Should().Be(expected);
        }

        public class PropertyChangedStub : BaseNotifyPropertyChanged
        {
            private readonly IEqualityComparer<int> _comparerForIntegerValue2;
            private int _integerValue;
            private int _integerValue2;
            private string _textValue;

            public PropertyChangedStub() { }

            public PropertyChangedStub(IEqualityComparer<int> comparerForIntegerValue2)
            {
                _comparerForIntegerValue2 = comparerForIntegerValue2;
            }

            public int IntegerValue
            {
                get => _integerValue;
                set => this.Set(out _integerValue, value);
            }

            public string TextValue
            {
                get => _textValue;
                set => this.SetIfDifferent(ref _textValue, value);
            }

            public int IntegerValue2
            {
                get => _integerValue2;
                set => this.SetIfDifferent(ref _integerValue2, value, _comparerForIntegerValue2);
            }
        }

        public sealed class EqualityComparerMock : IEqualityComparer<int>
        {
            private int _equalsCallCount;

            public bool Equals(int x, int y)
            {
                ++_equalsCallCount;
                return x == y;
            }

            public int GetHashCode(int obj) => obj;

            public void MustHaveBeenCalled() => _equalsCallCount.Should().BeGreaterThan(0);
        }
    }
}