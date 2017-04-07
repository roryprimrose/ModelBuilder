namespace ModelBuilder.UnitTests
{
    using System;
    using FluentAssertions;
    using Xunit;

    public class ModelTests
    {
        [Fact]
        public void CreateThrowsExceptionWithNullInstanceTypeTest()
        {
            Action action = () => Model.Create(null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void CreateWithThrowsExceptionWithNullInstanceTypeTest()
        {
            Action action = () => Model.CreateWith(null);

            action.ShouldThrow<ArgumentNullException>();
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

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void IgnoringUsesBuildStrategyToCreateInstanceTest()
        {
            var actual = Model.Ignoring<Person>(x => x.FirstName).Create<Person>();

            actual.FirstName.Should().BeNull();
        }

        [Fact]
        public void UsingReturnsNewBuilderStrategyTest()
        {
            var actual = Model.Using<NullBuildStrategy>();

            actual.Should().BeOfType<NullBuildStrategy>();
        }

        [Fact]
        public void UsingReturnsSpecifiedBuildStrategyTest()
        {
            var actual = Model.Using<DummyBuildStrategy>();

            actual.Should().BeOfType(typeof(DummyBuildStrategy));
        }

        [Fact]
        public void WithReturnsSpecifiedExecuteStrategyTest()
        {
            var actual = Model.With<DummyExecuteStrategy>();

            actual.Should().BeOfType(typeof(DummyExecuteStrategy));
        }
    }
}