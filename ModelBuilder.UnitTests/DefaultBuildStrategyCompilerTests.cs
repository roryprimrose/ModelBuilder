namespace ModelBuilder.UnitTests
{
    using System;
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using Xunit;

    public class DefaultBuildStrategyCompilerTests
    {
        [Fact]
        public void CreatesWithDefaultConfigurationTest()
        {
            var target = new DefaultBuildStrategyCompiler();

            target.IgnoreRules.Should().BeEmpty();
            target.ConstructorResolver.Should().BeOfType<DefaultConstructorResolver>();
            target.PropertyResolver.Should().BeOfType<DefaultPropertyResolver>();
            target.CreationRules.Should().BeEmpty();
            target.TypeMappingRules.Should().NotBeEmpty();
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
                var property = typeof(Person).GetProperty(nameof(Person.FirstName));

                Action action = () => rule.IsMatch(property);

                action.Should().NotThrow();
            }
        }
        
        [Fact]
        public void TypeCreatorsIncludesAllAvailableTypeCreatorsTest()
        {
            var types = from x in typeof(DefaultBuildStrategyCompiler).GetTypeInfo().Assembly.GetTypes()
                where typeof(ITypeCreator).IsAssignableFrom(x) && x.GetTypeInfo().IsAbstract == false
                                                               && x.GetTypeInfo().IsInterface == false
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
            var types = from x in typeof(DefaultBuildStrategyCompiler).GetTypeInfo().Assembly.GetTypes()
                where typeof(IValueGenerator).IsAssignableFrom(x) && x.GetTypeInfo().IsAbstract == false
                                                                  && x.GetTypeInfo().IsInterface == false
                                                                  && x != typeof(MailinatorEmailValueGenerator)
                select x;

            var target = new DefaultBuildStrategyCompiler();

            foreach (var type in types)
            {
                target.ValueGenerators.Should().Contain(x => x.GetType() == type);
            }
        }
    }
}