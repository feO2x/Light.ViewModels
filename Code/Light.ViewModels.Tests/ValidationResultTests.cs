using System;
using System.Linq;
using FluentAssertions;
using Light.GuardClauses.Exceptions;
using Xunit;

namespace Light.ViewModels.Tests
{
    public sealed class ValidationResultTests
    {
        [Fact]
        public void SingleError()
        {
            var validationResult = new ValidationResult<string>("Foo");

            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Should().HaveCount(1).And.Subject.First().Should().Be("Foo");
        }

        [Fact]
        public void MultipleErrors()
        {
            var errors = new[] { 1, 2, 3 };

            var validationResult = new ValidationResult<int>(errors);

            validationResult.IsValid.Should().BeFalse();
            validationResult.Errors.Should().BeSameAs(errors);
        }

        [Fact]
        public void Valid()
        {
            var validationResult = ValidationResult<string>.Valid;

            validationResult.IsValid.Should().BeTrue();
            validationResult.Errors.Should().BeNull();
        }

        [Fact]
        public void EqualityOnValid()
        {
            var first = new ValidationResult<int>();
            var second = new ValidationResult<int>();

            var equalityResult = first == second;
            var inequalityResult = first != second;

            equalityResult.Should().BeTrue();
            inequalityResult.Should().BeFalse();
        }

        [Theory]
        [InlineData(new[] { "Foo", "Bar" }, new[] { "Foo", "Bar" }, true)]
        [InlineData(new[] { "Foo", "Bar" }, new[] { "Bar", "Foo" }, true)]
        [InlineData(new[] { "Foo", "Bar" }, new[] { "Bar", "Baz" }, false)]
        [InlineData(new[] { "Foo", "Bar" }, new[] { "Qux" }, false)]
        public void EqualityOnError(string[] firstErrors, string[] secondErrors, bool expected)
        {
            var first = new ValidationResult<string>(firstErrors);
            var second = new ValidationResult<string>(secondErrors);

            var equalityResult = first == second;
            var inequalityResult = first != second;

            equalityResult.Should().Be(expected);
            inequalityResult.Should().Be(!expected);
        }

        [Fact]
        public void ImplicitConversion()
        {
            ValidationResult<string> result = "Foo";

            result.Errors.Should().HaveCount(1);
            result.Errors[0].Should().Be("Foo");
        }

        [Fact]
        public void ErrorsMustNotContainNull()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new ValidationResult<string>(new[] { "Foo", null });

            act.ShouldThrow<CollectionException>()
               .And.Message.Should().Contain("The specified errors collection must not contain null.");
        }
    }
}