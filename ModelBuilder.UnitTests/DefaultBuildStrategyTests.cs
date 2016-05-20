namespace ModelBuilder.UnitTests
{
    using FluentAssertions;
    using Xunit;

    public class DefaultBuildStrategyTests
    {
        [Fact]
        public void CreatesWithDefaultConfigurationTest()
        {
            var target = new DefaultBuildStrategy();

            target.IgnoreRules.ShouldAllBeEquivalentTo(DefaultBuildStrategy.DefaultIgnoreRules);
            target.BuildLog.Should().BeOfType<DefaultBuildLog>();
            target.ConstructorResolver.Should().BeOfType<DefaultConstructorResolver>();
            target.CreationRules.ShouldAllBeEquivalentTo(DefaultBuildStrategy.DefaultCreationRules);
            target.TypeCreators.ShouldAllBeEquivalentTo(DefaultBuildStrategy.DefaultTypeCreators);
            target.ValueGenerators.ShouldAllBeEquivalentTo(DefaultBuildStrategy.DefaultValueGenerators);
            target.ExecuteOrderRules.ShouldAllBeEquivalentTo(DefaultBuildStrategy.DefaultExecuteOrderRules);
        }

        [Fact]
        public void ExposesDefaultStaticConfigurationTest()
        {
            DefaultBuildStrategy.DefaultIgnoreRules.Should().BeEmpty();
            DefaultBuildStrategy.DefaultBuildLog.Should().BeOfType<DefaultBuildLog>();
            DefaultBuildStrategy.DefaultConstructorResolver.Should().BeOfType<DefaultConstructorResolver>();
            DefaultBuildStrategy.DefaultCreationRules.Should().BeEmpty();
            DefaultBuildStrategy.DefaultTypeCreators.Should().NotBeEmpty();
            DefaultBuildStrategy.DefaultValueGenerators.Should().NotBeEmpty();
            DefaultBuildStrategy.DefaultExecuteOrderRules.Should().NotBeEmpty();
        }

        [Fact]
        public void GetExecuteStrategyReturnsDefaultExecuteStrategyTest()
        {
            var target = new DefaultBuildStrategy();

            var actual = target.GetExecuteStrategy<Person>();

            actual.BuildStrategy.Should().BeSameAs(target);
        }
    }
}