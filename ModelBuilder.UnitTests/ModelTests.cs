namespace ModelBuilder.UnitTests
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder.UnitTests.Models;
    using Xunit;

    public class ModelTests
    {
        [Fact]
        public void CreateThrowsExceptionWithNullInstanceTypeTest()
        {
            Action action = () => Model.Create(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IgnoringThrowsExceptionWithNullExpressionTest()
        {
            Action action = () => Model.Ignoring<Person>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IgnoringUsesBuildStrategyToCreateInstanceTest()
        {
            var actual = Model.Ignoring<Person>(x => x.FirstName).Create<Person>();

            actual.FirstName.Should().BeNull();
        }

        [Fact]
        public void MappingUsesBuildStrategyToCreateInstanceTest()
        {
            var actual = Model.Mapping<ITestItem, TestItem>().Create<ITestItem>();

            actual.Should().BeOfType<TestItem>();
            actual.FirstName.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void UsingDefaultConfigurationReturnsNewInstanceTest()
        {
            var firstActual = Model.UsingDefaultConfiguration();
            var secondActual = Model.UsingDefaultConfiguration();

            firstActual.Should().NotBeSameAs(secondActual);
        }

        [Fact]
        public void UsingExecuteStrategyReturnsSpecifiedExecuteStrategyTest()
        {
            var actual = Model.UsingExecuteStrategy<DummyExecuteStrategy>();

            actual.Should().BeOfType(typeof(DummyExecuteStrategy));
        }

        [Fact]
        public void UsingModuleReturnsBuildStrategyWithModuleModificationsTest()
        {
            var actual = Model.UsingModule<TestConfigurationModule>();

            actual.ValueGenerators.FirstOrDefault(x => x.GetType() == typeof(DummyValueGenerator)).Should().NotBeNull();
        }
    }
}