﻿namespace ModelBuilder.UnitTests
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
        public void CreateReturnsDefaultWhenNullReturnedByProcessorTest()
        {
            var buildHistory = new BuildHistory();
            var typeCapability = new BuildCapability
            {
                SupportsPopulate = false,
                ImplementedByType = GetType(),
                AutoDetectConstructor = false,
                AutoPopulate = false,
                SupportsCreate = true
            };

            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var processor = Substitute.For<IBuildProcessor>();

            var target = new DefaultExecuteStrategy<int>(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(buildConfiguration, buildHistory, Arg.Any<BuildRequirement>(),
                    typeof(int))
                .Returns(typeCapability);
            processor.Build(target, typeof(int), null).Returns((object) null);

            target.Initialize(buildConfiguration);

            var actual = target.Create();

            actual.Should().Be(0);
        }

        [Fact]
        public void CreateReturnsNullCalculatedByProcessorTest()
        {
            var buildHistory = new BuildHistory();
            var typeCapability = new BuildCapability
            {
                SupportsPopulate = false,
                ImplementedByType = GetType(),
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

            var target = new DefaultExecuteStrategy<Person>(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(buildConfiguration, buildHistory, Arg.Any<BuildRequirement>(),
                    typeof(Person))
                .Returns(typeCapability);
            constructorResolver.Resolve(typeof(Person))
                .Returns(typeof(Person).GetConstructors().Single(x => x.GetParameters().Length == 0));
            processor.Build(target, typeof(Person), null).Returns((Person) null);

            target.Initialize(buildConfiguration);

            var actual = target.Create();

            actual.Should().BeNull();
        }

        [Fact]
        public void CreateReturnsValueCreatedFromProvidedArgumentsTest()
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
            var typeCapability = new BuildCapability
            {
                SupportsPopulate = false,
                ImplementedByType = GetType(),
                AutoDetectConstructor = true,
                AutoPopulate = false,
                SupportsCreate = true
            };

            var processor = Substitute.For<IBuildProcessor>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var propertyResolver = Substitute.For<IPropertyResolver>();

            var target = new DefaultExecuteStrategy<Person>(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(buildConfiguration, buildHistory, Arg.Any<BuildRequirement>(),
                    typeof(Person))
                .Returns(typeCapability);
            processor.Build(target, typeof(Person), args).Returns(expected);
            buildConfiguration.PropertyResolver.Returns(propertyResolver);

            target.Initialize(buildConfiguration);

            var actual = target.Create(args);

            actual.Should().BeSameAs(expected);
        }

        [Fact]
        public void CreateReturnsValueCreatedWithoutArgumentsTest()
        {
            var buildHistory = new BuildHistory();
            var expected = new Person();
            var typeCapability = new BuildCapability
            {
                SupportsPopulate = false,
                ImplementedByType = GetType(),
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

            var target = new DefaultExecuteStrategy<Person>(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(buildConfiguration, buildHistory, Arg.Any<BuildRequirement>(),
                    typeof(Person))
                .Returns(typeCapability);
            constructorResolver.Resolve(typeof(Person))
                .Returns(typeof(Person).GetConstructors().Single(x => x.GetParameters().Length == 0));
            processor.Build(target, typeof(Person), null).Returns(expected);
            processor.Populate(target, expected).Returns(expected);

            target.Initialize(buildConfiguration);

            var actual = target.Create();

            actual.Should().BeSameAs(expected);
        }

        [Fact]
        public void PopulateAssignsPropertyValuesToExistingInstanceTest()
        {
            var buildHistory = new BuildHistory();
            var model = new SlimModel();
            var expected = Guid.NewGuid();
            var typeCapability = new BuildCapability
            {
                SupportsPopulate = true,
                ImplementedByType = GetType(),
                AutoDetectConstructor = false,
                AutoPopulate = true,
                SupportsCreate = true
            };
            var valueCapability = new BuildCapability
            {
                SupportsPopulate = false,
                ImplementedByType = GetType(),
                AutoDetectConstructor = false,
                AutoPopulate = false,
                SupportsCreate = true
            };

            var processor = Substitute.For<IBuildProcessor>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var propertyResolver = Substitute.For<IPropertyResolver>();

            var target = new DefaultExecuteStrategy<SlimModel>(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(buildConfiguration, buildHistory, BuildRequirement.Populate,
                    typeof(SlimModel))
                .Returns(typeCapability);
            buildConfiguration.PropertyResolver.Returns(propertyResolver);
            propertyResolver.CanPopulate(Arg.Is<PropertyInfo>(x => x.Name == nameof(SlimModel.Value)))
                .Returns(true);
            propertyResolver.ShouldPopulateProperty(
                buildConfiguration,
                model,
                Arg.Is<PropertyInfo>(x => x.Name == nameof(SlimModel.Value)),
                Arg.Any<object[]>()).Returns(true);
            processor.GetBuildCapability(buildConfiguration, buildHistory, BuildRequirement.Create,
                    Arg.Is<PropertyInfo>(x => x.Name == nameof(SlimModel.Value)))
                .Returns(valueCapability);
            processor.Build(target, Arg.Is<PropertyInfo>(x => x.Name == nameof(SlimModel.Value)), null)
                .Returns(expected);
            processor.GetBuildCapability(buildConfiguration, buildHistory, BuildRequirement.Populate,
                    typeof(Guid))
                .Returns(valueCapability);
            processor.Populate(target, model).Returns(model);

            target.Initialize(buildConfiguration);

            var actual = target.Populate(model);

            actual.Value.Should().Be(expected);
        }
    }
}