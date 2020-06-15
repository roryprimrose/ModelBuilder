namespace ModelBuilder.UnitTests
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder.BuildActions;
    using ModelBuilder.UnitTests.Models;
    using NSubstitute;
    using Xunit;
    using Xunit.Abstractions;

    public class DefaultExecuteStrategyTTests
    {
        private readonly IBuildLog _buildLog;

        public DefaultExecuteStrategyTTests(ITestOutputHelper output)
        {
            _buildLog = new OutputBuildLog(output);
        }

        [Fact]
        public void CreateReturnsValueCreatedFromProvidedArguments()
        {
            var buildHistory = new BuildHistory();
            var expected = new Person();
            var args = new object[]
            {
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                DateTime.UtcNow,
                true,
                Guid.NewGuid(),
                Environment.TickCount
            };

            var processor = Substitute.For<IBuildProcessor>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var propertyResolver = Substitute.For<IPropertyResolver>();
            var typeCapability = Substitute.For<IBuildCapability>();

            typeCapability.SupportsCreate.Returns(true);
            typeCapability.ImplementedByType.Returns(typeof(DummyTypeCreator));

            var sut = new DefaultExecuteStrategy<Person>(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(sut, Arg.Any<BuildRequirement>(),
                    typeof(Person))
                .Returns(typeCapability);
            typeCapability.CreateType(sut, typeof(Person), args).Returns(expected);
            buildConfiguration.PropertyResolver.Returns(propertyResolver);

            sut.Initialize(buildConfiguration);

            var actual = sut.Create(args);

            actual.Should().BeSameAs(expected);
        }

        [Fact]
        public void CreateReturnsValueCreatedWithoutArguments()
        {
            var buildHistory = new BuildHistory();
            var expected = new Person();

            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var constructorResolver = Substitute.For<IConstructorResolver>();
            var typeResolver = Substitute.For<ITypeResolver>();
            var processor = Substitute.For<IBuildProcessor>();
            var typeCapability = Substitute.For<IBuildCapability>();

            typeCapability.SupportsCreate.Returns(true);
            typeCapability.ImplementedByType.Returns(typeof(DummyTypeCreator));

            buildConfiguration.ConstructorResolver.Returns(constructorResolver);
            buildConfiguration.TypeResolver.Returns(typeResolver);
            typeResolver.GetBuildType(buildConfiguration, typeof(Person)).Returns(x => x.Arg<Type>());

            var sut = new DefaultExecuteStrategy<Person>(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(sut, Arg.Any<BuildRequirement>(),
                    typeof(Person))
                .Returns(typeCapability);
            constructorResolver.Resolve(typeof(Person))
                .Returns(typeof(Person).GetConstructors().Single(x => x.GetParameters().Length == 0));
            typeCapability.CreateType(sut, typeof(Person), Arg.Any<object?[]?>()).Returns(expected);
            typeCapability.Populate(sut, expected).Returns(expected);

            sut.Initialize(buildConfiguration);

            var actual = sut.Create();

            actual.Should().BeSameAs(expected);
        }

        [Fact]
        public void CreateThrowsExceptionWhenProcessorReturnsNull()
        {
            var buildHistory = new BuildHistory();

            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var processor = Substitute.For<IBuildProcessor>();
            var typeCapability = Substitute.For<IBuildCapability>();

            typeCapability.SupportsCreate.Returns(true);
            typeCapability.ImplementedByType.Returns(typeof(DummyTypeCreator));

            var sut = new DefaultExecuteStrategy<int>(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(sut, Arg.Any<BuildRequirement>(),
                    typeof(int))
                .Returns(typeCapability);
            typeCapability.CreateType(sut, typeof(int), Arg.Any<object?[]?>()).Returns((object) null!);

            sut.Initialize(buildConfiguration);

            Action action = () => sut.Create();

            action.Should().Throw<BuildException>();
        }

        [Fact]
        public void PopulateAssignsPropertyValuesToExistingInstance()
        {
            var buildHistory = new BuildHistory();
            var model = new SlimModel();
            var expected = Guid.NewGuid();
            var properties = typeof(SlimModel).GetProperties();
            var propertyInfo = properties.Single();

            var processor = Substitute.For<IBuildProcessor>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var propertyResolver = Substitute.For<IPropertyResolver>();
            var typeCapability = Substitute.For<IBuildCapability>();
            var valueCapability = Substitute.For<IBuildCapability>();

            typeCapability.AutoPopulate.Returns(true);
            typeCapability.SupportsCreate.Returns(true);
            typeCapability.SupportsPopulate.Returns(true);
            typeCapability.ImplementedByType.Returns(typeof(DummyTypeCreator));
            valueCapability.SupportsCreate.Returns(true);
            valueCapability.ImplementedByType.Returns(typeof(DummyTypeCreator));

            var sut = new DefaultExecuteStrategy<SlimModel>(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(sut, BuildRequirement.Populate,
                    typeof(SlimModel))
                .Returns(typeCapability);
            buildConfiguration.PropertyResolver.Returns(propertyResolver);
            propertyResolver.GetOrderedProperties(buildConfiguration, typeof(SlimModel))
                .Returns(properties);
            processor.GetBuildCapability(sut, BuildRequirement.Create, propertyInfo)
                .Returns(valueCapability);
            valueCapability.CreateProperty(sut, propertyInfo, Arg.Any<object?[]?>())
                .Returns(expected);
            processor.GetBuildCapability(sut, BuildRequirement.Populate,
                    typeof(Guid))
                .Returns(valueCapability);
            typeCapability.Populate(sut, model).Returns(model);

            sut.Initialize(buildConfiguration);

            var actual = sut.Populate(model);

            actual.Value.Should().Be(expected);
        }

        [Fact]
        public void PopulateThrowsExceptionWithNullInstance()
        {
            var buildHistory = new BuildHistory();

            var processor = Substitute.For<IBuildProcessor>();

            var sut = new DefaultExecuteStrategy<Person>(buildHistory, _buildLog, processor);

            Action action = () => sut.Populate(null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}