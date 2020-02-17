namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using FluentAssertions;
    using ModelBuilder.BuildActions;
    using ModelBuilder.CreationRules;
    using ModelBuilder.IgnoreRules;
    using ModelBuilder.TypeCreators;
    using ModelBuilder.UnitTests.Models;
    using ModelBuilder.ValueGenerators;
    using NSubstitute;
    using Xunit;
    using Xunit.Abstractions;

    public class DefaultExecuteStrategyTests
    {
        private readonly IBuildLog _buildLog;
        private readonly ITestOutputHelper _output;

        public DefaultExecuteStrategyTests(ITestOutputHelper output)
        {
            _output = output;
            _buildLog = new OutputBuildLog(output);
        }

        [Fact]
        public void BuildChainShouldBeEmptyAfterCreateCompletedTest()
        {
            var buildHistory = new BuildHistory();
            var expected = new SlimModel();
            var typeCapability = new BuildCapability
            {
                SupportsPopulate = false,
                ImplementedByType = GetType(),
                AutoDetectConstructor = false,
                AutoPopulate = true,
                SupportsCreate = true
            };

            var processor = Substitute.For<IBuildProcessor>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();

            var target = new DefaultExecuteStrategy(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(buildConfiguration, buildHistory, Arg.Any<BuildRequirement>(),
                    typeof(SlimModel))
                .Returns(typeCapability);
            processor.Build(target, typeof(SlimModel), Arg.Any<object[]>()).Returns(expected);

            target.Initialize(buildConfiguration);

            target.BuildChain.Should().BeEmpty();

            target.Create(typeof(SlimModel));

            target.BuildChain.Should().BeEmpty();
        }

        [Fact]
        public void BuildThrowsExceptionWhenNotInitializedTest()
        {
            var type = typeof(Person);

            var target = new BuildWrapper();

            Action action = () => target.RunTest(type);

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void BuildThrowsExceptionWithNullTypeTest()
        {
            var target = new BuildWrapper();

            Action action = () => target.RunTest(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateDeterminesPropertiesToCreateByProvidingConstructorArgsForNestedTypeTest()
        {
            var buildHistory = new BuildHistory();
            var model = new SlimModel();
            var expected = new SimpleConstructor(model);
            var age = Environment.TickCount;
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

            var target = new DefaultExecuteStrategy(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(buildConfiguration, buildHistory, Arg.Any<BuildRequirement>(),
                    typeof(SimpleConstructor))
                .Returns(typeCapability);
            processor.GetBuildCapability(buildConfiguration, buildHistory, Arg.Any<BuildRequirement>(),
                    Arg.Any<PropertyInfo>())
                .Returns(valueCapability);
            processor.GetBuildCapability(buildConfiguration, buildHistory, Arg.Any<BuildRequirement>(),
                    typeof(int))
                .Returns(valueCapability);
            processor.Build(target, typeof(SimpleConstructor), Arg.Any<object[]>()).Returns(expected);
            processor.Build(target, Arg.Is<PropertyInfo>(x => x.Name == nameof(SimpleConstructor.Model)),
                Arg.Any<object[]>()).Returns(model);
            processor.Build(target, Arg.Is<PropertyInfo>(x => x.Name == nameof(SimpleConstructor.Age)),
                Arg.Any<object[]>()).Returns(age);
            processor.Populate(target, expected).Returns(expected);
            buildConfiguration.PropertyResolver.Returns(propertyResolver);
            propertyResolver.CanPopulate(Arg.Is<PropertyInfo>(x => x.Name == nameof(SimpleConstructor.Model)))
                .Returns(false);
            propertyResolver.CanPopulate(Arg.Is<PropertyInfo>(x => x.Name == nameof(SimpleConstructor.Age)))
                .Returns(true);
            propertyResolver.ShouldPopulateProperty(
                Arg.Any<IBuildConfiguration>(),
                expected,
                Arg.Is<PropertyInfo>(x => x.Name == nameof(SimpleConstructor.Age)),
                Arg.Any<object[]>()).Returns(true);

            target.Initialize(buildConfiguration);

            var actual = (SimpleConstructor) target.Create(typeof(SimpleConstructor), model);

            actual.Should().Be(expected);
            actual.Model.Should().Be(model);
            actual.Age.Should().Be(age);

            propertyResolver.Received(1).ShouldPopulateProperty(
                buildConfiguration,
                Arg.Is<object>(x => x.GetType() == typeof(SimpleConstructor)),
                Arg.Is<PropertyInfo>(x => x.Name == nameof(SimpleConstructor.Age)),
                Arg.Is<object[]>(x => x[0] == model));
        }

        [Fact]
        public void CreateDoesNotBuildPropertiesWhenCapabilityDoesNotSupportPopulationTest()
        {
            var buildHistory = new BuildHistory();
            var expected = new SlimModel();
            var typeCapability = new BuildCapability
            {
                SupportsPopulate = false,
                ImplementedByType = GetType(),
                AutoDetectConstructor = false,
                AutoPopulate = true,
                SupportsCreate = true
            };

            var processor = Substitute.For<IBuildProcessor>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();

            var target = new DefaultExecuteStrategy(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(buildConfiguration, buildHistory, Arg.Any<BuildRequirement>(),
                    typeof(SlimModel))
                .Returns(typeCapability);
            processor.Build(target, typeof(SlimModel), Arg.Any<object[]>()).Returns(expected);

            target.Initialize(buildConfiguration);

            var actual = (SlimModel) target.Create(typeof(SlimModel));

            actual.Should().Be(expected);
            actual.Value.Should().BeEmpty();

            processor.DidNotReceive().Populate(target, expected);
        }

        [Fact]
        public void CreateEvaluatesPostBuildActionsInOrderOfDescendingPriorityTest()
        {
            var firstAction = Substitute.For<IPostBuildAction>();
            var secondAction = Substitute.For<IPostBuildAction>();
            var buildConfiguration = Model.UsingDefaultConfiguration().Add(firstAction).Add(secondAction);
            var executeCount = 0;

            firstAction.IsMatch(typeof(Simple), Arg.Any<string>(), Arg.Any<IBuildChain>()).Returns(true);
            firstAction.Priority.Returns(int.MaxValue);
            secondAction.IsMatch(typeof(Simple), Arg.Any<string>(), Arg.Any<IBuildChain>()).Returns(true);
            secondAction.Priority.Returns(int.MinValue);
            firstAction.WhenForAnyArgs(x => x.Execute(null, null, null)).Do(
                x =>
                {
                    executeCount++;

                    executeCount.Should().Be(1);
                });
            secondAction.WhenForAnyArgs(x => x.Execute(null, null, null)).Do(
                x =>
                {
                    executeCount++;

                    executeCount.Should().Be(2);
                });

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            target.Create(typeof(Simple));

            firstAction.Received().Execute(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<IBuildChain>());
            secondAction.Received().Execute(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<IBuildChain>());
        }

        [Fact]
        public void CreateEvaluatesPostBuildActionsOfNestedInstancesExposedAsReadOnlyPropertiesTest()
        {
            var postBuildAction = Substitute.For<IPostBuildAction>();
            var buildConfiguration = Model.UsingDefaultConfiguration().Add(postBuildAction);

            postBuildAction.IsMatch(typeof(Company), nameof(ReadOnlyParent.Company), Arg.Any<IBuildChain>())
                .Returns(true);

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            target.Create(typeof(ReadOnlyParent));

            postBuildAction.Received().Execute(typeof(Company), nameof(ReadOnlyParent.Company), Arg.Any<IBuildChain>());
        }

        [Fact]
        public void CreateEvaluatesPostBuildActionsThatSupportTheBuildScenarioTest()
        {
            var firstAction = Substitute.For<IPostBuildAction>();
            var secondAction = Substitute.For<IPostBuildAction>();
            var buildConfiguration = Model.UsingDefaultConfiguration().Add(firstAction).Add(secondAction);

            firstAction.IsMatch(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<IBuildChain>()).Returns(false);
            secondAction.IsMatch(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<IBuildChain>()).Returns(true);

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            target.Create(typeof(Simple));

            firstAction.DidNotReceive().Execute(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<IBuildChain>());
            secondAction.Received().Execute(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<IBuildChain>());
        }

        [Fact]
        public void CreateEvaluatesPostBuildActionsWhenCapabilityDoesNotSupportPopulationTest()
        {
            var action = Substitute.For<IPostBuildAction>();
            var buildConfiguration = Model.UsingDefaultConfiguration().Add(action);

            action.IsMatch(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<IBuildChain>()).Returns(true);

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            target.Create(typeof(SlimModel));

            action.Received().Execute(typeof(SlimModel), null, Arg.Any<IBuildChain>());
            action.Received().Execute(typeof(Guid), nameof(SlimModel.Value), Arg.Any<IBuildChain>());
        }

        [Fact]
        public void CreatePopulatesWithProcessorWhenAutoPopulateDisabledTest()
        {
            var buildHistory = new BuildHistory();
            var expected = new SlimModel();
            var typeCapability = new BuildCapability
            {
                SupportsPopulate = true,
                ImplementedByType = GetType(),
                AutoDetectConstructor = false,
                AutoPopulate = false,
                SupportsCreate = true
            };

            var processor = Substitute.For<IBuildProcessor>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();

            var target = new DefaultExecuteStrategy(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(buildConfiguration, buildHistory, Arg.Any<BuildRequirement>(),
                    typeof(SlimModel))
                .Returns(typeCapability);
            processor.Build(target, typeof(SlimModel), Arg.Any<object[]>()).Returns(expected);
            processor.Populate(target, expected).Returns(expected);

            target.Initialize(buildConfiguration);

            var actual = (SlimModel) target.Create(typeof(SlimModel));

            actual.Should().Be(expected);
            actual.Value.Should().BeEmpty();

            processor.Received().Populate(target, expected);
        }

        [Fact]
        public void CreateReturnsNullFromProcessor()
        {
            var buildHistory = new BuildHistory();
            var typeCapability = new BuildCapability
            {
                SupportsPopulate = true,
                ImplementedByType = GetType(),
                AutoDetectConstructor = false,
                AutoPopulate = false,
                SupportsCreate = true
            };

            var processor = Substitute.For<IBuildProcessor>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();

            var target = new DefaultExecuteStrategy(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(buildConfiguration, buildHistory, Arg.Any<BuildRequirement>(),
                    typeof(SlimModel))
                .Returns(typeCapability);

            target.Initialize(buildConfiguration);

            var actual = target.Create(typeof(SlimModel));

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

            var target = new DefaultExecuteStrategy(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(buildConfiguration, buildHistory, Arg.Any<BuildRequirement>(),
                    typeof(Person))
                .Returns(typeCapability);
            processor.Build(target, typeof(Person), args).Returns(expected);
            buildConfiguration.PropertyResolver.Returns(propertyResolver);

            target.Initialize(buildConfiguration);

            var actual = target.Create(typeof(Person), args);

            actual.Should().BeSameAs(expected);
        }

        [Fact]
        public void CreateReturnsValueFromProcessorWithPropertyPopulationTest()
        {
            var buildHistory = new BuildHistory();
            var expected = new SlimModel();
            var value = Guid.NewGuid();
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

            var target = new DefaultExecuteStrategy(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(buildConfiguration, buildHistory, Arg.Any<BuildRequirement>(),
                    typeof(SlimModel))
                .Returns(typeCapability);
            processor.GetBuildCapability(buildConfiguration, buildHistory, Arg.Any<BuildRequirement>(),
                    Arg.Any<PropertyInfo>())
                .Returns(valueCapability);
            processor.GetBuildCapability(buildConfiguration, buildHistory, Arg.Any<BuildRequirement>(),
                    typeof(Guid))
                .Returns(valueCapability);
            processor.Build(target, typeof(SlimModel), Arg.Any<object[]>()).Returns(expected);
            processor.Build(target, Arg.Any<PropertyInfo>(), Arg.Any<object[]>()).Returns(value);
            processor.Populate(target, expected).Returns(expected);
            buildConfiguration.PropertyResolver.Returns(propertyResolver);
            propertyResolver.CanPopulate(Arg.Any<PropertyInfo>()).Returns(true);
            propertyResolver.ShouldPopulateProperty(
                Arg.Any<IBuildConfiguration>(),
                Arg.Any<object>(),
                Arg.Any<PropertyInfo>(),
                Arg.Any<object[]>()).Returns(true);

            target.Initialize(buildConfiguration);

            var actual = (SlimModel) target.Create(typeof(SlimModel));

            actual.Should().Be(expected);
            actual.Value.Should().Be(value);
        }

        [Fact]
        public void CreateReturnsValueFromResolvedConstructorAndBuiltParametersTest()
        {
            var value = Guid.NewGuid();
            var expected = new ReadOnlyModel(value);
            var buildHistory = new BuildHistory();
            var typeCapability = new BuildCapability
            {
                SupportsPopulate = false,
                ImplementedByType = GetType(),
                AutoDetectConstructor = true,
                AutoPopulate = false,
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
            var constructorResolver = Substitute.For<IConstructorResolver>();

            var target = new DefaultExecuteStrategy(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(buildConfiguration, buildHistory, Arg.Any<BuildRequirement>(),
                    typeof(ReadOnlyModel))
                .Returns(typeCapability);
            processor.GetBuildCapability(buildConfiguration, buildHistory, BuildRequirement.Create,
                    Arg.Any<ParameterInfo>())
                .Returns(valueCapability);
            processor.GetBuildCapability(buildConfiguration, buildHistory, BuildRequirement.Populate,
                    typeof(Guid))
                .Returns(valueCapability);
            processor.Build(target, Arg.Is<ParameterInfo>(x => x.Name == "value"),
                Arg.Any<object[]>()).Returns(value);
            processor.Build(target, typeof(ReadOnlyModel), Arg.Is<object[]>(x => x.Length == 1 && (Guid) x[0] == value))
                .Returns(expected);
            processor.Populate(target, expected).Returns(expected);
            buildConfiguration.ConstructorResolver.Returns(constructorResolver);
            constructorResolver.Resolve(typeof(ReadOnlyModel), null)
                .Returns(typeof(ReadOnlyModel).GetConstructors().Single());

            target.Initialize(buildConfiguration);

            var actual = (ReadOnlyModel) target.Create(typeof(ReadOnlyModel));

            actual.Should().Be(expected);
            actual.Value.Should().Be(value);
        }

        [Fact]
        public void CreateReturnsValueWithAutomaticTypeMappingTest()
        {
            var expected = new TestItem();
            var firstName = Guid.NewGuid().ToString();
            var buildHistory = new BuildHistory();
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

            var target = new DefaultExecuteStrategy(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(buildConfiguration, buildHistory, Arg.Any<BuildRequirement>(),
                    typeof(TestItem))
                .Returns(typeCapability);
            processor.GetBuildCapability(buildConfiguration, buildHistory, Arg.Any<BuildRequirement>(),
                    Arg.Any<PropertyInfo>())
                .Returns(valueCapability);
            processor.GetBuildCapability(buildConfiguration, buildHistory, Arg.Any<BuildRequirement>(),
                    typeof(string))
                .Returns(valueCapability);
            processor.Build(target, typeof(TestItem), Arg.Any<object[]>()).Returns(expected);
            processor.Build(target, Arg.Is<PropertyInfo>(x => x.Name == nameof(TestItem.FirstName)),
                Arg.Any<object[]>()).Returns(firstName);
            processor.Populate(target, expected).Returns(expected);
            buildConfiguration.PropertyResolver.Returns(propertyResolver);
            propertyResolver.CanPopulate(Arg.Is<PropertyInfo>(x => x.Name == nameof(TestItem.FirstName)))
                .Returns(true);
            propertyResolver.ShouldPopulateProperty(
                Arg.Any<IBuildConfiguration>(),
                expected,
                Arg.Is<PropertyInfo>(x => x.Name == nameof(TestItem.FirstName)),
                Arg.Any<object[]>()).Returns(true);

            target.Initialize(buildConfiguration);

            var actual = (ITestItem) target.Create(typeof(ITestItem));

            actual.Should().BeOfType<TestItem>();
            actual.FirstName.Should().Be(firstName);
        }

        [Fact]
        public void CreateReturnsValueWithNoArgumentsAndDetectConstructorEnabledCreatedUsingEmptyConstructorTest()
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
            var processor = Substitute.For<IBuildProcessor>();

            buildConfiguration.ConstructorResolver.Returns(constructorResolver);

            var target = new DefaultExecuteStrategy(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(buildConfiguration, buildHistory, Arg.Any<BuildRequirement>(),
                    typeof(Person))
                .Returns(typeCapability);
            constructorResolver.Resolve(typeof(Person), null)
                .Returns(typeof(Person).GetConstructors().Single(x => x.GetParameters().Length == 0));
            processor.Build(target, typeof(Person), null).Returns(expected);
            processor.Populate(target, expected).Returns(expected);

            target.Initialize(buildConfiguration);

            var actual = target.Create(typeof(Person));

            actual.Should().BeSameAs(expected);
        }

        [Fact]
        public void CreateThrowsExceptionWhenAutomaticTypeMappingCantFindMatchTest()
        {
            var buildConfiguration = Substitute.For<IBuildConfiguration>();

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            Action action = () => target.Create(typeof(ICantCreate));

            action.Should().Throw<BuildException>();
        }

        [Fact]
        public void CreateThrowsExceptionWhenDerivedImplementationSuppliesNullTypeTest()
        {
            var buildConfiguration = Substitute.For<IBuildConfiguration>();

            var target = new NullTypeBuildExecuteStrategy();

            target.Initialize(buildConfiguration);

            Action action = () => target.Create(typeof(int));

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateThrowsExceptionWhenNoCapabilityFoundForParameterTest()
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

            var processor = Substitute.For<IBuildProcessor>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var constructorResolver = Substitute.For<IConstructorResolver>();

            var target = new DefaultExecuteStrategy(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(buildConfiguration, buildHistory, Arg.Any<BuildRequirement>(),
                    typeof(ReadOnlyModel))
                .Returns(typeCapability);
            constructorResolver.Resolve(typeof(ReadOnlyModel), null)
                .Returns(typeof(ReadOnlyModel).GetConstructors().Single());
            buildConfiguration.ConstructorResolver.Returns(constructorResolver);

            target.Initialize(buildConfiguration);

            try
            {
                Action action = () => target.Create(typeof(ReadOnlyModel));

                action.Should().Throw<BuildException>();
            }
            finally
            {
                constructorResolver.Received().Resolve(typeof(ReadOnlyModel), null);
            }
        }

        [Fact]
        public void CreateThrowsExceptionWhenNoCapabilityFoundForPropertyTest()
        {
            var buildHistory = new BuildHistory();
            var expected = new SlimModel();
            var typeCapability = new BuildCapability
            {
                SupportsPopulate = true,
                ImplementedByType = GetType(),
                AutoDetectConstructor = false,
                AutoPopulate = true,
                SupportsCreate = true
            };

            var processor = Substitute.For<IBuildProcessor>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var propertyResolver = Substitute.For<IPropertyResolver>();

            var target = new DefaultExecuteStrategy(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(buildConfiguration, buildHistory, Arg.Any<BuildRequirement>(),
                    typeof(SlimModel))
                .Returns(typeCapability);
            processor.Build(target, typeof(SlimModel), null).Returns(expected);
            processor.Populate(target, expected).Returns(expected);
            buildConfiguration.PropertyResolver.Returns(propertyResolver);
            propertyResolver.CanPopulate(Arg.Is<PropertyInfo>(x => x.Name == nameof(SlimModel.Value)))
                .Returns(true);
            propertyResolver.ShouldPopulateProperty(
                Arg.Any<IBuildConfiguration>(),
                expected,
                Arg.Is<PropertyInfo>(x => x.Name == nameof(SlimModel.Value)),
                Arg.Any<object[]>()).Returns(true);

            target.Initialize(buildConfiguration);

            Action action = () => target.Create(typeof(SlimModel));

            action.Should().Throw<BuildException>();
        }

        [Fact]
        public void CreateThrowsExceptionWhenNoCapabilityFoundForTypeTest()
        {
            var buildHistory = new BuildHistory();

            var processor = Substitute.For<IBuildProcessor>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();

            var target = new DefaultExecuteStrategy(buildHistory, _buildLog, processor);

            target.Initialize(buildConfiguration);

            Action action = () => target.Create(typeof(SlimModel));

            action.Should().Throw<BuildException>();
        }

        [Fact]
        public void CreateThrowsExceptionWhenNotInitializedTest()
        {
            var target = new DefaultExecuteStrategy();

            Action action = () => target.Create(typeof(Person));

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void CreateThrowsExceptionWithNullTypeTest()
        {
            var buildConfiguration = Substitute.For<IBuildConfiguration>();

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            Action action = () => target.Create(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void InitializeStoresConfigurationTest()
        {
            var configuration = Substitute.For<IBuildConfiguration>();

            var target = new DefaultExecuteStrategy();

            target.Initialize(configuration);

            target.Configuration.Should().BeSameAs(configuration);
        }

        [Fact]
        public void InitializeThrowsExceptionWithNullBuildConfigurationTest()
        {
            var target = new DefaultExecuteStrategy();

            Action action = () => target.Initialize(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsCreatedWithBuildChainInstanceTest()
        {
            var target = new DefaultExecuteStrategy();

            target.BuildChain.Should().NotBeNull();
        }

        [Fact]
        public void IsCreatedWithNullBuildConfigurationAndBuildLogTest()
        {
            var target = new DefaultExecuteStrategy();

            target.Configuration.Should().BeNull();
            target.Log.Should().NotBeNull();
            target.BuildChain.Should().NotBeNull();
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

            var target = new DefaultExecuteStrategy(buildHistory, _buildLog, processor);

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

            var actual = (SlimModel)target.Populate(model);
            
            actual.Value.Should().Be(expected);
        }

        [Fact]
        public void PopulateEvaluatesPostBuildActionsInOrderOfDescendingPriorityTest()
        {
            var expected = new Simple();
            var typeCreator = Substitute.For<ITypeCreator>();
            var firstAction = Substitute.For<IPostBuildAction>();
            var secondAction = Substitute.For<IPostBuildAction>();
            var buildConfiguration = Model.UsingDefaultConfiguration().Add(firstAction).Add(secondAction);
            var executeCount = 0;

            typeCreator.CanPopulate(typeof(Simple), null, Arg.Any<IBuildChain>()).Returns(true);
            typeCreator.Populate(expected, Arg.Any<IExecuteStrategy>()).Returns(expected);
            firstAction.IsMatch(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<IBuildChain>()).Returns(true);
            secondAction.IsMatch(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<IBuildChain>()).Returns(true);
            firstAction.WhenForAnyArgs(x => x.Execute(null, null, null)).Do(
                x =>
                {
                    executeCount++;

                    executeCount.Should().Be(1);
                });
            secondAction.WhenForAnyArgs(x => x.Execute(null, null, null)).Do(
                x =>
                {
                    executeCount++;

                    executeCount.Should().Be(2);
                });

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            target.Populate(expected);

            firstAction.Received().Execute(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<IBuildChain>());
            secondAction.Received().Execute(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<IBuildChain>());
        }

        [Fact]
        public void PopulateEvaluatesPostBuildActionsThatSupportTheBuildScenarioTest()
        {
            var expected = new Simple();
            var typeCreator = Substitute.For<ITypeCreator>();
            var firstAction = Substitute.For<IPostBuildAction>();
            var secondAction = Substitute.For<IPostBuildAction>();
            var buildConfiguration = Model.UsingDefaultConfiguration().Add(firstAction).Add(secondAction);

            typeCreator.CanPopulate(typeof(Simple), null, Arg.Any<IBuildChain>()).Returns(true);
            typeCreator.Populate(expected, Arg.Any<IExecuteStrategy>()).Returns(expected);
            firstAction.IsMatch(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<IBuildChain>()).Returns(false);
            secondAction.IsMatch(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<IBuildChain>()).Returns(true);

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            target.Populate(expected);

            firstAction.DidNotReceive().Execute(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<IBuildChain>());
            secondAction.Received().Execute(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<IBuildChain>());
        }

        [Fact]
        public void PopulateIgnoresPropertyWhenMatchingIgnoreRuleFound()
        {
            var expected = new Company();

            var config = new BuildConfiguration().UsingModule<DefaultConfigurationModule>()
                .Ignoring<Company>(x => x.Name);

            var target = new DefaultExecuteStrategy();

            target.Initialize(config);

            var actual = (Company) target.Populate(expected);

            actual.Should().BeSameAs(expected);
            actual.Name.Should().BeNull();
            actual.Address.Should().NotBeNullOrWhiteSpace();
            actual.Staff.Should().NotBeEmpty();
        }

        [Fact]
        public void PopulateInstanceThrowsExceptionWhenNotInitializedTest()
        {
            var value = new Person();

            var target = new PopulateInstanceWrapper();

            Action action = () => target.RunTest(value);

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void PopulateInstanceThrowsExceptionWithNullInstanceTest()
        {
            var target = new PopulateInstanceWrapper();

            Action action = () => target.RunTest();

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PopulatePropertyThrowsExceptionWhenNotInitializedTest()
        {
            var value = new Person();
            var property = typeof(Person).GetProperty(nameof(Person.FirstName));

            var target = new PopulatePropertyWrapper();

            Action action = () => target.RunTest(value, property);

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void PopulatePropertyThrowsExceptionWithNullInstanceTest()
        {
            var target = new PopulatePropertyWrapper();

            var property = typeof(Person).GetProperty("FirstName");

            Action action = () => target.RunTest(null, property);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PopulatePropertyThrowsExceptionWithNullPropertyInfoTest()
        {
            var target = new PopulatePropertyWrapper();

            Action action = () => target.RunTest(target);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PopulatePushesInstanceIntoBuildChainWhileCreatingTest()
        {
            var instance = new SlimModel();
            var testPassed = false;

            var creator = Substitute.For<ITypeCreator>();
            var generator = Substitute.For<IValueGenerator>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var propertyResolver = Substitute.For<IPropertyResolver>();

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            buildConfiguration.PropertyResolver.Returns(propertyResolver);
            propertyResolver.CanPopulate(Arg.Any<PropertyInfo>()).Returns(true);
            propertyResolver.ShouldPopulateProperty(
                Arg.Any<IBuildConfiguration>(),
                Arg.Any<object>(),
                Arg.Any<PropertyInfo>(),
                Arg.Any<object[]>()).Returns(true);
            buildConfiguration.CreationRules.Returns(new Collection<ICreationRule>());
            buildConfiguration.ValueGenerators.Returns(
                new Collection<IValueGenerator>
                {
                    generator
                });
            buildConfiguration.TypeCreators.Returns(
                new Collection<ITypeCreator>
                {
                    creator
                });
            creator.CanCreate(typeof(SlimModel), null, Arg.Any<IBuildChain>()).Returns(true);
            creator.Create(typeof(SlimModel), null, Arg.Any<IExecuteStrategy>()).Returns(instance);
            creator.Populate(instance, target).Returns(instance);
            creator.AutoPopulate.Returns(true);
            generator.When(
                x => x.Generate(
                    typeof(Guid),
                    nameof(SlimModel.Value),
                    Arg.Is<IExecuteStrategy>(y => y.BuildChain.Last == instance))).Do(
                x =>
                {
                    target.BuildChain.Should().HaveCount(1);
                    target.BuildChain.First().Should().BeOfType<SlimModel>();
                    testPassed = true;
                });

            generator.IsMatch(typeof(Guid), nameof(SlimModel.Value), Arg.Is<IBuildChain>(x => x.Last == instance))
                .Returns(true);

            target.Create(typeof(SlimModel));

            testPassed.Should().BeTrue();
        }

        [Fact]
        public void PopulatePushesInstanceIntoBuildChainWhilePopulatingTest()
        {
            var expected = new SlimModel();
            var testPassed = false;

            var generator = Substitute.For<IValueGenerator>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var propertyResolver = Substitute.For<IPropertyResolver>();
            var typeCreator = Substitute.For<ITypeCreator>();
            var typeCreators = new Collection<ITypeCreator>
            {
                typeCreator
            };

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            buildConfiguration.TypeCreators.Returns(typeCreators);
            buildConfiguration.PropertyResolver.Returns(propertyResolver);
            typeCreator.CanPopulate(typeof(SlimModel), null, Arg.Any<IBuildChain>()).Returns(true);
            typeCreator.Populate(expected, target).Returns(expected);
            typeCreator.AutoPopulate.Returns(true);
            propertyResolver.CanPopulate(Arg.Any<PropertyInfo>()).Returns(true);
            propertyResolver.ShouldPopulateProperty(
                Arg.Any<IBuildConfiguration>(),
                Arg.Any<object>(),
                Arg.Any<PropertyInfo>(),
                Arg.Any<object[]>()).Returns(true);
            buildConfiguration.CreationRules.Returns(new Collection<ICreationRule>());
            buildConfiguration.ValueGenerators.Returns(
                new Collection<IValueGenerator>
                {
                    generator
                });
            generator.When(
                x => x.Generate(
                    typeof(Guid),
                    nameof(SlimModel.Value),
                    Arg.Is<IExecuteStrategy>(y => y.BuildChain.Last == expected))).Do(
                x =>
                {
                    target.BuildChain.Should().HaveCount(1);
                    target.BuildChain.Should().Contain(expected);
                    testPassed = true;
                });

            generator.IsMatch(typeof(Guid), nameof(SlimModel.Value), Arg.Is<IBuildChain>(x => x.Last == expected))
                .Returns(true);

            target.Populate(expected);

            testPassed.Should().BeTrue();
        }

        [Fact]
        public void PopulatePushesNestedInstanceIntoBuildChainWhileCreatingTest()
        {
            var office = new Office();
            var address = new Address();
            var testPassed = false;

            var creator = Substitute.For<ITypeCreator>();
            var generator = Substitute.For<IValueGenerator>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var propertyResolver = Substitute.For<IPropertyResolver>();

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            buildConfiguration.PropertyResolver.Returns(propertyResolver);
            propertyResolver.CanPopulate(Arg.Any<PropertyInfo>()).Returns(true);
            propertyResolver.ShouldPopulateProperty(
                Arg.Any<IBuildConfiguration>(),
                Arg.Any<object>(),
                Arg.Any<PropertyInfo>(),
                Arg.Any<object[]>()).Returns(true);
            buildConfiguration.CreationRules.Returns(new Collection<ICreationRule>());
            buildConfiguration.ValueGenerators.Returns(
                new Collection<IValueGenerator>
                {
                    generator
                });
            buildConfiguration.TypeCreators.Returns(
                new Collection<ITypeCreator>
                {
                    creator
                });
            creator.CanCreate(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<IBuildChain>()).Returns(true);
            creator.Create(typeof(Office), null, Arg.Any<IExecuteStrategy>()).Returns(office);
            creator.Create(typeof(Address), "Address", Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last == office))
                .Returns(address);
            creator.AutoPopulate.Returns(true);
            creator.Populate(Arg.Any<object>(), target).Returns(x => x[0]);
            generator.When(
                x => x.Generate(
                    typeof(string),
                    Arg.Any<string>(),
                    Arg.Is<IExecuteStrategy>(y => y.BuildChain.Last == address))).Do(
                x =>
                {
                    target.BuildChain.Should().HaveCount(2);
                    target.BuildChain.First.Should().Be(office);
                    target.BuildChain.Last.Should().Be(address);
                    testPassed = true;
                });

            generator.IsMatch(typeof(string), Arg.Any<string>(), Arg.Any<IBuildChain>()).Returns(true);
            generator.Generate(typeof(string), Arg.Any<string>(), Arg.Any<IExecuteStrategy>())
                .Returns(Guid.NewGuid().ToString());

            target.Create(typeof(Office));

            testPassed.Should().BeTrue();
        }

        [Fact]
        public void PopulateReturnsInstanceFromBuildChainWhenCircularReferenceDetectedTest()
        {
            var expected = new SelfReferrer();
            var id = Guid.NewGuid();

            var typeCreators = new Collection<ITypeCreator>();
            var valueGenerators = new Collection<IValueGenerator>();

            var typeCreator = Substitute.For<ITypeCreator>();
            var generator = Substitute.For<IValueGenerator>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var propertyResolver = Substitute.For<IPropertyResolver>();

            typeCreators.Add(typeCreator);
            valueGenerators.Add(generator);

            buildConfiguration.PropertyResolver.Returns(propertyResolver);
            buildConfiguration.TypeCreators.Returns(typeCreators);
            buildConfiguration.ValueGenerators.Returns(valueGenerators);

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            propertyResolver.CanPopulate(Arg.Any<PropertyInfo>()).Returns(true);
            propertyResolver.ShouldPopulateProperty(
                Arg.Any<IBuildConfiguration>(),
                Arg.Any<object>(),
                Arg.Any<PropertyInfo>(),
                Arg.Any<object[]>()).Returns(true);
            typeCreator.CanPopulate(typeof(SelfReferrer), null, Arg.Any<IBuildChain>()).Returns(true);
            typeCreator.Populate(expected, target).Returns(expected);
            typeCreator.AutoPopulate.Returns(true);
            generator.IsMatch(typeof(Guid), "Id", Arg.Is<IBuildChain>(x => x.Last == expected)).Returns(true);
            generator.Generate(typeof(Guid), "Id", Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last == expected))
                .Returns(id);

            var actual = (SelfReferrer) target.Populate(expected);

            actual.Should().Be(expected);
            actual.Id.Should().Be(id);
        }

        [Fact]
        public void PopulateSkipsPropertiesThatCannotBePopulatedTest()
        {
            var staff = new List<Person>();
            var name = Guid.NewGuid().ToString();
            var address = Guid.NewGuid().ToString();
            var expected = new Company();
            var ignoreRule = new ExpressionIgnoreRule<Company>(x => x.Name);
            var valueGenerators = new Collection<IValueGenerator>();
            var typeCreators = new Collection<ITypeCreator>();
            var ignoreRules = new Collection<IIgnoreRule>
            {
                ignoreRule
            };

            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var typeCreator = Substitute.For<ITypeCreator>();
            var enumerableTypeCreator = Substitute.For<ITypeCreator>();
            var valueGenerator = Substitute.For<IValueGenerator>();
            var propertyResolver = Substitute.For<IPropertyResolver>();

            typeCreators.Add(typeCreator);
            typeCreators.Add(enumerableTypeCreator);
            valueGenerators.Add(valueGenerator);

            buildConfiguration.PropertyResolver.Returns(propertyResolver);
            propertyResolver.CanPopulate(Arg.Any<PropertyInfo>()).Returns(true);
            propertyResolver.CanPopulate(Arg.Is<PropertyInfo>(x => x.Name == nameof(Company.Name))).Returns(false);
            propertyResolver.ShouldPopulateProperty(
                Arg.Any<IBuildConfiguration>(),
                Arg.Any<object>(),
                Arg.Any<PropertyInfo>(),
                Arg.Any<object[]>()).Returns(true);
            buildConfiguration.TypeCreators.Returns(typeCreators);
            buildConfiguration.ValueGenerators.Returns(valueGenerators);
            buildConfiguration.IgnoreRules.Returns(ignoreRules);

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            typeCreator.CanPopulate(typeof(Company), null, Arg.Any<IBuildChain>()).Returns(true);
            typeCreator.Populate(expected, target).Returns(expected);
            typeCreator.AutoPopulate.Returns(true);
            enumerableTypeCreator.AutoPopulate.Returns(false);
            enumerableTypeCreator.CanCreate(
                typeof(IEnumerable<Person>),
                "Staff",
                Arg.Is<IBuildChain>(x => x.Last == expected)).Returns(true);
            enumerableTypeCreator.Create(
                typeof(IEnumerable<Person>),
                "Staff",
                Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last == expected)).Returns(staff);
            enumerableTypeCreator.Populate(staff, target).Returns(staff);
            valueGenerator.IsMatch(typeof(string), "Name", Arg.Is<IBuildChain>(x => x.Last == expected))
                .Returns(true);
            valueGenerator.Generate(
                typeof(string),
                "Name",
                Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last == expected)).Returns(name);
            valueGenerator.IsMatch(typeof(string), "Address", Arg.Is<IBuildChain>(x => x.Last == expected))
                .Returns(true);
            valueGenerator.Generate(
                typeof(string),
                "Address",
                Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last == expected)).Returns(address);

            var actual = (Company) target.Populate(expected);

            actual.Should().BeSameAs(expected);
            actual.Name.Should().BeNullOrEmpty();
            actual.Address.Should().Be(address);
            actual.Staff.Should().BeSameAs(staff);
        }

        [Fact]
        public void PopulateSkipsPropertiesThatShouldNotBePopulatedTest()
        {
            var staff = new List<Person>();
            var name = Guid.NewGuid().ToString();
            var address = Guid.NewGuid().ToString();
            var expected = new Company();
            var ignoreRule = new ExpressionIgnoreRule<Company>(x => x.Name);
            var valueGenerators = new Collection<IValueGenerator>();
            var typeCreators = new Collection<ITypeCreator>();
            var ignoreRules = new Collection<IIgnoreRule>
            {
                ignoreRule
            };

            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var typeCreator = Substitute.For<ITypeCreator>();
            var enumerableTypeCreator = Substitute.For<ITypeCreator>();
            var valueGenerator = Substitute.For<IValueGenerator>();
            var propertyResolver = Substitute.For<IPropertyResolver>();

            typeCreators.Add(typeCreator);
            typeCreators.Add(enumerableTypeCreator);
            valueGenerators.Add(valueGenerator);

            buildConfiguration.PropertyResolver.Returns(propertyResolver);
            propertyResolver.CanPopulate(Arg.Any<PropertyInfo>()).Returns(true);
            propertyResolver.ShouldPopulateProperty(
                Arg.Any<IBuildConfiguration>(),
                Arg.Any<object>(),
                Arg.Any<PropertyInfo>(),
                Arg.Any<object[]>()).Returns(true);
            propertyResolver.ShouldPopulateProperty(
                Arg.Any<IBuildConfiguration>(),
                expected,
                Arg.Is<PropertyInfo>(x => x.Name == nameof(Company.Name)),
                Arg.Any<object[]>()).Returns(false);
            buildConfiguration.TypeCreators.Returns(typeCreators);
            buildConfiguration.ValueGenerators.Returns(valueGenerators);
            buildConfiguration.IgnoreRules.Returns(ignoreRules);

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            typeCreator.CanPopulate(typeof(Company), null, Arg.Any<IBuildChain>()).Returns(true);
            typeCreator.Populate(expected, target).Returns(expected);
            typeCreator.AutoPopulate.Returns(true);
            enumerableTypeCreator.AutoPopulate.Returns(false);
            enumerableTypeCreator.CanCreate(
                typeof(IEnumerable<Person>),
                "Staff",
                Arg.Is<IBuildChain>(x => x.Last == expected)).Returns(true);
            enumerableTypeCreator.Create(
                typeof(IEnumerable<Person>),
                "Staff",
                Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last == expected)).Returns(staff);
            enumerableTypeCreator.Populate(staff, target).Returns(staff);
            valueGenerator.IsMatch(typeof(string), "Name", Arg.Is<IBuildChain>(x => x.Last == expected))
                .Returns(true);
            valueGenerator.Generate(
                typeof(string),
                "Name",
                Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last == expected)).Returns(name);
            valueGenerator.IsMatch(typeof(string), "Address", Arg.Is<IBuildChain>(x => x.Last == expected))
                .Returns(true);
            valueGenerator.Generate(
                typeof(string),
                "Address",
                Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last == expected)).Returns(address);

            var actual = (Company) target.Populate(expected);

            actual.Should().BeSameAs(expected);
            actual.Name.Should().BeNullOrEmpty();
            actual.Address.Should().Be(address);
            actual.Staff.Should().BeSameAs(staff);
        }

        [Fact]
        public void PopulateThrowsExceptionWhenMatchingTypeCreatorNotFoundTest()
        {
            var item = new List<string>();
            var typeCreator = Substitute.For<ITypeCreator>();
            var typeCreators = new Collection<ITypeCreator>
            {
                typeCreator
            };

            var buildConfiguration = Substitute.For<IBuildConfiguration>();

            buildConfiguration.TypeCreators.Returns(typeCreators);
            typeCreator.CanCreate(item.GetType(), null, Arg.Any<IBuildChain>()).Returns(true);
            typeCreator.CanPopulate(item.GetType(), null, Arg.Any<IBuildChain>()).Returns(false);

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            Action action = () => target.Populate(item);

            action.Should().Throw<BuildException>();
        }

        [Fact]
        public void PopulateThrowsExceptionWhenNotInitializedTest()
        {
            var value = new Person();

            var target = new DefaultExecuteStrategy();

            Action action = () => target.Populate(value);

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void PopulateThrowsExceptionWhenNoTypeCreatorNotFoundTest()
        {
            var item = new List<string>();
            var typeCreators = new Collection<ITypeCreator>();

            var buildConfiguration = Substitute.For<IBuildConfiguration>();

            buildConfiguration.TypeCreators.Returns(typeCreators);

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            Action action = () => target.Populate(item);

            action.Should().Throw<BuildException>();
        }

        [Fact]
        public void PopulateThrowsExceptionWithNullInstanceTest()
        {
            var target = new DefaultExecuteStrategy();

            Action action = () => target.Populate(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PopulateUsesTypeCreatorWithHighestPriorityTest()
        {
            var value = Guid.NewGuid();
            var expected = new SlimModel();
            var valueGenerators = new Collection<IValueGenerator>();
            var typeCreators = new Collection<ITypeCreator>();

            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var typeCreator = Substitute.For<ITypeCreator>();
            var otherTypeCreator = Substitute.For<ITypeCreator>();
            var valueGenerator = Substitute.For<IValueGenerator>();
            var propertyResolver = Substitute.For<IPropertyResolver>();

            typeCreators.Add(typeCreator);
            typeCreators.Add(otherTypeCreator);
            valueGenerators.Add(valueGenerator);

            buildConfiguration.PropertyResolver.Returns(propertyResolver);
            propertyResolver.CanPopulate(Arg.Any<PropertyInfo>()).Returns(true);
            propertyResolver.ShouldPopulateProperty(
                Arg.Any<IBuildConfiguration>(),
                Arg.Any<object>(),
                Arg.Any<PropertyInfo>(),
                Arg.Any<object[]>()).Returns(true);
            buildConfiguration.TypeCreators.Returns(typeCreators);
            buildConfiguration.ValueGenerators.Returns(valueGenerators);

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            typeCreator.CanPopulate(typeof(SlimModel), null, Arg.Any<IBuildChain>()).Returns(true);
            typeCreator.Populate(expected, target).Returns(expected);
            typeCreator.AutoPopulate.Returns(true);
            typeCreator.Priority.Returns(100);
            otherTypeCreator.CanPopulate(typeof(SlimModel), null, Arg.Any<IBuildChain>()).Returns(true);
            otherTypeCreator.Priority.Returns(50);
            valueGenerator.IsMatch(typeof(Guid), "Value", Arg.Is<IBuildChain>(x => x.Last == expected))
                .Returns(true);
            valueGenerator.Generate(typeof(Guid), "Value", Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last == expected))
                .Returns(value);

            var actual = (SlimModel) target.Populate(expected);

            actual.Value.Should().Be(value);
            otherTypeCreator.DidNotReceive().Populate(Arg.Any<object>(), Arg.Any<IExecuteStrategy>());
        }

        private class AdditionalWrapper
        {
            public AdditionalWrapper(int number)
            {
                Number = number;
            }

            public ReadOnlyModelWrapper Child { get; set; }

            public int Number { get; }
        }

        private class BuildWrapper : DefaultExecuteStrategy
        {
            public void RunTest(Type type)
            {
                Build(type, null, null);
            }
        }

        private class PopulateInstanceWrapper : DefaultExecuteStrategy
        {
            public void RunTest(object instance = null)
            {
                Populate(instance, null);
            }
        }

        private class PopulatePropertyWrapper : DefaultExecuteStrategy
        {
            public void RunTest(object instance = null, PropertyInfo propertyInfo = null)
            {
                PopulateProperty(propertyInfo, instance);
            }
        }

        private class ReadOnlyModelWrapper
        {
            public ReadOnlyModelWrapper(ReadOnlyModel model)
            {
                Model = model;
            }

            public ReadOnlyModel Model { get; }

            public ReadOnlyModel Other { get; set; }
        }

        private class SimpleReadOnlyParent
        {
            public Simple Simple { get; } = new Simple();
        }
    }
}