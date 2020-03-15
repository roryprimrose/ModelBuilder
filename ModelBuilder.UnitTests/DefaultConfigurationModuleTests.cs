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
    using Xunit.Abstractions;

    public class DefaultConfigurationModuleTests
    {
        private readonly ITestOutputHelper _output;

        public DefaultConfigurationModuleTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void ConfigureAssignsDefaultConfiguration()
        {
            var configuration = new BuildConfiguration();

            var sut = new DefaultConfigurationModule();

            sut.Configure(configuration);

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
        public void ConfigureDoesNotIncludeMailinatorEmailValueGenerator()
        {
            var configuration = new BuildConfiguration();

            var sut = new DefaultConfigurationModule();

            sut.Configure(configuration);

            configuration.ValueGenerators.Should()
                .NotContain(x => x.GetType() == typeof(MailinatorEmailValueGenerator));
        }

        [Fact]
        public void ConfigureIncludesAllAvailableTypeCreators()
        {
            var types = from x in typeof(DefaultConfigurationModule).GetTypeInfo().Assembly.GetTypes()
                where typeof(ITypeCreator).IsAssignableFrom(x) && x.GetTypeInfo().IsAbstract == false &&
                      x.GetTypeInfo().IsInterface == false
                select x;

            var configuration = new BuildConfiguration();

            var sut = new DefaultConfigurationModule();

            sut.Configure(configuration);

            foreach (var type in types)
            {
                configuration.TypeCreators.Should().Contain(x => x.GetType() == type);
            }
        }

        [Fact]
        public void ConfigureIncludesAllAvailableValueGeneratorsExceptMailinator()
        {
            var types = from x in typeof(DefaultConfigurationModule).GetTypeInfo().Assembly.GetTypes()
                where typeof(IValueGenerator).IsAssignableFrom(x) && x.GetTypeInfo().IsAbstract == false &&
                      x.GetTypeInfo().IsInterface == false && x != typeof(MailinatorEmailValueGenerator)
                select x;

            var configuration = new BuildConfiguration();

            var sut = new DefaultConfigurationModule();

            sut.Configure(configuration);

            foreach (var type in types)
            {
                configuration.ValueGenerators.Should().Contain(x => x.GetType() == type);
            }
        }

        [Fact]
        public void ConfigureThrowsExceptionWithNullConfiguration()
        {
            var sut = new DefaultConfigurationModule();

            Action action = () => sut.Configure(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void DefaultExecuteOrderRulesReturnsExecutableRules()
        {
            var configuration = new BuildConfiguration();

            var sut = new DefaultConfigurationModule();

            sut.Configure(configuration);

            var rules = configuration.ExecuteOrderRules;

            rules.Should().NotBeEmpty();

            foreach (var rule in rules)
            {
                var property = typeof(Person).GetProperty(nameof(Person.FirstName));

                Action action = () => rule.IsMatch(property);

                action.Should().NotThrow();
            }
        }

        [Fact]
        public void OrderOfTypeCreators()
        {
            var configuration = new BuildConfiguration();

            var sut = new DefaultConfigurationModule();

            sut.Configure(configuration);

            var items = configuration.TypeCreators.OrderByDescending(x => x.Priority);

            _output.WriteLine("TypeCreators are evaluated in the following order");

            foreach (var item in items)
            {
                _output.WriteLine("{0} - {1}", item.Priority, item.GetType().Name);
            }
        }

        [Fact]
        public void OrderOfExecuteOrderRules()
        {
            var configuration = new BuildConfiguration();

            var sut = new DefaultConfigurationModule();

            sut.Configure(configuration);

            var items = configuration.ExecuteOrderRules.OrderByDescending(x => x.Priority);

            _output.WriteLine("ExecuteOrderRules are evaluated in the following order");

            foreach (var item in items)
            {
                _output.WriteLine("{0} - {1}", item.Priority, item);
            }
        }

        [Fact]
        public void OrderOfValueGenerators()
        {
            var configuration = new BuildConfiguration();

            var sut = new DefaultConfigurationModule();

            sut.Configure(configuration);

            var items = configuration.ValueGenerators.OrderByDescending(x => x.Priority);

            _output.WriteLine("ValueGenerators are evaluated in the following order");

            foreach (var item in items)
            {
                _output.WriteLine("{0} - {1}", item.Priority, item.GetType().Name);
            }
        }
    }
}