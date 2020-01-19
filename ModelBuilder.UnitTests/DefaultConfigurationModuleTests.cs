namespace ModelBuilder.UnitTests
{
    using System;
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using ModelBuilder.TypeCreators;
    using ModelBuilder.UnitTests.Models;
    using ModelBuilder.ValueGenerators;
    using Xunit;

    public class DefaultConfigurationModuleTests
    {
        [Fact]
        public void ConfigureAssignsDefaultConfiguration()
        {
            var configuration = new BuildConfiguration();

            var target = new DefaultConfigurationModule();

            target.Configure(configuration);

            configuration.IgnoreRules.Should().BeEmpty();
            configuration.ConstructorResolver.Should().BeOfType<DefaultConstructorResolver>();
            configuration.PropertyResolver.Should().BeOfType<DefaultPropertyResolver>();
            configuration.CreationRules.Should().BeEmpty();
            configuration.TypeMappingRules.Should().BeEmpty();
            configuration.TypeCreators.Should().NotBeEmpty();
            configuration.ValueGenerators.Should().NotBeEmpty();
            configuration.ExecuteOrderRules.Should().NotBeEmpty();
            configuration.PostBuildActions.Should().BeEmpty();
        }

        [Fact]
        public void ConfigureDoesNotIncludeMailinatorEmailValueGeneratorTest()
        {
            var configuration = new BuildConfiguration();

            var target = new DefaultConfigurationModule();

            target.Configure(configuration);

            configuration.ValueGenerators.Should()
                .NotContain(x => x.GetType() == typeof(MailinatorEmailValueGenerator));
        }

        [Fact]
        public void ConfigureIncludesAllAvailableTypeCreatorsTest()
        {
            var types = from x in typeof(DefaultConfigurationModule).GetTypeInfo().Assembly.GetTypes()
                where typeof(ITypeCreator).IsAssignableFrom(x) && x.GetTypeInfo().IsAbstract == false &&
                      x.GetTypeInfo().IsInterface == false
                select x;

            var configuration = new BuildConfiguration();

            var target = new DefaultConfigurationModule();

            target.Configure(configuration);

            foreach (var type in types)
            {
                configuration.TypeCreators.Should().Contain(x => x.GetType() == type);
            }
        }

        [Fact]
        public void ConfigureIncludesAllAvailableValueGeneratorsExceptMailinatorTest()
        {
            var types = from x in typeof(DefaultConfigurationModule).GetTypeInfo().Assembly.GetTypes()
                where typeof(IValueGenerator).IsAssignableFrom(x) && x.GetTypeInfo().IsAbstract == false &&
                      x.GetTypeInfo().IsInterface == false && x != typeof(MailinatorEmailValueGenerator)
                select x;

            var configuration = new BuildConfiguration();

            var target = new DefaultConfigurationModule();

            target.Configure(configuration);

            foreach (var type in types)
            {
                configuration.ValueGenerators.Should().Contain(x => x.GetType() == type);
            }
        }

        [Fact]
        public void ConfigureThrowsExceptionWithNullConfiguration()
        {
            var target = new DefaultConfigurationModule();

            Action action = () => target.Configure(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void DefaultExecuteOrderRulesReturnsExecutableRulesTest()
        {
            var configuration = new BuildConfiguration();

            var target = new DefaultConfigurationModule();

            target.Configure(configuration);

            var rules = configuration.ExecuteOrderRules;

            rules.Should().NotBeEmpty();

            foreach (var rule in rules)
            {
                var property = typeof(Person).GetProperty(nameof(Person.FirstName));

                Action action = () => rule.IsMatch(property);

                action.Should().NotThrow();
            }
        }
    }
}