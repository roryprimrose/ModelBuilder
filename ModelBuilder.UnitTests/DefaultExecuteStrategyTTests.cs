namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Reflection;
    using FluentAssertions;
    using ModelBuilder.TypeCreators;
    using ModelBuilder.UnitTests.Models;
    using ModelBuilder.ValueGenerators;
    using NSubstitute;
    using Xunit;

    public class DefaultExecuteStrategyTTests
    {
        [Fact]
        public void CreateReturnsDefaultValueWhenInstanceFailsToBeCreatedTest()
        {
            var typeCreators = new Collection<ITypeCreator>();

            var typeCreator = Substitute.For<ITypeCreator>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();

            typeCreators.Add(typeCreator);

            buildConfiguration.TypeCreators.Returns(typeCreators);
            typeCreator.CanCreate(typeof(SlimModel), null, buildConfiguration, Arg.Any<IBuildChain>()).Returns(true);
            typeCreator.CanPopulate(typeof(SlimModel), null, Arg.Any<IBuildChain>()).Returns(true);

            var target = new DefaultExecuteStrategy<SlimModel>();

            target.Initialize(buildConfiguration);

            var actual = target.Create();

            actual.Should().BeNull();
        }

        [Fact]
        public void CreateReturnsDefaultValueWhenNoReferenceTypeCreatedTest()
        {
            var typeCreators = new Collection<ITypeCreator>();

            var typeCreator = Substitute.For<ITypeCreator>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();

            typeCreators.Add(typeCreator);

            buildConfiguration.TypeCreators.Returns(typeCreators);
            typeCreator.CanCreate(typeof(MemoryStream), null, buildConfiguration, Arg.Any<IBuildChain>()).Returns(true);
            typeCreator.CanPopulate(typeof(MemoryStream), null, Arg.Any<IBuildChain>()).Returns(true);
            typeCreator.Create(typeof(MemoryStream), null, Arg.Any<IExecuteStrategy>()).Returns(null);

            var target = new DefaultExecuteStrategy<MemoryStream>();

            target.Initialize(buildConfiguration);

            var actual = target.Create();

            actual.Should().BeNull();
        }

        [Fact]
        public void CreateReturnsDefaultValueWhenNoValueTypeCreatedTest()
        {
            var valueGenerators = new Collection<IValueGenerator>();

            var valueGenerator = Substitute.For<IValueGenerator>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();

            valueGenerators.Add(valueGenerator);

            buildConfiguration.ValueGenerators.Returns(valueGenerators);
            valueGenerator.IsMatch(typeof(int), null, Arg.Any<IBuildChain>()).Returns(true);
            valueGenerator.Generate(typeof(int), null, Arg.Any<IExecuteStrategy>()).Returns(null);

            var target = new DefaultExecuteStrategy<int>();

            target.Initialize(buildConfiguration);

            var actual = target.Create();

            actual.Should().Be(0);
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

            buildConfiguration.TypeCreators.Returns(typeCreators);
            buildConfiguration.ValueGenerators.Returns(valueGenerators);

            var target = new DefaultExecuteStrategy<SlimModel>();

            target.Initialize(buildConfiguration);

            buildConfiguration.PropertyResolver.Returns(propertyResolver);
            propertyResolver.CanPopulate(Arg.Any<PropertyInfo>()).Returns(true);
            propertyResolver.ShouldPopulateProperty(
                Arg.Any<IBuildConfiguration>(),
                Arg.Any<object>(),
                Arg.Any<PropertyInfo>(),
                Arg.Any<object[]>()).Returns(true);
            typeCreator.CanCreate(typeof(SlimModel), null, buildConfiguration, Arg.Any<IBuildChain>()).Returns(true);
            typeCreator.CanPopulate(typeof(SlimModel), null, Arg.Any<IBuildChain>()).Returns(true);
            typeCreator.Create(typeof(SlimModel), null, Arg.Any<IExecuteStrategy>()).Returns(expected);
            typeCreator.Populate(expected, target).Returns(expected);
            typeCreator.AutoPopulate.Returns(true);
            generator.IsMatch(typeof(Guid), "Value", Arg.Is<IBuildChain>(x => x.Last == expected)).Returns(true);
            generator.Generate(typeof(Guid), "Value", Arg.Is<IExecuteStrategy>(x => x.BuildChain.Last == expected))
                .Returns(value);

            var actual = target.Create();

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

            var target = new DefaultExecuteStrategy<Person>();

            target.Initialize(buildConfiguration);

            typeCreator.CanCreate(typeof(Person), null, buildConfiguration, Arg.Any<IBuildChain>()).Returns(true);
            typeCreator.CanPopulate(typeof(Person), null, Arg.Any<IBuildChain>()).Returns(true);
            typeCreator.Create(typeof(Person), null, Arg.Any<IExecuteStrategy>(), args).Returns(expected);
            typeCreator.Populate(expected, target).Returns(expected);
            typeCreator.AutoPopulate.Returns(false);

            var actual = target.Create(args);

            actual.Should().BeSameAs(expected);
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

            typeCreators.Add(enumerableTypeCreator);
            typeCreators.Add(typeCreator);
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

            var target = new DefaultExecuteStrategy<Company>();

            target.Initialize(buildConfiguration);

            typeCreator.CanPopulate(typeof(Company), null, Arg.Any<IBuildChain>()).Returns(true);
            typeCreator.Populate(expected, target).Returns(expected);
            typeCreator.AutoPopulate.Returns(true);
            enumerableTypeCreator.AutoPopulate.Returns(false);
            enumerableTypeCreator.CanCreate(
                typeof(IEnumerable<Person>),
                "Staff", buildConfiguration,
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

            var actual = target.Populate(expected);

            actual.Should().BeSameAs(expected);
            actual.Name.Should().Be(name);
            actual.Address.Should().Be(address);
            actual.Staff.Should().BeSameAs(staff);
        }

        [Fact]
        public void PopulatePropertyThrowsExceptionWithNullInstanceTest()
        {
            var property = typeof(Person).GetProperty(nameof(Person.FirstName));

            var target = new PopulatePropertyWrapper(property, null);

            Action action = () => target.RunTest();

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PopulatePropertyThrowsExceptionWithNullPropertyTest()
        {
            var instance = new Person();

            var target = new PopulatePropertyWrapper(null, instance);

            Action action = () => target.RunTest();

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PopulateThrowsExceptionWithNullInstanceTest()
        {
            var target = new DefaultExecuteStrategy<Person>();

            Action action = () => target.Populate(null);

            action.Should().Throw<ArgumentNullException>();
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

        private class PopulatePropertyWrapper : DefaultExecuteStrategy<Company>
        {
            private readonly PropertyInfo _propertyInfo;
            private readonly object _instance;

            public PopulatePropertyWrapper(PropertyInfo propertyInfo, object instance)
            {
                _propertyInfo = propertyInfo;
                _instance = instance;
            }
            public void RunTest()
            {
                PopulateProperty(_propertyInfo, _instance);
            }
        }

        private class Top
        {
            public Child Next { get; set; }

            public string Value { get; set; }
        }
    }
}