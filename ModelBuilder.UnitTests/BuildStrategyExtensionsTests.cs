namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.ObjectModel;
    using FluentAssertions;
    using ModelBuilder.UnitTests.Models;
    using NSubstitute;
    using Xunit;

    public class BuildStrategyExtensionsTests
    {
        [Fact]
        public void UsingExecuteStrategyReturnsExecuteStrategyWithBuildStrategyConfigurationsTest()
        {
            var strategy = Model.BuildStrategy;
            var ignoreRules = new Collection<IgnoreRule>
            {
                new IgnoreRule(typeof(string), "Stuff")
            };

            var buildLog = Substitute.For<IBuildLog>();
            var target = Substitute.For<IBuildStrategy>();

            target.GetBuildLog().Returns(buildLog);

            target.ConstructorResolver.Returns(strategy.ConstructorResolver);
            target.ExecuteOrderRules.Returns(strategy.ExecuteOrderRules);
            target.TypeCreators.Returns(strategy.TypeCreators);
            target.ValueGenerators.Returns(strategy.ValueGenerators);
            target.IgnoreRules.Returns(ignoreRules);

            var actual = target.UsingExecuteStrategy<DefaultExecuteStrategy<Person>>();

            actual.Configuration.Should().BeSameAs(target);
            actual.Log.Should().BeSameAs(buildLog);
        }

        [Fact]
        public void UsingExecuteStrategyThrowsExceptionWhenStrategyReturnsNullBuildLogTest()
        {
            var target = Substitute.For<IBuildStrategy>();

            target.GetBuildLog().Returns((IBuildLog) null);

            Action action = () => target.UsingExecuteStrategy<DefaultExecuteStrategy>();

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void UsingExecuteStrategyThrowsExceptionWithNullStrategyTest()
        {
            Action action = () => ((IBuildStrategy) null).UsingExecuteStrategy<NullExecuteStrategy>();

            action.Should().Throw<ArgumentNullException>();
        }
    }
}