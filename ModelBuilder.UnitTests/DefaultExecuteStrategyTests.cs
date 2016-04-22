using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace ModelBuilder.UnitTests
{
    public class DefaultExecuteStrategyTests
    {
        [Fact]
        public void CreateWithDoesNotBuildPropertiesWhenTypeCreatorDisablesAutoPopulateTest()
        {
            var model = new SlimModel();

            var creator = Substitute.For<ITypeCreator>();

            var target = new DefaultExecuteStrategy<SlimModel>();

            creator.IsSupported(typeof(SlimModel), null, null).Returns(true);
            creator.Create(typeof(SlimModel), null, null).Returns(model);
            creator.Priority.Returns(1);
            creator.AutoPopulate.Returns(false);
            creator.Populate(model, target).Returns(model);

            target.TypeCreators.Add(creator);

            var actual = target.CreateWith();

            actual.Should().BeSameAs(model);
            actual.Value.Should().BeEmpty();
        }

        [Fact]
        public void CreateWithReturnsDefaultValueWhenInstanceFailsToBeCreatedTest()
        {
            var creator = Substitute.For<ITypeCreator>();

            var target = new DefaultExecuteStrategy<SlimModel>();

            creator.IsSupported(typeof(SlimModel), null, null).Returns(true);

            target.TypeCreators.Add(creator);

            var actual = target.CreateWith();

            actual.Should().BeNull();
        }

        [Fact]
        public void CreateWithReturnsDefaultValueWhenNoReferenceTypeCreatedTest()
        {
            var typeCreator = Substitute.For<ITypeCreator>();

            typeCreator.IsSupported(typeof(Stream), null, null).Returns(true);
            typeCreator.Create(typeof(Stream), null, null).Returns(null);

            var target = new DefaultExecuteStrategy<Stream>();

            target.TypeCreators.Add(typeCreator);

            var actual = target.CreateWith();

            actual.Should().BeNull();
        }

        [Fact]
        public void CreateWithReturnsDefaultValueWhenNoValueTypeCreatedTest()
        {
            var valueGenerator = Substitute.For<IValueGenerator>();

            valueGenerator.IsSupported(typeof(int), null, null).Returns(true);
            valueGenerator.Generate(typeof(int), null, null).Returns(null);

            var target = new DefaultExecuteStrategy<int>();

            target.ValueGenerators.Add(valueGenerator);

            var actual = target.CreateWith();

            actual.Should().Be(0);
        }

        [Fact]
        public void CreateWithReturnsReferenceTypeFromCreatorTest()
        {
            var expected = new SlimModel();
            var value = Guid.NewGuid();

            var generator = Substitute.For<IValueGenerator>();
            var creator = Substitute.For<ITypeCreator>();

            var target = new DefaultExecuteStrategy<SlimModel>();

            creator.IsSupported(typeof(SlimModel), null, null).Returns(true);
            creator.Create(typeof(SlimModel), null, null).Returns(expected);
            creator.Populate(expected, target).Returns(expected);
            creator.AutoPopulate.Returns(true);
            generator.IsSupported(typeof(Guid), "Value", expected).Returns(true);
            generator.Generate(typeof(Guid), "Value", expected).Returns(value);

            target.TypeCreators.Add(creator);
            target.ValueGenerators.Add(generator);

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

            var creator = Substitute.For<ITypeCreator>();

            var target = new DefaultExecuteStrategy<Person>();

            creator.IsSupported(typeof(Person), null, null).Returns(true);
            creator.Create(typeof(Person), null, null, args).Returns(expected);
            creator.Populate(expected, target).Returns(expected);
            creator.AutoPopulate.Returns(false);

            target.TypeCreators.Add(creator);

            var actual = target.CreateWith(args);

            actual.Should().BeSameAs(expected);
        }

        [Fact]
        public void CreateWithReturnsValueFromCreatorWithHighestPriorityTest()
        {
            var firstModel = new SlimModel();
            var secondModel = new SlimModel();
            var value = Guid.NewGuid();

            var firstCreator = Substitute.For<ITypeCreator>();
            var secondCreator = Substitute.For<ITypeCreator>();
            var generator = Substitute.For<IValueGenerator>();

            var target = new DefaultExecuteStrategy<SlimModel>();

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

            target.TypeCreators.Add(firstCreator);
            target.TypeCreators.Add(secondCreator);
            target.ValueGenerators.Add(generator);

            var actual = target.CreateWith();

            actual.Should().BeSameAs(secondModel);
            actual.Value.Should().Be(value);
        }

        [Fact]
        public void CreateWithReturnsValueFromCreatorWithNoArgumentsAndDetectConstructorDisabledTest()
        {
            var expected = new Person();

            var creator = Substitute.For<ITypeCreator>();

            var target = new DefaultExecuteStrategy<Person>();

            creator.IsSupported(typeof(Person), null, null).Returns(true);
            creator.Create(typeof(Person), null, null).Returns(expected);
            creator.Populate(expected, target).Returns(expected);
            creator.AutoPopulate.Returns(false);
            creator.AutoDetectConstructor.Returns(false);

            target.TypeCreators.Add(creator);

            var actual = target.CreateWith();

            actual.Should().BeSameAs(expected);
        }

        [Fact]
        public void CreateWithReturnsValueFromCreatorWithNoArgumentsAndDetectConstructorEnabledTest()
        {
            var expected = new Person();

            var creator = Substitute.For<ITypeCreator>();
            var resolver = Substitute.For<IConstructorResolver>();

            var target = new DefaultExecuteStrategy<Person>
            {
                ConstructorResolver = resolver
            };

            resolver.Resolve(typeof(Person))
                .Returns(typeof(Person).GetConstructors().Single(x => x.GetParameters().Length == 0));
            creator.IsSupported(typeof(Person), null, null).Returns(true);
            creator.Create(typeof(Person), null, null).Returns(expected);
            creator.Populate(expected, target).Returns(expected);
            creator.AutoPopulate.Returns(false);
            creator.AutoDetectConstructor.Returns(true);

            target.TypeCreators.Add(creator);

            var actual = target.CreateWith();

            actual.Should().BeSameAs(expected);
        }

        [Fact]
        public void CreateWithReturnsValueFromGeneratorWithHighestPriorityTest()
        {
            var firstValue = Guid.NewGuid();
            var secondValue = Guid.NewGuid();

            var firstGenerator = Substitute.For<IValueGenerator>();
            var secondGenerator = Substitute.For<IValueGenerator>();

            firstGenerator.IsSupported(typeof(Guid), null, null).Returns(true);
            firstGenerator.Generate(typeof(Guid), null, null).Returns(firstValue);
            firstGenerator.Priority.Returns(1);
            secondGenerator.IsSupported(typeof(Guid), null, null).Returns(true);
            secondGenerator.Generate(typeof(Guid), null, null).Returns(secondValue);
            secondGenerator.Priority.Returns(2);

            var target = new DefaultExecuteStrategy<Guid>();

            target.ValueGenerators.Add(firstGenerator);
            target.ValueGenerators.Add(secondGenerator);

            var actual = target.CreateWith();

            actual.Should().Be(secondValue);
        }

        [Fact]
        public void CreateWithReturnsValueFromResolvedConstructorAndBuiltParametersTest()
        {
            var value = Guid.NewGuid();
            var expected = new ReadOnlyModel(value);

            var generator = Substitute.For<IValueGenerator>();
            var creator = Substitute.For<ITypeCreator>();

            var target = new DefaultExecuteStrategy<ReadOnlyModel>();

            creator.IsSupported(typeof(ReadOnlyModel), null, null).Returns(true);
            creator.Create(typeof(ReadOnlyModel), null, null, value).Returns(expected);
            creator.Populate(expected, target).Returns(expected);
            creator.AutoPopulate.Returns(true);
            creator.AutoDetectConstructor.Returns(true);
            generator.IsSupported(typeof(Guid), "value", null).Returns(true);
            generator.Generate(typeof(Guid), "value", null).Returns(value);

            target.TypeCreators.Add(creator);
            target.ValueGenerators.Add(generator);

            var actual = target.CreateWith();

            actual.Should().Be(expected);
            actual.Value.Should().Be(value);
        }

        [Fact]
        public void CreateWithReturnsValueFromSupportedCreatorTest()
        {
            var firstModel = new SlimModel();
            var secondModel = new SlimModel();
            var value = Guid.NewGuid();

            var firstCreator = Substitute.For<ITypeCreator>();
            var secondCreator = Substitute.For<ITypeCreator>();
            var generator = Substitute.For<IValueGenerator>();

            var target = new DefaultExecuteStrategy<SlimModel>();

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

            target.TypeCreators.Add(firstCreator);
            target.TypeCreators.Add(secondCreator);
            target.ValueGenerators.Add(generator);

            var actual = target.CreateWith();

            actual.Should().BeSameAs(secondModel);
            actual.Value.Should().Be(value);
        }

        [Fact]
        public void CreateWithReturnsValueFromSupportedGeneratorTest()
        {
            var firstValue = Guid.NewGuid();
            var secondValue = Guid.NewGuid();

            var firstGenerator = Substitute.For<IValueGenerator>();
            var secondGenerator = Substitute.For<IValueGenerator>();

            firstGenerator.IsSupported(typeof(Guid), null, null).Returns(false);
            firstGenerator.Generate(typeof(Guid), null, null).Returns(firstValue);
            firstGenerator.Priority.Returns(10);
            secondGenerator.IsSupported(typeof(Guid), null, null).Returns(true);
            secondGenerator.Generate(typeof(Guid), null, null).Returns(secondValue);
            secondGenerator.Priority.Returns(2);

            var target = new DefaultExecuteStrategy<Guid>();

            target.ValueGenerators.Add(firstGenerator);
            target.ValueGenerators.Add(secondGenerator);

            var actual = target.CreateWith();

            actual.Should().Be(secondValue);
        }

        [Fact]
        public void CreateWithReturnsValueTypeFromGeneratorTest()
        {
            var expected = Guid.NewGuid().ToString();

            var valueGenerator = Substitute.For<IValueGenerator>();

            var target = new DefaultExecuteStrategy<string>();

            valueGenerator.IsSupported(typeof(string), null, null).Returns(true);
            valueGenerator.Generate(typeof(string), null, null).Returns(expected);

            target.ValueGenerators.Add(valueGenerator);

            var actual = target.CreateWith();

            actual.Should().Be(expected);
        }

        [Fact]
        public void CreateWithThrowsExceptionWhenConstructorParameterCannotBeCreatedTest()
        {
            var creator = new DefaultTypeCreator();

            var target = new DefaultExecuteStrategy<ReadOnlyModel>();

            target.TypeCreators.Add(creator);

            Action action = () => target.CreateWith();

            action.ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public void CreateWithThrowsExceptionWhenDerivedImplementationSuppliesNullTypeTest()
        {
            var target = new NullTypeBuildExecuteStrategy<int>();

            Action action = () => target.CreateWith();

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void CreateWithThrowsExceptionWhenNoGeneratorOrCreatorMatchFoundTest()
        {
            var target = new DefaultExecuteStrategy<string>();

            Action action = () => target.CreateWith();

            action.ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public void CreateWithThrowsExceptionWhenPropertyCannotBeCreatedTest()
        {
            var creator = new DefaultTypeCreator();

            var target = new DefaultExecuteStrategy<SlimModel>();

            target.TypeCreators.Add(creator);

            Action action = () => target.CreateWith();

            action.ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public void CreateWithThrowsExceptionWithNullTypeTest()
        {
            var target = new DefaultExecuteStrategy<int>();

            Action action = () => target.CreateWith((Type) null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void IsCreatedWithEmptyConfigurationTest()
        {
            var target = new DefaultExecuteStrategy<string>();

            target.ConstructorResolver.Should().BeOfType<DefaultConstructorResolver>();
            target.IgnoreRules.Should().BeEmpty();
            target.TypeCreators.Should().BeEmpty();
            target.ValueGenerators.Should().BeEmpty();
            target.ExecuteOrderRules.Should().BeEmpty();
        }

        [Fact]
        public void PopulateAsObjectAssignsPropertyValuesToExistingInstanceTest()
        {
            var staff = new List<Person>();
            var name = Guid.NewGuid().ToString();
            var address = Guid.NewGuid().ToString();
            var expected = new Company();

            var typeCreator = Substitute.For<ITypeCreator>();
            var valueGenerator = Substitute.For<IValueGenerator>();

            var target = new DefaultExecuteStrategy<Company>();

            typeCreator.IsSupported(typeof(IEnumerable<Person>), "Staff", expected).Returns(true);
            typeCreator.Create(typeof(IEnumerable<Person>), "Staff", expected).Returns(staff);
            typeCreator.Populate(staff, target).Returns(staff);
            valueGenerator.IsSupported(typeof(string), "Name", expected).Returns(true);
            valueGenerator.Generate(typeof(string), "Name", expected).Returns(name);
            valueGenerator.IsSupported(typeof(string), "Address", expected).Returns(true);
            valueGenerator.Generate(typeof(string), "Address", expected).Returns(address);

            target.TypeCreators.Add(typeCreator);
            target.ValueGenerators.Add(valueGenerator);

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

            var typeCreator = Substitute.For<ITypeCreator>();
            var valueGenerator = Substitute.For<IValueGenerator>();

            var target = new DefaultExecuteStrategy<Company>();

            typeCreator.IsSupported(typeof(IEnumerable<Person>), "Staff", expected).Returns(true);
            typeCreator.Create(typeof(IEnumerable<Person>), "Staff", expected).Returns(staff);
            typeCreator.Populate(staff, target).Returns(staff);
            valueGenerator.IsSupported(typeof(string), "Name", expected).Returns(true);
            valueGenerator.Generate(typeof(string), "Name", expected).Returns(name);
            valueGenerator.IsSupported(typeof(string), "Address", expected).Returns(true);
            valueGenerator.Generate(typeof(string), "Address", expected).Returns(address);

            target.TypeCreators.Add(typeCreator);
            target.ValueGenerators.Add(valueGenerator);

            var actual = target.Populate(expected);

            actual.Should().BeSameAs(expected);
            actual.Name.Should().Be(name);
            actual.Address.Should().Be(address);
            actual.Staff.Should().BeSameAs(staff);
        }

        [Fact]
        public void PopulateAssignsValueUsingDefaultExecutionOrderRulesTest()
        {
            var first = SimpleEnum.Seventh;
            var second = Environment.TickCount;
            var third = Guid.NewGuid().ToString();
            var fourth = new Person();
            var expected = new PopulateOrderItem();

            var typeCreator = Substitute.For<ITypeCreator>();
            var valueGenerator = Substitute.For<IValueGenerator>();

            var target = new DefaultExecuteStrategy<PopulateOrderItem>();

            DefaultBuildStrategy.DefaultExecuteOrderRules.ToList().ForEach(x => target.ExecuteOrderRules.Add(x));
            
            valueGenerator.IsSupported(typeof(SimpleEnum), "Z", expected).Returns(true);
            valueGenerator.Generate(typeof(SimpleEnum), "Z", expected).Returns(first);
            valueGenerator.IsSupported(typeof(int), "Y", expected).Returns(true);
            valueGenerator.Generate(typeof(int), "Y", expected).Returns(second);
            valueGenerator.IsSupported(typeof(string), "X", expected).Returns(true);
            valueGenerator.Generate(typeof(string), "X", expected).Returns(third);
            typeCreator.IsSupported(typeof(Person), "W", expected).Returns(true);
            typeCreator.Create(typeof(Person), "W", expected).Returns(fourth);
            typeCreator.Populate(fourth, target).Returns(fourth);

            target.TypeCreators.Add(typeCreator);
            target.ValueGenerators.Add(valueGenerator);

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

            var typeCreator = Substitute.For<ITypeCreator>();
            var valueGenerator = Substitute.For<IValueGenerator>();

            var target = new DefaultExecuteStrategy<Company>();

            typeCreator.IsSupported(typeof(IEnumerable<Person>), "Staff", expected).Returns(true);
            typeCreator.Create(typeof(IEnumerable<Person>), "Staff", expected).Returns(staff);
            typeCreator.Populate(staff, target).Returns(staff);
            valueGenerator.IsSupported(typeof(string), "Name", expected).Returns(true);
            valueGenerator.Generate(typeof(string), "Name", expected).Returns(name);
            valueGenerator.IsSupported(typeof(string), "Address", expected).Returns(true);
            valueGenerator.Generate(typeof(string), "Address", expected).Returns(address);

            target.TypeCreators.Add(typeCreator);
            target.ValueGenerators.Add(valueGenerator);
            target.IgnoreRules.Add(ignoreRule);

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

            var typeCreator = Substitute.For<ITypeCreator>();
            var valueGenerator = Substitute.For<IValueGenerator>();

            var target = new DefaultExecuteStrategy<Company>();

            typeCreator.IsSupported(typeof(IEnumerable<Person>), "Staff", expected).Returns(true);
            typeCreator.Create(typeof(IEnumerable<Person>), "Staff", expected).Returns(staff);
            typeCreator.Populate(staff, target).Returns(staff);
            valueGenerator.IsSupported(typeof(string), "Name", expected).Returns(true);
            valueGenerator.Generate(typeof(string), "Name", expected).Returns(name);
            valueGenerator.IsSupported(typeof(string), "Address", expected).Returns(true);
            valueGenerator.Generate(typeof(string), "Address", expected).Returns(address);

            target.TypeCreators.Add(typeCreator);
            target.ValueGenerators.Add(valueGenerator);
            target.IgnoreRules.Add(ignoreRule);

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

            var typeCreator = Substitute.For<ITypeCreator>();
            var valueGenerator = Substitute.For<IValueGenerator>();

            var target = new DefaultExecuteStrategy<Company>();

            typeCreator.IsSupported(typeof(IEnumerable<Person>), "Staff", expected).Returns(true);
            typeCreator.Create(typeof(IEnumerable<Person>), "Staff", expected).Returns(staff);
            typeCreator.Populate(staff, target).Returns(staff);
            valueGenerator.IsSupported(typeof(string), "Name", expected).Returns(true);
            valueGenerator.Generate(typeof(string), "Name", expected).Returns(name);
            valueGenerator.IsSupported(typeof(string), "Address", expected).Returns(true);
            valueGenerator.Generate(typeof(string), "Address", expected).Returns(address);

            target.TypeCreators.Add(typeCreator);
            target.ValueGenerators.Add(valueGenerator);
            target.IgnoreRules.Add(ignoreRule);

            var actual = target.Populate(expected);

            actual.Should().BeSameAs(expected);
            actual.Name.Should().Be(name);
            actual.Address.Should().Be(address);
            actual.Staff.Should().BeSameAs(staff);
        }

        [Fact]
        public void PopulateOnlySetsPublicPropertiesTest()
        {
            var expected = new PropertyScopes();
            var value = Guid.NewGuid();

            var valueGenerator = Substitute.For<IValueGenerator>();

            var target = new DefaultExecuteStrategy<PropertyScopes>();

            valueGenerator.IsSupported(typeof(Guid), Arg.Any<string>(), Arg.Any<object>()).Returns(true);
            valueGenerator.Generate(typeof(Guid), Arg.Any<string>(), Arg.Any<object>()).Returns(value);

            target.ValueGenerators.Add(valueGenerator);

            var actual = target.Populate(expected);

            actual.Should().BeSameAs(expected);
            actual.Public.Should().Be(value);
            actual.InternalSet.Should().BeEmpty();
            actual.ReadOnly.Should().BeEmpty();
            PropertyScopes.GlobalValue.Should().BeEmpty();
        }

        [Fact]
        public void PopulateSkipsPropertiesMarkedWithIgnoreRuleTest()
        {
            var staff = new List<Person>();
            var name = Guid.NewGuid().ToString();
            var address = Guid.NewGuid().ToString();
            var expected = new Company();
            var ignoreRule = new IgnoreRule(typeof(Company), "Name");

            var typeCreator = Substitute.For<ITypeCreator>();
            var valueGenerator = Substitute.For<IValueGenerator>();

            var target = new DefaultExecuteStrategy<Company>();

            typeCreator.IsSupported(typeof(IEnumerable<Person>), "Staff", expected).Returns(true);
            typeCreator.Create(typeof(IEnumerable<Person>), "Staff", expected).Returns(staff);
            typeCreator.Populate(staff, target).Returns(staff);
            valueGenerator.IsSupported(typeof(string), "Name", expected).Returns(true);
            valueGenerator.Generate(typeof(string), "Name", expected).Returns(name);
            valueGenerator.IsSupported(typeof(string), "Address", expected).Returns(true);
            valueGenerator.Generate(typeof(string), "Address", expected).Returns(address);

            target.TypeCreators.Add(typeCreator);
            target.ValueGenerators.Add(valueGenerator);
            target.IgnoreRules.Add(ignoreRule);

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

            var typeCreator = Substitute.For<ITypeCreator>();
            var valueGenerator = Substitute.For<IValueGenerator>();

            var target = new DefaultExecuteStrategy<SpecificCompany>();

            typeCreator.IsSupported(typeof(IEnumerable<Person>), "Staff", expected).Returns(true);
            typeCreator.Create(typeof(IEnumerable<Person>), "Staff", expected).Returns(staff);
            typeCreator.Populate(staff, target).Returns(staff);
            valueGenerator.IsSupported(typeof(string), Arg.Any<string>(), expected).Returns(true);
            valueGenerator.Generate(typeof(string), Arg.Any<string>(), expected).Returns(name);

            target.TypeCreators.Add(typeCreator);
            target.ValueGenerators.Add(valueGenerator);
            target.IgnoreRules.Add(ignoreRule);

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
    }
}