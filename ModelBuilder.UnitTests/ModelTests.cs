namespace ModelBuilder.UnitTests
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder.UnitTests.Models;
    using Xunit;
    using Xunit.Abstractions;

    public class ModelTests
    {
        private readonly ITestOutputHelper _output;

        public ModelTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void CreateReturnsInstance()
        {
            var actual = Model.Create(typeof(SlimModel));

            actual.Should().NotBeNull();
            actual.As<SlimModel>().Value.Should().NotBeEmpty();
        }

        [Fact]
        public void CreateReturnsInstanceUsingConstructorParameters()
        {
            var value = Guid.NewGuid();

            var actual = Model.Create(typeof(ReadOnlyModel), value);

            actual.Should().NotBeNull();
            actual.As<ReadOnlyModel>().Value.Should().Be(value);
        }

        [Fact]
        public void CreateReturnsInstanceWithPopulatedProperties()
        {
            var actual = Model.Create<EmailParts>();

            actual.Should().NotBeNull();
            actual.FirstName.Should().NotBeNull();
            actual.LastName.Should().NotBeNull();
            actual.Domain.Should().NotBeNull();
            actual.Email.Should().NotBeNull();
        }

        [Fact]
        public void CreateThrowsExceptionWithNullInstanceType()
        {
            Action action = () => Model.Create(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateTReturnsInstance()
        {
            var actual = Model.Create<SlimModel>()!;

            actual.Should().NotBeNull();
            actual.Value.Should().NotBeEmpty();
        }

        [Fact]
        public void CreateTReturnsInstanceUsingConstructorParameters()
        {
            var value = Guid.NewGuid();

            var actual = Model.Create<ReadOnlyModel>(value)!;

            actual.Should().NotBeNull();
            actual.Value.Should().Be(value);
        }

        [Fact]
        public void CreateWithInternalConstructorReturnsInstance()
        {
            var model = Model.Create<EntityWithInternalConstructor>();
            model.Should().NotBeNull();
            model.EntityId.Should().NotBeEmpty();
            model.EntityName.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void IgnoringThrowsExceptionWithNullExpression()
        {
            Action action = () => Model.Ignoring<Person>(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IgnoringUsesConfigurationToCreateInstance()
        {
            var actual = Model.Ignoring<Person>(x => x.FirstName).Create<Person>()!;

            actual.FirstName.Should().BeNull();
        }

        [Fact]
        public void MappingUsesConfigurationToCreateInstance()
        {
            var configuration = Model.Mapping<ITestItem, TestItem>();
            var executeStrategy = configuration.UsingExecuteStrategy<DefaultExecuteStrategy<ITestItem>>();

            var actual = executeStrategy.Create();

            _output.WriteLine(executeStrategy.Log.Output);

            actual.Should().BeOfType<TestItem>();
            actual.FirstName.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void PopulateReturnsProvidedInstanceWithPopulatedProperties()
        {
            var expected = new Person();

            var actual = Model.Populate(expected);

            actual.Should().Be(expected);
            actual.Address.Should().NotBeNull();
            actual.FirstName.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void UsingDefaultConfigurationReturnsNewInstance()
        {
            var firstActual = Model.UsingDefaultConfiguration();
            var secondActual = Model.UsingDefaultConfiguration();

            firstActual.Should().NotBeSameAs(secondActual);
        }

        [Fact]
        public void UsingExecuteStrategyReturnsSpecifiedExecuteStrategy()
        {
            var actual = Model.UsingExecuteStrategy<DummyExecuteStrategy>();

            actual.Should().BeOfType(typeof(DummyExecuteStrategy));
        }

        [Fact]
        public void UsingExecuteStrategyReturnsStrategyWithDefaultConfiguration()
        {
            var defaultConfig = Model.UsingDefaultConfiguration();

            var actual = Model.UsingExecuteStrategy<DefaultExecuteStrategy>();

            actual.Configuration.Should().BeEquivalentTo(defaultConfig);
        }

        [Fact]
        public void UsingModelAppliesDefaultConfigurationAsBaseConfiguration()
        {
            var defaultConfig = Model.UsingDefaultConfiguration();

            var actual = Model.UsingModule<TestConfigurationModule>();

            actual.ConstructorResolver.Should().BeOfType<DefaultConstructorResolver>();
            actual.PropertyResolver.Should().BeOfType<DefaultPropertyResolver>();

            foreach (var item in defaultConfig.ExecuteOrderRules)
            {
                actual.ExecuteOrderRules.Any(x => x.GetType() == item.GetType()).Should().BeTrue();
            }

            foreach (var item in defaultConfig.CreationRules)
            {
                actual.CreationRules.Any(x => x.GetType() == item.GetType()).Should().BeTrue();
            }

            foreach (var item in defaultConfig.IgnoreRules)
            {
                actual.IgnoreRules.Any(x => x.GetType() == item.GetType()).Should().BeTrue();
            }

            foreach (var item in defaultConfig.PostBuildActions)
            {
                actual.PostBuildActions.Any(x => x.GetType() == item.GetType()).Should().BeTrue();
            }

            foreach (var item in defaultConfig.TypeCreators)
            {
                actual.TypeCreators.Any(x => x.GetType() == item.GetType()).Should().BeTrue();
            }

            foreach (var item in defaultConfig.TypeMappingRules)
            {
                actual.TypeMappingRules.Any(x => x.GetType() == item.GetType()).Should().BeTrue();
            }

            foreach (var item in defaultConfig.ValueGenerators)
            {
                actual.ValueGenerators.Any(x => x.GetType() == item.GetType()).Should().BeTrue();
            }
        }

        [Fact]
        public void UsingModuleReturnsBuildConfigurationWithModuleModificationsApplied()
        {
            var actual = Model.UsingModule<TestConfigurationModule>();

            actual.ValueGenerators.FirstOrDefault(x => x.GetType() == typeof(DummyValueGenerator)).Should().NotBeNull();
        }

        [Fact]
        public void WriteLogReturnsExecuteStrategyWithLogging()
        {
            var actual = Model.WriteLog(_output.WriteLine);

            actual.Should().BeAssignableTo<IExecuteStrategy>();
            actual.Should().NotBeOfType<DefaultExecuteStrategy>();
        }

        [Fact]
        public void WriteLogThrowsExceptionWithNullAction()
        {
            Action action = () => Model.WriteLog(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void WriteLogTReturnsExecuteStrategyWithLogging()
        {
            var actual = Model.WriteLog<Person>(_output.WriteLine);

            actual.Should().BeAssignableTo<IExecuteStrategy<Person>>();
            actual.Should().NotBeOfType<DefaultExecuteStrategy<Person>>();
        }

        [Fact]
        public void WriteLogTThrowsExceptionWithNullAction()
        {
            Action action = () => Model.WriteLog<Person>(null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}