namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using FluentAssertions;
    using Models;
    using NSubstitute;
    using Xunit;

    public class DefaultExecuteStrategyTTests
    {
        [Fact]
        public void CreateReturnsDefaultValueWhenInstanceFailsToBeCreatedTest()
        {
            var typeCreators = new List<ITypeCreator>();

            var typeCreator = Substitute.For<ITypeCreator>();
            var buildStrategy = Substitute.For<IBuildStrategy>();

            typeCreators.Add(typeCreator);

            buildStrategy.TypeCreators.Returns(typeCreators.AsReadOnly());
            typeCreator.CanCreate(typeof(SlimModel), null, Arg.Any<IBuildChain>()).Returns(true);
            typeCreator.CanPopulate(typeof(SlimModel), null, Arg.Any<IBuildChain>()).Returns(true);

            var target = new DefaultExecuteStrategy<SlimModel>();

            target.Initialize(buildStrategy, buildStrategy.GetBuildLog());

            var actual = target.Create();

            actual.Should().BeNull();
        }

        [Fact]
        public void CreateReturnsDefaultValueWhenNoReferenceTypeCreatedTest()
        {
            var typeCreators = new List<ITypeCreator>();

            var typeCreator = Substitute.For<ITypeCreator>();
            var buildStrategy = Substitute.For<IBuildStrategy>();

            typeCreators.Add(typeCreator);

            buildStrategy.TypeCreators.Returns(typeCreators.AsReadOnly());
            typeCreator.CanCreate(typeof(MemoryStream), null, Arg.Any<IBuildChain>()).Returns(true);
            typeCreator.CanPopulate(typeof(MemoryStream), null, Arg.Any<IBuildChain>()).Returns(true);
            typeCreator.Create(typeof(MemoryStream), null, Arg.Any<IExecuteStrategy>()).Returns(null);

            var target = new DefaultExecuteStrategy<MemoryStream>();

            target.Initialize(buildStrategy, buildStrategy.GetBuildLog());

            var actual = target.Create();

            actual.Should().BeNull();
        }

        [Fact]
        public void CreateReturnsDefaultValueWhenNoValueTypeCreatedTest()
        {
            var valueGenerators = new List<IValueGenerator>();

            var valueGenerator = Substitute.For<IValueGenerator>();
            var buildStrategy = Substitute.For<IBuildStrategy>();

            valueGenerators.Add(valueGenerator);

            buildStrategy.ValueGenerators.Returns(valueGenerators.AsReadOnly());
            valueGenerator.IsSupported(typeof(int), null, Arg.Any<IBuildChain>()).Returns(true);
            valueGenerator.Generate(typeof(int), null, Arg.Any<IExecuteStrategy>()).Returns(null);

            var target = new DefaultExecuteStrategy<int>();

            target.Initialize(buildStrategy, buildStrategy.GetBuildLog());

            var actual = target.Create();

            actual.Should().Be(0);
        }

        [Fact]
        public void CreateReturnsReferenceTypeFromCreatorTest()
        {
            var expected = new SlimModel();
            var value = Guid.NewGuid();

            var typeCreators = new List<ITypeCreator>();
            var valueGenerators = new List<IValueGenerator>();

            var typeCreator = Substitute.For<ITypeCreator>();
            var generator = Substitute.For<IValueGenerator>();
            var buildStrategy = Substitute.For<IBuildStrategy>();
            var propertyResolver = Substitute.For<IPropertyResolver>();

            typeCreators.Add(typeCreator);
            valueGenerators.Add(generator);

            buildStrategy.TypeCreators.Returns(typeCreators.AsReadOnly());
            buildStrategy.ValueGenerators.Returns(valueGenerators.AsReadOnly());

            var target = new DefaultExecuteStrategy<SlimModel>();

            target.Initialize(buildStrategy, buildStrategy.GetBuildLog());

            buildStrategy.PropertyResolver.Returns(propertyResolver);
            propertyResolver.CanPopulate(Arg.Any<PropertyInfo>()).Returns(true);
            propertyResolver.ShouldPopulateProperty(
                Arg.Any<IBuildConfiguration>(),
                Arg.Any<object>(),
                Arg.Any<PropertyInfo>(),
                Arg.Any<object[]>()).Returns(true);
            typeCreator.CanCreate(typeof(SlimModel), null, Arg.Any<IBuildChain>()).Returns(true);
            typeCreator.CanPopulate(typeof(SlimModel), null, Arg.Any<IBuildChain>()).Returns(true);
            typeCreator.Create(typeof(SlimModel), null, Arg.Any<IExecuteStrategy>()).Returns(expected);
            typeCreator.Populate(expected, target).Returns(expected);
            typeCreator.AutoPopulate.Returns(true);
            generator.IsSupported(typeof(Guid), "Value", Arg.Is<IBuildChain>(x => x.Last == expected)).Returns(true);
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
            var typeCreators = new List<ITypeCreator>();

            var typeCreator = Substitute.For<ITypeCreator>();
            var buildStrategy = Substitute.For<IBuildStrategy>();

            typeCreators.Add(typeCreator);

            buildStrategy.TypeCreators.Returns(typeCreators.AsReadOnly());

            var target = new DefaultExecuteStrategy<Person>();

            target.Initialize(buildStrategy, buildStrategy.GetBuildLog());

            typeCreator.CanCreate(typeof(Person), null, Arg.Any<IBuildChain>()).Returns(true);
            typeCreator.CanPopulate(typeof(Person), null, Arg.Any<IBuildChain>()).Returns(true);
            typeCreator.Create(typeof(Person), null, Arg.Any<IExecuteStrategy>(), args).Returns(expected);
            typeCreator.Populate(expected, target).Returns(expected);
            typeCreator.AutoPopulate.Returns(false);

            var actual = target.Create(args);

            actual.Should().BeSameAs(expected);
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
            var enumerableTypeCreator = Substitute.For<ITypeCreator>();
            var valueGenerator = Substitute.For<IValueGenerator>();
            var propertyResolver = Substitute.For<IPropertyResolver>();

            typeCreators.Add(enumerableTypeCreator);
            typeCreators.Add(typeCreator);
            valueGenerators.Add(valueGenerator);

            buildStrategy.PropertyResolver.Returns(propertyResolver);
            propertyResolver.CanPopulate(Arg.Any<PropertyInfo>()).Returns(true);
            propertyResolver.ShouldPopulateProperty(
                Arg.Any<IBuildConfiguration>(),
                Arg.Any<object>(),
                Arg.Any<PropertyInfo>(),
                Arg.Any<object[]>()).Returns(true);
            buildStrategy.TypeCreators.Returns(typeCreators.AsReadOnly());
            buildStrategy.ValueGenerators.Returns(valueGenerators.AsReadOnly());

            var target = new DefaultExecuteStrategy<Company>();

            target.Initialize(buildStrategy, buildStrategy.GetBuildLog());

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

            var actual = (Company) target.Populate((object) expected);

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

        private class PopulateInstanceWrapper : DefaultExecuteStrategy<Company>
        {
            public void RunTest()
            {
                AutoPopulateInstance(null, null);
            }
        }

        private class Top
        {
            public Child Next { get; set; }

            public string Value { get; set; }
        }
    }
}