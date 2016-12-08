namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using FluentAssertions;
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
            var configuration = Model.BuildStrategy;
            var buildLog = configuration.GetBuildLog();

            var target = new DefaultExecuteStrategy();

            target.Initialize(configuration, buildLog);

            target.BuildChain.Should().BeEmpty();

            target.Create(typeof(Company));

            target.BuildChain.Should().BeEmpty();
        }

        [Fact]
        public void CreateEvaluatesPostBuildActionsInOrderOfDescendingPriorityTest()
        {
            var firstAction = Substitute.For<IPostBuildAction>();
            var secondAction = Substitute.For<IPostBuildAction>();
            var buildStrategy = new DefaultBuildStrategyCompiler().Add(firstAction).Add(secondAction).Compile();
            var executeCount = 0;

            firstAction.IsSupported(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<LinkedList<object>>()).Returns(true);
            secondAction.IsSupported(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<LinkedList<object>>()).Returns(true);
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

            target.Initialize(buildStrategy, buildStrategy.GetBuildLog());

            target.Create(typeof(Simple));

            firstAction.Received().Execute(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<LinkedList<object>>());
            secondAction.Received().Execute(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<LinkedList<object>>());
        }

        [Fact]
        public void CreateEvaluatesPostBuildActionsThatSupportTheBuildScenarioTest()
        {
            var firstAction = Substitute.For<IPostBuildAction>();
            var secondAction = Substitute.For<IPostBuildAction>();
            var buildStrategy = new DefaultBuildStrategyCompiler().Add(firstAction).Add(secondAction).Compile();

            firstAction.IsSupported(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<LinkedList<object>>()).Returns(false);
            secondAction.IsSupported(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<LinkedList<object>>()).Returns(true);

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildStrategy, buildStrategy.GetBuildLog());

            target.Create(typeof(Simple));

            firstAction.DidNotReceive().Execute(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<LinkedList<object>>());
            secondAction.Received().Execute(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<LinkedList<object>>());
        }

        [Fact]
        public void CreatePopulatesReadOnlyReferencePropertiesTest()
        {
            var configuration = Model.BuildStrategy;
            var buildLog = configuration.GetBuildLog();

            var target = new DefaultExecuteStrategy<ReadOnlyParent>();

            target.Initialize(configuration, buildLog);

            var actual = target.Create();

            actual.Company.Address.Should().NotBeNullOrWhiteSpace();
            actual.AssignablePeople.Should().NotBeEmpty();
            actual.People.Should().NotBeEmpty();
            actual.RestrictedPeople.Should().BeEmpty();
            actual.Unassigned.Should().BeNull();
        }

        [Fact]
        public void CreatesCircularReferenceWithInstanceFromBuildChainTest()
        {
            var configuration = Model.BuildStrategy;
            var buildLog = configuration.GetBuildLog();

            var target = new DefaultExecuteStrategy<ReadOnlyParent>();

            target.Initialize(configuration, buildLog);

            var actual = (Top)target.Create(typeof(Top));

            actual.Should().NotBeNull();
            actual.Value.Should().NotBeNullOrWhiteSpace();
            actual.Next.Should().NotBeNull();
            actual.Next.Value.Should().NotBeNullOrWhiteSpace();
            actual.Next.End.Should().NotBeNull();
            actual.Next.End.Value.Should().NotBeNullOrWhiteSpace();
            actual.Next.End.Root.Should().BeSameAs(actual);
        }

        [Fact]
        public void CreatesPropertyOfSameTypeWithCreatedInstanceTest()
        {
            var configuration = Model.BuildStrategy;
            var buildLog = configuration.GetBuildLog();

            var target = new DefaultExecuteStrategy<ReadOnlyParent>();

            target.Initialize(configuration, buildLog);

            var actual = (Looper)target.Create(typeof(Looper));

            actual.Should().NotBeNull();
            actual.Stuff.Should().NotBeNullOrWhiteSpace();
            actual.Other.Should().BeSameAs(actual);
        }

        [Fact]
        public void CreateThrowsExceptionWhenNotInitializedTest()
        {
            var target = new DefaultExecuteStrategy();

            Action action = () => target.Create(typeof(Person));

            action.ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void CreateWithDoesNotBuildPropertiesWhenTypeCreatorDisablesAutoPopulateTest()
        {
            var model = new SlimModel();
            var typeCreators = new List<ITypeCreator>();
            var buildLog = new DefaultBuildLog();

            var typeCreator = Substitute.For<ITypeCreator>();
            var buildStrategy = Substitute.For<IBuildStrategy>();

            typeCreators.Add(typeCreator);

            buildStrategy.GetBuildLog().Returns(buildLog);
            buildStrategy.TypeCreators.Returns(typeCreators.AsReadOnly());

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildStrategy, buildStrategy.GetBuildLog());

            typeCreator.CanCreate(typeof(SlimModel), null, Arg.Any<LinkedList<object>>()).Returns(true);
            typeCreator.Create(typeof(SlimModel), null, Arg.Any<IExecuteStrategy>()).Returns(model);
            typeCreator.Priority.Returns(1);
            typeCreator.AutoPopulate.Returns(false);
            typeCreator.Populate(model, target).Returns(model);

            var actual = (SlimModel)target.CreateWith(typeof(SlimModel));

            actual.Should().BeSameAs(model);
            actual.Value.Should().BeEmpty();
        }

        [Fact]
        public void CreateWithReturnsNullWhenInstanceFailsToBeCreatedTest()
        {
            var typeCreators = new List<ITypeCreator>();

            var typeCreator = Substitute.For<ITypeCreator>();
            var buildStrategy = Substitute.For<IBuildStrategy>();

            typeCreators.Add(typeCreator);

            buildStrategy.TypeCreators.Returns(typeCreators.AsReadOnly());
            typeCreator.CanCreate(typeof(SlimModel), null, Arg.Any<LinkedList<object>>()).Returns(true);

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildStrategy, buildStrategy.GetBuildLog());

            var actual = target.CreateWith(typeof(SlimModel));

            actual.Should().BeNull();
        }

        [Fact]
        public void CreateWithReturnsNullWhenNoReferenceTypeCreatedTest()
        {
            var typeCreators = new List<ITypeCreator>();

            var typeCreator = Substitute.For<ITypeCreator>();
            var buildStrategy = Substitute.For<IBuildStrategy>();

            typeCreators.Add(typeCreator);

            buildStrategy.TypeCreators.Returns(typeCreators.AsReadOnly());
            typeCreator.CanCreate(typeof(Stream), null, Arg.Any<LinkedList<object>>()).Returns(true);
            typeCreator.Create(typeof(Stream), null, Arg.Any<IExecuteStrategy>()).Returns(null);

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildStrategy, buildStrategy.GetBuildLog());

            var actual = target.CreateWith(typeof(Stream));

            actual.Should().BeNull();
        }

        [Fact]
        public void CreateWithReturnsNullWhenNoValueTypeCreatedTest()
        {
            var valueGenerators = new List<IValueGenerator>();

            var valueGenerator = Substitute.For<IValueGenerator>();
            var buildStrategy = Substitute.For<IBuildStrategy>();

            valueGenerators.Add(valueGenerator);

            buildStrategy.ValueGenerators.Returns(valueGenerators.AsReadOnly());
            valueGenerator.IsSupported(typeof(int), null, Arg.Any<LinkedList<object>>()).Returns(true);
            valueGenerator.Generate(typeof(int), null, Arg.Any<IExecuteStrategy>()).Returns(null);

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildStrategy, buildStrategy.GetBuildLog());

            var actual = target.CreateWith(typeof(int));

            actual.Should().BeNull();
        }

        [Fact]
        public void CreateWithReturnsReferenceTypeFromCreatorTest()
        {
            var expected = new SlimModel();
            var value = Guid.NewGuid();

            var typeCreators = new List<ITypeCreator>();
            var valueGenerators = new List<IValueGenerator>();

            var typeCreator = Substitute.For<ITypeCreator>();
            var generator = Substitute.For<IValueGenerator>();
            var buildStrategy = Substitute.For<IBuildStrategy>();

            typeCreators.Add(typeCreator);
            valueGenerators.Add(generator);

            buildStrategy.TypeCreators.Returns(typeCreators.AsReadOnly());
            buildStrategy.ValueGenerators.Returns(valueGenerators.AsReadOnly());

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildStrategy, buildStrategy.GetBuildLog());

            typeCreator.CanCreate(typeof(SlimModel), null, Arg.Any<LinkedList<object>>()).Returns(true);
            typeCreator.Create(typeof(SlimModel), null, Arg.Any<IExecuteStrategy>()).Returns(expected);
            typeCreator.Populate(expected, target).Returns(expected);
            typeCreator.AutoPopulate.Returns(true);
            generator.IsSupported(typeof(Guid), "Value", Arg.Is<LinkedList<object>>(x => x.Last.Value == expected))
                .Returns(true);
            generator.Generate(
                typeof(Guid),
                "Value",
                Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last.Value == expected)).Returns(value);

            var actual = (SlimModel)target.CreateWith(typeof(SlimModel));

            actual.Should().Be(expected);
            actual.Value.Should().Be(value);
        }

        [Fact]
        public void CreateWithReturnsValueCreatedFromProvidedArgumentsTest()
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
            var typeCreators = new List<ITypeCreator>();

            var typeCreator = Substitute.For<ITypeCreator>();
            var buildStrategy = Substitute.For<IBuildStrategy>();

            typeCreators.Add(typeCreator);

            buildStrategy.TypeCreators.Returns(typeCreators.AsReadOnly());

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildStrategy, buildStrategy.GetBuildLog());

            typeCreator.CanCreate(typeof(Person), null, Arg.Any<LinkedList<object>>()).Returns(true);
            typeCreator.Create(typeof(Person), null, Arg.Any<IExecuteStrategy>(), args).Returns(expected);
            typeCreator.Populate(expected, target).Returns(expected);
            typeCreator.AutoPopulate.Returns(false);

            var actual = target.CreateWith(typeof(Person), args);

            actual.Should().BeSameAs(expected);
        }

        [Fact]
        public void CreateWithReturnsValueFromCreatorWithHighestPriorityTest()
        {
            var firstModel = new SlimModel();
            var secondModel = new SlimModel();
            var value = Guid.NewGuid();
            var typeCreators = new List<ITypeCreator>();
            var valueGenerators = new List<IValueGenerator>();

            var buildStrategy = Substitute.For<IBuildStrategy>();
            var firstCreator = Substitute.For<ITypeCreator>();
            var secondCreator = Substitute.For<ITypeCreator>();
            var generator = Substitute.For<IValueGenerator>();

            typeCreators.Add(firstCreator);
            typeCreators.Add(secondCreator);
            valueGenerators.Add(generator);

            buildStrategy.TypeCreators.Returns(typeCreators.AsReadOnly());
            buildStrategy.ValueGenerators.Returns(valueGenerators.AsReadOnly());

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildStrategy, buildStrategy.GetBuildLog());

            firstCreator.CanCreate(typeof(SlimModel), null, Arg.Any<LinkedList<object>>()).Returns(true);
            firstCreator.Create(typeof(SlimModel), null, Arg.Any<IExecuteStrategy>()).Returns(firstModel);
            firstCreator.Priority.Returns(1);
            firstCreator.AutoPopulate.Returns(true);
            firstCreator.Populate(firstModel, target).Returns(firstModel);
            secondCreator.CanCreate(typeof(SlimModel), null, Arg.Any<LinkedList<object>>()).Returns(true);
            secondCreator.Create(typeof(SlimModel), null, Arg.Any<IExecuteStrategy>()).Returns(secondModel);
            secondCreator.Priority.Returns(2);
            secondCreator.AutoPopulate.Returns(true);
            secondCreator.Populate(secondModel, target).Returns(secondModel);
            generator.IsSupported(typeof(Guid), "Value", Arg.Is<LinkedList<object>>(x => x.Last.Value == secondModel))
                .Returns(true);
            generator.Generate(
                typeof(Guid),
                "Value",
                Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last.Value == secondModel)).Returns(value);

            var actual = (SlimModel)target.CreateWith(typeof(SlimModel));

            actual.Should().BeSameAs(secondModel);
            actual.Value.Should().Be(value);
        }

        [Fact]
        public void CreateWithReturnsValueFromCreatorWithNoArgumentsAndDetectConstructorDisabledTest()
        {
            var expected = new Person();
            var typeCreators = new List<ITypeCreator>();

            var typeCreator = Substitute.For<ITypeCreator>();
            var buildStrategy = Substitute.For<IBuildStrategy>();

            typeCreators.Add(typeCreator);

            buildStrategy.TypeCreators.Returns(typeCreators.AsReadOnly());

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildStrategy, buildStrategy.GetBuildLog());

            typeCreator.CanCreate(typeof(Person), null, Arg.Any<LinkedList<object>>()).Returns(true);
            typeCreator.Create(typeof(Person), null, Arg.Any<IExecuteStrategy>()).Returns(expected);
            typeCreator.Populate(expected, target).Returns(expected);
            typeCreator.AutoPopulate.Returns(false);
            typeCreator.AutoDetectConstructor.Returns(false);

            var actual = target.CreateWith(typeof(Person));

            actual.Should().BeSameAs(expected);
        }

        [Fact]
        public void CreateWithReturnsValueFromCreatorWithNoArgumentsAndDetectConstructorEnabledTest()
        {
            var expected = new Person();
            var typeCreators = new List<ITypeCreator>();

            var typeCreator = Substitute.For<ITypeCreator>();
            var buildStrategy = Substitute.For<IBuildStrategy>();
            var resolver = Substitute.For<IConstructorResolver>();

            typeCreators.Add(typeCreator);

            buildStrategy.TypeCreators.Returns(typeCreators.AsReadOnly());
            buildStrategy.ConstructorResolver.Returns(resolver);

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildStrategy, buildStrategy.GetBuildLog());

            resolver.Resolve(typeof(Person))
                .Returns(typeof(Person).GetConstructors().Single(x => x.GetParameters().Length == 0));
            typeCreator.CanCreate(typeof(Person), null, Arg.Any<LinkedList<object>>()).Returns(true);
            typeCreator.Create(typeof(Person), null, Arg.Any<IExecuteStrategy>()).Returns(expected);
            typeCreator.Populate(expected, target).Returns(expected);
            typeCreator.AutoPopulate.Returns(false);
            typeCreator.AutoDetectConstructor.Returns(true);

            var actual = target.CreateWith(typeof(Person));

            actual.Should().BeSameAs(expected);
        }

        [Fact]
        public void CreateWithReturnsValueFromGeneratorWithHighestPriorityTest()
        {
            var firstValue = Guid.NewGuid();
            var secondValue = Guid.NewGuid();
            var valueGenerators = new List<IValueGenerator>();

            var buildStrategy = Substitute.For<IBuildStrategy>();
            var firstGenerator = Substitute.For<IValueGenerator>();
            var secondGenerator = Substitute.For<IValueGenerator>();

            valueGenerators.Add(firstGenerator);
            valueGenerators.Add(secondGenerator);

            buildStrategy.ValueGenerators.Returns(valueGenerators.AsReadOnly());
            firstGenerator.IsSupported(typeof(Guid), null, Arg.Any<LinkedList<object>>()).Returns(true);
            firstGenerator.Generate(typeof(Guid), null, Arg.Any<IExecuteStrategy>()).Returns(firstValue);
            firstGenerator.Priority.Returns(1);
            secondGenerator.IsSupported(typeof(Guid), null, Arg.Any<LinkedList<object>>()).Returns(true);
            secondGenerator.Generate(typeof(Guid), null, Arg.Any<IExecuteStrategy>()).Returns(secondValue);
            secondGenerator.Priority.Returns(2);

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildStrategy, buildStrategy.GetBuildLog());

            var actual = target.CreateWith(typeof(Guid));

            actual.Should().Be(secondValue);
        }

        [Fact]
        public void CreateWithReturnsValueFromResolvedConstructorAndBuiltParametersTest()
        {
            var value = Guid.NewGuid();
            var expected = new ReadOnlyModel(value);
            var valueGenerators = new List<IValueGenerator>();
            var typeCreators = new List<ITypeCreator>();

            var buildStrategy = Substitute.For<IBuildStrategy>();
            var resolver = Substitute.For<IConstructorResolver>();
            var generator = Substitute.For<IValueGenerator>();
            var typeCreator = Substitute.For<ITypeCreator>();

            typeCreators.Add(typeCreator);
            valueGenerators.Add(generator);

            resolver.Resolve(typeof(ReadOnlyModel), Arg.Any<object[]>())
                .Returns(typeof(ReadOnlyModel).GetConstructors()[0]);
            buildStrategy.ConstructorResolver.Returns(resolver);
            buildStrategy.TypeCreators.Returns(typeCreators.AsReadOnly());
            buildStrategy.ValueGenerators.Returns(valueGenerators.AsReadOnly());

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildStrategy, buildStrategy.GetBuildLog());

            typeCreator.CanCreate(typeof(ReadOnlyModel), null, Arg.Any<LinkedList<object>>()).Returns(true);
            typeCreator.Create(typeof(ReadOnlyModel), null, Arg.Any<IExecuteStrategy>(), value).Returns(expected);
            typeCreator.Populate(expected, target).Returns(expected);
            typeCreator.AutoPopulate.Returns(true);
            typeCreator.AutoDetectConstructor.Returns(true);
            generator.IsSupported(typeof(Guid), "value", Arg.Any<LinkedList<object>>()).Returns(true);
            generator.Generate(typeof(Guid), "value", Arg.Any<IExecuteStrategy>()).Returns(value);

            var actual = (ReadOnlyModel)target.CreateWith(typeof(ReadOnlyModel));

            actual.Should().Be(expected);
            actual.Value.Should().Be(value);
        }

        [Fact]
        public void CreateWithReturnsValueFromSupportedCreationRuleTest()
        {
            var firstValue = Guid.NewGuid().ToString();
            var secondValue = Guid.NewGuid();

            var buildStrategy =
                Model.DefaultBuildStrategy.Clone()
                    .Add(new CreationRule(typeof(Address), "Id", 100, firstValue))
                    .Add(new CreationRule(typeof(Person), "Id", 20, secondValue))
                    .Compile();

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildStrategy, buildStrategy.GetBuildLog());

            var actual = (Person)target.CreateWith(typeof(Person));

            actual.Id.Should().Be(secondValue);
        }

        [Fact]
        public void CreateWithReturnsValueFromSupportedCreationRuleWithHigherPriorityTest()
        {
            var firstValue = Guid.NewGuid();
            var secondValue = Guid.NewGuid();

            var buildStrategy =
                Model.DefaultBuildStrategy.Clone()
                    .Add(new CreationRule(typeof(Person), "Id", 10, firstValue))
                    .Add(new CreationRule(typeof(Person), "Id", 20, secondValue))
                    .Compile();

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildStrategy, buildStrategy.GetBuildLog());

            var actual = (Person)target.CreateWith(typeof(Person));

            actual.Id.Should().Be(secondValue);
        }

        [Fact]
        public void CreateWithReturnsValueFromSupportedCreatorTest()
        {
            var firstModel = new SlimModel();
            var secondModel = new SlimModel();
            var value = Guid.NewGuid();
            var valueGenerators = new List<IValueGenerator>();
            var typeCreators = new List<ITypeCreator>();

            var buildStrategy = Substitute.For<IBuildStrategy>();
            var firstCreator = Substitute.For<ITypeCreator>();
            var secondCreator = Substitute.For<ITypeCreator>();
            var generator = Substitute.For<IValueGenerator>();

            typeCreators.Add(firstCreator);
            typeCreators.Add(secondCreator);
            valueGenerators.Add(generator);

            buildStrategy.TypeCreators.Returns(typeCreators.AsReadOnly());
            buildStrategy.ValueGenerators.Returns(valueGenerators.AsReadOnly());

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildStrategy, buildStrategy.GetBuildLog());

            firstCreator.CanCreate(typeof(SlimModel), null, Arg.Any<LinkedList<object>>()).Returns(false);
            firstCreator.Create(typeof(SlimModel), null, Arg.Any<IExecuteStrategy>()).Returns(firstModel);
            firstCreator.Priority.Returns(10);
            firstCreator.AutoPopulate.Returns(true);
            firstCreator.Populate(firstModel, target).Returns(firstModel);
            secondCreator.CanCreate(typeof(SlimModel), null, Arg.Any<LinkedList<object>>()).Returns(true);
            secondCreator.Create(typeof(SlimModel), null, Arg.Any<IExecuteStrategy>()).Returns(secondModel);
            secondCreator.Priority.Returns(2);
            secondCreator.AutoPopulate.Returns(true);
            secondCreator.Populate(secondModel, target).Returns(secondModel);
            generator.IsSupported(typeof(Guid), "Value", Arg.Is<LinkedList<object>>(x => x.Last.Value == secondModel))
                .Returns(true);
            generator.Generate(
                typeof(Guid),
                "Value",
                Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last.Value == secondModel)).Returns(value);

            var actual = (SlimModel)target.CreateWith(typeof(SlimModel));

            actual.Should().BeSameAs(secondModel);
            actual.Value.Should().Be(value);
        }

        [Fact]
        public void CreateWithReturnsValueFromSupportedGeneratorTest()
        {
            var firstValue = Guid.NewGuid();
            var secondValue = Guid.NewGuid();
            var valueGenerators = new List<IValueGenerator>();

            var buildStrategy = Substitute.For<IBuildStrategy>();
            var firstGenerator = Substitute.For<IValueGenerator>();
            var secondGenerator = Substitute.For<IValueGenerator>();

            valueGenerators.Add(firstGenerator);
            valueGenerators.Add(secondGenerator);

            buildStrategy.ValueGenerators.Returns(valueGenerators.AsReadOnly());
            firstGenerator.IsSupported(typeof(Guid), null, Arg.Any<LinkedList<object>>()).Returns(false);
            firstGenerator.Generate(typeof(Guid), null, Arg.Any<IExecuteStrategy>()).Returns(firstValue);
            firstGenerator.Priority.Returns(10);
            secondGenerator.IsSupported(typeof(Guid), null, Arg.Any<LinkedList<object>>()).Returns(true);
            secondGenerator.Generate(typeof(Guid), null, Arg.Any<IExecuteStrategy>()).Returns(secondValue);
            secondGenerator.Priority.Returns(2);

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildStrategy, buildStrategy.GetBuildLog());

            var actual = target.CreateWith(typeof(Guid));

            actual.Should().Be(secondValue);
        }

        [Fact]
        public void CreateWithReturnsValueTypeFromGeneratorTest()
        {
            var expected = Guid.NewGuid().ToString();
            var valueGenerators = new List<IValueGenerator>();

            var buildStrategy = Substitute.For<IBuildStrategy>();
            var valueGenerator = Substitute.For<IValueGenerator>();

            valueGenerators.Add(valueGenerator);

            buildStrategy.ValueGenerators.Returns(valueGenerators.AsReadOnly());
            valueGenerator.IsSupported(typeof(string), null, Arg.Any<LinkedList<object>>()).Returns(true);
            valueGenerator.Generate(typeof(string), null, Arg.Any<IExecuteStrategy>()).Returns(expected);

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildStrategy, buildStrategy.GetBuildLog());

            var actual = target.CreateWith(typeof(string));

            actual.Should().Be(expected);
        }

        [Fact]
        public void CreateWithThrowsExceptionWhenConstructorParameterCannotBeCreatedTest()
        {
            var typeCreator = new DefaultTypeCreator();
            var typeCreators = new List<ITypeCreator>();

            var buildStrategy = Substitute.For<IBuildStrategy>();
            var resolver = Substitute.For<IConstructorResolver>();

            typeCreators.Add(typeCreator);

            resolver.Resolve(typeof(ReadOnlyModel), Arg.Any<object[]>())
                .Returns(typeof(ReadOnlyModel).GetConstructors()[0]);
            buildStrategy.ConstructorResolver.Returns(resolver);
            buildStrategy.TypeCreators.Returns(typeCreators.AsReadOnly());

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildStrategy, buildStrategy.GetBuildLog());

            Action action = () => target.CreateWith(typeof(ReadOnlyModel));

            action.ShouldThrow<BuildException>();
        }

        [Fact]
        public void CreateWithThrowsExceptionWhenCreatingTypeFailsTest()
        {
            var typeCreator = Substitute.For<ITypeCreator>();

            typeCreator.CanCreate(typeof(Address), "Address", Arg.Any<LinkedList<object>>()).Returns(true);
            typeCreator.Priority.Returns(int.MaxValue);
            typeCreator.AutoDetectConstructor.Returns(true);
            typeCreator.AutoPopulate.Returns(true);
            typeCreator.Create(typeof(Address), "Address", Arg.Any<IExecuteStrategy>())
                .Throws(new InvalidOperationException());

            var buildStrategy = new DefaultBuildStrategyCompiler().Add(typeCreator).Compile();

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildStrategy, buildStrategy.GetBuildLog());

            Action action = () => target.CreateWith(typeof(Person));

            var exception =
                action.ShouldThrow<BuildException>().Where(x => x.Message != null).Where(x => x.BuildLog != null).Which;

            _output.WriteLine(exception.Message);
        }

        [Fact]
        public void CreateWithThrowsExceptionWhenDerivedImplementationSuppliesNullTypeTest()
        {
            var buildLog = new DefaultBuildLog();

            var buildStrategy = Substitute.For<IBuildStrategy>();

            buildStrategy.GetBuildLog().Returns(buildLog);

            var target = new NullTypeBuildExecuteStrategy();

            target.Initialize(buildStrategy, buildStrategy.GetBuildLog());

            Action action = () => target.CreateWith(typeof(int));

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void CreateWithThrowsExceptionWhenGeneratingValueFailsTest()
        {
            var person = new Person();
            var generators = new List<IValueGenerator>();
            var creators = new List<ITypeCreator>();

            var valueGenerator = Substitute.For<IValueGenerator>();
            var typeCreator = Substitute.For<ITypeCreator>();
            var buildStrategy = Substitute.For<IBuildStrategy>();

            generators.Add(valueGenerator);
            creators.Add(typeCreator);

            typeCreator.CanCreate(typeof(Person), null, Arg.Any<LinkedList<object>>()).Returns(true);
            typeCreator.Create(typeof(Person), null, null, null).Returns(person);
            valueGenerator.IsSupported(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<LinkedList<object>>()).Returns(true);
            valueGenerator.Generate(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<IExecuteStrategy>())
                .Throws(new InvalidOperationException());
            buildStrategy.TypeCreators.Returns(creators.AsReadOnly());
            buildStrategy.ValueGenerators.Returns(generators.AsReadOnly());

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildStrategy, buildStrategy.GetBuildLog());

            Action action = () => target.CreateWith(typeof(Person));

            var exception =
                action.ShouldThrow<BuildException>().Where(x => x.Message != null).Where(x => x.BuildLog != null).Which;

            _output.WriteLine(exception.Message);
        }

        [Fact]
        public void CreateWithThrowsExceptionWhenGeneratingValueThrowsBuildExceptionTest()
        {
            var person = new Person();
            var generators = new List<IValueGenerator>();
            var creators = new List<ITypeCreator>();

            var valueGenerator = Substitute.For<IValueGenerator>();
            var typeCreator = Substitute.For<ITypeCreator>();
            var buildStrategy = Substitute.For<IBuildStrategy>();

            generators.Add(valueGenerator);
            creators.Add(typeCreator);

            typeCreator.CanCreate(typeof(Person), null, Arg.Any<LinkedList<object>>()).Returns(true);
            typeCreator.Create(typeof(Person), null, Arg.Any<IExecuteStrategy>()).Returns(person);
            typeCreator.AutoPopulate.Returns(true);
            valueGenerator.IsSupported(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<LinkedList<object>>()).Returns(true);
            valueGenerator.Generate(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<IExecuteStrategy>())
                .Throws(new BuildException());
            buildStrategy.TypeCreators.Returns(creators.AsReadOnly());
            buildStrategy.ValueGenerators.Returns(generators.AsReadOnly());

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildStrategy, buildStrategy.GetBuildLog());

            Action action = () => target.CreateWith(typeof(Person));

            action.ShouldThrow<BuildException>();
        }

        [Fact]
        public void CreateWithThrowsExceptionWhenNoGeneratorOrCreatorMatchFoundForChildPropertyTest()
        {
            var person = new Person();
            var generators = new List<IValueGenerator>();
            var creators = new List<ITypeCreator>();

            var valueGenerator = Substitute.For<IValueGenerator>();
            var typeCreator = Substitute.For<ITypeCreator>();
            var buildStrategy = Substitute.For<IBuildStrategy>();

            generators.Add(valueGenerator);
            creators.Add(typeCreator);

            typeCreator.CanCreate(typeof(Person), null, Arg.Any<LinkedList<object>>()).Returns(true);
            typeCreator.Create(typeof(Person), null, Arg.Any<IExecuteStrategy>()).Returns(person);
            typeCreator.AutoPopulate.Returns(true);
            valueGenerator.IsSupported(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<LinkedList<object>>()).Returns(false);
            buildStrategy.TypeCreators.Returns(creators.AsReadOnly());
            buildStrategy.ValueGenerators.Returns(generators.AsReadOnly());

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildStrategy, buildStrategy.GetBuildLog());

            Action action = () => target.CreateWith(typeof(Person));

            action.ShouldThrow<BuildException>();
        }

        [Fact]
        public void CreateWithThrowsExceptionWhenNoGeneratorOrCreatorMatchFoundForConstructorParameterTest()
        {
            var generators = new List<IValueGenerator>();
            var creators = new List<ITypeCreator>();
            var constructorResolver = new DefaultConstructorResolver();

            var valueGenerator = Substitute.For<IValueGenerator>();
            var typeCreator = Substitute.For<ITypeCreator>();
            var buildStrategy = Substitute.For<IBuildStrategy>();

            generators.Add(valueGenerator);
            creators.Add(typeCreator);

            typeCreator.CanCreate(typeof(KeyValuePair<string, Person>), null, Arg.Any<LinkedList<object>>())
                .Returns(true);
            typeCreator.AutoDetectConstructor.Returns(true);
            typeCreator.AutoPopulate.Returns(true);
            valueGenerator.IsSupported(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<LinkedList<object>>()).Returns(false);
            buildStrategy.TypeCreators.Returns(creators.AsReadOnly());
            buildStrategy.ValueGenerators.Returns(generators.AsReadOnly());
            buildStrategy.ConstructorResolver.Returns(constructorResolver);

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildStrategy, buildStrategy.GetBuildLog());

            Action action = () => target.CreateWith(typeof(KeyValuePair<string, Person>));

            action.ShouldThrow<BuildException>();
        }

        [Fact]
        public void CreateWithThrowsExceptionWhenNoGeneratorOrCreatorMatchFoundTest()
        {
            var buildStrategy = Substitute.For<IBuildStrategy>();

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildStrategy, buildStrategy.GetBuildLog());

            Action action = () => target.CreateWith(typeof(string));

            action.ShouldThrow<BuildException>();
        }

        [Fact]
        public void CreateWithThrowsExceptionWhenNotInitializedTest()
        {
            var target = new DefaultExecuteStrategy();

            Action action = () => target.CreateWith(typeof(Person));

            action.ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void CreateWithThrowsExceptionWhenPropertyCannotBeCreatedTest()
        {
            var typeCreator = new DefaultTypeCreator();
            var typeCreators = new List<ITypeCreator>();

            var buildStrategy = Substitute.For<IBuildStrategy>();
            var resolver = Substitute.For<IConstructorResolver>();

            typeCreators.Add(typeCreator);

            resolver.Resolve(typeof(SlimModel), Arg.Any<object[]>()).Returns(typeof(SlimModel).GetConstructors()[0]);
            buildStrategy.ConstructorResolver.Returns(resolver);
            buildStrategy.TypeCreators.Returns(typeCreators.AsReadOnly());

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildStrategy, buildStrategy.GetBuildLog());

            Action action = () => target.CreateWith(typeof(SlimModel));

            action.ShouldThrow<BuildException>();
        }

        [Fact]
        public void CreateWithThrowsExceptionWithNullTypeTest()
        {
            var buildStrategy = Substitute.For<IBuildStrategy>();

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildStrategy, buildStrategy.GetBuildLog());

            Action action = () => target.CreateWith(null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void InitializeStoresConfigurationAndBuildLogTest()
        {
            var configuration = Substitute.For<IBuildConfiguration>();
            var buildLog = Substitute.For<IBuildLog>();

            var target = new DefaultExecuteStrategy();

            target.Initialize(configuration, buildLog);

            target.Configuration.Should().BeSameAs(configuration);
            target.Log.Should().BeSameAs(buildLog);
        }

        [Fact]
        public void InitializeThrowsExceptionWithNullBuildConfigurationTest()
        {
            var buildLog = Substitute.For<IBuildLog>();

            var target = new DefaultExecuteStrategy();

            Action action = () => target.Initialize(null, buildLog);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void InitializeThrowsExceptionWithNullBuildLogTest()
        {
            var configuration = Substitute.For<IBuildConfiguration>();

            var target = new DefaultExecuteStrategy();

            Action action = () => target.Initialize(configuration, null);

            action.ShouldThrow<ArgumentNullException>();
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
            target.Log.Should().BeNull();
            target.BuildChain.Should().NotBeNull();
        }

        [Fact]
        public void PopulateAsObjectAssignsPropertyValuesToExistingInstanceTest()
        {
            var staff = new List<Person>();
            var name = Guid.NewGuid().ToString();
            var address = Guid.NewGuid().ToString();
            var expected = new Company();
            var valueGenerators = new List<IValueGenerator>();
            var typeCreators = new List<ITypeCreator>();

            var buildStrategy = Substitute.For<IBuildStrategy>();
            var typeCreator = Substitute.For<ITypeCreator>();
            var valueGenerator = Substitute.For<IValueGenerator>();

            typeCreators.Add(typeCreator);
            valueGenerators.Add(valueGenerator);

            buildStrategy.TypeCreators.Returns(typeCreators.AsReadOnly());
            buildStrategy.ValueGenerators.Returns(valueGenerators.AsReadOnly());

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildStrategy, buildStrategy.GetBuildLog());

            typeCreator.CanCreate(
                typeof(IEnumerable<Person>),
                "Staff",
                Arg.Is<LinkedList<object>>(x => x.Last.Value == expected)).Returns(true);
            typeCreator.Create(
                typeof(IEnumerable<Person>),
                "Staff",
                Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last.Value == expected)).Returns(staff);
            typeCreator.Populate(staff, target).Returns(staff);
            valueGenerator.IsSupported(
                typeof(string),
                "Name",
                Arg.Is<LinkedList<object>>(x => x.Last.Value == expected)).Returns(true);
            valueGenerator.Generate(
                typeof(string),
                "Name",
                Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last.Value == expected)).Returns(name);
            valueGenerator.IsSupported(
                typeof(string),
                "Address",
                Arg.Is<LinkedList<object>>(x => x.Last.Value == expected)).Returns(true);
            valueGenerator.Generate(
                typeof(string),
                "Address",
                Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last.Value == expected)).Returns(address);

            var actual = (Company)target.Populate(expected);

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
            var valueGenerators = new List<IValueGenerator>();
            var typeCreators = new List<ITypeCreator>();

            var buildStrategy = Substitute.For<IBuildStrategy>();
            var typeCreator = Substitute.For<ITypeCreator>();
            var valueGenerator = Substitute.For<IValueGenerator>();

            typeCreators.Add(typeCreator);
            valueGenerators.Add(valueGenerator);

            buildStrategy.TypeCreators.Returns(typeCreators.AsReadOnly());
            buildStrategy.ValueGenerators.Returns(valueGenerators.AsReadOnly());

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildStrategy, buildStrategy.GetBuildLog());

            typeCreator.CanCreate(
                typeof(IEnumerable<Person>),
                "Staff",
                Arg.Is<LinkedList<object>>(x => x.Last.Value == expected)).Returns(true);
            typeCreator.Create(
                typeof(IEnumerable<Person>),
                "Staff",
                Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last.Value == expected)).Returns(staff);
            typeCreator.Populate(staff, target).Returns(staff);
            valueGenerator.IsSupported(
                typeof(string),
                "Name",
                Arg.Is<LinkedList<object>>(x => x.Last.Value == expected)).Returns(true);
            valueGenerator.Generate(
                typeof(string),
                "Name",
                Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last.Value == expected)).Returns(name);
            valueGenerator.IsSupported(
                typeof(string),
                "Address",
                Arg.Is<LinkedList<object>>(x => x.Last.Value == expected)).Returns(true);
            valueGenerator.Generate(
                typeof(string),
                "Address",
                Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last.Value == expected)).Returns(address);

            var actual = (Company)target.Populate(expected);

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
            var valueGenerators = new List<IValueGenerator>();
            var typeCreators = new List<ITypeCreator>();
            var executeOrderRules = Model.BuildStrategy.ExecuteOrderRules;

            var buildStrategy = Substitute.For<IBuildStrategy>();
            var typeCreator = Substitute.For<ITypeCreator>();
            var valueGenerator = Substitute.For<IValueGenerator>();

            typeCreators.Add(typeCreator);
            valueGenerators.Add(valueGenerator);

            buildStrategy.TypeCreators.Returns(typeCreators.AsReadOnly());
            buildStrategy.ValueGenerators.Returns(valueGenerators.AsReadOnly());
            buildStrategy.ExecuteOrderRules.Returns(executeOrderRules);

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildStrategy, buildStrategy.GetBuildLog());

            valueGenerator.IsSupported(
                typeof(SimpleEnum),
                "Z",
                Arg.Is<LinkedList<object>>(x => x.Last.Value == expected)).Returns(true);
            valueGenerator.Generate(
                typeof(SimpleEnum),
                "Z",
                Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last.Value == expected)).Returns(first);
            valueGenerator.IsSupported(typeof(int), "Y", Arg.Is<LinkedList<object>>(x => x.Last.Value == expected))
                .Returns(true);
            valueGenerator.Generate(
                typeof(int),
                "Y",
                Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last.Value == expected)).Returns(second);
            valueGenerator.IsSupported(typeof(string), "X", Arg.Is<LinkedList<object>>(x => x.Last.Value == expected))
                .Returns(true);
            valueGenerator.Generate(
                typeof(string),
                "X",
                Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last.Value == expected)).Returns(third);
            typeCreator.CanCreate(typeof(Person), "W", Arg.Is<LinkedList<object>>(x => x.Last.Value == expected))
                .Returns(true);
            typeCreator.Create(typeof(Person), "W", Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last.Value == expected))
                .Returns(fourth);
            typeCreator.Populate(fourth, target).Returns(fourth);

            var actual = (PopulateOrderItem)target.Populate(expected);

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
            var valueGenerators = new List<IValueGenerator>();
            var typeCreators = new List<ITypeCreator>();
            var ignoreRules = new List<IgnoreRule>
            {
                ignoreRule
            };

            var buildStrategy = Substitute.For<IBuildStrategy>();
            var typeCreator = Substitute.For<ITypeCreator>();
            var valueGenerator = Substitute.For<IValueGenerator>();

            typeCreators.Add(typeCreator);
            valueGenerators.Add(valueGenerator);

            buildStrategy.TypeCreators.Returns(typeCreators.AsReadOnly());
            buildStrategy.ValueGenerators.Returns(valueGenerators.AsReadOnly());
            buildStrategy.IgnoreRules.Returns(ignoreRules.AsReadOnly());

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildStrategy, buildStrategy.GetBuildLog());

            typeCreator.CanCreate(
                typeof(IEnumerable<Person>),
                "Staff",
                Arg.Is<LinkedList<object>>(x => x.Last.Value == expected)).Returns(true);
            typeCreator.Create(
                typeof(IEnumerable<Person>),
                "Staff",
                Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last.Value == expected)).Returns(staff);
            typeCreator.Populate(staff, target).Returns(staff);
            valueGenerator.IsSupported(
                typeof(string),
                "Name",
                Arg.Is<LinkedList<object>>(x => x.Last.Value == expected)).Returns(true);
            valueGenerator.Generate(
                typeof(string),
                "Name",
                Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last.Value == expected)).Returns(name);
            valueGenerator.IsSupported(
                typeof(string),
                "Address",
                Arg.Is<LinkedList<object>>(x => x.Last.Value == expected)).Returns(true);
            valueGenerator.Generate(
                typeof(string),
                "Address",
                Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last.Value == expected)).Returns(address);

            var actual = (Company)target.Populate(expected);

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
            var valueGenerators = new List<IValueGenerator>();
            var typeCreators = new List<ITypeCreator>();
            var ignoreRules = new List<IgnoreRule>
            {
                ignoreRule
            };

            var buildStrategy = Substitute.For<IBuildStrategy>();
            var typeCreator = Substitute.For<ITypeCreator>();
            var valueGenerator = Substitute.For<IValueGenerator>();

            typeCreators.Add(typeCreator);
            valueGenerators.Add(valueGenerator);

            buildStrategy.TypeCreators.Returns(typeCreators.AsReadOnly());
            buildStrategy.ValueGenerators.Returns(valueGenerators.AsReadOnly());
            buildStrategy.IgnoreRules.Returns(ignoreRules.AsReadOnly());

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildStrategy, buildStrategy.GetBuildLog());

            typeCreator.CanCreate(
                typeof(IEnumerable<Person>),
                "Staff",
                Arg.Is<LinkedList<object>>(x => x.Last.Value == expected)).Returns(true);
            typeCreator.Create(
                typeof(IEnumerable<Person>),
                "Staff",
                Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last.Value == expected)).Returns(staff);
            typeCreator.Populate(staff, target).Returns(staff);
            valueGenerator.IsSupported(
                typeof(string),
                "Name",
                Arg.Is<LinkedList<object>>(x => x.Last.Value == expected)).Returns(true);
            valueGenerator.Generate(
                typeof(string),
                "Name",
                Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last.Value == expected)).Returns(name);
            valueGenerator.IsSupported(
                typeof(string),
                "Address",
                Arg.Is<LinkedList<object>>(x => x.Last.Value == expected)).Returns(true);
            valueGenerator.Generate(
                typeof(string),
                "Address",
                Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last.Value == expected)).Returns(address);

            var actual = (Company)target.Populate(expected);

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
            var valueGenerators = new List<IValueGenerator>();
            var typeCreators = new List<ITypeCreator>();
            var ignoreRules = new List<IgnoreRule>
            {
                ignoreRule
            };

            var buildStrategy = Substitute.For<IBuildStrategy>();
            var typeCreator = Substitute.For<ITypeCreator>();
            var valueGenerator = Substitute.For<IValueGenerator>();

            typeCreators.Add(typeCreator);
            valueGenerators.Add(valueGenerator);

            buildStrategy.TypeCreators.Returns(typeCreators.AsReadOnly());
            buildStrategy.ValueGenerators.Returns(valueGenerators.AsReadOnly());
            buildStrategy.IgnoreRules.Returns(ignoreRules.AsReadOnly());

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildStrategy, buildStrategy.GetBuildLog());

            typeCreator.CanCreate(
                typeof(IEnumerable<Person>),
                "Staff",
                Arg.Is<LinkedList<object>>(x => x.Last.Value == expected)).Returns(true);
            typeCreator.Create(
                typeof(IEnumerable<Person>),
                "Staff",
                Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last.Value == expected)).Returns(staff);
            typeCreator.Populate(staff, target).Returns(staff);
            valueGenerator.IsSupported(
                typeof(string),
                "Name",
                Arg.Is<LinkedList<object>>(x => x.Last.Value == expected)).Returns(true);
            valueGenerator.Generate(
                typeof(string),
                "Name",
                Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last.Value == expected)).Returns(name);
            valueGenerator.IsSupported(
                typeof(string),
                "Address",
                Arg.Is<LinkedList<object>>(x => x.Last.Value == expected)).Returns(true);
            valueGenerator.Generate(
                typeof(string),
                "Address",
                Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last.Value == expected)).Returns(address);

            var actual = (Company)target.Populate(expected);

            actual.Should().BeSameAs(expected);
            actual.Name.Should().Be(name);
            actual.Address.Should().Be(address);
            actual.Staff.Should().BeSameAs(staff);
        }

        [Fact]
        public void PopulateInstanceThrowsExceptionWhenNotInitializedTest()
        {
            var value = new Person();

            var target = new PopulateInstanceWrapper();

            Action action = () => target.RunTest(value);

            action.ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void PopulateInstanceThrowsExceptionWithNullInstanceTest()
        {
            var target = new PopulateInstanceWrapper();

            Action action = () => target.RunTest();

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void PopulateOnlySetsPublicPropertiesTest()
        {
            var expected = new PropertyScopes();
            var value = Guid.NewGuid();
            var valueGenerators = new List<IValueGenerator>();

            var buildStrategy = Substitute.For<IBuildStrategy>();
            var valueGenerator = Substitute.For<IValueGenerator>();

            valueGenerators.Add(valueGenerator);

            buildStrategy.ValueGenerators.Returns(valueGenerators.AsReadOnly());

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildStrategy, buildStrategy.GetBuildLog());

            valueGenerator.IsSupported(typeof(Guid), Arg.Any<string>(), Arg.Any<LinkedList<object>>()).Returns(true);
            valueGenerator.Generate(typeof(Guid), Arg.Any<string>(), Arg.Any<IExecuteStrategy>()).Returns(value);

            var actual = (PropertyScopes)target.Populate(expected);

            actual.Should().BeSameAs(expected);
            actual.Public.Should().Be(value);
            actual.InternalSet.Should().BeEmpty();
            actual.ReadOnly.Should().BeEmpty();
            PropertyScopes.GlobalValue.Should().BeEmpty();
        }

        [Fact]
        public void PopulatePopulatesReadOnlyReferencePropertiesTest()
        {
            var configuration = Model.BuildStrategy;
            var buildLog = configuration.GetBuildLog();

            var target = new DefaultExecuteStrategy<ReadOnlyParent>();

            target.Initialize(configuration, buildLog);

            var actual = new ReadOnlyParent();

            actual = target.Populate(actual);

            actual.Company.Address.Should().NotBeNullOrWhiteSpace();
            actual.AssignablePeople.Should().NotBeEmpty();
            actual.People.Should().NotBeEmpty();
            actual.RestrictedPeople.Should().BeEmpty();
            actual.Unassigned.Should().BeNull();
        }

        [Fact]
        public void PopulatePushesInstanceIntoBuildChainWhileCreatingTest()
        {
            var instance = new SlimModel();
            var testPassed = false;

            var creator = Substitute.For<ITypeCreator>();
            var generator = Substitute.For<IValueGenerator>();
            var buildStrategy = Substitute.For<IBuildStrategy>();

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildStrategy, buildStrategy.GetBuildLog());

            buildStrategy.CreationRules.Returns(new List<CreationRule>().AsReadOnly());
            buildStrategy.ValueGenerators.Returns(
                new List<IValueGenerator>
                {
                    generator
                }.AsReadOnly());
            buildStrategy.TypeCreators.Returns(
                new List<ITypeCreator>
                {
                    creator
                }.AsReadOnly());
            creator.CanCreate(typeof(SlimModel), null, Arg.Any<LinkedList<object>>()).Returns(true);
            creator.Create(typeof(SlimModel), null, Arg.Any<IExecuteStrategy>()).Returns(instance);
            creator.AutoPopulate.Returns(true);
            generator.When(
                x =>
                    x.Generate(
                        typeof(Guid),
                        nameof(SlimModel.Value),
                        Arg.Is<IExecuteStrategy>(y => y.BuildChain.Last.Value == instance))).Do(
                x =>
                {
                    target.BuildChain.Should().HaveCount(1);
                    target.BuildChain.First().Should().BeOfType<SlimModel>();
                    testPassed = true;
                });

            generator.IsSupported(
                typeof(Guid),
                nameof(SlimModel.Value),
                Arg.Is<LinkedList<object>>(x => x.Last.Value == instance)).Returns(true);

            target.Create(typeof(SlimModel));

            testPassed.Should().BeTrue();
        }

        [Fact]
        public void PopulatePushesInstanceIntoBuildChainWhilePopulatingTest()
        {
            var instance = new SlimModel();
            var testPassed = false;

            var generator = Substitute.For<IValueGenerator>();
            var buildStrategy = Substitute.For<IBuildStrategy>();

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildStrategy, buildStrategy.GetBuildLog());

            buildStrategy.CreationRules.Returns(new List<CreationRule>().AsReadOnly());
            buildStrategy.ValueGenerators.Returns(
                new List<IValueGenerator>
                {
                    generator
                }.AsReadOnly());
            generator.When(
                x =>
                    x.Generate(
                        typeof(Guid),
                        nameof(SlimModel.Value),
                        Arg.Is<IExecuteStrategy>(y => y.BuildChain.Last.Value == instance))).Do(
                x =>
                {
                    target.BuildChain.Should().HaveCount(1);
                    target.BuildChain.Should().Contain(instance);
                    testPassed = true;
                });

            generator.IsSupported(
                typeof(Guid),
                nameof(SlimModel.Value),
                Arg.Is<LinkedList<object>>(x => x.Last.Value == instance)).Returns(true);

            target.Populate(instance);

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
            var buildStrategy = Substitute.For<IBuildStrategy>();

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildStrategy, buildStrategy.GetBuildLog());

            buildStrategy.CreationRules.Returns(new List<CreationRule>().AsReadOnly());
            buildStrategy.ValueGenerators.Returns(
                new List<IValueGenerator>
                {
                    generator
                }.AsReadOnly());
            buildStrategy.TypeCreators.Returns(
                new List<ITypeCreator>
                {
                    creator
                }.AsReadOnly());
            creator.CanCreate(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<LinkedList<object>>()).Returns(true);
            creator.Create(typeof(Office), null, Arg.Any<IExecuteStrategy>()).Returns(office);
            creator.Create(typeof(Address), "Address", Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last.Value == office))
                .Returns(address);
            creator.AutoPopulate.Returns(true);
            generator.When(
                x =>
                    x.Generate(
                        typeof(string),
                        Arg.Any<string>(),
                        Arg.Is<IExecuteStrategy>(y => y.BuildChain.Last.Value == address))).Do(
                x =>
                {
                    target.BuildChain.Should().HaveCount(2);
                    target.BuildChain.First.Value.Should().Be(office);
                    target.BuildChain.Last.Value.Should().Be(address);
                    testPassed = true;
                });

            generator.IsSupported(typeof(string), Arg.Any<string>(), Arg.Any<LinkedList<object>>()).Returns(true);
            generator.Generate(typeof(string), Arg.Any<string>(), Arg.Any<IExecuteStrategy>())
                .Returns(Guid.NewGuid().ToString());

            target.Create(typeof(Office));

            testPassed.Should().BeTrue();
        }

        [Fact]
        public void PopulateSkipsPropertiesMarkedWithIgnoreRuleTest()
        {
            var staff = new List<Person>();
            var name = Guid.NewGuid().ToString();
            var address = Guid.NewGuid().ToString();
            var expected = new Company();
            var ignoreRule = new IgnoreRule(typeof(Company), "Name");
            var valueGenerators = new List<IValueGenerator>();
            var typeCreators = new List<ITypeCreator>();
            var ignoreRules = new List<IgnoreRule>
            {
                ignoreRule
            };

            var buildStrategy = Substitute.For<IBuildStrategy>();
            var typeCreator = Substitute.For<ITypeCreator>();
            var valueGenerator = Substitute.For<IValueGenerator>();

            typeCreators.Add(typeCreator);
            valueGenerators.Add(valueGenerator);

            buildStrategy.TypeCreators.Returns(typeCreators.AsReadOnly());
            buildStrategy.ValueGenerators.Returns(valueGenerators.AsReadOnly());
            buildStrategy.IgnoreRules.Returns(ignoreRules.AsReadOnly());

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildStrategy, buildStrategy.GetBuildLog());

            typeCreator.CanCreate(
                typeof(IEnumerable<Person>),
                "Staff",
                Arg.Is<LinkedList<object>>(x => x.Last.Value == expected)).Returns(true);
            typeCreator.Create(
                typeof(IEnumerable<Person>),
                "Staff",
                Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last.Value == expected)).Returns(staff);
            typeCreator.Populate(staff, target).Returns(staff);
            valueGenerator.IsSupported(
                typeof(string),
                "Name",
                Arg.Is<LinkedList<object>>(x => x.Last.Value == expected)).Returns(true);
            valueGenerator.Generate(
                typeof(string),
                "Name",
                Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last.Value == expected)).Returns(name);
            valueGenerator.IsSupported(
                typeof(string),
                "Address",
                Arg.Is<LinkedList<object>>(x => x.Last.Value == expected)).Returns(true);
            valueGenerator.Generate(
                typeof(string),
                "Address",
                Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last.Value == expected)).Returns(address);

            var actual = (Company)target.Populate(expected);

            actual.Should().BeSameAs(expected);
            actual.Name.Should().BeNullOrEmpty();
            actual.Address.Should().Be(address);
            actual.Staff.Should().BeSameAs(staff);
        }

        [Fact]
        public void PopulateSkipsPropertiesMarkedWithIgnoreRuleWithTypeMatchesBaseClassTest()
        {
            var staff = new List<Person>();
            var name = Guid.NewGuid().ToString();
            var expected = new SpecificCompany();
            var ignoreRule = new IgnoreRule(typeof(Company), "Name");
            var valueGenerators = new List<IValueGenerator>();
            var typeCreators = new List<ITypeCreator>();
            var ignoreRules = new List<IgnoreRule>
            {
                ignoreRule
            };

            var buildStrategy = Substitute.For<IBuildStrategy>();
            var typeCreator = Substitute.For<ITypeCreator>();
            var valueGenerator = Substitute.For<IValueGenerator>();

            typeCreators.Add(typeCreator);
            valueGenerators.Add(valueGenerator);

            buildStrategy.TypeCreators.Returns(typeCreators.AsReadOnly());
            buildStrategy.ValueGenerators.Returns(valueGenerators.AsReadOnly());
            buildStrategy.IgnoreRules.Returns(ignoreRules.AsReadOnly());

            var target = new DefaultExecuteStrategy();

            target.Initialize(buildStrategy, buildStrategy.GetBuildLog());

            typeCreator.CanCreate(
                typeof(IEnumerable<Person>),
                "Staff",
                Arg.Is<LinkedList<object>>(x => x.Last.Value == expected)).Returns(true);
            typeCreator.Create(
                typeof(IEnumerable<Person>),
                "Staff",
                Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last.Value == expected)).Returns(staff);
            typeCreator.Populate(staff, target).Returns(staff);
            valueGenerator.IsSupported(
                typeof(string),
                Arg.Any<string>(),
                Arg.Is<LinkedList<object>>(x => x.Last.Value == expected)).Returns(true);
            valueGenerator.Generate(
                typeof(string),
                Arg.Any<string>(),
                Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last.Value == expected)).Returns(name);

            var actual = (SpecificCompany)target.Populate(expected);

            actual.Should().BeSameAs(expected);
            actual.Name.Should().BeNullOrEmpty();
            actual.Staff.Should().BeSameAs(staff);
        }

        [Fact]
        public void PopulateThrowsExceptionWhenNotInitializedTest()
        {
            var value = new Person();

            var target = new DefaultExecuteStrategy();

            Action action = () => target.Populate(value);

            action.ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void PopulateThrowsExceptionWithNullInstanceTest()
        {
            var target = new DefaultExecuteStrategy();

            Action action = () => target.Populate(null);

            action.ShouldThrow<ArgumentNullException>();
        }

        private class Bottom
        {
            public Top Root { get; set; }

            public string Value { get; set; }
        }

        private class Child
        {
            public Bottom End { get; set; }

            public string Value { get; set; }
        }

        private class Looper
        {
            public Looper Other { get; set; }

            public string Stuff { get; set; }
        }

        private class PopulateInstanceWrapper : DefaultExecuteStrategy
        {
            public void RunTest(object instance = null)
            {
                PopulateInstance(instance, null);
            }
        }

        private class Top
        {
            public Child Next { get; set; }

            public string Value { get; set; }
        }
    }
}