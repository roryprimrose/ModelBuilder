namespace ModelBuilder.UnitTests
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using Xunit;

    public class DefaultBuildStrategyCompilerTests
    {
        [Fact]
        public void CreatesWithDefaultConfigurationTest()
        {
            var target = new DefaultBuildStrategyCompiler();

            target.IgnoreRules.Should().BeEmpty();
            target.BuildLog.Should().BeOfType<DefaultBuildLog>();
            target.ConstructorResolver.Should().BeOfType<DefaultConstructorResolver>();
            target.CreationRules.Should().BeEmpty();
            target.TypeCreators.Should().NotBeEmpty();
            target.ValueGenerators.Should().NotBeEmpty();
            target.ExecuteOrderRules.Should().NotBeEmpty();
            target.PostBuildActions.Should().BeEmpty();
        }

        [Fact]
        public void DefaultExecuteOrderRulesReturnsExecutableRulesTest()
        {
            var rules = new DefaultBuildStrategyCompiler().ExecuteOrderRules.ToList();

            rules.Should().NotBeEmpty();

            foreach (var rule in rules)
            {
                Action action = () => rule.IsMatch(typeof(string), "Stuff");

                action.ShouldNotThrow();
            }
        }

        [Fact]
        public void TypeCreatorsIncludesAllAvailableTypeCreatorsTest()
        {
            var types = from x in typeof(DefaultBuildStrategyCompiler).Assembly.GetTypes()
                where typeof(ITypeCreator).IsAssignableFrom(x) && x.IsAbstract == false && x.IsInterface == false
                select x;

            var target = new DefaultBuildStrategyCompiler();

            foreach (var type in types)
            {
                target.TypeCreators.Should().Contain(x => x.GetType() == type);
            }
        }

        [Fact]
        public void ValueGeneratorsDoesNotIncludeMailinatorEmailValueGeneratorTest()
        {
            var target = new DefaultBuildStrategyCompiler();

            target.ValueGenerators.Should().NotContain(x => x.GetType() == typeof(MailinatorEmailValueGenerator));
        }

        [Fact]
        public void ValueGeneratorsIncludesAllAvailableValueGeneratorsExceptMailinatorTest()
        {
            var types = from x in typeof(DefaultBuildStrategyCompiler).Assembly.GetTypes()
                where
                    typeof(IValueGenerator).IsAssignableFrom(x) && x.IsAbstract == false && x.IsInterface == false &&
                    x != typeof(MailinatorEmailValueGenerator)
                select x;

            var target = new DefaultBuildStrategyCompiler();

            foreach (var type in types)
            {
                target.ValueGenerators.Should().Contain(x => x.GetType() == type);
            }
        }
    }
}