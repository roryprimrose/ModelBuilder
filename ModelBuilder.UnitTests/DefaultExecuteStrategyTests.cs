namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using ModelBuilder.BuildActions;
    using ModelBuilder.CreationRules;
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
        public void BuildParameterThrowsExceptionWhenNotInitializedTest()
        {
            var value = new Person();
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            var target = new BuildParameterWrapper();

            Action action = () => target.RunTest(parameterInfo);

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void BuildParameterThrowsExceptionWithNullPropertyInfoTest()
        {
            var target = new BuildParameterWrapper();

            Action action = () => target.RunTest();

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildPropertyThrowsExceptionWhenNotInitializedTest()
        {
            var value = new Person();
            var property = typeof(Person).GetProperty(nameof(Person.FirstName));

            var target = new BuildPropertyWrapper();

            Action action = () => target.RunTest(property);

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void BuildPropertyThrowsExceptionWithNullPropertyInfoTest()
        {
            var target = new BuildPropertyWrapper();

            Action action = () => target.RunTest();

            action.Should().Throw<ArgumentNullException>();
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
        public void BuildTypeThrowsExceptionWhenNotInitializedTest()
        {
            var type = typeof(Person);

            var target = new BuildTypeWrapper();

            Action action = () => target.RunTest(type);

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void BuildTypeThrowsExceptionWithNullTypeTest()
        {
            var target = new BuildTypeWrapper();

            Action action = () => target.RunTest();

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
        public void CreatePushesInstanceIntoBuildChainTest()
        {
            var testPassed = false;
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
            processor.When(
                x => x.Build(
                    Arg.Is<IExecuteStrategy>(y => y.BuildChain.Last == expected), Arg.Any<PropertyInfo>(),
                    Arg.Any<object[]>())).Do(
                x =>
                {
                    target.BuildChain.Should().HaveCount(1);
                    target.BuildChain.First().Should().BeOfType<SlimModel>();
                    testPassed = true;
                });
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

            target.BuildChain.Count.Should().Be(0);
            testPassed.Should().BeTrue();
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
            constructorResolver.Resolve(typeof(ReadOnlyModel))
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
            constructorResolver.Resolve(typeof(Person))
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
                constructorResolver.Received().Resolve(typeof(ReadOnlyModel));
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

            var actual = (SlimModel) target.Populate(model);

            actual.Value.Should().Be(expected);
        }

        [Fact]
        public void PopulatePropertyThrowsExceptionWithNullInstanceTest()
        {
            var target = new PopulatePropertyWrapper();

            var property = typeof(Person).GetProperty("FirstName");

            Action action = () => target.RunTest(property);

            action.Should().Throw<ArgumentNullException>();
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

        private class AdditionalWrapper
        {
            public AdditionalWrapper(int number)
            {
                Number = number;
            }

            public ReadOnlyModelWrapper Child { get; set; }

            public int Number { get; }
        }

        private class BuildParameterWrapper : DefaultExecuteStrategy
        {
            public void RunTest(ParameterInfo parameterInfo = null)
            {
                Build(parameterInfo);
            }
        }

        private class BuildPropertyWrapper : DefaultExecuteStrategy
        {
            public void RunTest(PropertyInfo propertyInfo = null)
            {
                Build(propertyInfo);
            }
        }

        private class BuildTypeWrapper : DefaultExecuteStrategy
        {
            public void RunTest(Type type = null)
            {
                Build(type);
            }
        }

        private class BuildWrapper : DefaultExecuteStrategy
        {
            public void RunTest(Type type)
            {
                Build(type, null, null);
            }
        }

        private class PopulatePropertyWrapper : DefaultExecuteStrategy
        {
            public void RunTest(PropertyInfo propertyInfo = null, object instance = null)
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