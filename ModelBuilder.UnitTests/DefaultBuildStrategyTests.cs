using FluentAssertions;
using Xunit;

namespace ModelBuilder.UnitTests
{
    public class DefaultBuildStrategyTests
    {
        [Fact]
        public void CreatesWithDefaultConfigurationTest()
        {
            var target = new DefaultBuildStrategy();

            target.IgnoreRules.ShouldAllBeEquivalentTo(DefaultBuildStrategy.DefaultIgnoreRules);
            target.ConstructorResolver.Should().BeOfType<DefaultConstructorResolver>();
            target.TypeCreators.ShouldAllBeEquivalentTo(DefaultBuildStrategy.DefaultTypeCreators);
            target.ValueGenerators.ShouldAllBeEquivalentTo(DefaultBuildStrategy.DefaultValueGenerators);
            target.ExecuteOrderRules.ShouldAllBeEquivalentTo(DefaultBuildStrategy.DefaultExecuteOrderRules);
        }

        [Fact]
        public void GetExecuteStrategyReturnsDefaultExecuteStrategyTest()
        {
            var target = new DefaultBuildStrategy();

            var actual = target.GetExecuteStrategy<Person>();

            actual.IgnoreRules.ShouldAllBeEquivalentTo(target.IgnoreRules);
            actual.ConstructorResolver.Should().BeOfType<DefaultConstructorResolver>();
            actual.TypeCreators.ShouldAllBeEquivalentTo(target.TypeCreators);
            actual.ValueGenerators.ShouldAllBeEquivalentTo(target.ValueGenerators);
            actual.ExecuteOrderRules.ShouldAllBeEquivalentTo(target.ExecuteOrderRules);
        }
    }
}