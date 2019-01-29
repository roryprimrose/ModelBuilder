namespace ModelBuilder.UnitTests
{
    using System;
    using System.Linq;
    using FluentAssertions;
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
        public void CreateWithThrowsExceptionWithNullInstanceTypeTest()
        {
            Action action = () => Model.CreateWith(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void DefaultBuildStrategyReturnsSameInstanceTest()
        {
            var firstActual = Model.DefaultBuildStrategy;
            var secondActual = Model.DefaultBuildStrategy;

            firstActual.Should().BeSameAs(secondActual);
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
        public void UsingBuildStrategyReturnsNewBuilderStrategyTest()
        {
            var actual = Model.UsingBuildStrategy<NullBuildStrategy>();

            actual.Should().BeOfType<NullBuildStrategy>();
        }

        [Fact]
        public void UsingBuildStrategyReturnsSpecifiedBuildStrategyTest()
        {
            var actual = Model.UsingBuildStrategy<DummyBuildStrategy>();

            actual.Should().BeOfType(typeof(DummyBuildStrategy));
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
            var actual = Model.UsingModule<TestCompilerModule>();

            actual.ValueGenerators.FirstOrDefault(x => x.GetType() == typeof(DummyValueGenerator)).Should().NotBeNull();
        }
    }
}