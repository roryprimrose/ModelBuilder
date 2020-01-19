namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using ModelBuilder.TypeCreators;
    using ModelBuilder.UnitTests.Models;
    using ModelBuilder.ValueGenerators;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using Xunit;
    using Xunit.Abstractions;

    public class DefaultExecuteStrategyTests
    {
        private readonly ITestOutputHelper _output;

        public DefaultExecuteStrategyTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void BuildChainShouldBeEmptyAfterCreateCompletedTest()
        {
            var configuration = Model.UsingDefaultConfiguration();

            var target = new DefaultExecuteStrategy();

            target.Initialize(configuration);

            target.BuildChain.Should().BeEmpty();

            target.Create(typeof(Company));

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
            var number = Environment.TickCount;
            var value = Guid.NewGuid();
            var valueGenerators = new Collection<IValueGenerator>();
            var typeCreators = new Collection<ITypeCreator>();

            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var typeCreator = Substitute.For<ITypeCreator>();
            var valueGenerator = Substitute.For<IValueGenerator>();
            var propertyResolver = Substitute.For<IPropertyResolver>();

            typeCreators.Add(typeCreator);
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
                Arg.Any<object>(),
                Arg.Is<PropertyInfo>(x => x.Name == nameof(AdditionalWrapper.Number)),
                Arg.Any<object[]>()).Returns(false);
            propertyResolver.ShouldPopulateProperty(
                Arg.Any<IBuildConfiguration>(),
                Arg.Any<object>(),
                Arg.Is<PropertyInfo>(x => x.Name == nameof(ReadOnlyModel.Value)),
                Arg.Any<object[]>()).Returns(false);
            propertyResolver.ShouldPopulateProperty(
                Arg.Any<IBuildConfiguration>(),
                Arg.Is<object>(x => x.GetType() == typeof(ReadOnlyModelWrapper)),
                Arg.Any<PropertyInfo>(),
                Arg.Any<object[]>()).Returns(false);
            buildConfiguration.TypeCreators.Returns(typeCreators);
            buildConfiguration.ValueGenerators.Returns(valueGenerators);
            buildConfiguration.IgnoreRules.Returns(new Collection<IgnoreRule>());
            buildConfiguration.ConstructorResolver.Returns(new DefaultConstructorResolver());

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            typeCreator.AutoPopulate.Returns(true);
            typeCreator.AutoDetectConstructor.Returns(true);
            typeCreator.CanCreate(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<IBuildChain>()).Returns(true);
            typeCreator.Create(typeof(AdditionalWrapper), null, Arg.Any<IExecuteStrategy>(), Arg.Any<object[]>())
                .Returns(x => new AdditionalWrapper((int) ((object[]) x[3])[0]));
            typeCreator.Create(typeof(ReadOnlyModel), "model", Arg.Any<IExecuteStrategy>(), value)
                .Returns(x => new ReadOnlyModel((Guid) ((object[]) x[3])[0]));
            typeCreator.Create(
                typeof(ReadOnlyModelWrapper),
                nameof(AdditionalWrapper.Child),
                Arg.Any<IExecuteStrategy>(),
                Arg.Any<object[]>()).Returns(x => new ReadOnlyModelWrapper((ReadOnlyModel) ((object[]) x[3])[0]));
            typeCreator.Populate(Arg.Any<object>(), Arg.Any<IExecuteStrategy>()).Returns(x => x[0]);
            valueGenerator.IsSupported(typeof(Guid), "value", Arg.Any<IBuildChain>()).Returns(true);
            valueGenerator.Generate(typeof(Guid), "value", Arg.Any<IExecuteStrategy>()).Returns(value);

            var actual = target.Create(typeof(AdditionalWrapper), number);

            actual.Should().NotBeNull();
            propertyResolver.Received(1).ShouldPopulateProperty(
                buildConfiguration,
                Arg.Is<object>(x => x.GetType() == typeof(ReadOnlyModel)),
                Arg.Is<PropertyInfo>(x => x.Name == nameof(ReadOnlyModel.Value)),
                Arg.Is<object[]>(x => (Guid) x[0] == value));
        }

        [Fact]
        public void CreateDoesNotBuildPropertiesWhenTypeCreatorDisablesAutoPopulateTest()
        {
            var model = new SlimModel();
            var typeCreators = new Collection<ITypeCreator>();

            var typeCreator = Substitute.For<ITypeCreator>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();

            typeCreators.Add(typeCreator);

            buildConfiguration.TypeCreators.Returns(typeCreators);

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            typeCreator.CanCreate(typeof(SlimModel), null, Arg.Any<IBuildChain>()).Returns(true);
            typeCreator.Create(typeof(SlimModel), null, Arg.Any<IExecuteStrategy>()).Returns(model);
            typeCreator.Priority.Returns(1);
            typeCreator.AutoPopulate.Returns(false);
            typeCreator.Populate(model, target).Returns(model);

            var actual = (SlimModel) target.Create(typeof(SlimModel));

            actual.Should().BeSameAs(model);
            actual.Value.Should().BeEmpty();
        }

        [Fact]
        public void CreateEvaluatesPostBuildActionsInOrderOfDescendingPriorityTest()
        {
            var firstAction = Substitute.For<IPostBuildAction>();
            var secondAction = Substitute.For<IPostBuildAction>();
            var buildConfiguration = Model.UsingDefaultConfiguration().Add(firstAction).Add(secondAction);
            var executeCount = 0;

            firstAction.IsSupported(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<IBuildChain>()).Returns(true);
            secondAction.IsSupported(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<IBuildChain>()).Returns(true);
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

            postBuildAction.IsSupported(typeof(Company), nameof(ReadOnlyParent.Company), Arg.Any<IBuildChain>())
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

            firstAction.IsSupported(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<IBuildChain>()).Returns(false);
            secondAction.IsSupported(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<IBuildChain>()).Returns(true);

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            target.Create(typeof(Simple));

            firstAction.DidNotReceive().Execute(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<IBuildChain>());
            secondAction.Received().Execute(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<IBuildChain>());
        }

        [Fact]
        public void CreateReturnsInstanceFromBuildChainWhenCircularReferenceDetectedTest()
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
            typeCreator.CanCreate(typeof(SelfReferrer), null, Arg.Any<IBuildChain>()).Returns(true);
            typeCreator.Create(typeof(SelfReferrer), null, Arg.Any<IExecuteStrategy>()).Returns(expected);
            typeCreator.Populate(expected, target).Returns(expected);
            typeCreator.AutoPopulate.Returns(true);
            generator.IsSupported(typeof(Guid), "Id", Arg.Is<IBuildChain>(x => x.Last == expected)).Returns(true);
            generator.Generate(typeof(Guid), "Id", Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last == expected))
                .Returns(id);

            var actual = (SelfReferrer) target.Create(typeof(SelfReferrer));

            actual.Should().Be(expected);
            actual.Id.Should().Be(id);
        }

        [Fact]
        public void CreateReturnsNullWhenInstanceFailsToBeCreatedTest()
        {
            var typeCreators = new Collection<ITypeCreator>();

            var typeCreator = Substitute.For<ITypeCreator>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();

            typeCreators.Add(typeCreator);

            buildConfiguration.TypeCreators.Returns(typeCreators);
            typeCreator.CanCreate(typeof(SlimModel), null, Arg.Any<IBuildChain>()).Returns(true);

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            var actual = target.Create(typeof(SlimModel));

            actual.Should().BeNull();
        }

        [Fact]
        public void CreateReturnsNullWhenNoReferenceTypeCreatedTest()
        {
            var typeCreators = new Collection<ITypeCreator>();

            var typeCreator = Substitute.For<ITypeCreator>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();

            typeCreators.Add(typeCreator);

            buildConfiguration.TypeCreators.Returns(typeCreators);
            typeCreator.CanCreate(typeof(MemoryStream), null, Arg.Any<IBuildChain>()).Returns(true);
            typeCreator.Create(typeof(MemoryStream), null, Arg.Any<IExecuteStrategy>()).Returns(null);

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            var actual = target.Create(typeof(MemoryStream));

            actual.Should().BeNull();
        }

        [Fact]
        public void CreateReturnsNullWhenNoValueTypeCreatedTest()
        {
            var valueGenerators = new Collection<IValueGenerator>();

            var valueGenerator = Substitute.For<IValueGenerator>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();

            valueGenerators.Add(valueGenerator);

            buildConfiguration.ValueGenerators.Returns(valueGenerators);
            valueGenerator.IsSupported(typeof(int), null, Arg.Any<IBuildChain>()).Returns(true);
            valueGenerator.Generate(typeof(int), null, Arg.Any<IExecuteStrategy>()).Returns(null);

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            var actual = target.Create(typeof(int));

            actual.Should().BeNull();
        }

        [Fact]
        public void CreateReturnsReferenceTypeFromCreatorTest()
        {
            var expected = new SlimModel();
            var value = Guid.NewGuid();

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
            typeCreator.CanCreate(typeof(SlimModel), null, Arg.Any<IBuildChain>()).Returns(true);
            typeCreator.Create(typeof(SlimModel), null, Arg.Any<IExecuteStrategy>()).Returns(expected);
            typeCreator.Populate(expected, target).Returns(expected);
            typeCreator.AutoPopulate.Returns(true);
            generator.IsSupported(typeof(Guid), "Value", Arg.Is<IBuildChain>(x => x.Last == expected)).Returns(true);
            generator.Generate(typeof(Guid), "Value", Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last == expected))
                .Returns(value);

            var actual = (SlimModel) target.Create(typeof(SlimModel));

            actual.Should().Be(expected);
            actual.Value.Should().Be(value);
        }

        [Fact]
        public void CreateReturnsReferenceTypeFromCreatorWhenExecuteOrderRulesIsNullTest()
        {
            var expected = new SlimModel();
            var value = Guid.NewGuid();

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
            buildConfiguration.ExecuteOrderRules.Returns((ICollection<ExecuteOrderRule>)null);

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            propertyResolver.CanPopulate(Arg.Any<PropertyInfo>()).Returns(true);
            propertyResolver.ShouldPopulateProperty(
                Arg.Any<IBuildConfiguration>(),
                Arg.Any<object>(),
                Arg.Any<PropertyInfo>(),
                Arg.Any<object[]>()).Returns(true);
            typeCreator.CanCreate(typeof(SlimModel), null, Arg.Any<IBuildChain>()).Returns(true);
            typeCreator.Create(typeof(SlimModel), null, Arg.Any<IExecuteStrategy>()).Returns(expected);
            typeCreator.Populate(expected, target).Returns(expected);
            typeCreator.AutoPopulate.Returns(true);
            generator.IsSupported(typeof(Guid), "Value", Arg.Is<IBuildChain>(x => x.Last == expected)).Returns(true);
            generator.Generate(typeof(Guid), "Value", Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last == expected))
                .Returns(value);

            var actual = (SlimModel)target.Create(typeof(SlimModel));

            actual.Should().Be(expected);
            actual.Value.Should().Be(value);
        }

        [Fact]
        public void CreateReturnsValueCreatedFromProvidedArgumentsTest()
        {
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
            var typeCreators = new Collection<ITypeCreator>();

            var typeCreator = Substitute.For<ITypeCreator>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();

            typeCreators.Add(typeCreator);

            buildConfiguration.TypeCreators.Returns(typeCreators);

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            typeCreator.CanCreate(typeof(Person), null, Arg.Any<IBuildChain>()).Returns(true);
            typeCreator.Create(typeof(Person), null, Arg.Any<IExecuteStrategy>(), args).Returns(expected);
            typeCreator.Populate(expected, target).Returns(expected);
            typeCreator.AutoPopulate.Returns(false);

            var actual = target.Create(typeof(Person), args);

            actual.Should().BeSameAs(expected);
        }

        [Fact]
        public void CreateReturnsValueFromCreatorWithAutomaticTypeMappingTest()
        {
            var model = new TestItem();
            var value = Guid.NewGuid().ToString();
            var typeCreators = new Collection<ITypeCreator>();
            var valueGenerators = new Collection<IValueGenerator>();
            var typeMappingRules = new Collection<TypeMappingRule>();

            var propertyResolver = Substitute.For<IPropertyResolver>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var creator = Substitute.For<ITypeCreator>();
            var generator = Substitute.For<IValueGenerator>();

            typeCreators.Add(creator);
            valueGenerators.Add(generator);

            buildConfiguration.PropertyResolver.Returns(propertyResolver);
            buildConfiguration.TypeCreators.Returns(typeCreators);
            buildConfiguration.ValueGenerators.Returns(valueGenerators);
            buildConfiguration.TypeMappingRules.Returns(typeMappingRules);

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            propertyResolver.CanPopulate(Arg.Any<PropertyInfo>()).Returns(true);
            propertyResolver.ShouldPopulateProperty(
                Arg.Any<IBuildConfiguration>(),
                Arg.Any<object>(),
                Arg.Any<PropertyInfo>(),
                Arg.Any<object[]>()).Returns(true);
            creator.CanCreate(typeof(TestItem), null, Arg.Any<IBuildChain>()).Returns(true);
            creator.Create(typeof(TestItem), null, Arg.Any<IExecuteStrategy>()).Returns(model);
            creator.Priority.Returns(1);
            creator.AutoPopulate.Returns(true);
            creator.Populate(model, target).Returns(model);
            generator.IsSupported(typeof(string), "FirstName", Arg.Is<IBuildChain>(x => x.Last == model)).Returns(true);
            generator.Generate(typeof(string), "FirstName", Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last == model))
                .Returns(value);

            var actual = (ITestItem) target.Create(typeof(ITestItem));

            actual.Should().BeOfType<TestItem>();
            actual.FirstName.Should().Be(value);
        }

        [Fact]
        public void CreateReturnsValueFromCreatorWithHighestPriorityTest()
        {
            var firstModel = new SlimModel();
            var secondModel = new SlimModel();
            var value = Guid.NewGuid();
            var typeCreators = new Collection<ITypeCreator>();
            var valueGenerators = new Collection<IValueGenerator>();

            var propertyResolver = Substitute.For<IPropertyResolver>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var firstCreator = Substitute.For<ITypeCreator>();
            var secondCreator = Substitute.For<ITypeCreator>();
            var generator = Substitute.For<IValueGenerator>();

            typeCreators.Add(firstCreator);
            typeCreators.Add(secondCreator);
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
            firstCreator.CanCreate(typeof(SlimModel), null, Arg.Any<IBuildChain>()).Returns(true);
            firstCreator.Create(typeof(SlimModel), null, Arg.Any<IExecuteStrategy>()).Returns(firstModel);
            firstCreator.Priority.Returns(1);
            firstCreator.AutoPopulate.Returns(true);
            firstCreator.Populate(firstModel, target).Returns(firstModel);
            secondCreator.CanCreate(typeof(SlimModel), null, Arg.Any<IBuildChain>()).Returns(true);
            secondCreator.Create(typeof(SlimModel), null, Arg.Any<IExecuteStrategy>()).Returns(secondModel);
            secondCreator.Priority.Returns(2);
            secondCreator.AutoPopulate.Returns(true);
            secondCreator.Populate(secondModel, target).Returns(secondModel);
            generator.IsSupported(typeof(Guid), "Value", Arg.Is<IBuildChain>(x => x.Last == secondModel)).Returns(true);
            generator.Generate(typeof(Guid), "Value", Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last == secondModel))
                .Returns(value);

            var actual = (SlimModel) target.Create(typeof(SlimModel));

            actual.Should().BeSameAs(secondModel);
            actual.Value.Should().Be(value);
        }

        [Fact]
        public void CreateReturnsValueFromCreatorWithNoArgumentsAndDetectConstructorDisabledTest()
        {
            var expected = new Person();
            var typeCreators = new Collection<ITypeCreator>();

            var typeCreator = Substitute.For<ITypeCreator>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();

            typeCreators.Add(typeCreator);

            buildConfiguration.TypeCreators.Returns(typeCreators);

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            typeCreator.CanCreate(typeof(Person), null, Arg.Any<IBuildChain>()).Returns(true);
            typeCreator.Create(typeof(Person), null, Arg.Any<IExecuteStrategy>()).Returns(expected);
            typeCreator.Populate(expected, target).Returns(expected);
            typeCreator.AutoPopulate.Returns(false);
            typeCreator.AutoDetectConstructor.Returns(false);

            var actual = target.Create(typeof(Person));

            actual.Should().BeSameAs(expected);
        }

        [Fact]
        public void CreateReturnsValueFromCreatorWithNoArgumentsAndDetectConstructorEnabledTest()
        {
            var expected = new Person();
            var typeCreators = new Collection<ITypeCreator>();

            var typeCreator = Substitute.For<ITypeCreator>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var resolver = Substitute.For<IConstructorResolver>();

            typeCreators.Add(typeCreator);

            buildConfiguration.TypeCreators.Returns(typeCreators);
            buildConfiguration.ConstructorResolver.Returns(resolver);

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            resolver.Resolve(typeof(Person))
                .Returns(typeof(Person).GetConstructors().Single(x => x.GetParameters().Length == 0));
            typeCreator.CanCreate(typeof(Person), null, Arg.Any<IBuildChain>()).Returns(true);
            typeCreator.Create(typeof(Person), null, Arg.Any<IExecuteStrategy>()).Returns(expected);
            typeCreator.Populate(expected, target).Returns(expected);
            typeCreator.AutoPopulate.Returns(false);
            typeCreator.AutoDetectConstructor.Returns(true);

            var actual = target.Create(typeof(Person));

            actual.Should().BeSameAs(expected);
        }

        [Fact]
        public void CreateReturnsValueFromCreatorWithTypeMappingTest()
        {
            var model = new TestItem();
            var value = Guid.NewGuid().ToString();
            var typeCreators = new Collection<ITypeCreator>();
            var valueGenerators = new Collection<IValueGenerator>();
            var typeMapping = new TypeMappingRule(typeof(ITestItem), typeof(TestItem));
            var typeMappingRules = new Collection<TypeMappingRule>
            {
                typeMapping
            };

            var propertyResolver = Substitute.For<IPropertyResolver>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var creator = Substitute.For<ITypeCreator>();
            var generator = Substitute.For<IValueGenerator>();

            typeCreators.Add(creator);
            valueGenerators.Add(generator);

            buildConfiguration.PropertyResolver.Returns(propertyResolver);
            buildConfiguration.TypeCreators.Returns(typeCreators);
            buildConfiguration.ValueGenerators.Returns(valueGenerators);
            buildConfiguration.TypeMappingRules.Returns(typeMappingRules);

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            propertyResolver.CanPopulate(Arg.Any<PropertyInfo>()).Returns(true);
            propertyResolver.ShouldPopulateProperty(
                Arg.Any<IBuildConfiguration>(),
                Arg.Any<object>(),
                Arg.Any<PropertyInfo>(),
                Arg.Any<object[]>()).Returns(true);
            creator.CanCreate(typeof(TestItem), null, Arg.Any<IBuildChain>()).Returns(true);
            creator.Create(typeof(TestItem), null, Arg.Any<IExecuteStrategy>()).Returns(model);
            creator.Priority.Returns(1);
            creator.AutoPopulate.Returns(true);
            creator.Populate(model, target).Returns(model);
            generator.IsSupported(typeof(string), "FirstName", Arg.Is<IBuildChain>(x => x.Last == model)).Returns(true);
            generator.Generate(typeof(string), "FirstName", Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last == model))
                .Returns(value);

            var actual = (ITestItem) target.Create(typeof(ITestItem));

            actual.Should().BeOfType<TestItem>();
            actual.FirstName.Should().Be(value);
        }

        [Fact]
        public void CreateReturnsValueFromGeneratorWithHighestPriorityTest()
        {
            var firstValue = Guid.NewGuid();
            var secondValue = Guid.NewGuid();
            var valueGenerators = new Collection<IValueGenerator>();

            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var firstGenerator = Substitute.For<IValueGenerator>();
            var secondGenerator = Substitute.For<IValueGenerator>();

            valueGenerators.Add(firstGenerator);
            valueGenerators.Add(secondGenerator);

            buildConfiguration.ValueGenerators.Returns(valueGenerators);
            firstGenerator.IsSupported(typeof(Guid), null, Arg.Any<IBuildChain>()).Returns(true);
            firstGenerator.Generate(typeof(Guid), null, Arg.Any<IExecuteStrategy>()).Returns(firstValue);
            firstGenerator.Priority.Returns(1);
            secondGenerator.IsSupported(typeof(Guid), null, Arg.Any<IBuildChain>()).Returns(true);
            secondGenerator.Generate(typeof(Guid), null, Arg.Any<IExecuteStrategy>()).Returns(secondValue);
            secondGenerator.Priority.Returns(2);

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            var actual = target.Create(typeof(Guid));

            actual.Should().Be(secondValue);
        }

        [Fact]
        public void CreateReturnsValueFromResolvedConstructorAndBuiltParametersTest()
        {
            var value = Guid.NewGuid();
            var expected = new ReadOnlyModel(value);
            var valueGenerators = new Collection<IValueGenerator>();
            var typeCreators = new Collection<ITypeCreator>();

            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var resolver = Substitute.For<IConstructorResolver>();
            var generator = Substitute.For<IValueGenerator>();
            var typeCreator = Substitute.For<ITypeCreator>();

            typeCreators.Add(typeCreator);
            valueGenerators.Add(generator);

            resolver.Resolve(typeof(ReadOnlyModel), Arg.Any<object[]>())
                .Returns(typeof(ReadOnlyModel).GetConstructors()[0]);
            buildConfiguration.ConstructorResolver.Returns(resolver);
            buildConfiguration.TypeCreators.Returns(typeCreators);
            buildConfiguration.ValueGenerators.Returns(valueGenerators);

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            typeCreator.CanCreate(typeof(ReadOnlyModel), null, Arg.Any<IBuildChain>()).Returns(true);
            typeCreator.Create(typeof(ReadOnlyModel), null, Arg.Any<IExecuteStrategy>(), value).Returns(expected);
            typeCreator.Populate(expected, target).Returns(expected);
            typeCreator.AutoPopulate.Returns(true);
            typeCreator.AutoDetectConstructor.Returns(true);
            generator.IsSupported(typeof(Guid), "value", Arg.Any<IBuildChain>()).Returns(true);
            generator.Generate(typeof(Guid), "value", Arg.Any<IExecuteStrategy>()).Returns(value);

            var actual = (ReadOnlyModel) target.Create(typeof(ReadOnlyModel));

            actual.Should().Be(expected);
            actual.Value.Should().Be(value);
        }

        [Fact]
        public void CreateReturnsValueFromSupportedCreationRuleTest()
        {
            var firstValue = Guid.NewGuid().ToString();
            var secondValue = Guid.NewGuid();
            
            var buildConfiguration = Model.UsingDefaultConfiguration()
                .Add(new CreationRule(typeof(Address), "Id", 100, firstValue))
                .Add(new CreationRule(typeof(Person), "Id", 20, secondValue));

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            var actual = (Person) target.Create(typeof(Person));

            actual.Id.Should().Be(secondValue);
        }

        [Fact]
        public void CreateReturnsValueFromSupportedCreationRuleWithHigherPriorityTest()
        {
            var firstValue = Guid.NewGuid();
            var secondValue = Guid.NewGuid();

            var buildConfiguration = Model.UsingDefaultConfiguration()
                .Add(new CreationRule(typeof(Person), "Id", 10, firstValue))
                .Add(new CreationRule(typeof(Person), "Id", 20, secondValue));

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            var actual = (Person) target.Create(typeof(Person));

            actual.Id.Should().Be(secondValue);
        }

        [Fact]
        public void CreateReturnsValueFromSupportedCreatorTest()
        {
            var firstModel = new SlimModel();
            var secondModel = new SlimModel();
            var value = Guid.NewGuid();
            var valueGenerators = new Collection<IValueGenerator>();
            var typeCreators = new Collection<ITypeCreator>();

            var propertyResolver = Substitute.For<IPropertyResolver>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var firstCreator = Substitute.For<ITypeCreator>();
            var secondCreator = Substitute.For<ITypeCreator>();
            var generator = Substitute.For<IValueGenerator>();

            typeCreators.Add(firstCreator);
            typeCreators.Add(secondCreator);
            valueGenerators.Add(generator);

            buildConfiguration.TypeCreators.Returns(typeCreators);
            buildConfiguration.ValueGenerators.Returns(valueGenerators);

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            buildConfiguration.PropertyResolver.Returns(propertyResolver);
            propertyResolver.CanPopulate(Arg.Any<PropertyInfo>()).Returns(true);
            propertyResolver.ShouldPopulateProperty(
                Arg.Any<IBuildConfiguration>(),
                Arg.Any<object>(),
                Arg.Any<PropertyInfo>(),
                Arg.Any<object[]>()).Returns(true);
            firstCreator.CanCreate(typeof(SlimModel), null, Arg.Any<IBuildChain>()).Returns(false);
            firstCreator.Create(typeof(SlimModel), null, Arg.Any<IExecuteStrategy>()).Returns(firstModel);
            firstCreator.Priority.Returns(10);
            firstCreator.AutoPopulate.Returns(true);
            firstCreator.Populate(firstModel, target).Returns(firstModel);
            secondCreator.CanCreate(typeof(SlimModel), null, Arg.Any<IBuildChain>()).Returns(true);
            secondCreator.Create(typeof(SlimModel), null, Arg.Any<IExecuteStrategy>()).Returns(secondModel);
            secondCreator.Priority.Returns(2);
            secondCreator.AutoPopulate.Returns(true);
            secondCreator.Populate(secondModel, target).Returns(secondModel);
            generator.IsSupported(typeof(Guid), "Value", Arg.Is<IBuildChain>(x => x.Last == secondModel)).Returns(true);
            generator.Generate(typeof(Guid), "Value", Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last == secondModel))
                .Returns(value);

            var actual = (SlimModel) target.Create(typeof(SlimModel));

            actual.Should().BeSameAs(secondModel);
            actual.Value.Should().Be(value);
        }

        [Fact]
        public void CreateReturnsValueFromSupportedGeneratorTest()
        {
            var firstValue = Guid.NewGuid();
            var secondValue = Guid.NewGuid();
            var valueGenerators = new Collection<IValueGenerator>();

            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var firstGenerator = Substitute.For<IValueGenerator>();
            var secondGenerator = Substitute.For<IValueGenerator>();

            valueGenerators.Add(firstGenerator);
            valueGenerators.Add(secondGenerator);

            buildConfiguration.ValueGenerators.Returns(valueGenerators);
            firstGenerator.IsSupported(typeof(Guid), null, Arg.Any<IBuildChain>()).Returns(false);
            firstGenerator.Generate(typeof(Guid), null, Arg.Any<IExecuteStrategy>()).Returns(firstValue);
            firstGenerator.Priority.Returns(10);
            secondGenerator.IsSupported(typeof(Guid), null, Arg.Any<IBuildChain>()).Returns(true);
            secondGenerator.Generate(typeof(Guid), null, Arg.Any<IExecuteStrategy>()).Returns(secondValue);
            secondGenerator.Priority.Returns(2);

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            var actual = target.Create(typeof(Guid));

            actual.Should().Be(secondValue);
        }

        [Fact]
        public void CreateReturnsValueTypeFromGeneratorTest()
        {
            var expected = Guid.NewGuid().ToString();
            var valueGenerators = new Collection<IValueGenerator>();

            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var valueGenerator = Substitute.For<IValueGenerator>();

            valueGenerators.Add(valueGenerator);

            buildConfiguration.ValueGenerators.Returns(valueGenerators);
            valueGenerator.IsSupported(typeof(string), null, Arg.Any<IBuildChain>()).Returns(true);
            valueGenerator.Generate(typeof(string), null, Arg.Any<IExecuteStrategy>()).Returns(expected);

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            var actual = target.Create(typeof(string));

            actual.Should().Be(expected);
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
        public void CreateThrowsExceptionWhenConstructorParameterCannotBeCreatedTest()
        {
            var typeCreator = new DefaultTypeCreator();
            var typeCreators = new Collection<ITypeCreator>();

            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var resolver = Substitute.For<IConstructorResolver>();

            typeCreators.Add(typeCreator);

            resolver.Resolve(typeof(ReadOnlyModel), Arg.Any<object[]>())
                .Returns(typeof(ReadOnlyModel).GetConstructors()[0]);
            buildConfiguration.ConstructorResolver.Returns(resolver);
            buildConfiguration.TypeCreators.Returns(typeCreators);

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            Action action = () => target.Create(typeof(ReadOnlyModel));

            action.Should().Throw<BuildException>();
        }

        [Fact]
        public void CreateThrowsExceptionWhenCreatingTypeFailsTest()
        {
            var typeCreator = Substitute.For<ITypeCreator>();

            typeCreator.CanCreate(typeof(Address), "Address", Arg.Any<IBuildChain>()).Returns(true);
            typeCreator.Priority.Returns(int.MaxValue);
            typeCreator.AutoDetectConstructor.Returns(true);
            typeCreator.AutoPopulate.Returns(true);
            typeCreator.Create(typeof(Address), "Address", Arg.Any<IExecuteStrategy>())
                .Throws(new InvalidOperationException());

            var buildConfiguration = Model.UsingDefaultConfiguration().Add(typeCreator);

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            Action action = () => target.Create(typeof(Person));

            var exception = action.Should().Throw<BuildException>().Where(x => x.Message != null)
                .Where(x => x.BuildLog != null).Which;

            _output.WriteLine(exception.Message);
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
        public void CreateThrowsExceptionWhenGeneratingValueFailsTest()
        {
            var person = new Person();
            var generators = new Collection<IValueGenerator>();
            var creators = new Collection<ITypeCreator>();

            var valueGenerator = Substitute.For<IValueGenerator>();
            var typeCreator = Substitute.For<ITypeCreator>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();

            generators.Add(valueGenerator);
            creators.Add(typeCreator);

            typeCreator.CanCreate(typeof(Person), null, Arg.Any<IBuildChain>()).Returns(true);
            typeCreator.Create(typeof(Person), null, null, null).Returns(person);
            valueGenerator.IsSupported(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<IBuildChain>()).Returns(true);
            valueGenerator.Generate(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<IExecuteStrategy>())
                .Throws(new InvalidOperationException());
            buildConfiguration.TypeCreators.Returns(creators);
            buildConfiguration.ValueGenerators.Returns(generators);

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            Action action = () => target.Create(typeof(Person));

            var exception = action.Should().Throw<BuildException>().Where(x => x.Message != null)
                .Where(x => x.BuildLog != null).Which;

            _output.WriteLine(exception.Message);
        }

        [Fact]
        public void CreateThrowsExceptionWhenGeneratingValueThrowsBuildExceptionTest()
        {
            var person = new Person();
            var generators = new Collection<IValueGenerator>();
            var creators = new Collection<ITypeCreator>();

            var valueGenerator = Substitute.For<IValueGenerator>();
            var typeCreator = Substitute.For<ITypeCreator>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();

            generators.Add(valueGenerator);
            creators.Add(typeCreator);

            typeCreator.CanCreate(typeof(Person), null, Arg.Any<IBuildChain>()).Returns(true);
            typeCreator.Create(typeof(Person), null, Arg.Any<IExecuteStrategy>()).Returns(person);
            typeCreator.AutoPopulate.Returns(true);
            valueGenerator.IsSupported(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<IBuildChain>()).Returns(true);
            valueGenerator.Generate(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<IExecuteStrategy>())
                .Throws(new BuildException());
            buildConfiguration.TypeCreators.Returns(creators);
            buildConfiguration.ValueGenerators.Returns(generators);

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            Action action = () => target.Create(typeof(Person));

            action.Should().Throw<BuildException>();
        }

        [Fact]
        public void CreateThrowsExceptionWhenNoCreatorMatchFoundForReadOnlyChildPropertyTest()
        {
            var expected = new SimpleReadOnlyParent();
            var generators = new Collection<IValueGenerator>();
            var creators = new Collection<ITypeCreator>();

            var propertyResolver = Substitute.For<IPropertyResolver>();
            var valueGenerator = Substitute.For<IValueGenerator>();
            var typeCreator = Substitute.For<ITypeCreator>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();

            buildConfiguration.PropertyResolver.Returns(propertyResolver);
            generators.Add(valueGenerator);
            creators.Add(typeCreator);

            propertyResolver.CanPopulate(Arg.Any<PropertyInfo>()).Returns(true);
            propertyResolver.ShouldPopulateProperty(
                Arg.Any<IBuildConfiguration>(),
                Arg.Any<object>(),
                Arg.Any<PropertyInfo>(),
                Arg.Any<object[]>()).Returns(true);
            typeCreator.CanCreate(typeof(SimpleReadOnlyParent), null, Arg.Any<IBuildChain>()).Returns(true);
            typeCreator.Create(typeof(SimpleReadOnlyParent), null, Arg.Any<IExecuteStrategy>()).Returns(expected);
            typeCreator.AutoPopulate.Returns(true);
            valueGenerator.IsSupported(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<IBuildChain>()).Returns(false);
            buildConfiguration.TypeCreators.Returns(creators);
            buildConfiguration.ValueGenerators.Returns(generators);

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            Action action = () => target.Create(typeof(SimpleReadOnlyParent));

            action.Should().Throw<BuildException>();
        }

        [Fact]
        public void CreateThrowsExceptionWhenNoGeneratorOrCreatorMatchFoundForChildPropertyTest()
        {
            var person = new Person();
            var generators = new Collection<IValueGenerator>();
            var creators = new Collection<ITypeCreator>();

            var propertyResolver = Substitute.For<IPropertyResolver>();
            var valueGenerator = Substitute.For<IValueGenerator>();
            var typeCreator = Substitute.For<ITypeCreator>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();

            buildConfiguration.PropertyResolver.Returns(propertyResolver);
            generators.Add(valueGenerator);
            creators.Add(typeCreator);

            propertyResolver.CanPopulate(Arg.Any<PropertyInfo>()).Returns(true);
            propertyResolver.ShouldPopulateProperty(
                Arg.Any<IBuildConfiguration>(),
                Arg.Any<object>(),
                Arg.Any<PropertyInfo>(),
                Arg.Any<object[]>()).Returns(true);
            typeCreator.CanCreate(typeof(Person), null, Arg.Any<IBuildChain>()).Returns(true);
            typeCreator.Create(typeof(Person), null, Arg.Any<IExecuteStrategy>()).Returns(person);
            typeCreator.AutoPopulate.Returns(true);
            valueGenerator.IsSupported(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<IBuildChain>()).Returns(false);
            buildConfiguration.TypeCreators.Returns(creators);
            buildConfiguration.ValueGenerators.Returns(generators);

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            Action action = () => target.Create(typeof(Person));

            action.Should().Throw<BuildException>();
        }

        [Fact]
        public void CreateThrowsExceptionWhenNoGeneratorOrCreatorMatchFoundForConstructorParameterTest()
        {
            var generators = new Collection<IValueGenerator>();
            var creators = new Collection<ITypeCreator>();
            var constructorResolver = new DefaultConstructorResolver();

            var valueGenerator = Substitute.For<IValueGenerator>();
            var typeCreator = Substitute.For<ITypeCreator>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();

            generators.Add(valueGenerator);
            creators.Add(typeCreator);

            typeCreator.CanCreate(typeof(KeyValuePair<string, Person>), null, Arg.Any<IBuildChain>()).Returns(true);
            typeCreator.AutoDetectConstructor.Returns(true);
            typeCreator.AutoPopulate.Returns(true);
            valueGenerator.IsSupported(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<IBuildChain>()).Returns(false);
            buildConfiguration.TypeCreators.Returns(creators);
            buildConfiguration.ValueGenerators.Returns(generators);
            buildConfiguration.ConstructorResolver.Returns(constructorResolver);

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            Action action = () => target.Create(typeof(KeyValuePair<string, Person>));

            action.Should().Throw<BuildException>();
        }

        [Fact]
        public void CreateThrowsExceptionWhenNoGeneratorOrCreatorMatchFoundTest()
        {
            var buildConfiguration = Substitute.For<IBuildConfiguration>();

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            Action action = () => target.Create(typeof(string));

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
        public void CreateThrowsExceptionWhenPropertyCannotBeCreatedTest()
        {
            var typeCreator = new DefaultTypeCreator();
            var typeCreators = new Collection<ITypeCreator>();

            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var resolver = Substitute.For<IConstructorResolver>();
            var propertyResolver = Substitute.For<IPropertyResolver>();

            typeCreators.Add(typeCreator);

            buildConfiguration.PropertyResolver.Returns(propertyResolver);
            propertyResolver.CanPopulate(Arg.Any<PropertyInfo>()).Returns(true);
            propertyResolver.ShouldPopulateProperty(
                Arg.Any<IBuildConfiguration>(),
                Arg.Any<object>(),
                Arg.Any<PropertyInfo>(),
                Arg.Any<object[]>()).Returns(true);
            resolver.Resolve(typeof(SlimModel), Arg.Any<object[]>()).Returns(typeof(SlimModel).GetConstructors()[0]);
            buildConfiguration.ConstructorResolver.Returns(resolver);
            buildConfiguration.TypeCreators.Returns(typeCreators);

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            Action action = () => target.Create(typeof(SlimModel));

            action.Should().Throw<BuildException>();
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
        public void PopulateAsObjectAssignsPropertyValuesToExistingInstanceTest()
        {
            var staff = new List<Person>();
            var name = Guid.NewGuid().ToString();
            var address = Guid.NewGuid().ToString();
            var expected = new Company();
            var valueGenerators = new Collection<IValueGenerator>();
            var typeCreators = new Collection<ITypeCreator>();

            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var typeCreator = Substitute.For<ITypeCreator>();
            var enumerableTypeCreator = Substitute.For<ITypeCreator>();
            var valueGenerator = Substitute.For<IValueGenerator>();
            var propertyResolver = Substitute.For<IPropertyResolver>();

            typeCreators.Add(typeCreator);
            typeCreators.Add(enumerableTypeCreator);
            valueGenerators.Add(valueGenerator);

            buildConfiguration.TypeCreators.Returns(typeCreators);
            buildConfiguration.ValueGenerators.Returns(valueGenerators);

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            buildConfiguration.PropertyResolver.Returns(propertyResolver);
            propertyResolver.CanPopulate(Arg.Any<PropertyInfo>()).Returns(true);
            propertyResolver.ShouldPopulateProperty(
                Arg.Any<IBuildConfiguration>(),
                Arg.Any<object>(),
                Arg.Any<PropertyInfo>(),
                Arg.Any<object[]>()).Returns(true);
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
            valueGenerator.IsSupported(typeof(string), "Name", Arg.Is<IBuildChain>(x => x.Last == expected))
                .Returns(true);
            valueGenerator.Generate(
                typeof(string),
                "Name",
                Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last == expected)).Returns(name);
            valueGenerator.IsSupported(typeof(string), "Address", Arg.Is<IBuildChain>(x => x.Last == expected))
                .Returns(true);
            valueGenerator.Generate(
                typeof(string),
                "Address",
                Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last == expected)).Returns(address);

            var actual = (Company) target.Populate(expected);

            actual.Should().BeSameAs(expected);
            actual.Name.Should().Be(name);
            actual.Address.Should().Be(address);
            actual.Staff.Should().BeSameAs(staff);
        }

        [Fact]
        public void PopulateAssignsPropertyValuesToExistingInstanceTest()
        {
            var staff = new List<Person>();
            var name = Guid.NewGuid().ToString();
            var address = Guid.NewGuid().ToString();
            var expected = new Company();
            var valueGenerators = new Collection<IValueGenerator>();
            var typeCreators = new Collection<ITypeCreator>();

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
            buildConfiguration.TypeCreators.Returns(typeCreators);
            buildConfiguration.ValueGenerators.Returns(valueGenerators);

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
            valueGenerator.IsSupported(typeof(string), "Name", Arg.Is<IBuildChain>(x => x.Last == expected))
                .Returns(true);
            valueGenerator.Generate(
                typeof(string),
                "Name",
                Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last == expected)).Returns(name);
            valueGenerator.IsSupported(typeof(string), "Address", Arg.Is<IBuildChain>(x => x.Last == expected))
                .Returns(true);
            valueGenerator.Generate(
                typeof(string),
                "Address",
                Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last == expected)).Returns(address);

            var actual = (Company) target.Populate(expected);

            actual.Should().BeSameAs(expected);
            actual.Name.Should().Be(name);
            actual.Address.Should().Be(address);
            actual.Staff.Should().BeSameAs(staff);
        }

        [Fact]
        public void PopulateAssignsValueUsingDefaultExecuteOrderRulesTest()
        {
            var first = SimpleEnum.Seventh;
            var second = Environment.TickCount;
            var third = Guid.NewGuid().ToString();
            var fourth = new Person();
            var expected = new PopulateOrderItem();
            var valueGenerators = new Collection<IValueGenerator>();
            var typeCreators = new Collection<ITypeCreator>();
            var executeOrderRules = Model.UsingDefaultConfiguration().ExecuteOrderRules;

            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var typeCreator = Substitute.For<ITypeCreator>();
            var personTypeCreator = Substitute.For<ITypeCreator>();
            var valueGenerator = Substitute.For<IValueGenerator>();
            var propertyResolver = Substitute.For<IPropertyResolver>();

            typeCreators.Add(typeCreator);
            typeCreators.Add(personTypeCreator);
            valueGenerators.Add(valueGenerator);

            buildConfiguration.TypeCreators.Returns(typeCreators);
            buildConfiguration.ValueGenerators.Returns(valueGenerators);
            buildConfiguration.ExecuteOrderRules.Returns(executeOrderRules);

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            buildConfiguration.PropertyResolver.Returns(propertyResolver);
            typeCreator.CanPopulate(typeof(PopulateOrderItem), null, Arg.Is<IBuildChain>(x => x.Last == expected))
                .Returns(true);
            typeCreator.AutoPopulate.Returns(true);
            typeCreator.Populate(expected, target).Returns(expected);
            propertyResolver.CanPopulate(Arg.Any<PropertyInfo>()).Returns(true);
            propertyResolver.ShouldPopulateProperty(
                Arg.Any<IBuildConfiguration>(),
                Arg.Any<object>(),
                Arg.Any<PropertyInfo>(),
                Arg.Any<object[]>()).Returns(true);
            valueGenerator.IsSupported(typeof(SimpleEnum), "Z", Arg.Is<IBuildChain>(x => x.Last == expected))
                .Returns(true);
            valueGenerator.Generate(
                typeof(SimpleEnum),
                "Z",
                Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last == expected)).Returns(first);
            valueGenerator.IsSupported(typeof(int), "Y", Arg.Is<IBuildChain>(x => x.Last == expected)).Returns(true);
            valueGenerator.Generate(typeof(int), "Y", Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last == expected))
                .Returns(second);
            valueGenerator.IsSupported(typeof(string), "X", Arg.Is<IBuildChain>(x => x.Last == expected)).Returns(true);
            valueGenerator.Generate(typeof(string), "X", Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last == expected))
                .Returns(third);
            personTypeCreator.CanCreate(typeof(Person), "W", Arg.Is<IBuildChain>(x => x.Last == expected))
                .Returns(true);
            personTypeCreator.Create(typeof(Person), "W", Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last == expected))
                .Returns(fourth);
            personTypeCreator.AutoPopulate.Returns(false);
            personTypeCreator.Populate(fourth, target).Returns(fourth);

            var actual = (PopulateOrderItem) target.Populate(expected);

            actual.Should().BeSameAs(expected);
            actual.Z.Should().Be(first);
            actual.Y.Should().Be(second);
            actual.X.Should().Be(third);
            actual.W.Should().BeSameAs(fourth);
        }

        [Fact]
        public void PopulateDoesNotApplyIgnoreRuleWhenPropertyNameHasDifferentCaseTest()
        {
            var staff = new List<Person>();
            var name = Guid.NewGuid().ToString();
            var address = Guid.NewGuid().ToString();
            var expected = new Company();
            var ignoreRule = new IgnoreRule(typeof(Company), "name");
            var valueGenerators = new Collection<IValueGenerator>();
            var typeCreators = new Collection<ITypeCreator>();
            var ignoreRules = new Collection<IgnoreRule>
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
            valueGenerator.IsSupported(typeof(string), "Name", Arg.Is<IBuildChain>(x => x.Last == expected))
                .Returns(true);
            valueGenerator.Generate(
                typeof(string),
                "Name",
                Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last == expected)).Returns(name);
            valueGenerator.IsSupported(typeof(string), "Address", Arg.Is<IBuildChain>(x => x.Last == expected))
                .Returns(true);
            valueGenerator.Generate(
                typeof(string),
                "Address",
                Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last == expected)).Returns(address);

            var actual = (Company) target.Populate(expected);

            actual.Should().BeSameAs(expected);
            actual.Name.Should().Be(name);
            actual.Address.Should().Be(address);
            actual.Staff.Should().BeSameAs(staff);
        }

        [Fact]
        public void PopulateDoesNotApplyIgnoreRuleWhenPropertyNameNotMatchedTest()
        {
            var staff = new List<Person>();
            var name = Guid.NewGuid().ToString();
            var address = Guid.NewGuid().ToString();
            var expected = new Company();
            var ignoreRule = new IgnoreRule(typeof(Company), "Names");
            var valueGenerators = new Collection<IValueGenerator>();
            var typeCreators = new Collection<ITypeCreator>();
            var ignoreRules = new Collection<IgnoreRule>
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
            valueGenerator.IsSupported(typeof(string), "Name", Arg.Is<IBuildChain>(x => x.Last == expected))
                .Returns(true);
            valueGenerator.Generate(
                typeof(string),
                "Name",
                Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last == expected)).Returns(name);
            valueGenerator.IsSupported(typeof(string), "Address", Arg.Is<IBuildChain>(x => x.Last == expected))
                .Returns(true);
            valueGenerator.Generate(
                typeof(string),
                "Address",
                Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last == expected)).Returns(address);

            var actual = (Company) target.Populate(expected);

            actual.Should().BeSameAs(expected);
            actual.Name.Should().Be(name);
            actual.Address.Should().Be(address);
            actual.Staff.Should().BeSameAs(staff);
        }

        [Fact]
        public void PopulateDoesNotApplyIgnoreRuleWhenPropertyTypeNotMatchedTest()
        {
            var staff = new List<Person>();
            var name = Guid.NewGuid().ToString();
            var address = Guid.NewGuid().ToString();
            var expected = new Company();
            var ignoreRule = new IgnoreRule(typeof(Stream), "Name");
            var valueGenerators = new Collection<IValueGenerator>();
            var typeCreators = new Collection<ITypeCreator>();
            var ignoreRules = new Collection<IgnoreRule>
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
            valueGenerator.IsSupported(typeof(string), "Name", Arg.Is<IBuildChain>(x => x.Last == expected))
                .Returns(true);
            valueGenerator.Generate(
                typeof(string),
                "Name",
                Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last == expected)).Returns(name);
            valueGenerator.IsSupported(typeof(string), "Address", Arg.Is<IBuildChain>(x => x.Last == expected))
                .Returns(true);
            valueGenerator.Generate(
                typeof(string),
                "Address",
                Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last == expected)).Returns(address);

            var actual = (Company) target.Populate(expected);

            actual.Should().BeSameAs(expected);
            actual.Name.Should().Be(name);
            actual.Address.Should().Be(address);
            actual.Staff.Should().BeSameAs(staff);
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
            firstAction.IsSupported(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<IBuildChain>()).Returns(true);
            secondAction.IsSupported(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<IBuildChain>()).Returns(true);
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
            firstAction.IsSupported(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<IBuildChain>()).Returns(false);
            secondAction.IsSupported(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<IBuildChain>()).Returns(true);

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildConfiguration);

            target.Populate(expected);

            firstAction.DidNotReceive().Execute(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<IBuildChain>());
            secondAction.Received().Execute(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<IBuildChain>());
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
            buildConfiguration.CreationRules.Returns(new Collection<CreationRule>());
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

            generator.IsSupported(typeof(Guid), nameof(SlimModel.Value), Arg.Is<IBuildChain>(x => x.Last == instance))
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
            buildConfiguration.CreationRules.Returns(new Collection<CreationRule>());
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

            generator.IsSupported(typeof(Guid), nameof(SlimModel.Value), Arg.Is<IBuildChain>(x => x.Last == expected))
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
            buildConfiguration.CreationRules.Returns(new Collection<CreationRule>());
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

            generator.IsSupported(typeof(string), Arg.Any<string>(), Arg.Any<IBuildChain>()).Returns(true);
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
            generator.IsSupported(typeof(Guid), "Id", Arg.Is<IBuildChain>(x => x.Last == expected)).Returns(true);
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
            var ignoreRule = new IgnoreRule(typeof(Company), "Name");
            var valueGenerators = new Collection<IValueGenerator>();
            var typeCreators = new Collection<ITypeCreator>();
            var ignoreRules = new Collection<IgnoreRule>
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
            valueGenerator.IsSupported(typeof(string), "Name", Arg.Is<IBuildChain>(x => x.Last == expected))
                .Returns(true);
            valueGenerator.Generate(
                typeof(string),
                "Name",
                Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last == expected)).Returns(name);
            valueGenerator.IsSupported(typeof(string), "Address", Arg.Is<IBuildChain>(x => x.Last == expected))
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
            var ignoreRule = new IgnoreRule(typeof(Company), "Name");
            var valueGenerators = new Collection<IValueGenerator>();
            var typeCreators = new Collection<ITypeCreator>();
            var ignoreRules = new Collection<IgnoreRule>
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
            valueGenerator.IsSupported(typeof(string), "Name", Arg.Is<IBuildChain>(x => x.Last == expected))
                .Returns(true);
            valueGenerator.Generate(
                typeof(string),
                "Name",
                Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last == expected)).Returns(name);
            valueGenerator.IsSupported(typeof(string), "Address", Arg.Is<IBuildChain>(x => x.Last == expected))
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
            valueGenerator.IsSupported(typeof(Guid), "Value", Arg.Is<IBuildChain>(x => x.Last == expected))
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
                AutoPopulateInstance(instance, null);
            }
        }

        private class PopulatePropertyWrapper : DefaultExecuteStrategy
        {
            public void RunTest(object instance = null, PropertyInfo propertyInfo = null)
            {
                PopulateProperty(instance, propertyInfo);
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