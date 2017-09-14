using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace Light.ViewModels.Tests
{
    public sealed class ValidationManagerTests
    {
        [Fact]
        public void NoErrorAfterInstantiation()
        {
            var testTarget = new ValidationManager<string>();

            testTarget.HasErrors.Should().BeFalse();
        }

        [Fact]
        public void NewError()
        {
            var testTarget = new ValidationManager<string>();
            var validationResult = new ValidationResult<string>("Foo");
            var spy = new RaiseErrorsChangedSpy();

            var actualResult = testTarget.Validate(42, value => validationResult, spy);

            actualResult.Should().Be(validationResult);
            spy.MustHaveBeenCalledExactlyOnceWithPropertyName(nameof(NewError));
            testTarget.HasErrors.Should().BeTrue();
            testTarget.GetErrors(nameof(NewError)).Should().BeSameAs(validationResult.Errors);
        }

        [Fact]
        public void RemoveExistingError()
        {
            var testTarget = new ValidationManager<string>();
            var spy = new RaiseErrorsChangedSpy();
            testTarget.Validate(42, value => new ValidationResult<string>("Foo"), spy);
            testTarget.HasErrors.Should().BeTrue();
            spy.Reset();

            var actualResult = testTarget.Validate(42, value => ValidationResult<string>.Valid, spy);

            actualResult.Should().Be(ValidationResult<string>.Valid);
            testTarget.HasErrors.Should().BeFalse();
            testTarget.GetErrors(nameof(RemoveExistingError)).Should().BeNull();
            spy.MustHaveBeenCalledExactlyOnceWithPropertyName(nameof(RemoveExistingError));
        }

        [Fact]
        public void ReplaceExistingError()
        {
            var testTarget = new ValidationManager<string>();
            var spy = new RaiseErrorsChangedSpy();
            testTarget.Validate(42, value => new ValidationResult<string>("Foo"), spy);
            testTarget.HasErrors.Should().BeTrue();
            spy.Reset();

            testTarget.Validate(87, value => new ValidationResult<string>("Bar"), spy);

            testTarget.HasErrors.Should().BeTrue();
            var errors = testTarget.GetErrors(nameof(ReplaceExistingError)).As<IReadOnlyList<string>>();
            errors.Should().HaveCount(1);
            errors[0].Should().Be("Bar").And.NotBe("Foo");
            spy.MustHaveBeenCalledExactlyOnceWithPropertyName(nameof(ReplaceExistingError));
        }

        public sealed class RaiseErrorsChangedSpy : IRaiseErrorsChanged
        {
            private readonly List<string> _capturedPropertyNames = new List<string>();

            public void OnErrorsChanged(string propertyName)
            {
                _capturedPropertyNames.Add(propertyName);
            }

            public void MustHaveBeenCalledExactlyOnceWithPropertyName(string propertyName)
            {
                _capturedPropertyNames.Should().HaveCount(1);
                _capturedPropertyNames[0].Should().Be(propertyName);
            }

            public void Reset()
            {
                _capturedPropertyNames.Clear();
            }
        }

        [Fact]
        public void DerivedValidationManagerOnlyCountsValidationMessagesWithLevelError()
        {
            var errors = new Dictionary<string, ValidationResult<ValidationMessage>>
                         {
                             ["Foo"] = new ValidationMessage("Bar"),
                             ["Baz"] = new ValidationMessage("Qux", ValidationMessageLevel.Warning)
                         };
            var validationManager = new ValidationManager(errors);
            validationManager.HasErrors.Should().BeTrue();

            errors.Remove("Foo");

            validationManager.HasErrors.Should().BeFalse();
        }
    }
}