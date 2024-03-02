namespace ModelBuilder.UnitTests.BuildActions
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder.BuildActions;
    using ModelBuilder.CreationRules;
    using ModelBuilder.TypeCreators;
    using ModelBuilder.UnitTests.Models;
    using ModelBuilder.ValueGenerators;
    using NSubstitute;
    using Xunit;

    public class BuildCapabilityTests
    {
        [Fact]
        public void AutoPopulateReturnsFalseForCreationRule()
        {
            var creationRule = new DummyCreationRule();

            var sut = new BuildCapability(creationRule);

            sut.AutoPopulate.Should().BeFalse();
        }

        [Fact]
        public void AutoPopulateReturnsFalseForValueGenerator()
        {
            var generator = Substitute.For<IValueGenerator>();

            var sut = new BuildCapability(generator);

            sut.AutoPopulate.Should().BeFalse();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void AutoPopulateReturnsTypeCreatorValue(bool autoPopulate)
        {
            var typeCreator = Substitute.For<ITypeCreator>();

            typeCreator.AutoPopulate.Returns(autoPopulate);

            var sut = new BuildCapability(typeCreator, false, autoPopulate);

            sut.AutoPopulate.Should().Be(autoPopulate);
        }

        [Fact]
        public void CreateParameterReturnsValueFromCreationRule()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var value = Guid.NewGuid().ToString();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var creationRule = Substitute.For<ICreationRule>();

            creationRule.Create(executeStrategy, parameterInfo)
                .Returns(value);

            var sut = new BuildCapability(creationRule);

            var actual = sut.CreateParameter(executeStrategy, parameterInfo, null);

            actual.Should().Be(value);
        }

        [Fact]
        public void CreateParameterReturnsValueFromTypeCreator()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var value = Guid.NewGuid().ToString();
            var args = new object?[]
            {
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString()
            };

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var typeCreator = Substitute.For<ITypeCreator>();

            typeCreator.Create(executeStrategy, parameterInfo, args)
                .Returns(value);

            var sut = new BuildCapability(typeCreator, true, false);

            var actual = sut.CreateParameter(executeStrategy, parameterInfo, args);

            actual.Should().Be(value);
        }

        [Fact]
        public void CreateParameterReturnsValueFromValueGenerator()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var value = Guid.NewGuid().ToString();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var valueGenerator = Substitute.For<IValueGenerator>();

            valueGenerator.Generate(executeStrategy, parameterInfo)
                .Returns(value);

            var sut = new BuildCapability(valueGenerator);

            var actual = sut.CreateParameter(executeStrategy, parameterInfo, null);

            actual.Should().Be(value);
        }

        [Fact]
        public void CreateParameterThrowsExceptionWithNullExecuteStrategy()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            var valueGenerator = Substitute.For<IValueGenerator>();

            var sut = new BuildCapability(valueGenerator);

            Action action = () => sut.CreateParameter(null!, parameterInfo, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateParameterThrowsExceptionWithNullParameter()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var valueGenerator = Substitute.For<IValueGenerator>();

            var sut = new BuildCapability(valueGenerator);

            Action action = () => sut.CreateParameter(executeStrategy, null!, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreatePropertyReturnsValueFromCreationRule()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName))!;
            var value = Guid.NewGuid().ToString();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var creationRule = Substitute.For<ICreationRule>();

            creationRule.Create(executeStrategy, propertyInfo).Returns(value);

            var sut = new BuildCapability(creationRule);

            var actual = sut.CreateProperty(executeStrategy, propertyInfo, null);

            actual.Should().Be(value);
        }

        [Fact]
        public void CreatePropertyReturnsValueFromTypeCreator()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName))!;
            var value = Guid.NewGuid().ToString();
            var args = new object?[]
            {
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString()
            };

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var typeCreator = Substitute.For<ITypeCreator>();

            typeCreator.Create(executeStrategy, propertyInfo, args).Returns(value);

            var sut = new BuildCapability(typeCreator, true, false);

            var actual = sut.CreateProperty(executeStrategy, propertyInfo, args);

            actual.Should().Be(value);
        }

        [Fact]
        public void CreatePropertyReturnsValueFromValueGenerator()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName))!;
            var value = Guid.NewGuid().ToString();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var valueGenerator = Substitute.For<IValueGenerator>();

            valueGenerator.Generate(executeStrategy, propertyInfo).Returns(value);

            var sut = new BuildCapability(valueGenerator);

            var actual = sut.CreateProperty(executeStrategy, propertyInfo, null);

            actual.Should().Be(value);
        }

        [Fact]
        public void CreatePropertyThrowsExceptionWithNullExecuteStrategy()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName))!;

            var valueGenerator = Substitute.For<IValueGenerator>();

            var sut = new BuildCapability(valueGenerator);

            Action action = () => sut.CreateProperty(null!, propertyInfo, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreatePropertyThrowsExceptionWithNullProperty()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var valueGenerator = Substitute.For<IValueGenerator>();

            var sut = new BuildCapability(valueGenerator);

            Action action = () => sut.CreateProperty(executeStrategy, null!, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateTypeReturnsValueFromCreationRule()
        {
            var type = typeof(string);
            var value = Guid.NewGuid().ToString();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var creationRule = Substitute.For<ICreationRule>();

            creationRule.Create(executeStrategy, type).Returns(value);

            var sut = new BuildCapability(creationRule);

            var actual = sut.CreateType(executeStrategy, type, null);

            actual.Should().Be(value);
        }

        [Fact]
        public void CreateTypeReturnsValueFromTypeCreator()
        {
            var type = typeof(string);
            var value = Guid.NewGuid().ToString();
            var args = new object?[]
            {
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString()
            };

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var typeCreator = Substitute.For<ITypeCreator>();

            typeCreator.Create(executeStrategy, type, args).Returns(value);

            var sut = new BuildCapability(typeCreator, true, false);

            var actual = sut.CreateType(executeStrategy, type, args);

            actual.Should().Be(value);
        }

        [Fact]
        public void CreateTypeReturnsValueFromValueGenerator()
        {
            var type = typeof(string);
            var value = Guid.NewGuid().ToString();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var valueGenerator = Substitute.For<IValueGenerator>();

            valueGenerator.Generate(executeStrategy, type).Returns(value);

            var sut = new BuildCapability(valueGenerator);

            var actual = sut.CreateType(executeStrategy, type, null);

            actual.Should().Be(value);
        }

        [Fact]
        public void CreateTypeThrowsExceptionWithNullExecuteStrategy()
        {
            var targetType = typeof(string);

            var valueGenerator = Substitute.For<IValueGenerator>();

            var sut = new BuildCapability(valueGenerator);

            Action action = () => sut.CreateType(null!, targetType, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateTypeThrowsExceptionWithNullType()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var valueGenerator = Substitute.For<IValueGenerator>();

            var sut = new BuildCapability(valueGenerator);

            Action action = () => sut.CreateType(executeStrategy, null!, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ImplementedByTypeReturnsCreationRuleType()
        {
            var creationRule = new DummyCreationRule();

            var sut = new BuildCapability(creationRule);

            sut.ImplementedByType.Should().Be<DummyCreationRule>();
        }

        [Fact]
        public void ImplementedByTypeReturnsGeneratorType()
        {
            var generator = new EmailValueGenerator();

            var sut = new BuildCapability(generator);

            sut.ImplementedByType.Should().Be<EmailValueGenerator>();
        }

        [Fact]
        public void ImplementedByTypeReturnsTypeCreatorType()
        {
            var creator = new DefaultTypeCreator();

            var sut = new BuildCapability(creator, false, false);

            sut.ImplementedByType.Should().Be<DefaultTypeCreator>();
        }

        [Fact]
        public void PopulateThrowsExceptionWithNullExecuteStrategy()
        {
            var value = new Person();

            var typeCreator = Substitute.For<ITypeCreator>();

            var sut = new BuildCapability(typeCreator, true, true);

            Action action = () => sut.Populate(null!, value);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PopulateThrowsExceptionWithNullProperty()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var typeCreator = Substitute.For<ITypeCreator>();

            var sut = new BuildCapability(typeCreator, true, true);

            Action action = () => sut.Populate(executeStrategy, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PopulateWithCreationRuleThrowsException()
        {
            var value = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var creationRule = Substitute.For<ICreationRule>();

            var sut = new BuildCapability(creationRule);

            Action action = () => sut.Populate(executeStrategy, value);

            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void PopulateWithTypeCreatorReturnsValue()
        {
            var expected = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var typeCreator = Substitute.For<ITypeCreator>();

            typeCreator.Populate(executeStrategy, expected).Returns(expected);

            var sut = new BuildCapability(typeCreator, false, true);

            var actual = sut.Populate(executeStrategy, expected);

            actual.Should().Be(expected);
        }

        [Fact]
        public void PopulateWithValueGeneratorThrowsException()
        {
            var value = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var valueGenerator = Substitute.For<IValueGenerator>();

            var sut = new BuildCapability(valueGenerator);

            Action action = () => sut.Populate(executeStrategy, value);

            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void SupportsCreateReturnsTrueForCreationRule()
        {
            var creationRule = new DummyCreationRule();

            var sut = new BuildCapability(creationRule);

            sut.SupportsCreate.Should().BeTrue();
        }

        [Fact]
        public void SupportsCreateReturnsTrueForValueGenerator()
        {
            var generator = Substitute.For<IValueGenerator>();

            var sut = new BuildCapability(generator);

            sut.SupportsCreate.Should().BeTrue();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void SupportsCreateReturnsTypeCreatorValue(bool supportsCreate)
        {
            var typeCreator = Substitute.For<ITypeCreator>();

            var sut = new BuildCapability(typeCreator, supportsCreate, false);

            sut.SupportsCreate.Should().Be(supportsCreate);
        }

        [Fact]
        public void SupportsPopulateReturnsFalseForCreationRule()
        {
            var creationRule = new DummyCreationRule();

            var sut = new BuildCapability(creationRule);

            sut.SupportsPopulate.Should().BeFalse();
        }

        [Fact]
        public void SupportsPopulateReturnsFalseForValueGenerator()
        {
            var generator = Substitute.For<IValueGenerator>();

            var sut = new BuildCapability(generator);

            sut.SupportsPopulate.Should().BeFalse();
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void SupportsPopulateReturnsTypeCreatorValue(bool supportsPopulate)
        {
            var typeCreator = Substitute.For<ITypeCreator>();

            var sut = new BuildCapability(typeCreator, false, supportsPopulate);

            sut.SupportsPopulate.Should().Be(supportsPopulate);
        }

        [Fact]
        public void ThrowsExceptionWithNullCreationRule()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new BuildCapability((ICreationRule)null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWithNullTypeCreator()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new BuildCapability(null!, false, false);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWithNullValueGenerator()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new BuildCapability((IValueGenerator)null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}