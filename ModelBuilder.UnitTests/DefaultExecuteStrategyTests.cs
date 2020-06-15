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

            var processor = Substitute.For<IBuildProcessor>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildCapability = Substitute.For<IBuildCapability>();

            buildCapability.AutoPopulate.Returns(true);
            buildCapability.SupportsCreate.Returns(true);
            buildCapability.ImplementedByType.Returns(typeof(DummyTypeCreator));

            var sut = new DefaultExecuteStrategy(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(sut, Arg.Any<BuildRequirement>(), typeof(SlimModel)).Returns(buildCapability);
            buildCapability.CreateType(sut, typeof(SlimModel), Arg.Any<object[]>()).Returns(expected);

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
            var property = typeof(Person).GetProperty(nameof(Person.FirstName))!;

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

            action.Should().Throw<BuildException>();
        }

        [Fact]
        public void BuildThrowsExceptionWithNullType()
        {
            var sut = new BuildWrapper();

            Action action = () => sut.RunTest(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildTypeThrowsExceptionWhenNotInitialized()
        {
            var type = typeof(Person);

            var sut = new BuildTypeWrapper();

            Action action = () => sut.RunTest(type);

            action.Should().Throw<BuildException>();
        }

        [Fact]
        public void BuildTypeThrowsExceptionWithNullType()
        {
            var sut = new BuildTypeWrapper();

            Action action = () => sut.RunTest();

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ConfigurationThrowsExceptionWhenNotInitialized()
        {
            var sut = new DefaultExecuteStrategy();

            Action action = () =>
            {
                // ReSharper disable once UnusedVariable
                var config = sut.Configuration;
            };

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void CreateCanAssignNullProperty()
        {
            var buildHistory = new BuildHistory();
            var expected = new NullablePropertyModel<Person>();

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

            var sut = new DefaultExecuteStrategy(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(sut, Arg.Any<BuildRequirement>(),
                    typeof(NullablePropertyModel<Person>))
                .Returns(typeCapability);
            processor.GetBuildCapability(sut, Arg.Any<BuildRequirement>(),
                    Arg.Any<PropertyInfo>())
                .Returns(valueCapability);
            typeCapability.CreateType(sut, typeof(NullablePropertyModel<Person>), Arg.Any<object[]>())
                .Returns(expected);
            valueCapability.CreateProperty(sut, Arg.Any<PropertyInfo>(), Arg.Any<object[]>()).Returns(null);
            typeCapability.Populate(sut, expected).Returns(expected);
            buildConfiguration.PropertyResolver.Returns(propertyResolver);
            propertyResolver.GetOrderedProperties(buildConfiguration, typeof(NullablePropertyModel<Person>))
                .Returns(typeof(NullablePropertyModel<Person>).GetProperties());

            sut.Initialize(buildConfiguration);

            var actual = (NullablePropertyModel<Person>) sut.Create(typeof(NullablePropertyModel<Person>))!;

            actual.Should().Be(expected);
            actual.Value.Should().BeNull();
        }

        [Fact]
        public void CreateDeterminesPropertiesToCreateByProvidingConstructorArgsForNestedType()
        {
            var buildHistory = new BuildHistory();
            var model = new SlimModel();
            var expected = new SimpleConstructor(model);
            var age = Environment.TickCount;

            var typeCapability = Substitute.For<IBuildCapability>();
            var valueCapability = Substitute.For<IBuildCapability>();
            var processor = Substitute.For<IBuildProcessor>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var propertyResolver = Substitute.For<IPropertyResolver>();

            typeCapability.SupportsPopulate.Returns(true);
            typeCapability.AutoPopulate.Returns(true);
            typeCapability.SupportsCreate.Returns(true);
            typeCapability.ImplementedByType.Returns(typeof(DummyTypeCreator));
            valueCapability.SupportsCreate.Returns(true);
            valueCapability.ImplementedByType.Returns(typeof(DummyTypeCreator));

            var sut = new DefaultExecuteStrategy(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(sut, Arg.Any<BuildRequirement>(),
                    typeof(SimpleConstructor))
                .Returns(typeCapability);
            processor.GetBuildCapability(sut, Arg.Any<BuildRequirement>(),
                    Arg.Any<PropertyInfo>())
                .Returns(valueCapability);
            processor.GetBuildCapability(sut, Arg.Any<BuildRequirement>(),
                    typeof(int))
                .Returns(valueCapability);
            typeCapability.CreateType(sut, typeof(SimpleConstructor), Arg.Any<object[]>()).Returns(expected);
            valueCapability.CreateProperty(sut, Arg.Is<PropertyInfo>(x => x.Name == nameof(SimpleConstructor.Model)),
                Arg.Any<object[]>()).Returns(model);
            valueCapability.CreateProperty(sut, Arg.Is<PropertyInfo>(x => x.Name == nameof(SimpleConstructor.Age)),
                Arg.Any<object[]>()).Returns(age);
            typeCapability.Populate(sut, expected).Returns(expected);
            buildConfiguration.PropertyResolver.Returns(propertyResolver);
            propertyResolver.GetOrderedProperties(buildConfiguration, typeof(SimpleConstructor))
                .Returns(typeof(SimpleConstructor).GetProperties());

            sut.Initialize(buildConfiguration);

            var actual = (SimpleConstructor) sut.Create(typeof(SimpleConstructor), model)!;

            actual.Should().Be(expected);
            actual.Model.Should().Be(model);
            actual.Age.Should().Be(age);
        }

        [Fact]
        public void CreateDoesNotBuildPropertiesWhenCapabilityDoesNotSupportPopulation()
        {
            var buildHistory = new BuildHistory();
            var expected = new SlimModel();

            var processor = Substitute.For<IBuildProcessor>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var typeCapability = Substitute.For<IBuildCapability>();

            typeCapability.AutoPopulate.Returns(true);
            typeCapability.SupportsCreate.Returns(true);
            typeCapability.ImplementedByType.Returns(typeof(DummyTypeCreator));

            var sut = new DefaultExecuteStrategy(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(sut, Arg.Any<BuildRequirement>(),
                    typeof(SlimModel))
                .Returns(typeCapability);
            typeCapability.CreateType(sut, typeof(SlimModel), Arg.Any<object[]>()).Returns(expected);

            sut.Initialize(buildConfiguration);

            var actual = (SlimModel) sut.Create(typeof(SlimModel))!;

            actual.Should().Be(expected);
            actual.Value.Should().BeEmpty();

            typeCapability.DidNotReceive().Populate(Arg.Any<IExecuteStrategy>(), Arg.Any<object>());
        }

        [Fact]
        public void CreateDoesNotEvaluateEmptyPostBuildActions()
        {
            var buildConfiguration = Model.UsingDefaultConfiguration();

            var sut = new DefaultExecuteStrategy();

            sut.Initialize(buildConfiguration);

            var actual = sut.Create(typeof(SimpleConstructor));

            actual.Should().NotBeNull();
        }

        [Fact]
        public void CreateDoesNotEvaluateNullPostBuildActions()
        {
            var defaultConfiguration = Model.UsingDefaultConfiguration();

            var sut = new DefaultExecuteStrategy();

            sut.Initialize(defaultConfiguration);

            var actual = sut.Create(typeof(SimpleConstructor));

            actual.Should().NotBeNull();
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
            firstAction.WhenForAnyArgs(x => x.Execute(null!, null!, (Type) null!)).Do(
                x =>
                {
                    executeCount++;

                    executeCount.Should().Be(1);
                });
            secondAction.WhenForAnyArgs(x => x.Execute(null!, null!, (Type) null!)).Do(
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
            var propertyInfo = typeof(ReadOnlyParent).GetProperty(nameof(ReadOnlyParent.Company))!;

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
        public void CreateParametersCanReturnNullParameter()
        {
            var buildHistory = new BuildHistory();
            var method = typeof(SimpleConstructor).GetConstructors().First();
            var parameters = method.GetParameters().OrderBy(x => x.Name);

            var parameterCapability = Substitute.For<IBuildCapability>();
            var processor = Substitute.For<IBuildProcessor>();
            var parameterResolver = Substitute.For<IParameterResolver>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();

            var sut = new DefaultExecuteStrategy(buildHistory, _buildLog, processor);

            parameterCapability.AutoPopulate.Returns(false);
            parameterCapability.SupportsCreate.Returns(true);
            parameterCapability.SupportsPopulate.Returns(false);
            parameterCapability.ImplementedByType.Returns(GetType());
            parameterCapability.CreateParameter(sut, Arg.Any<ParameterInfo>(), null).Returns(null);
            processor.GetBuildCapability(sut, BuildRequirement.Create,
                Arg.Any<ParameterInfo>()).Returns(parameterCapability);
            parameterResolver.GetOrderedParameters(buildConfiguration, method).Returns(parameters);
            buildConfiguration.ParameterResolver.Returns(parameterResolver);

            sut.Initialize(buildConfiguration);

            var actual = sut.CreateParameters(method)!;

            actual.Should().HaveCount(1);
            actual[0].Should().BeNull();
        }

        [Fact]
        public void CreateParametersReturnsNullWhenNoOrderedParametersReturned()
        {
            var buildHistory = new BuildHistory();
            var method = typeof(SlimModel).GetConstructors().First();
            var parameters = Array.Empty<ParameterInfo>();

            var processor = Substitute.For<IBuildProcessor>();
            var parameterResolver = Substitute.For<IParameterResolver>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();

            parameterResolver.GetOrderedParameters(buildConfiguration, method).Returns(parameters);
            buildConfiguration.ParameterResolver.Returns(parameterResolver);

            var sut = new DefaultExecuteStrategy(buildHistory, _buildLog, processor);

            sut.Initialize(buildConfiguration);

            var actual = sut.CreateParameters(method);

            actual.Should().BeNull();
        }

        [Fact]
        public void CreateParametersReturnsOriginalParameterOrderCreatedInOrderOfPriority()
        {
            var buildHistory = new BuildHistory();
            var method = typeof(OrderedConstructorParameters).GetConstructors().First();
            var parameters = method.GetParameters().OrderBy(x => x.Name);

            var parameterCapability = Substitute.For<IBuildCapability>();
            var processor = Substitute.For<IBuildProcessor>();
            var parameterResolver = Substitute.For<IParameterResolver>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();

            var sut = new DefaultExecuteStrategy(buildHistory, _buildLog, processor);

            parameterCapability.AutoPopulate.Returns(false);
            parameterCapability.SupportsCreate.Returns(true);
            parameterCapability.SupportsPopulate.Returns(false);
            parameterCapability.ImplementedByType.Returns(GetType());
            parameterCapability
                .CreateParameter(sut, Arg.Is<ParameterInfo>(x => x.ParameterType == typeof(string)), null)
                .Returns("first", "second", "third", "fourth");
            parameterCapability
                .CreateParameter(sut, Arg.Is<ParameterInfo>(x => x.ParameterType == typeof(Gender)), null)
                .Returns(Gender.Female);
            processor.GetBuildCapability(sut, BuildRequirement.Create,
                Arg.Any<ParameterInfo>()).Returns(parameterCapability);
            parameterResolver.GetOrderedParameters(buildConfiguration, method).Returns(parameters);
            buildConfiguration.ParameterResolver.Returns(parameterResolver);

            sut.Initialize(buildConfiguration);

            var actual = sut.CreateParameters(method)!;

            actual.Should().HaveCount(5);
            actual[0].Should().Be("second");
            actual[1].Should().Be("first");
            actual[2].Should().Be("fourth");
            actual[3].Should().Be("third");
            actual[4].Should().Be(Gender.Female);
        }

        [Fact]
        public void CreateParametersThrowsExceptionWithNullMethod()
        {
            var buildHistory = new BuildHistory();

            var processor = Substitute.For<IBuildProcessor>();

            var sut = new DefaultExecuteStrategy(buildHistory, _buildLog, processor);

            Action action = () => sut.CreateParameters(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreatePopulatesWithProcessorWhenAutoPopulateDisabled()
        {
            var buildHistory = new BuildHistory();
            var expected = new SlimModel();

            var processor = Substitute.For<IBuildProcessor>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var typeCapability = Substitute.For<IBuildCapability>();

            typeCapability.SupportsPopulate.Returns(true);
            typeCapability.SupportsCreate.Returns(true);
            typeCapability.ImplementedByType.Returns(typeof(DummyTypeCreator));

            var sut = new DefaultExecuteStrategy(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(sut, Arg.Any<BuildRequirement>(),
                    typeof(SlimModel))
                .Returns(typeCapability);
            typeCapability.CreateType(sut, typeof(SlimModel), Arg.Any<object[]>()).Returns(expected);
            typeCapability.Populate(sut, expected).Returns(expected);

            sut.Initialize(buildConfiguration);

            var actual = (SlimModel) sut.Create(typeof(SlimModel))!;

            actual.Should().Be(expected);
            actual.Value.Should().BeEmpty();

            typeCapability.Received().Populate(sut, expected);
        }

        [Fact]
        public void CreatePushesInstanceIntoBuildChain()
        {
            var testPassed = false;
            var buildHistory = new BuildHistory();
            var expected = new SlimModel();
            var value = Guid.NewGuid();

            var processor = Substitute.For<IBuildProcessor>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var propertyResolver = Substitute.For<IPropertyResolver>();
            var typeCapability = Substitute.For<IBuildCapability>();
            var valueCapability = Substitute.For<IBuildCapability>();

            typeCapability.SupportsPopulate.Returns(true);
            typeCapability.AutoPopulate.Returns(true);
            typeCapability.SupportsCreate.Returns(true);
            typeCapability.ImplementedByType.Returns(typeof(DummyTypeCreator));
            valueCapability.SupportsCreate.Returns(true);
            valueCapability.ImplementedByType.Returns(typeof(DummyTypeCreator));

            var sut = new DefaultExecuteStrategy(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(sut, Arg.Any<BuildRequirement>(),
                    typeof(SlimModel))
                .Returns(typeCapability);
            processor.GetBuildCapability(sut, Arg.Any<BuildRequirement>(),
                    Arg.Any<PropertyInfo>())
                .Returns(valueCapability);
            processor.GetBuildCapability(sut, Arg.Any<BuildRequirement>(),
                    typeof(Guid))
                .Returns(valueCapability);
            typeCapability.CreateType(sut, typeof(SlimModel), Arg.Any<object[]>()).Returns(expected);
            valueCapability.CreateProperty(sut, Arg.Any<PropertyInfo>(), Arg.Any<object[]>()).Returns(value);
            valueCapability.When(
                x => x.CreateProperty(
                    Arg.Is<IExecuteStrategy>(y => y.BuildChain.Last == expected), Arg.Any<PropertyInfo>(),
                    Arg.Any<object[]>())).Do(
                x =>
                {
                    sut.BuildChain.Should().HaveCount(1);
                    sut.BuildChain.First().Should().BeOfType<SlimModel>();
                    testPassed = true;
                });
            typeCapability.Populate(sut, expected).Returns(expected);
            buildConfiguration.PropertyResolver.Returns(propertyResolver);
            propertyResolver.GetOrderedProperties(buildConfiguration, typeof(SlimModel))
                .Returns(typeof(SlimModel).GetProperties());

            sut.Initialize(buildConfiguration);

            var actual = (SlimModel) sut.Create(typeof(SlimModel))!;

            actual.Should().Be(expected);
            actual.Value.Should().Be(value);

            sut.BuildChain.Count.Should().Be(0);
            testPassed.Should().BeTrue();
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

            var sut = new DefaultExecuteStrategy(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(sut, Arg.Any<BuildRequirement>(),
                    typeof(Person))
                .Returns(typeCapability);
            typeCapability.CreateType(sut, typeof(Person), args).Returns(expected);
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

            var sut = new DefaultExecuteStrategy(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(sut, Arg.Any<BuildRequirement>(),
                    typeof(SlimModel))
                .Returns(typeCapability);
            processor.GetBuildCapability(sut, Arg.Any<BuildRequirement>(),
                    Arg.Any<PropertyInfo>())
                .Returns(valueCapability);
            processor.GetBuildCapability(sut, Arg.Any<BuildRequirement>(),
                    typeof(Guid))
                .Returns(valueCapability);
            typeCapability.CreateType(sut, typeof(SlimModel), Arg.Any<object[]>()).Returns(expected);
            valueCapability.CreateProperty(sut, Arg.Any<PropertyInfo>(), Arg.Any<object[]>()).Returns(value);
            typeCapability.Populate(sut, expected).Returns(expected);
            buildConfiguration.PropertyResolver.Returns(propertyResolver);
            propertyResolver.GetOrderedProperties(buildConfiguration, typeof(SlimModel))
                .Returns(typeof(SlimModel).GetProperties());

            sut.Initialize(buildConfiguration);

            var actual = (SlimModel) sut.Create(typeof(SlimModel))!;

            actual.Should().Be(expected);
            actual.Value.Should().Be(value);
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
        public void CreateThrowsExceptionWhenNoCapabilityFoundForProperty()
        {
            var buildHistory = new BuildHistory();
            var expected = new SlimModel();

            var processor = Substitute.For<IBuildProcessor>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var propertyResolver = Substitute.For<IPropertyResolver>();
            var typeCapability = Substitute.For<IBuildCapability>();

            typeCapability.AutoPopulate.Returns(true);
            typeCapability.SupportsCreate.Returns(true);
            typeCapability.SupportsPopulate.Returns(true);
            typeCapability.ImplementedByType.Returns(typeof(DummyTypeCreator));

            var sut = new DefaultExecuteStrategy(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(sut, Arg.Any<BuildRequirement>(),
                    typeof(SlimModel))
                .Returns(typeCapability);
            typeCapability.CreateType(sut, typeof(SlimModel), null!).Returns(expected);
            typeCapability.Populate(sut, expected).Returns(expected);
            buildConfiguration.PropertyResolver.Returns(propertyResolver);
            propertyResolver.GetOrderedProperties(buildConfiguration, typeof(SlimModel))
                .Returns(typeof(SlimModel).GetProperties());

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

            action.Should().Throw<BuildException>();
        }

        [Fact]
        public void CreateThrowsExceptionWhenProcessorReturnsNull()
        {
            var buildHistory = new BuildHistory();

            var processor = Substitute.For<IBuildProcessor>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var typeCapability = Substitute.For<IBuildCapability>();

            typeCapability.SupportsPopulate.Returns(true);
            typeCapability.SupportsCreate.Returns(true);
            typeCapability.ImplementedByType.Returns(typeof(DummyTypeCreator));

            var sut = new DefaultExecuteStrategy(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(sut, Arg.Any<BuildRequirement>(),
                    typeof(SlimModel))
                .Returns(typeCapability);

            sut.Initialize(buildConfiguration);

            Action action = () => sut.Create(typeof(SlimModel));

            action.Should().Throw<BuildException>();
        }

        [Fact]
        public void CreateThrowsExceptionWithNullType()
        {
            var buildConfiguration = Substitute.For<IBuildConfiguration>();

            var sut = new DefaultExecuteStrategy();

            sut.Initialize(buildConfiguration);

            Action action = () => sut.Create(null!);

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

            Action action = () => sut.Initialize(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsCreatedWithBuildChainInstance()
        {
            var sut = new DefaultExecuteStrategy();

            sut.BuildChain.Should().NotBeNull();
        }

        [Fact]
        public void IsCreatedWithNullBuildChainAndBuildLog()
        {
            var sut = new DefaultExecuteStrategy();

            sut.Log.Should().NotBeNull();
            sut.BuildChain.Should().NotBeNull();
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

            var sut = new DefaultExecuteStrategy(buildHistory, _buildLog, processor);

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

            var actual = (SlimModel) sut.Populate(model);

            actual.Value.Should().Be(expected);
        }

        [Fact]
        public void PopulateDoesNotAssignIgnoredProperty()
        {
            var buildHistory = new BuildHistory();
            var model = new SlimModel();

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

            var sut = new DefaultExecuteStrategy(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(sut, BuildRequirement.Populate,
                    typeof(SlimModel))
                .Returns(typeCapability);
            buildConfiguration.PropertyResolver.Returns(propertyResolver);
            propertyResolver.GetOrderedProperties(buildConfiguration, typeof(SlimModel))
                .Returns(typeof(SlimModel).GetProperties());
            processor.GetBuildCapability(sut, BuildRequirement.Create,
                    Arg.Is<PropertyInfo>(x => x.Name == nameof(SlimModel.Value)))
                .Returns(valueCapability);
            processor.GetBuildCapability(sut, BuildRequirement.Populate,
                    typeof(Guid))
                .Returns(valueCapability);
            typeCapability.Populate(sut, model).Returns(model);
            propertyResolver.IsIgnored(
                Arg.Any<IBuildConfiguration>(),
                Arg.Any<object>(),
                Arg.Any<PropertyInfo>(),
                Arg.Any<object[]>()).Returns(true);

            sut.Initialize(buildConfiguration);

            var actual = (SlimModel) sut.Populate(model);

            actual.Value.Should().BeEmpty();

            typeCapability.DidNotReceive()
                .CreateProperty(sut, Arg.Is<PropertyInfo>(x => x.Name == nameof(SlimModel.Value)), null!);
        }

        [Fact]
        public void PopulatePropertyThrowsExceptionWithNullInstance()
        {
            var sut = new PopulatePropertyWrapper();

            var property = typeof(Person).GetProperty(nameof(Person.FirstName))!;

            Action action = () => sut.RunTest(property);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PopulatePropertyThrowsExceptionWithNullProperty()
        {
            var instance = new SlimModel();

            var sut = new PopulatePropertyWrapper();

            Action action = () => sut.RunTest(null!, instance);

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

            action.Should().Throw<BuildException>();
        }

        [Fact]
        public void PopulateThrowsExceptionWhenProcessorReturnsNull()
        {
            var model = new SlimModel();
            var buildHistory = new BuildHistory();

            var processor = Substitute.For<IBuildProcessor>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var typeCapability = Substitute.For<IBuildCapability>();

            typeCapability.SupportsCreate.Returns(true);
            typeCapability.SupportsPopulate.Returns(true);
            typeCapability.ImplementedByType.Returns(typeof(DummyTypeCreator));

            var sut = new DefaultExecuteStrategy(buildHistory, _buildLog, processor);

            processor.GetBuildCapability(sut, Arg.Any<BuildRequirement>(),
                    typeof(SlimModel))
                .Returns(typeCapability);

            sut.Initialize(buildConfiguration);

            Action action = () => sut.Populate(model);

            action.Should().Throw<BuildException>();
        }

        [Fact]
        public void PopulateThrowsExceptionWithNullInstance()
        {
            var sut = new DefaultExecuteStrategy();

            Action action = () => sut.Populate(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWithNullBuildHistory()
        {
            var buildLog = Substitute.For<IBuildLog>();
            var buildProcessor = Substitute.For<IBuildProcessor>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new DefaultExecuteStrategy(null!, buildLog, buildProcessor);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWithNullBuildLog()
        {
            var buildHistory = Substitute.For<IBuildHistory>();
            var buildProcessor = Substitute.For<IBuildProcessor>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new DefaultExecuteStrategy(buildHistory, null!, buildProcessor);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWithNullBuildProcessor()
        {
            var buildHistory = Substitute.For<IBuildHistory>();
            var buildLog = Substitute.For<IBuildLog>();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new DefaultExecuteStrategy(buildHistory, buildLog, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        private class AdditionalWrapper
        {
            public AdditionalWrapper(int number)
            {
                Number = number;
            }

            public ReadOnlyModelWrapper? Child { get; set; } = null;

            public int Number { get; }
        }

        private class BuildParameterWrapper : DefaultExecuteStrategy
        {
            public void RunTest(ParameterInfo parameterInfo = null!)
            {
                Build(parameterInfo);
            }
        }

        private class BuildPropertyWrapper : DefaultExecuteStrategy
        {
            public void RunTest(PropertyInfo propertyInfo = null!)
            {
                Build(propertyInfo);
            }
        }

        private class BuildTypeWrapper : DefaultExecuteStrategy
        {
            public void RunTest(Type type = null!)
            {
                Build(type);
            }
        }

        private class BuildWrapper : DefaultExecuteStrategy
        {
            public void RunTest(Type type)
            {
                Build(type, null!, null!);
            }
        }

        private class PopulatePropertyWrapper : DefaultExecuteStrategy
        {
            public void RunTest(PropertyInfo propertyInfo = null!, object instance = null!)
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

            public ReadOnlyModel? Model { get; }

            public ReadOnlyModel? Other { get; set; } = null;
        }

        private class SimpleReadOnlyParent
        {
            public Simple Simple { get; } = new Simple();
        }
    }
}