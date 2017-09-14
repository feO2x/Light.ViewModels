using System.ComponentModel;
using System.Reflection;
using FluentAssertions;
using Xunit;

namespace Light.ViewModels.Tests
{
    public sealed class BaseNotifyDataErrorInfoTests
    {
        [Fact]
        public void InheritsFromBaseNotifyPropertyChanged()
        {
            typeof(BaseNotifyDataErrorInfo).Should().BeDerivedFrom<BaseNotifyPropertyChanged>();
        }

        [Fact]
        public void IsAbstract()
        {
            typeof(BaseNotifyDataErrorInfo).GetTypeInfo().IsAbstract.Should().BeTrue();
        }

        [Fact]
        public void ImplementsINotifyDataErrorInfo()
        {
            typeof(BaseNotifyDataErrorInfo).Should().Implement<INotifyDataErrorInfo>();
        }

        [Fact]
        public void ImplementsIRaiseErrorsChanged()
        {
            typeof(BaseNotifyDataErrorInfo).Should().Implement<IRaiseErrorsChanged>();
        }

        [Fact]
        public void HasValidationManager()
        {
            new BaseNotifyDataErrorInfoDummy().ValidationManager.Should().NotBeNull();
        }

        public sealed class BaseNotifyDataErrorInfoDummy : BaseNotifyDataErrorInfo
        {
            public new ValidationManager ValidationManager => base.ValidationManager;
        }
    }
}