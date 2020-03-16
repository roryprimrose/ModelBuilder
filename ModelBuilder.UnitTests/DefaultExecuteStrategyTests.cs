namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using ModelBuilder.BuildActions;
    using ModelBuilder.ExecuteOrderRules;
    using ModelBuilder.TypeCreators;
    using ModelBuilder.UnitTests.Models;
    using ModelBuilder.ValueGenerators;
    using NSubstitute;
    using Xunit;
    using Xunit.Abstractions;

    public class DefaultExecuteStrategyTests
    {
        private readonly IBuildLog _buildLog;

        public DefaultExecuteStrategyTests(ITestOutputHelper output)
        {
            _buildLog = new OutputBuildLog(output);
        }

        [Fact]
        public void BuildChainShouldBeEmptyAfterCreateCompleted()
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

            var sut = new DefaultExecuteStrategy(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(buildConfiguration, buildHistory, Arg.Any<BuildRequirement>(),
                    typeof(SlimModel))
                .Returns(typeCapability);
            processor.Build(sut, typeof(SlimModel), Arg.Any<object[]>()).Returns(expected);

            sut.Initialize(buildConfiguration);

            sut.BuildChain.Should().BeEmpty();

            sut.Create(typeof(SlimModel));

            sut.BuildChain.Should().BeEmpty();
        }

        [Fact]
        public void BuildParameterThrowsExceptionWhenNotInitialized()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            var sut = new BuildParameterWrapper();

            Action action = () => sut.RunTest(parameterInfo);

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void BuildParameterThrowsExceptionWithNullPropertyInfo()
        {
            var sut = new BuildParameterWrapper();

            Action action = () => sut.RunTest();

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildPropertyThrowsExceptionWhenNotInitialized()
        {
            var property = typeof(Person).GetProperty(nameof(Person.FirstName));

            var sut = new BuildPropertyWrapper();

            Action action = () => sut.RunTest(property);

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void BuildPropertyThrowsExceptionWithNullPropertyInfo()
        {
            var sut = new BuildPropertyWrapper();

            Action action = () => sut.RunTest();

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildThrowsExceptionWhenNotInitialized()
        {
            var type = typeof(Person);

            var sut = new BuildWrapper();

            Action action = () => sut.RunTest(type);

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void BuildThrowsExceptionWithNullType()
        {
            var sut = new BuildWrapper();

            Action action = () => sut.RunTest(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildTypeThrowsExceptionWhenNotInitialized()
        {
            var type = typeof(Person);

            var sut = new BuildTypeWrapper();

            Action action = () => sut.RunTest(type);

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void BuildTypeThrowsExceptionWithNullType()
        {
            var sut = new BuildTypeWrapper();

            Action action = () => sut.RunTest();

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateDeterminesPropertiesToCreateByProvidingConstructorArgsForNestedType()
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

            var sut = new DefaultExecuteStrategy(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(buildConfiguration, buildHistory, Arg.Any<BuildRequirement>(),
                    typeof(SimpleConstructor))
                .Returns(typeCapability);
            processor.GetBuildCapability(buildConfiguration, buildHistory, Arg.Any<BuildRequirement>(),
                    Arg.Any<PropertyInfo>())
                .Returns(valueCapability);
            processor.GetBuildCapability(buildConfiguration, buildHistory, Arg.Any<BuildRequirement>(),
                    typeof(int))
                .Returns(valueCapability);
            processor.Build(sut, typeof(SimpleConstructor), Arg.Any<object[]>()).Returns(expected);
            processor.Build(sut, Arg.Is<PropertyInfo>(x => x.Name == nameof(SimpleConstructor.Model)),
                Arg.Any<object[]>()).Returns(model);
            processor.Build(sut, Arg.Is<PropertyInfo>(x => x.Name == nameof(SimpleConstructor.Age)),
                Arg.Any<object[]>()).Returns(age);
            processor.Populate(sut, expected).Returns(expected);
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

            sut.Initialize(buildConfiguration);

            var actual = (SimpleConstructor) sut.Create(typeof(SimpleConstructor), model);

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
        public void CreateDoesNotBuildPropertiesWhenCapabilityDoesNotSupportPopulation()
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

            var sut = new DefaultExecuteStrategy(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(buildConfiguration, buildHistory, Arg.Any<BuildRequirement>(),
                    typeof(SlimModel))
                .Returns(typeCapability);
            processor.Build(sut, typeof(SlimModel), Arg.Any<object[]>()).Returns(expected);

            sut.Initialize(buildConfiguration);

            var actual = (SlimModel) sut.Create(typeof(SlimModel));

            actual.Should().Be(expected);
            actual.Value.Should().BeEmpty();

            processor.DidNotReceive().Populate(sut, expected);
        }

        [Fact]
        public void CreateEvaluatesPostBuildActionsForParameters()
        {
            var postBuildAction = Substitute.For<IPostBuildAction>();
            var buildConfiguration = Model.UsingDefaultConfiguration().Add(postBuildAction);
            var parameterInfo = typeof(SimpleConstructor).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "model").GetParameters().First();

            postBuildAction.IsMatch(Arg.Any<IBuildChain>(), parameterInfo)
                .Returns(true);

            var sut = new DefaultExecuteStrategy();

            sut.Initialize(buildConfiguration);

            sut.Create(typeof(SimpleConstructor));

            postBuildAction.Received().Execute(Arg.Any<IBuildChain>(), Arg.Any<SlimModel>(), parameterInfo);
        }

        [Fact]
        public void CreateEvaluatesPostBuildActionsInOrderOfDescendingPriority()
        {
            var firstAction = Substitute.For<IPostBuildAction>();
            var secondAction = Substitute.For<IPostBuildAction>();
            var buildConfiguration = Model.UsingDefaultConfiguration().Add(firstAction).Add(secondAction);
            var executeCount = 0;

            firstAction.IsMatch(Arg.Any<IBuildChain>(), typeof(Simple)).Returns(true);
            firstAction.Priority.Returns(int.MaxValue);
            secondAction.IsMatch(Arg.Any<IBuildChain>(), typeof(Simple)).Returns(true);
            secondAction.Priority.Returns(int.MinValue);
            firstAction.WhenForAnyArgs(x => x.Execute(null, null, (Type) null)).Do(
                x =>
                {
                    executeCount++;

                    executeCount.Should().Be(1);
                });
            secondAction.WhenForAnyArgs(x => x.Execute(null, null, (Type) null)).Do(
                x =>
                {
                    executeCount++;

                    executeCount.Should().Be(2);
                });

            var sut = new DefaultExecuteStrategy();

            sut.Initialize(buildConfiguration);

            sut.Create(typeof(Simple));

            firstAction.Received().Execute(Arg.Any<IBuildChain>(), Arg.Any<Simple>(), Arg.Any<Type>());
            secondAction.Received().Execute(Arg.Any<IBuildChain>(), Arg.Any<Simple>(), Arg.Any<Type>());
        }

        [Fact]
        public void CreateEvaluatesPostBuildActionsOfNestedInstancesExposedAsReadOnlyProperties()
        {
            var postBuildAction = Substitute.For<IPostBuildAction>();
            var buildConfiguration = Model.UsingDefaultConfiguration().Add(postBuildAction);
            var propertyInfo = typeof(ReadOnlyParent).GetProperty(nameof(ReadOnlyParent.Company));

            postBuildAction.IsMatch(Arg.Any<IBuildChain>(), propertyInfo)
                .Returns(true);

            var sut = new DefaultExecuteStrategy();

            sut.Initialize(buildConfiguration);

            sut.Create(typeof(ReadOnlyParent));

            postBuildAction.Received().Execute(Arg.Any<IBuildChain>(), Arg.Any<Company>(), propertyInfo);
        }

        [Fact]
        public void CreateEvaluatesPostBuildActionsThatSupportTheBuildScenario()
        {
            var firstAction = Substitute.For<IPostBuildAction>();
            var secondAction = Substitute.For<IPostBuildAction>();
            var buildConfiguration = Model.UsingDefaultConfiguration().Add(firstAction).Add(secondAction);

            firstAction.IsMatch(Arg.Any<IBuildChain>(), Arg.Any<Type>()).Returns(false);
            secondAction.IsMatch(Arg.Any<IBuildChain>(), Arg.Any<Type>()).Returns(true);

            var sut = new DefaultExecuteStrategy();

            sut.Initialize(buildConfiguration);

            sut.Create(typeof(Simple));

            firstAction.DidNotReceive().Execute(Arg.Any<IBuildChain>(), Arg.Any<object>(), Arg.Any<Type>());
            secondAction.Received().Execute(Arg.Any<IBuildChain>(), Arg.Any<object>(), Arg.Any<Type>());
        }

        [Fact]
        public void CreateEvaluatesPostBuildActionsWhenCapabilityDoesNotSupportPopulation()
        {
            var action = Substitute.For<IPostBuildAction>();
            var buildConfiguration = Model.UsingDefaultConfiguration().Add(action);

            action.IsMatch(Arg.Any<IBuildChain>(), Arg.Any<Type>()).Returns(true);

            var sut = new DefaultExecuteStrategy();

            sut.Initialize(buildConfiguration);

            sut.Create(typeof(SlimModel));

            action.Received().Execute(Arg.Any<IBuildChain>(), Arg.Any<SlimModel>(), typeof(SlimModel));
            action.Received().Execute(Arg.Any<IBuildChain>(), Arg.Any<SlimModel>(), typeof(SlimModel));
        }

        [Fact]
        public void CreatePopulatesPropertiesWhenExecuteOrderRulesIsNull()
        {
            var expected = new SlimModel();
            var value = Guid.NewGuid();

            var creator = Substitute.For<ITypeCreator>();
            var generator = Substitute.For<IValueGenerator>();
            var resolver = Substitute.For<IPropertyResolver>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();

            creator.CanCreate(buildConfiguration, Arg.Any<IBuildChain>(), typeof(SlimModel)).Returns(true);
            creator.CanPopulate(buildConfiguration, Arg.Any<IBuildChain>(), typeof(SlimModel)).Returns(true);
            creator.Create(Arg.Any<IExecuteStrategy>(), typeof(SlimModel), null).Returns(expected);
            creator.Populate(Arg.Any<IExecuteStrategy>(), expected).Returns(expected);
            creator.AutoPopulate.Returns(true);
            resolver.CanPopulate(Arg.Any<PropertyInfo>()).Returns(true);
            resolver.ShouldPopulateProperty(buildConfiguration, expected, Arg.Any<PropertyInfo>(),
                Array.Empty<object>()).Returns(true);
            generator.IsMatch(Arg.Any<IBuildChain>(), Arg.Any<PropertyInfo>()).Returns(true);
            generator.Generate(Arg.Any<IExecuteStrategy>(), Arg.Any<PropertyInfo>()).Returns(value);
            buildConfiguration.PropertyResolver.Returns(resolver);
            buildConfiguration.ExecuteOrderRules.Returns((ICollection<IExecuteOrderRule>) null);
            buildConfiguration.TypeCreators.Returns(new List<ITypeCreator> {creator});
            buildConfiguration.ValueGenerators.Returns(new List<IValueGenerator> {generator});

            var sut = new DefaultExecuteStrategy();

            sut.Initialize(buildConfiguration);

            var actual = (SlimModel) sut.Create(typeof(SlimModel));

            actual.Should().Be(expected);
            resolver.Received().CanPopulate(Arg.Any<PropertyInfo>());
            actual.Value.Should().Be(value);
        }

        [Fact]
        public void CreatePopulatesWithProcessorWhenAutoPopulateDisabled()
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

            var sut = new DefaultExecuteStrategy(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(buildConfiguration, buildHistory, Arg.Any<BuildRequirement>(),
                    typeof(SlimModel))
                .Returns(typeCapability);
            processor.Build(sut, typeof(SlimModel), Arg.Any<object[]>()).Returns(expected);
            processor.Populate(sut, expected).Returns(expected);

            sut.Initialize(buildConfiguration);

            var actual = (SlimModel) sut.Create(typeof(SlimModel));

            actual.Should().Be(expected);
            actual.Value.Should().BeEmpty();

            processor.Received().Populate(sut, expected);
        }

        [Fact]
        public void CreatePushesInstanceIntoBuildChain()
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

            var sut = new DefaultExecuteStrategy(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(buildConfiguration, buildHistory, Arg.Any<BuildRequirement>(),
                    typeof(SlimModel))
                .Returns(typeCapability);
            processor.GetBuildCapability(buildConfiguration, buildHistory, Arg.Any<BuildRequirement>(),
                    Arg.Any<PropertyInfo>())
                .Returns(valueCapability);
            processor.GetBuildCapability(buildConfiguration, buildHistory, Arg.Any<BuildRequirement>(),
                    typeof(Guid))
                .Returns(valueCapability);
            processor.Build(sut, typeof(SlimModel), Arg.Any<object[]>()).Returns(expected);
            processor.Build(sut, Arg.Any<PropertyInfo>(), Arg.Any<object[]>()).Returns(value);
            processor.When(
                x => x.Build(
                    Arg.Is<IExecuteStrategy>(y => y.BuildChain.Last == expected), Arg.Any<PropertyInfo>(),
                    Arg.Any<object[]>())).Do(
                x =>
                {
                    sut.BuildChain.Should().HaveCount(1);
                    sut.BuildChain.First().Should().BeOfType<SlimModel>();
                    testPassed = true;
                });
            processor.Populate(sut, expected).Returns(expected);
            buildConfiguration.PropertyResolver.Returns(propertyResolver);
            propertyResolver.CanPopulate(Arg.Any<PropertyInfo>()).Returns(true);
            propertyResolver.ShouldPopulateProperty(
                Arg.Any<IBuildConfiguration>(),
                Arg.Any<object>(),
                Arg.Any<PropertyInfo>(),
                Arg.Any<object[]>()).Returns(true);

            sut.Initialize(buildConfiguration);

            var actual = (SlimModel) sut.Create(typeof(SlimModel));

            actual.Should().Be(expected);
            actual.Value.Should().Be(value);

            sut.BuildChain.Count.Should().Be(0);
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

            var sut = new DefaultExecuteStrategy(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(buildConfiguration, buildHistory, Arg.Any<BuildRequirement>(),
                    typeof(SlimModel))
                .Returns(typeCapability);

            sut.Initialize(buildConfiguration);

            var actual = sut.Create(typeof(SlimModel));

            actual.Should().BeNull();
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

            var sut = new DefaultExecuteStrategy(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(buildConfiguration, buildHistory, Arg.Any<BuildRequirement>(),
                    typeof(Person))
                .Returns(typeCapability);
            processor.Build(sut, typeof(Person), args).Returns(expected);
            buildConfiguration.PropertyResolver.Returns(propertyResolver);

            sut.Initialize(buildConfiguration);

            var actual = sut.Create(typeof(Person), args);

            actual.Should().BeSameAs(expected);
        }

        [Fact]
        public void CreateReturnsValueFromProcessorWithPropertyPopulation()
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

            var sut = new DefaultExecuteStrategy(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(buildConfiguration, buildHistory, Arg.Any<BuildRequirement>(),
                    typeof(SlimModel))
                .Returns(typeCapability);
            processor.GetBuildCapability(buildConfiguration, buildHistory, Arg.Any<BuildRequirement>(),
                    Arg.Any<PropertyInfo>())
                .Returns(valueCapability);
            processor.GetBuildCapability(buildConfiguration, buildHistory, Arg.Any<BuildRequirement>(),
                    typeof(Guid))
                .Returns(valueCapability);
            processor.Build(sut, typeof(SlimModel), Arg.Any<object[]>()).Returns(expected);
            processor.Build(sut, Arg.Any<PropertyInfo>(), Arg.Any<object[]>()).Returns(value);
            processor.Populate(sut, expected).Returns(expected);
            buildConfiguration.PropertyResolver.Returns(propertyResolver);
            propertyResolver.CanPopulate(Arg.Any<PropertyInfo>()).Returns(true);
            propertyResolver.ShouldPopulateProperty(
                Arg.Any<IBuildConfiguration>(),
                Arg.Any<object>(),
                Arg.Any<PropertyInfo>(),
                Arg.Any<object[]>()).Returns(true);

            sut.Initialize(buildConfiguration);

            var actual = (SlimModel) sut.Create(typeof(SlimModel));

            actual.Should().Be(expected);
            actual.Value.Should().Be(value);
        }

        [Fact]
        public void CreateReturnsValueFromResolvedConstructorAndBuiltParameters()
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
            var typeResolver = Substitute.For<ITypeResolver>();

            var sut = new DefaultExecuteStrategy(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(buildConfiguration, buildHistory, Arg.Any<BuildRequirement>(),
                    typeof(ReadOnlyModel))
                .Returns(typeCapability);
            processor.GetBuildCapability(buildConfiguration, buildHistory, BuildRequirement.Create,
                    Arg.Any<ParameterInfo>())
                .Returns(valueCapability);
            processor.GetBuildCapability(buildConfiguration, buildHistory, BuildRequirement.Populate,
                    typeof(Guid))
                .Returns(valueCapability);
            processor.Build(sut, Arg.Is<ParameterInfo>(x => x.Name == "value"),
                Arg.Any<object[]>()).Returns(value);
            processor.Build(sut, typeof(ReadOnlyModel), Arg.Is<object[]>(x => x.Length == 1 && (Guid) x[0] == value))
                .Returns(expected);
            processor.Populate(sut, expected).Returns(expected);
            buildConfiguration.ConstructorResolver.Returns(constructorResolver);
            buildConfiguration.TypeResolver.Returns(typeResolver);
            typeResolver.GetBuildType(buildConfiguration, Arg.Any<Type>()).Returns(x => x.Arg<Type>());
            constructorResolver.Resolve(typeof(ReadOnlyModel))
                .Returns(typeof(ReadOnlyModel).GetConstructors().Single());

            sut.Initialize(buildConfiguration);

            var actual = (ReadOnlyModel) sut.Create(typeof(ReadOnlyModel));

            actual.Should().Be(expected);
            actual.Value.Should().Be(value);
        }

        [Fact]
        public void CreateReturnsValueWithNoArgumentsAndDetectConstructorEnabledCreatedUsingEmptyConstructor()
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
            typeResolver.GetBuildType(buildConfiguration, Arg.Any<Type>()).Returns(x => x.Arg<Type>());

            var sut = new DefaultExecuteStrategy(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(buildConfiguration, buildHistory, Arg.Any<BuildRequirement>(),
                    typeof(Person))
                .Returns(typeCapability);
            constructorResolver.Resolve(typeof(Person))
                .Returns(typeof(Person).GetConstructors().Single(x => x.GetParameters().Length == 0));
            processor.Build(sut, typeof(Person), null).Returns(expected);
            processor.Populate(sut, expected).Returns(expected);

            sut.Initialize(buildConfiguration);

            var actual = sut.Create(typeof(Person));

            actual.Should().BeSameAs(expected);
        }

        [Fact]
        public void CreateThrowsExceptionWhenAutomaticTypeMappingCantFindMatch()
        {
            var buildConfiguration = Substitute.For<IBuildConfiguration>();

            var sut = new DefaultExecuteStrategy();

            sut.Initialize(buildConfiguration);

            Action action = () => sut.Create(typeof(ICantCreate));

            action.Should().Throw<BuildException>();
        }

        [Fact]
        public void CreateThrowsExceptionWhenDerivedImplementationSuppliesNullType()
        {
            var buildConfiguration = Substitute.For<IBuildConfiguration>();

            var sut = new NullTypeBuildExecuteStrategy();

            sut.Initialize(buildConfiguration);

            Action action = () => sut.Create(typeof(int));

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateThrowsExceptionWhenNoCapabilityFoundForParameter()
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
            var typeResolver = Substitute.For<ITypeResolver>();

            var sut = new DefaultExecuteStrategy(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(buildConfiguration, buildHistory, Arg.Any<BuildRequirement>(),
                    typeof(ReadOnlyModel))
                .Returns(typeCapability);
            buildConfiguration.TypeResolver.Returns(typeResolver);
            typeResolver.GetBuildType(buildConfiguration, Arg.Any<Type>()).Returns(x => x.Arg<Type>());
            constructorResolver.Resolve(typeof(ReadOnlyModel), null)
                .Returns(typeof(ReadOnlyModel).GetConstructors().Single());
            buildConfiguration.ConstructorResolver.Returns(constructorResolver);

            sut.Initialize(buildConfiguration);

            try
            {
                Action action = () => sut.Create(typeof(ReadOnlyModel));

                action.Should().Throw<BuildException>();
            }
            finally
            {
                constructorResolver.Received().Resolve(typeof(ReadOnlyModel));
            }
        }

        [Fact]
        public void CreateThrowsExceptionWhenNoCapabilityFoundForProperty()
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

            var sut = new DefaultExecuteStrategy(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(buildConfiguration, buildHistory, Arg.Any<BuildRequirement>(),
                    typeof(SlimModel))
                .Returns(typeCapability);
            processor.Build(sut, typeof(SlimModel), null).Returns(expected);
            processor.Populate(sut, expected).Returns(expected);
            buildConfiguration.PropertyResolver.Returns(propertyResolver);
            propertyResolver.CanPopulate(Arg.Is<PropertyInfo>(x => x.Name == nameof(SlimModel.Value)))
                .Returns(true);
            propertyResolver.ShouldPopulateProperty(
                Arg.Any<IBuildConfiguration>(),
                expected,
                Arg.Is<PropertyInfo>(x => x.Name == nameof(SlimModel.Value)),
                Arg.Any<object[]>()).Returns(true);

            sut.Initialize(buildConfiguration);

            Action action = () => sut.Create(typeof(SlimModel));

            action.Should().Throw<BuildException>();
        }

        [Fact]
        public void CreateThrowsExceptionWhenNoCapabilityFoundForType()
        {
            var buildHistory = new BuildHistory();

            var processor = Substitute.For<IBuildProcessor>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();

            var sut = new DefaultExecuteStrategy(buildHistory, _buildLog, processor);

            sut.Initialize(buildConfiguration);

            Action action = () => sut.Create(typeof(SlimModel));

            action.Should().Throw<BuildException>();
        }

        [Fact]
        public void CreateThrowsExceptionWhenNotInitialized()
        {
            var sut = new DefaultExecuteStrategy();

            Action action = () => sut.Create(typeof(Person));

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void CreateThrowsExceptionWithNullType()
        {
            var buildConfiguration = Substitute.For<IBuildConfiguration>();

            var sut = new DefaultExecuteStrategy();

            sut.Initialize(buildConfiguration);

            Action action = () => sut.Create(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void InitializeStoresConfiguration()
        {
            var configuration = Substitute.For<IBuildConfiguration>();

            var sut = new DefaultExecuteStrategy();

            sut.Initialize(configuration);

            sut.Configuration.Should().BeSameAs(configuration);
        }

        [Fact]
        public void InitializeThrowsExceptionWithNullBuildConfiguration()
        {
            var sut = new DefaultExecuteStrategy();

            Action action = () => sut.Initialize(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsCreatedWithBuildChainInstance()
        {
            var sut = new DefaultExecuteStrategy();

            sut.BuildChain.Should().NotBeNull();
        }

        [Fact]
        public void IsCreatedWithNullBuildConfigurationAndBuildLog()
        {
            var sut = new DefaultExecuteStrategy();

            sut.Configuration.Should().BeNull();
            sut.Log.Should().NotBeNull();
            sut.BuildChain.Should().NotBeNull();
        }

        [Fact]
        public void PopulateAssignsPropertyValuesToExistingInstance()
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

            var sut = new DefaultExecuteStrategy(buildHistory, _buildLog, processor);

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
            processor.Build(sut, Arg.Is<PropertyInfo>(x => x.Name == nameof(SlimModel.Value)), null)
                .Returns(expected);
            processor.GetBuildCapability(buildConfiguration, buildHistory, BuildRequirement.Populate,
                    typeof(Guid))
                .Returns(valueCapability);
            processor.Populate(sut, model).Returns(model);

            sut.Initialize(buildConfiguration);

            var actual = (SlimModel) sut.Populate(model);

            actual.Value.Should().Be(expected);
        }

        [Fact]
        public void PopulatePropertyThrowsExceptionWithNullInstance()
        {
            var sut = new PopulatePropertyWrapper();

            var property = typeof(Person).GetProperty("FirstName");

            Action action = () => sut.RunTest(property);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PopulatePropertyThrowsExceptionWithNullProperty()
        {
            var instance = new SlimModel();

            var sut = new PopulatePropertyWrapper();

            Action action = () => sut.RunTest(null, instance);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PopulateReturnsInstanceWhenNoBuildActionCapabilityFound()
        {
            var buildHistory = new BuildHistory();
            var model = new SlimModel();

            var processor = Substitute.For<IBuildProcessor>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();

            var sut = new DefaultExecuteStrategy(buildHistory, _buildLog, processor);

            sut.Initialize(buildConfiguration);

            var actual = sut.Populate(model);

            actual.Should().Be(model);
        }

        [Fact]
        public void PopulateThrowsExceptionWhenNotInitialized()
        {
            var value = new Person();

            var sut = new DefaultExecuteStrategy();

            Action action = () => sut.Populate(value);

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void PopulateThrowsExceptionWithNullInstance()
        {
            var sut = new DefaultExecuteStrategy();

            Action action = () => sut.Populate(null);

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