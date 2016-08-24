namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using FluentAssertions;
    using NSubstitute;
    using Xunit;
    using Xunit.Abstractions;

    public class DefaultExecuteStrategyTTests
    {
        private readonly ITestOutputHelper _output;

        public DefaultExecuteStrategyTTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void CreateWithReturnsDefaultValueWhenInstanceFailsToBeCreatedTest()
        {
            var typeCreators = new List<ITypeCreator>();

            var typeCreator = Substitute.For<ITypeCreator>();
            var buildStrategy = Substitute.For<IBuildStrategy>();

            typeCreators.Add(typeCreator);

            buildStrategy.TypeCreators.Returns(typeCreators.AsReadOnly());
            typeCreator.CanCreate(typeof(SlimModel), null, Arg.Any<LinkedList<object>>()).Returns(true);
            typeCreator.CanPopulate(typeof(SlimModel), null, Arg.Any<LinkedList<object>>()).Returns(true);

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

            buildStrategy.TypeCreators.Returns(typeCreators.AsReadOnly());
            typeCreator.CanCreate(typeof(Stream), null, Arg.Any<LinkedList<object>>()).Returns(true);
            typeCreator.CanPopulate(typeof(Stream), null, Arg.Any<LinkedList<object>>()).Returns(true);
            typeCreator.Create(typeof(Stream), null, Arg.Any<LinkedList<object>>()).Returns(null);

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

            buildStrategy.ValueGenerators.Returns(valueGenerators.AsReadOnly());
            valueGenerator.IsSupported(typeof(int), null, Arg.Any<LinkedList<object>>()).Returns(true);
            valueGenerator.Generate(typeof(int), null, Arg.Any<LinkedList<object>>()).Returns(null);

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

            buildStrategy.TypeCreators.Returns(typeCreators.AsReadOnly());
            buildStrategy.ValueGenerators.Returns(valueGenerators.AsReadOnly());

            var target = new DefaultExecuteStrategy<SlimModel>
            {
                BuildStrategy = buildStrategy
            };

            typeCreator.CanCreate(typeof(SlimModel), null, Arg.Any<LinkedList<object>>()).Returns(true);
            typeCreator.CanPopulate(typeof(SlimModel), null, Arg.Any<LinkedList<object>>()).Returns(true);
            typeCreator.Create(typeof(SlimModel), null, Arg.Any<LinkedList<object>>()).Returns(expected);
            typeCreator.Populate(expected, target).Returns(expected);
            typeCreator.AutoPopulate.Returns(true);
            generator.IsSupported(typeof(Guid), "Value", Arg.Is<LinkedList<object>>(x => x.Last.Value == expected))
                .Returns(true);
            generator.Generate(typeof(Guid), "Value", Arg.Is<LinkedList<object>>(x => x.Last.Value == expected))
                .Returns(value);

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

            buildStrategy.TypeCreators.Returns(typeCreators.AsReadOnly());

            var target = new DefaultExecuteStrategy<Person>
            {
                BuildStrategy = buildStrategy
            };

            typeCreator.CanCreate(typeof(Person), null, Arg.Any<LinkedList<object>>()).Returns(true);
            typeCreator.CanPopulate(typeof(Person), null, Arg.Any<LinkedList<object>>()).Returns(true);
            typeCreator.Create(typeof(Person), null, Arg.Any<LinkedList<object>>(), args).Returns(expected);
            typeCreator.Populate(expected, target).Returns(expected);
            typeCreator.AutoPopulate.Returns(false);

            var actual = target.CreateWith(args);

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
            var valueGenerator = Substitute.For<IValueGenerator>();

            typeCreators.Add(typeCreator);
            valueGenerators.Add(valueGenerator);

            buildStrategy.TypeCreators.Returns(typeCreators.AsReadOnly());
            buildStrategy.ValueGenerators.Returns(valueGenerators.AsReadOnly());

            var target = new DefaultExecuteStrategy<Company>
            {
                BuildStrategy = buildStrategy
            };

            typeCreator.CanCreate(
                typeof(IEnumerable<Person>),
                "Staff",
                Arg.Is<LinkedList<object>>(x => x.Last.Value == expected)).Returns(true);
            typeCreator.CanPopulate(
                typeof(IEnumerable<Person>),
                "Staff",
                Arg.Is<LinkedList<object>>(x => x.Last.Value == expected)).Returns(true);
            typeCreator.Create(
                typeof(IEnumerable<Person>),
                "Staff",
                Arg.Is<LinkedList<object>>(x => x.Last.Value == expected)).Returns(staff);
            typeCreator.Populate(staff, target).Returns(staff);
            valueGenerator.IsSupported(
                typeof(string),
                "Name",
                Arg.Is<LinkedList<object>>(x => x.Last.Value == expected)).Returns(true);
            valueGenerator.Generate(typeof(string), "Name", Arg.Is<LinkedList<object>>(x => x.Last.Value == expected))
                .Returns(name);
            valueGenerator.IsSupported(
                typeof(string),
                "Address",
                Arg.Is<LinkedList<object>>(x => x.Last.Value == expected)).Returns(true);
            valueGenerator.Generate(
                typeof(string),
                "Address",
                Arg.Is<LinkedList<object>>(x => x.Last.Value == expected)).Returns(address);

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

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void PopulateThrowsExceptionWithNullInstanceTest()
        {
            var target = new DefaultExecuteStrategy<Person>();

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

        private class PopulateInstanceWrapper : DefaultExecuteStrategy<Company>
        {
            public void RunTest()
            {
                PopulateInstance(null);
            }
        }

        private class Top
        {
            public Child Next { get; set; }

            public string Value { get; set; }
        }
    }
}