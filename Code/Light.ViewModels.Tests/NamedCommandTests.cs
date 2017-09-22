using System;
using FluentAssertions;
using Xunit;

namespace Light.ViewModels.Tests
{
    public sealed class NamedCommandTests
    {
        private static readonly Action DoNothing = () => { };

        [Fact]
        public void DerivesFromDelegateCommand()
        {
            typeof(NamedCommand).Should().BeDerivedFrom<DelegateCommand>();
        }

        [Theory]
        [InlineData("Foo")]
        [InlineData("Bar")]
        [InlineData("Baz")]
        public void RetrieveName(string name)
        {
            var testTarget = new NamedCommand(name, DoNothing);

            testTarget.Name.Should().BeSameAs(name);
        }

        [Fact]
        public void NameNull()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action act = () => new NamedCommand(null, DoNothing);

            act.ShouldThrow<ArgumentNullException>()
               .And.ParamName.Should().Be("name");
        }
    }
}