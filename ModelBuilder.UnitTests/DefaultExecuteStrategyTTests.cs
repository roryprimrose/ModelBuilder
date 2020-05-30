namespace ModelBuilder.UnitTests
{
    using System;
    using System.Linq;
    using System.Reflection;
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
            var typeCapability = new BuildCapability(GetType())
            {
                SupportsPopulate = false,
                AutoDetectConstructor = true,
                AutoPopulate = false,
                SupportsCreate = true
            };

            var processor = Substitute.For<IBuildProcessor>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var propertyResolver = Substitute.For<IPropertyResolver>();

            var sut = new DefaultExecuteStrategy<Person>(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(buildConfiguration, buildHistory, Arg.Any<BuildRequirement>(),
                    typeof(Person))
                .Returns(typeCapability);
            processor.Build(sut, typeof(Person), args).Returns(expected);
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
            var typeCapability = new BuildCapability(GetType())
            {
                SupportsPopulate = false,
                AutoDetectConstructor = true,
                AutoPopulate = false,
                SupportsCreate = true
            };

            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var constructorResolver = Substitute.For<IConstructorResolver>();
            var typeResolver = Substitute.For<ITypeResolver>();
            var processor = Substitute.For<IBuildProcessor>();

            buildConfiguration.ConstructorResolver.Returns(constructorResolver);
            buildConfiguration.TypeResolver.Returns(typeResolver);
            typeResolver.GetBuildType(buildConfiguration, typeof(Person)).Returns(x => x.Arg<Type>());

            var sut = new DefaultExecuteStrategy<Person>(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(buildConfiguration, buildHistory, Arg.Any<BuildRequirement>(),
                    typeof(Person))
                .Returns(typeCapability);
            constructorResolver.Resolve(typeof(Person))
                .Returns(typeof(Person).GetConstructors().Single(x => x.GetParameters().Length == 0));
            processor.Build(sut, typeof(Person), null!).Returns(expected);
            processor.Populate(sut, expected).Returns(expected);

            sut.Initialize(buildConfiguration);

            var actual = sut.Create();

            actual.Should().BeSameAs(expected);
        }

        [Fact]
        public void CreateThrowsExceptionWhenProcessorReturnsNull()
        {
            var buildHistory = new BuildHistory();
            var typeCapability = new BuildCapability(GetType())
            {
                SupportsPopulate = false,
                AutoDetectConstructor = false,
                AutoPopulate = false,
                SupportsCreate = true
            };

            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var processor = Substitute.For<IBuildProcessor>();

            var sut = new DefaultExecuteStrategy<int>(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(buildConfiguration, buildHistory, Arg.Any<BuildRequirement>(),
                    typeof(int))
                .Returns(typeCapability);
            processor.Build(sut, typeof(int), null!).Returns((object) null!);

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
            var typeCapability = new BuildCapability(GetType())
            {
                SupportsPopulate = true,
                AutoDetectConstructor = false,
                AutoPopulate = true,
                SupportsCreate = true
            };
            var valueCapability = new BuildCapability(GetType())
            {
                SupportsPopulate = false,
                AutoDetectConstructor = false,
                AutoPopulate = false,
                SupportsCreate = true
            };

            var processor = Substitute.For<IBuildProcessor>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var propertyResolver = Substitute.For<IPropertyResolver>();

            var sut = new DefaultExecuteStrategy<SlimModel>(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(buildConfiguration, buildHistory, BuildRequirement.Populate,
                    typeof(SlimModel))
                .Returns(typeCapability);
            buildConfiguration.PropertyResolver.Returns(propertyResolver);
            propertyResolver.GetOrderedProperties(buildConfiguration, typeof(SlimModel))
                .Returns(typeof(SlimModel).GetProperties());
            processor.GetBuildCapability(buildConfiguration, buildHistory, BuildRequirement.Create,
                    Arg.Is<PropertyInfo>(x => x.Name == nameof(SlimModel.Value)))
                .Returns(valueCapability);
            processor.Build(sut, Arg.Is<PropertyInfo>(x => x.Name == nameof(SlimModel.Value)), null!)
                .Returns(expected);
            processor.GetBuildCapability(buildConfiguration, buildHistory, BuildRequirement.Populate,
                    typeof(Guid))
                .Returns(valueCapability);
            processor.Populate(sut, model).Returns(model);

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