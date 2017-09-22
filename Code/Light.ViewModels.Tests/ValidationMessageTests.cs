using FluentAssertions;
using Xunit;

namespace Light.ViewModels.Tests
{
    public sealed class ValidationMessageTests
    {
        [Fact]
        public void RetrieveMessageAndLevel()
        {
            var validationMessage = new ValidationMessage("Foo", ValidationMessageLevel.Warning);

            validationMessage.Message.Should().Be("Foo");
            validationMessage.Level.Should().Be(ValidationMessageLevel.Warning);
        }

        [Fact]
        public void DefaultLevelIsError()
        {
            var validationMessage = new ValidationMessage("Foo");

            validationMessage.Level.Should().Be(ValidationMessageLevel.Error);
        }

        [Theory]
        [InlineData("Foo", ValidationMessageLevel.Error, "Foo", ValidationMessageLevel.Error, true)]
        [InlineData("Foo", ValidationMessageLevel.Error, "Bar", ValidationMessageLevel.Error, false)]
        [InlineData("Foo", ValidationMessageLevel.Error, "Foo", ValidationMessageLevel.Warning, false)]
        [InlineData("Foo", ValidationMessageLevel.Error, "Baz", ValidationMessageLevel.Warning, false)]
        public void Equality(string firstMessage, string firstLevel, string secondMessage, string secondLevel, bool expected)
        {
            var first = new ValidationMessage(firstMessage, firstLevel);
            var second = new ValidationMessage(secondMessage, secondLevel);

            var equalityResult = first == second;

            equalityResult.Should().Be(expected);
            if (expected)
                first.GetHashCode().Should().Be(second.GetHashCode());
        }

        [Theory]
        [InlineData("Foo")]
        [InlineData("Bar")]
        [InlineData("Baz")]
        public void ToStringReturnsMessage(string message)
        {
            var validationMessage = new ValidationMessage(message);

            var result = validationMessage.ToString();

            result.Should().Be(message);
        }
    }
}