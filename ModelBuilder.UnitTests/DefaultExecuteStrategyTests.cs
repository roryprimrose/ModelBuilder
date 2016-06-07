using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;
using Xunit.Abstractions;

namespace ModelBuilder.UnitTests
{
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
            var target = new DefaultBuildStrategy().GetExecuteStrategy<Company>();

            target.BuildChain.Should().BeEmpty();

            target.Create();

            target.BuildChain.Should().BeEmpty();
        }

        [Fact]
        public void CreateWithDoesNotBuildPropertiesWhenTypeCreatorDisablesAutoPopulateTest()
        {
            var model = new SlimModel();
            var typeCreators = new List<ITypeCreator>();

            var typeCreator = Substitute.For<ITypeCreator>();
            var buildStrategy = Substitute.For<IBuildStrategy>();

            typeCreators.Add(typeCreator);

            buildStrategy.TypeCreators.Returns(typeCreators);

            var target = new DefaultExecuteStrategy<SlimModel>
            {
                BuildStrategy = buildStrategy
            };

            typeCreator.IsSupported(typeof(SlimModel), null, null).Returns(true);
            typeCreator.Create(typeof(SlimModel), null, null).Returns(model);
            typeCreator.Priority.Returns(1);
            typeCreator.AutoPopulate.Returns(false);
            typeCreator.Populate(model, target).Returns(model);

            var actual = target.CreateWith();

            actual.Should().BeSameAs(model);
            actual.Value.Should().BeEmpty();
        }

        [Fact]
        public void CreateWithReturnsDefaultValueWhenInstanceFailsToBeCreatedTest()
        {
            var typeCreators = new List<ITypeCreator>();

            var typeCreator = Substitute.For<ITypeCreator>();
            var buildStrategy = Substitute.For<IBuildStrategy>();

            typeCreators.Add(typeCreator);

            buildStrategy.TypeCreators.Returns(typeCreators);
            typeCreator.IsSupported(typeof(SlimModel), null, null).Returns(true);

            var target = new DefaultExecuteStrategy<SlimModel>
            {
                BuildStrategy = buildStrategy
            };

            var actual = target.CreateWith();

            actual.Should().BeNull();
        }

        [Fact]
        public void CreateWithReturnsDefaultValueWhenNoReferenceTypeCreatedTest()
        {
            var typeCreators = new List<ITypeCreator>();

            var typeCreator = Substitute.For<ITypeCreator>();
            var buildStrategy = Substitute.For<IBuildStrategy>();

            typeCreators.Add(typeCreator);

            buildStrategy.TypeCreators.Returns(typeCreators);
            typeCreator.IsSupported(typeof(Stream), null, null).Returns(true);
            typeCreator.Create(typeof(Stream), null, null).Returns(null);

            var target = new DefaultExecuteStrategy<Stream>
            {
                BuildStrategy = buildStrategy
            };

            var actual = target.CreateWith();

            actual.Should().BeNull();
        }

        [Fact]
        public void CreateWithReturnsDefaultValueWhenNoValueTypeCreatedTest()
        {
            var valueGenerators = new List<IValueGenerator>();

            var valueGenerator = Substitute.For<IValueGenerator>();
            var buildStrategy = Substitute.For<IBuildStrategy>();

            valueGenerators.Add(valueGenerator);

            buildStrategy.ValueGenerators.Returns(valueGenerators);
            valueGenerator.IsSupported(typeof(int), null, null).Returns(true);
            valueGenerator.Generate(typeof(int), null, null).Returns(null);

            var target = new DefaultExecuteStrategy<int>
            {
                BuildStrategy = buildStrategy
            };

            var actual = target.CreateWith();

            actual.Should().Be(0);
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

            buildStrategy.TypeCreators.Returns(typeCreators);
            buildStrategy.ValueGenerators.Returns(valueGenerators);

            var target = new DefaultExecuteStrategy<SlimModel>
            {
                BuildStrategy = buildStrategy
            };

            typeCreator.IsSupported(typeof(SlimModel), null, null).Returns(true);
            typeCreator.Create(typeof(SlimModel), null, null).Returns(expected);
            typeCreator.Populate(expected, target).Returns(expected);
            typeCreator.AutoPopulate.Returns(true);
            generator.IsSupported(typeof(Guid), "Value", expected).Returns(true);
            generator.Generate(typeof(Guid), "Value", expected).Returns(value);

            var actual = target.CreateWith();

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

            buildStrategy.TypeCreators.Returns(typeCreators);

            var target = new DefaultExecuteStrategy<Person>
            {
                BuildStrategy = buildStrategy
            };

            typeCreator.IsSupported(typeof(Person), null, null).Returns(true);
            typeCreator.Create(typeof(Person), null, null, args).Returns(expected);
            typeCreator.Populate(expected, target).Returns(expected);
            typeCreator.AutoPopulate.Returns(false);

            var actual = target.CreateWith(args);

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

            buildStrategy.TypeCreators.Returns(typeCreators);
            buildStrategy.ValueGenerators.Returns(valueGenerators);

            var target = new DefaultExecuteStrategy<SlimModel>
            {
                BuildStrategy = buildStrategy
            };

            firstCreator.IsSupported(typeof(SlimModel), null, null).Returns(true);
            firstCreator.Create(typeof(SlimModel), null, null).Returns(firstModel);
            firstCreator.Priority.Returns(1);
            firstCreator.AutoPopulate.Returns(true);
            firstCreator.Populate(firstModel, target).Returns(firstModel);
            secondCreator.IsSupported(typeof(SlimModel), null, null).Returns(true);
            secondCreator.Create(typeof(SlimModel), null, null).Returns(secondModel);
            secondCreator.Priority.Returns(2);
            secondCreator.AutoPopulate.Returns(true);
            secondCreator.Populate(secondModel, target).Returns(secondModel);
            generator.IsSupported(typeof(Guid), "Value", secondModel).Returns(true);
            generator.Generate(typeof(Guid), "Value", secondModel).Returns(value);

            var actual = target.CreateWith();

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

            buildStrategy.TypeCreators.Returns(typeCreators);

            var target = new DefaultExecuteStrategy<Person>
            {
                BuildStrategy = buildStrategy
            };

            typeCreator.IsSupported(typeof(Person), null, null).Returns(true);
            typeCreator.Create(typeof(Person), null, null).Returns(expected);
            typeCreator.Populate(expected, target).Returns(expected);
            typeCreator.AutoPopulate.Returns(false);
            typeCreator.AutoDetectConstructor.Returns(false);

            var actual = target.CreateWith();

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

            buildStrategy.TypeCreators.Returns(typeCreators);
            buildStrategy.ConstructorResolver.Returns(resolver);

            var target = new DefaultExecuteStrategy<Person>
            {
                BuildStrategy = buildStrategy
            };

            resolver.Resolve(typeof(Person))
                .Returns(typeof(Person).GetConstructors().Single(x => x.GetParameters().Length == 0));
            typeCreator.IsSupported(typeof(Person), null, null).Returns(true);
            typeCreator.Create(typeof(Person), null, null).Returns(expected);
            typeCreator.Populate(expected, target).Returns(expected);
            typeCreator.AutoPopulate.Returns(false);
            typeCreator.AutoDetectConstructor.Returns(true);

            var actual = target.CreateWith();

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

            buildStrategy.ValueGenerators.Returns(valueGenerators);
            firstGenerator.IsSupported(typeof(Guid), null, null).Returns(true);
            firstGenerator.Generate(typeof(Guid), null, null).Returns(firstValue);
            firstGenerator.Priority.Returns(1);
            secondGenerator.IsSupported(typeof(Guid), null, null).Returns(true);
            secondGenerator.Generate(typeof(Guid), null, null).Returns(secondValue);
            secondGenerator.Priority.Returns(2);

            var target = new DefaultExecuteStrategy<Guid>
            {
                BuildStrategy = buildStrategy
            };

            var actual = target.CreateWith();

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
            var buildLog = Substitute.For<IBuildLog>();
            var generator = Substitute.For<IValueGenerator>();
            var typeCreator = Substitute.For<ITypeCreator>();

            typeCreators.Add(typeCreator);
            valueGenerators.Add(generator);

            resolver.Resolve(typeof(ReadOnlyModel), Arg.Any<object[]>())
                .Returns(typeof(ReadOnlyModel).GetConstructors()[0]);
            buildStrategy.ConstructorResolver.Returns(resolver);
            buildStrategy.BuildLog.Returns(buildLog);
            buildStrategy.TypeCreators.Returns(typeCreators);
            buildStrategy.ValueGenerators.Returns(valueGenerators);

            var target = new DefaultExecuteStrategy<ReadOnlyModel>
            {
                BuildStrategy = buildStrategy
            };

            typeCreator.IsSupported(typeof(ReadOnlyModel), null, null).Returns(true);
            typeCreator.Create(typeof(ReadOnlyModel), null, null, value).Returns(expected);
            typeCreator.Populate(expected, target).Returns(expected);
            typeCreator.AutoPopulate.Returns(true);
            typeCreator.AutoDetectConstructor.Returns(true);
            generator.IsSupported(typeof(Guid), "value", null).Returns(true);
            generator.Generate(typeof(Guid), "value", null).Returns(value);

            var actual = target.CreateWith();

            actual.Should().Be(expected);
            actual.Value.Should().Be(value);
        }

        [Fact]
        public void CreateWithReturnsValueFromSupportedCreationRuleTest()
        {
            var firstValue = Guid.NewGuid().ToString();
            var secondValue = Guid.NewGuid();

            var buildStrategy = Model.DefaultBuildStrategy.Clone()
                .Add(new CreationRule(typeof(Address), "Id", 100, firstValue))
                .Add(new CreationRule(typeof(Person), "Id", 20, secondValue))
                .Compile();

            var target = new DefaultExecuteStrategy<Person>
            {
                BuildStrategy = buildStrategy
            };

            var actual = target.CreateWith();

            actual.Id.Should().Be(secondValue);
        }

        [Fact]
        public void CreateWithReturnsValueFromSupportedCreationRuleWithHigherPriorityTest()
        {
            var firstValue = Guid.NewGuid();
            var secondValue = Guid.NewGuid();

            var buildStrategy = Model.DefaultBuildStrategy.Clone()
                .Add(new CreationRule(typeof(Person), "Id", 10, firstValue))
                .Add(new CreationRule(typeof(Person), "Id", 20, secondValue))
                .Compile();

            var target = new DefaultExecuteStrategy<Person>
            {
                BuildStrategy = buildStrategy
            };

            var actual = target.CreateWith();

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

            buildStrategy.TypeCreators.Returns(typeCreators);
            buildStrategy.ValueGenerators.Returns(valueGenerators);

            var target = new DefaultExecuteStrategy<SlimModel>
            {
                BuildStrategy = buildStrategy
            };

            firstCreator.IsSupported(typeof(SlimModel), null, null).Returns(false);
            firstCreator.Create(typeof(SlimModel), null, null).Returns(firstModel);
            firstCreator.Priority.Returns(10);
            firstCreator.AutoPopulate.Returns(true);
            firstCreator.Populate(firstModel, target).Returns(firstModel);
            secondCreator.IsSupported(typeof(SlimModel), null, null).Returns(true);
            secondCreator.Create(typeof(SlimModel), null, null).Returns(secondModel);
            secondCreator.Priority.Returns(2);
            secondCreator.AutoPopulate.Returns(true);
            secondCreator.Populate(secondModel, target).Returns(secondModel);
            generator.IsSupported(typeof(Guid), "Value", secondModel).Returns(true);
            generator.Generate(typeof(Guid), "Value", secondModel).Returns(value);

            var actual = target.CreateWith();

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

            buildStrategy.ValueGenerators.Returns(valueGenerators);
            firstGenerator.IsSupported(typeof(Guid), null, null).Returns(false);
            firstGenerator.Generate(typeof(Guid), null, null).Returns(firstValue);
            firstGenerator.Priority.Returns(10);
            secondGenerator.IsSupported(typeof(Guid), null, null).Returns(true);
            secondGenerator.Generate(typeof(Guid), null, null).Returns(secondValue);
            secondGenerator.Priority.Returns(2);

            var target = new DefaultExecuteStrategy<Guid>
            {
                BuildStrategy = buildStrategy
            };

            var actual = target.CreateWith();

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

            buildStrategy.ValueGenerators.Returns(valueGenerators);
            valueGenerator.IsSupported(typeof(string), null, null).Returns(true);
            valueGenerator.Generate(typeof(string), null, null).Returns(expected);

            var target = new DefaultExecuteStrategy<string>
            {
                BuildStrategy = buildStrategy
            };

            var actual = target.CreateWith();

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
            buildStrategy.TypeCreators.Returns(typeCreators);

            var target = new DefaultExecuteStrategy<ReadOnlyModel>
            {
                BuildStrategy = buildStrategy
            };

            Action action = () => target.CreateWith();

            action.ShouldThrow<BuildException>();
        }

        [Fact]
        public void CreateWithThrowsExceptionWhenCreatingTypeFailsTest()
        {
            var typeCreator = Substitute.For<ITypeCreator>();

            typeCreator.IsSupported(typeof(Address), "Address", Arg.Any<object>()).Returns(true);
            typeCreator.Priority.Returns(int.MaxValue);
            typeCreator.AutoDetectConstructor.Returns(true);
            typeCreator.AutoPopulate.Returns(true);
            typeCreator.Create(typeof(Address), "Address", Arg.Any<object>()).Throws(new InvalidOperationException());

            var buildStrategy = new DefaultBuildStrategy().Clone().Add(typeCreator).Compile();

            var target = new DefaultExecuteStrategy<Person>
            {
                BuildStrategy = buildStrategy
            };

            Action action = () => target.CreateWith();

            var exception =
                action.ShouldThrow<BuildException>().Where(x => x.Message != null).Where(x => x.BuildLog != null).Which;

            _output.WriteLine(exception.Message);
        }

        [Fact]
        public void CreateWithThrowsExceptionWhenDerivedImplementationSuppliesNullTypeTest()
        {
            var buildStrategy = Substitute.For<IBuildStrategy>();

            var target = new NullTypeBuildExecuteStrategy<int>
            {
                BuildStrategy = buildStrategy
            };

            Action action = () => target.CreateWith();

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void CreateWithThrowsExceptionWhenGeneratingValueFailsTest()
        {
            var person = new Person();
            var buildLog = new DefaultBuildLog();
            var generators = new List<IValueGenerator>();
            var creators = new List<ITypeCreator>();

            var valueGenerator = Substitute.For<IValueGenerator>();
            var typeCreator = Substitute.For<ITypeCreator>();
            var buildStrategy = Substitute.For<IBuildStrategy>();

            generators.Add(valueGenerator);
            creators.Add(typeCreator);

            typeCreator.IsSupported(typeof(Person), null, null).Returns(true);
            typeCreator.Create(typeof(Person), null, null, null).Returns(person);
            valueGenerator.IsSupported(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<object>()).Returns(true);
            valueGenerator.Generate(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<object>())
                .Throws(new InvalidOperationException());
            buildStrategy.TypeCreators.Returns(creators);
            buildStrategy.ValueGenerators.Returns(generators);
            buildStrategy.BuildLog.Returns(buildLog);

            var target = new DefaultExecuteStrategy<Person>
            {
                BuildStrategy = buildStrategy
            };

            Action action = () => target.CreateWith();

            var exception =
                action.ShouldThrow<BuildException>().Where(x => x.Message != null).Where(x => x.BuildLog != null).Which;

            _output.WriteLine(exception.Message);
        }

        [Fact]
        public void CreateWithThrowsExceptionWhenGeneratingValueThrowsBuildExceptionTest()
        {
            var person = new Person();
            var buildLog = new DefaultBuildLog();
            var generators = new List<IValueGenerator>();
            var creators = new List<ITypeCreator>();

            var valueGenerator = Substitute.For<IValueGenerator>();
            var typeCreator = Substitute.For<ITypeCreator>();
            var buildStrategy = Substitute.For<IBuildStrategy>();

            generators.Add(valueGenerator);
            creators.Add(typeCreator);

            typeCreator.IsSupported(typeof(Person), null, null).Returns(true);
            typeCreator.Create(typeof(Person), null, null, null).Returns(person);
            valueGenerator.IsSupported(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<object>()).Returns(true);
            valueGenerator.Generate(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<object>()).Throws(new BuildException());
            buildStrategy.TypeCreators.Returns(creators);
            buildStrategy.ValueGenerators.Returns(generators);
            buildStrategy.BuildLog.Returns(buildLog);

            var target = new DefaultExecuteStrategy<Person>
            {
                BuildStrategy = buildStrategy
            };

            Action action = () => target.CreateWith();

            action.ShouldThrow<BuildException>();
        }

        [Fact]
        public void CreateWithThrowsExceptionWhenNoGeneratorOrCreatorMatchFoundTest()
        {
            var buildStrategy = Substitute.For<IBuildStrategy>();

            var target = new DefaultExecuteStrategy<string>
            {
                BuildStrategy = buildStrategy
            };

            Action action = () => target.CreateWith();

            action.ShouldThrow<BuildException>();
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
            buildStrategy.TypeCreators.Returns(typeCreators);

            var target = new DefaultExecuteStrategy<SlimModel>
            {
                BuildStrategy = buildStrategy
            };

            Action action = () => target.CreateWith();

            action.ShouldThrow<BuildException>();
        }

        [Fact]
        public void CreateWithThrowsExceptionWithNullTypeTest()
        {
            var buildStrategy = Substitute.For<IBuildStrategy>();

            var target = new DefaultExecuteStrategy<int>
            {
                BuildStrategy = buildStrategy
            };

            Action action = () => target.CreateWith((Type) null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void IsCreatedWithBuildChainInstanceTest()
        {
            var target = new DefaultExecuteStrategy<Company>();

            target.BuildChain.Should().NotBeNull();
        }

        [Fact]
        public void IsCreatedWithBuildStrategyTest()
        {
            var target = new DefaultExecuteStrategy<string>();

            target.BuildStrategy.Should().BeOfType<DefaultBuildStrategy>();
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

            buildStrategy.TypeCreators.Returns(typeCreators);
            buildStrategy.ValueGenerators.Returns(valueGenerators);

            var target = new DefaultExecuteStrategy<Company>
            {
                BuildStrategy = buildStrategy
            };

            typeCreator.IsSupported(typeof(IEnumerable<Person>), "Staff", expected).Returns(true);
            typeCreator.Create(typeof(IEnumerable<Person>), "Staff", expected).Returns(staff);
            typeCreator.Populate(staff, target).Returns(staff);
            valueGenerator.IsSupported(typeof(string), "Name", expected).Returns(true);
            valueGenerator.Generate(typeof(string), "Name", expected).Returns(name);
            valueGenerator.IsSupported(typeof(string), "Address", expected).Returns(true);
            valueGenerator.Generate(typeof(string), "Address", expected).Returns(address);

            var actual = (Company) target.Populate((object) expected);

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

            buildStrategy.TypeCreators.Returns(typeCreators);
            buildStrategy.ValueGenerators.Returns(valueGenerators);

            var target = new DefaultExecuteStrategy<Company>
            {
                BuildStrategy = buildStrategy
            };

            typeCreator.IsSupported(typeof(IEnumerable<Person>), "Staff", expected).Returns(true);
            typeCreator.Create(typeof(IEnumerable<Person>), "Staff", expected).Returns(staff);
            typeCreator.Populate(staff, target).Returns(staff);
            valueGenerator.IsSupported(typeof(string), "Name", expected).Returns(true);
            valueGenerator.Generate(typeof(string), "Name", expected).Returns(name);
            valueGenerator.IsSupported(typeof(string), "Address", expected).Returns(true);
            valueGenerator.Generate(typeof(string), "Address", expected).Returns(address);

            var actual = target.Populate(expected);

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

            var buildStrategy = Substitute.For<IBuildStrategy>();
            var typeCreator = Substitute.For<ITypeCreator>();
            var valueGenerator = Substitute.For<IValueGenerator>();

            typeCreators.Add(typeCreator);
            valueGenerators.Add(valueGenerator);

            buildStrategy.TypeCreators.Returns(typeCreators);
            buildStrategy.ValueGenerators.Returns(valueGenerators);
            buildStrategy.ExecuteOrderRules.Returns(
                new ReadOnlyCollection<ExecuteOrderRule>(DefaultBuildStrategy.DefaultExecuteOrderRules.ToList()));

            var target = new DefaultExecuteStrategy<PopulateOrderItem>
            {
                BuildStrategy = buildStrategy
            };

            valueGenerator.IsSupported(typeof(SimpleEnum), "Z", expected).Returns(true);
            valueGenerator.Generate(typeof(SimpleEnum), "Z", expected).Returns(first);
            valueGenerator.IsSupported(typeof(int), "Y", expected).Returns(true);
            valueGenerator.Generate(typeof(int), "Y", expected).Returns(second);
            valueGenerator.IsSupported(typeof(string), "X", expected).Returns(true);
            valueGenerator.Generate(typeof(string), "X", expected).Returns(third);
            typeCreator.IsSupported(typeof(Person), "W", expected).Returns(true);
            typeCreator.Create(typeof(Person), "W", expected).Returns(fourth);
            typeCreator.Populate(fourth, target).Returns(fourth);

            var actual = target.Populate(expected);

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

            buildStrategy.TypeCreators.Returns(typeCreators);
            buildStrategy.ValueGenerators.Returns(valueGenerators);
            buildStrategy.IgnoreRules.Returns(ignoreRules);

            var target = new DefaultExecuteStrategy<Company>
            {
                BuildStrategy = buildStrategy
            };

            typeCreator.IsSupported(typeof(IEnumerable<Person>), "Staff", expected).Returns(true);
            typeCreator.Create(typeof(IEnumerable<Person>), "Staff", expected).Returns(staff);
            typeCreator.Populate(staff, target).Returns(staff);
            valueGenerator.IsSupported(typeof(string), "Name", expected).Returns(true);
            valueGenerator.Generate(typeof(string), "Name", expected).Returns(name);
            valueGenerator.IsSupported(typeof(string), "Address", expected).Returns(true);
            valueGenerator.Generate(typeof(string), "Address", expected).Returns(address);

            var actual = target.Populate(expected);

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

            buildStrategy.TypeCreators.Returns(typeCreators);
            buildStrategy.ValueGenerators.Returns(valueGenerators);
            buildStrategy.IgnoreRules.Returns(ignoreRules);

            var target = new DefaultExecuteStrategy<Company>
            {
                BuildStrategy = buildStrategy
            };

            typeCreator.IsSupported(typeof(IEnumerable<Person>), "Staff", expected).Returns(true);
            typeCreator.Create(typeof(IEnumerable<Person>), "Staff", expected).Returns(staff);
            typeCreator.Populate(staff, target).Returns(staff);
            valueGenerator.IsSupported(typeof(string), "Name", expected).Returns(true);
            valueGenerator.Generate(typeof(string), "Name", expected).Returns(name);
            valueGenerator.IsSupported(typeof(string), "Address", expected).Returns(true);
            valueGenerator.Generate(typeof(string), "Address", expected).Returns(address);

            var actual = target.Populate(expected);

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

            buildStrategy.TypeCreators.Returns(typeCreators);
            buildStrategy.ValueGenerators.Returns(valueGenerators);
            buildStrategy.IgnoreRules.Returns(ignoreRules);

            var target = new DefaultExecuteStrategy<Company>
            {
                BuildStrategy = buildStrategy
            };

            typeCreator.IsSupported(typeof(IEnumerable<Person>), "Staff", expected).Returns(true);
            typeCreator.Create(typeof(IEnumerable<Person>), "Staff", expected).Returns(staff);
            typeCreator.Populate(staff, target).Returns(staff);
            valueGenerator.IsSupported(typeof(string), "Name", expected).Returns(true);
            valueGenerator.Generate(typeof(string), "Name", expected).Returns(name);
            valueGenerator.IsSupported(typeof(string), "Address", expected).Returns(true);
            valueGenerator.Generate(typeof(string), "Address", expected).Returns(address);

            var actual = target.Populate(expected);

            actual.Should().BeSameAs(expected);
            actual.Name.Should().Be(name);
            actual.Address.Should().Be(address);
            actual.Staff.Should().BeSameAs(staff);
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

            buildStrategy.ValueGenerators.Returns(valueGenerators);

            var target = new DefaultExecuteStrategy<PropertyScopes>
            {
                BuildStrategy = buildStrategy
            };

            valueGenerator.IsSupported(typeof(Guid), Arg.Any<string>(), Arg.Any<object>()).Returns(true);
            valueGenerator.Generate(typeof(Guid), Arg.Any<string>(), Arg.Any<object>()).Returns(value);

            var actual = target.Populate(expected);

            actual.Should().BeSameAs(expected);
            actual.Public.Should().Be(value);
            actual.InternalSet.Should().BeEmpty();
            actual.ReadOnly.Should().BeEmpty();
            PropertyScopes.GlobalValue.Should().BeEmpty();
        }

        [Fact]
        public void PopulatePushesInstanceIntoBuildChainWhileCreatingTest()
        {
            var creator = Substitute.For<ITypeCreator>();
            var generator = Substitute.For<IValueGenerator>();
            var buildStrategy = Substitute.For<IBuildStrategy>();
            var log = Substitute.For<IBuildLog>();

            var instance = new SlimModel();
            var target = new DefaultExecuteStrategy<SlimModel>
            {
                BuildStrategy = buildStrategy
            };
            var testPassed = false;

            buildStrategy.BuildLog.Returns(log);
            buildStrategy.CreationRules.Returns(new List<CreationRule>().AsReadOnly());
            buildStrategy.ValueGenerators.Returns(new List<IValueGenerator> {generator}.AsReadOnly());
            buildStrategy.TypeCreators.Returns(new List<ITypeCreator> {creator}.AsReadOnly());
            creator.IsSupported(typeof(SlimModel), null, null).Returns(true);
            creator.Create(typeof(SlimModel), null, null).Returns(instance);
            creator.AutoPopulate.Returns(true);
            generator.When(x => x.Generate(typeof(Guid), nameof(SlimModel.Value), instance)).Do(x =>
            {
                target.BuildChain.Should().HaveCount(1);
                target.BuildChain.First().Should().BeOfType<SlimModel>();
                testPassed = true;
            });

            generator.IsSupported(typeof(Guid), nameof(SlimModel.Value), instance).Returns(true);

            target.Create();

            testPassed.Should().BeTrue();
        }

        [Fact]
        public void PopulatePushesInstanceIntoBuildChainWhilePopulatingTest()
        {
            var generator = Substitute.For<IValueGenerator>();
            var buildStrategy = Substitute.For<IBuildStrategy>();
            var log = Substitute.For<IBuildLog>();

            var instance = new SlimModel();
            var target = new DefaultExecuteStrategy<SlimModel>
            {
                BuildStrategy = buildStrategy
            };
            var testPassed = false;

            buildStrategy.BuildLog.Returns(log);
            buildStrategy.CreationRules.Returns(new List<CreationRule>().AsReadOnly());
            buildStrategy.ValueGenerators.Returns(new List<IValueGenerator> {generator}.AsReadOnly());
            generator.When(x => x.Generate(typeof(Guid), nameof(SlimModel.Value), instance)).Do(x =>
            {
                target.BuildChain.Should().HaveCount(1);
                target.BuildChain.Should().Contain(instance);
                testPassed = true;
            });

            generator.IsSupported(typeof(Guid), nameof(SlimModel.Value), instance).Returns(true);

            target.Populate(instance);

            testPassed.Should().BeTrue();
        }

        [Fact]
        public void PopulatePushesNestedInstanceIntoBuildChainWhileCreatingTest()
        {
            var creator = Substitute.For<ITypeCreator>();
            var generator = Substitute.For<IValueGenerator>();
            var buildStrategy = Substitute.For<IBuildStrategy>();
            var log = Substitute.For<IBuildLog>();

            var office = new Office();
            var address = new Address();
            var target = new DefaultExecuteStrategy<Office>
            {
                BuildStrategy = buildStrategy
            };
            var testPassed = false;

            buildStrategy.BuildLog.Returns(log);
            buildStrategy.CreationRules.Returns(new List<CreationRule>().AsReadOnly());
            buildStrategy.ValueGenerators.Returns(new List<IValueGenerator> {generator}.AsReadOnly());
            buildStrategy.TypeCreators.Returns(new List<ITypeCreator> {creator}.AsReadOnly());
            creator.IsSupported(Arg.Any<Type>(), Arg.Any<string>(), Arg.Any<object>()).Returns(true);
            creator.Create(typeof(Office), null, null).Returns(office);
            creator.Create(typeof(Address), "Address", office).Returns(address);
            creator.AutoPopulate.Returns(true);
            generator.When(x => x.Generate(typeof(string), Arg.Any<string>(), address)).Do(x =>
            {
                target.BuildChain.Should().HaveCount(2);
                target.BuildChain.First.Value.Should().Be(office);
                target.BuildChain.Last.Value.Should().Be(address);
                testPassed = true;
            });

            generator.IsSupported(typeof(string), Arg.Any<string>(), Arg.Any<object>()).Returns(true);
            generator.Generate(typeof(string), Arg.Any<string>(), Arg.Any<object>()).Returns(Guid.NewGuid().ToString());

            target.Create();

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

            buildStrategy.TypeCreators.Returns(typeCreators);
            buildStrategy.ValueGenerators.Returns(valueGenerators);
            buildStrategy.IgnoreRules.Returns(ignoreRules);

            var target = new DefaultExecuteStrategy<Company>
            {
                BuildStrategy = buildStrategy
            };

            typeCreator.IsSupported(typeof(IEnumerable<Person>), "Staff", expected).Returns(true);
            typeCreator.Create(typeof(IEnumerable<Person>), "Staff", expected).Returns(staff);
            typeCreator.Populate(staff, target).Returns(staff);
            valueGenerator.IsSupported(typeof(string), "Name", expected).Returns(true);
            valueGenerator.Generate(typeof(string), "Name", expected).Returns(name);
            valueGenerator.IsSupported(typeof(string), "Address", expected).Returns(true);
            valueGenerator.Generate(typeof(string), "Address", expected).Returns(address);

            var actual = target.Populate(expected);

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

            buildStrategy.TypeCreators.Returns(typeCreators);
            buildStrategy.ValueGenerators.Returns(valueGenerators);
            buildStrategy.IgnoreRules.Returns(ignoreRules);

            var target = new DefaultExecuteStrategy<SpecificCompany>
            {
                BuildStrategy = buildStrategy
            };

            typeCreator.IsSupported(typeof(IEnumerable<Person>), "Staff", expected).Returns(true);
            typeCreator.Create(typeof(IEnumerable<Person>), "Staff", expected).Returns(staff);
            typeCreator.Populate(staff, target).Returns(staff);
            valueGenerator.IsSupported(typeof(string), Arg.Any<string>(), expected).Returns(true);
            valueGenerator.Generate(typeof(string), Arg.Any<string>(), expected).Returns(name);

            var actual = target.Populate(expected);

            actual.Should().BeSameAs(expected);
            actual.Name.Should().BeNullOrEmpty();
            actual.Staff.Should().BeSameAs(staff);
        }

        [Fact]
        public void PopulateThrowsExceptionWithNullInstanceTest()
        {
            var target = new DefaultExecuteStrategy<int>();

            Action action = () => target.Populate(null);

            action.ShouldThrow<ArgumentNullException>();
        }

        private class PopulateInstanceWrapper : DefaultExecuteStrategy<Company>
        {
            public void RunTest()
            {
                PopulateInstance(null);
            }
        }
    }
}