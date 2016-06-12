namespace ModelBuilder.UnitTests
{
    using System;
    using System.Linq;
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
        public void DefaultExecuteOrderRulesReturnsExecutableRulesTest()
        {
            var rules = DefaultBuildStrategy.DefaultExecuteOrderRules.ToList();

            rules.Should().NotBeEmpty();

            foreach (var rule in rules)
            {
                Action action = () => rule.IsMatch(typeof(string), "Stuff");

                action.ShouldNotThrow();
            }
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

        [Fact]
        public void TypeCreatorsIncludesAllAvailableTypeCreatorsTest()
        {
            var types = from x in typeof(DefaultBuildStrategy).Assembly.GetTypes()
                where typeof(ITypeCreator).IsAssignableFrom(x) && x.IsAbstract == false && x.IsInterface == false
                select x;

            var target = new DefaultBuildStrategy();

            foreach (var type in types)
            {
                target.TypeCreators.Should().Contain(x => x.GetType() == type);
            }
        }

        [Fact]
        public void ValueGeneratorsDoesNotIncludeMailinatorEmailValueGeneratorTest()
        {
            var target = new DefaultBuildStrategy();

            target.ValueGenerators.Should().NotContain(x => x.GetType() == typeof(MailinatorEmailValueGenerator));
        }

        [Fact]
        public void ValueGeneratorsIncludesAllAvailableValueGeneratorsExceptMailinatorTest()
        {
            var types = from x in typeof(DefaultBuildStrategy).Assembly.GetTypes()
                where
                    typeof(IValueGenerator).IsAssignableFrom(x) && x.IsAbstract == false && x.IsInterface == false &&
                    x != typeof(MailinatorEmailValueGenerator)
                select x;

            var target = new DefaultBuildStrategy();

            foreach (var type in types)
            {
                target.ValueGenerators.Should().Contain(x => x.GetType() == type);
            }
        }
    }
}