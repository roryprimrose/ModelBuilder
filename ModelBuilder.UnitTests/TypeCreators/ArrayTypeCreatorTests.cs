namespace ModelBuilder.UnitTests.TypeCreators
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using ModelBuilder.TypeCreators;
    using ModelBuilder.UnitTests.Models;
    using NSubstitute;
    using Xunit;
    using Xunit.Abstractions;

    public class ArrayTypeCreatorTests
    {
        private readonly ITestOutputHelper _output;

        public ArrayTypeCreatorTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData(typeof(string), false)]
        [InlineData(typeof(Stream), false)]
        [InlineData(typeof(int), false)]
        [InlineData(typeof(ReadOnlyCollection<int>), false)]
        [InlineData(typeof(IDictionary<string, int>), false)]
        [InlineData(typeof(Tuple<string, bool>), false)]
        [InlineData(typeof(Dictionary<string, int>), false)]
        [InlineData(typeof(IEnumerable<string>), false)]
        [InlineData(typeof(ICollection<string>), false)]
        [InlineData(typeof(Collection<string>), false)]
        [InlineData(typeof(IList<string>), false)]
        [InlineData(typeof(List<string>), false)]
        [InlineData(typeof(IReadOnlyCollection<int>), false)]
        [InlineData(typeof(IReadOnlyList<int>), false)]
        [InlineData(typeof(HashSet<string>), false)]
        [InlineData(typeof(LinkedList<string>), false)]
        [InlineData(typeof(ArraySegment<string>), false)]
        [InlineData(typeof(byte[]), true)]
        [InlineData(typeof(int[]), true)]
        [InlineData(typeof(Guid[]), true)]
        [InlineData(typeof(Person[]), true)]
        public void CanCreateReturnsWhetherTypeIsSupportedTest(Type type, bool supported)
        {
            var configuration = Substitute.For<IBuildConfiguration>();

            var sut = new ArrayTypeCreator();

            var actual = sut.CanCreate(configuration, null!, type);

            actual.Should().Be(supported);
        }

        [Fact]
        public void CanCreateThrowsExceptionWithNullParameter()
        {
            var configuration = Substitute.For<IBuildConfiguration>();

            var sut = new ArrayTypeCreator();

            Action action = () => sut.CanCreate(configuration, null!, (ParameterInfo) null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CanCreateThrowsExceptionWithNullProperty()
        {
            var configuration = Substitute.For<IBuildConfiguration>();

            var sut = new ArrayTypeCreator();

            Action action = () => sut.CanCreate(configuration, null!, (PropertyInfo) null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CanCreateThrowsExceptionWithNullType()
        {
            var configuration = Substitute.For<IBuildConfiguration>();

            var sut = new ArrayTypeCreator();

            Action action = () => sut.CanCreate(configuration, null!, (Type) null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(typeof(string), false)]
        [InlineData(typeof(Stream), false)]
        [InlineData(typeof(int), false)]
        [InlineData(typeof(ReadOnlyCollection<int>), false)]
        [InlineData(typeof(IDictionary<string, int>), false)]
        [InlineData(typeof(Tuple<string, bool>), false)]
        [InlineData(typeof(Dictionary<string, int>), false)]
        [InlineData(typeof(IEnumerable<string>), false)]
        [InlineData(typeof(ICollection<string>), false)]
        [InlineData(typeof(Collection<string>), false)]
        [InlineData(typeof(IList<string>), false)]
        [InlineData(typeof(List<string>), false)]
        [InlineData(typeof(IReadOnlyCollection<int>), false)]
        [InlineData(typeof(IReadOnlyList<int>), false)]
        [InlineData(typeof(HashSet<string>), false)]
        [InlineData(typeof(LinkedList<string>), false)]
        [InlineData(typeof(ArraySegment<string>), false)]
        [InlineData(typeof(byte[]), true)]
        [InlineData(typeof(int[]), true)]
        [InlineData(typeof(Guid[]), true)]
        [InlineData(typeof(Person[]), true)]
        public void CanPopulateReturnsWhetherTypeIsSupportedTest(Type type, bool supported)
        {
            _output.WriteLine("Testing " + type.FullName);

            var configuration = Substitute.For<IBuildConfiguration>();

            var sut = new ArrayTypeCreator();

            var actual = sut.CanPopulate(configuration, null!, type);

            actual.Should().Be(supported);
        }

        [Fact]
        public void CanPopulateThrowsExceptionWithNullParameter()
        {
            var configuration = Substitute.For<IBuildConfiguration>();

            var sut = new ArrayTypeCreator();

            Action action = () => sut.CanPopulate(configuration, null!, (ParameterInfo) null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CanPopulateThrowsExceptionWithNullProperty()
        {
            var configuration = Substitute.For<IBuildConfiguration>();

            var sut = new ArrayTypeCreator();

            Action action = () => sut.CanPopulate(configuration, null!, (PropertyInfo) null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CanPopulateThrowsExceptionWithNullType()
        {
            var configuration = Substitute.For<IBuildConfiguration>();

            var sut = new ArrayTypeCreator();

            Action action = () => sut.CanPopulate(configuration, null!, (Type) null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateChildItemThrowsExceptionWithNullExecuteStrategy()
        {
            var person = new Person();

            var sut = new ArrayTypeCreatorWrapper();

            Action action = () => sut.CreateItem(typeof(Person[]), null!, person);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateInstanceThrowsExceptionWithNullType()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new ArrayTypeCreatorWrapper();

            Action action = () => sut.RunCreateInstance(null!, null!, executeStrategy);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateReturnsBetweenMinAndMaxCountItems()
        {
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var typeResolver = Substitute.For<ITypeResolver>();
            var configuration = Substitute.For<IBuildConfiguration>();

            configuration.TypeResolver.Returns(typeResolver);
            typeResolver.GetBuildType(configuration, Arg.Any<Type>()).Returns(x => x.Arg<Type>());
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(configuration);

            var sut = new ArrayTypeCreator
            {
                MinCount = 5,
                MaxCount = 15
            };

            var actual = (Person[]) sut.Create(executeStrategy, typeof(Person[]))!;

            actual.Length.Should().BeGreaterOrEqualTo(sut.MinCount);
            actual.Length.Should().BeLessOrEqualTo(sut.MaxCount);
        }

        [Theory]
        [InlineData(typeof(byte[]))]
        [InlineData(typeof(int[]))]
        [InlineData(typeof(Guid[]))]
        [InlineData(typeof(Person[]))]
        public void CreateReturnsInstanceTest(Type type)
        {
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var typeResolver = Substitute.For<ITypeResolver>();
            var configuration = Substitute.For<IBuildConfiguration>();

            configuration.TypeResolver.Returns(typeResolver);
            typeResolver.GetBuildType(configuration, Arg.Any<Type>()).Returns(x => x.Arg<Type>());
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(configuration);

            var sut = new ArrayTypeCreator();

            var actual = sut.Create(executeStrategy, type);

            actual.Should().NotBeNull();
        }

        [Theory]
        [InlineData(typeof(string), false)]
        [InlineData(typeof(Stream), false)]
        [InlineData(typeof(int), false)]
        [InlineData(typeof(ReadOnlyCollection<int>), false)]
        [InlineData(typeof(IDictionary<string, int>), false)]
        [InlineData(typeof(Tuple<string, bool>), false)]
        [InlineData(typeof(Dictionary<string, int>), false)]
        [InlineData(typeof(IEnumerable<string>), false)]
        [InlineData(typeof(ICollection<string>), false)]
        [InlineData(typeof(Collection<string>), false)]
        [InlineData(typeof(IList<string>), false)]
        [InlineData(typeof(List<string>), false)]
        [InlineData(typeof(IReadOnlyCollection<int>), false)]
        [InlineData(typeof(IReadOnlyList<int>), false)]
        [InlineData(typeof(HashSet<string>), false)]
        [InlineData(typeof(LinkedList<string>), false)]
        [InlineData(typeof(ArraySegment<string>), false)]
        [InlineData(typeof(byte[]), true)]
        [InlineData(typeof(int[]), true)]
        [InlineData(typeof(Guid[]), true)]
        [InlineData(typeof(Person[]), true)]
        public void CreateValidatesWhetherTypeIsSupportedTest(Type type, bool supported)
        {
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var typeResolver = Substitute.For<ITypeResolver>();
            var configuration = Substitute.For<IBuildConfiguration>();

            configuration.TypeResolver.Returns(typeResolver);
            typeResolver.GetBuildType(configuration, Arg.Any<Type>()).Returns(x => x.Arg<Type>());
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(configuration);

            var sut = new ArrayTypeCreator();

            Action action = () => sut.Create(executeStrategy, type);

            if (supported)
            {
                action.Should().NotThrow();
            }
            else
            {
                action.Should().Throw<NotSupportedException>();
            }
        }

        [Fact]
        public void DisablesAutoConstructorDetection()
        {
            var sut = new ArrayTypeCreatorWrapper();

            sut.AutoDetectConstructor.Should().BeFalse();
        }

        [Fact]
        public void DisablesAutoPopulate()
        {
            var sut = new ArrayTypeCreatorWrapper();

            sut.AutoPopulate.Should().BeFalse();
        }

        [Fact]
        public void PopulateAddsItemsToCollectionFromExecuteStrategy()
        {
            var expected = new Guid[15];

            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var typeResolver = Substitute.For<ITypeResolver>();
            var configuration = Substitute.For<IBuildConfiguration>();

            configuration.TypeResolver.Returns(typeResolver);
            typeResolver.GetBuildType(configuration, Arg.Any<Type>()).Returns(x => x.Arg<Type>());
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(configuration);
            executeStrategy.Create(typeof(Guid)).Returns(Guid.NewGuid());

            var sut = new ArrayTypeCreator
            {
                MaxCount = 15
            };

            var actual = sut.Populate(executeStrategy, expected);

            actual.Should().BeSameAs(expected);

            var set = (Guid[]) actual;

            set.Should().HaveCount(sut.MaxCount);
            set.All(x => x != Guid.Empty).Should().BeTrue();
        }

        [Fact]
        public void PopulateAddsItemsToListFromExecuteStrategy()
        {
            var expected = new Guid[15];

            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Create(typeof(Guid)).Returns(Guid.NewGuid());

            var sut = new ArrayTypeCreator
            {
                MaxCount = 15
            };

            var actual = sut.Populate(executeStrategy, expected);

            actual.Should().BeSameAs(expected);

            var set = (Guid[]) actual;

            set.Should().HaveCount(sut.MaxCount);
            set.All(x => x != Guid.Empty).Should().BeTrue();
        }

        [Fact]
        public void PopulateCanAddItemsBasedOnPreviousItem()
        {
            var actual = new int[15];
            var executeStrategy = Model.UsingDefaultConfiguration()
                .UsingExecuteStrategy<DefaultExecuteStrategy<List<Company>>>();

            var sut = new IncrementingArrayTypeCreator();

            var result = (int[]) sut.Populate(executeStrategy, actual);

            var baseValue = result[0];
            var expected = new int[actual.Length];

            for (var index = 0; index < expected.Length; index++)
            {
                expected[index] = baseValue + index;
            }

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void PopulateInfersItemTypeByArrayTypeWhenFirstItemIsNull()
        {
            var actual = new Person[15];
            var executeStrategy = Model.UsingDefaultConfiguration()
                .UsingExecuteStrategy<DefaultExecuteStrategy<List<Company>>>();

            var sut = new ArrayTypeCreator();

            var result = (Person[]) sut.Populate(executeStrategy, actual);

            result.All(x => x != null!).Should().BeTrue();
        }

        [Fact]
        public void PopulateInstanceThrowsExceptionWithNullExecuteStrategy()
        {
            var value = new List<string>();

            var sut = new ArrayTypeCreatorWrapper();

            Action action = () => sut.RunPopulateInstance(value, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PopulateInstanceThrowsExceptionWithNullInstance()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new ArrayTypeCreatorWrapper();

            Action action = () => sut.RunPopulateInstance(null!, executeStrategy);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        [SuppressMessage(
            "Microsoft.Design",
            "CA1825",
            Justification = "The Array.Empty<T> is not available on net452.")]
        public void PopulateReturnsEmptyArrayWhenSourceHasZeroLength()
        {
            var expected = new Guid[0];

            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Create(typeof(Guid)).Returns(Guid.NewGuid());

            var sut = new ArrayTypeCreator
            {
                MaxCount = 15
            };

            var actual = sut.Populate(executeStrategy, expected);

            actual.Should().BeSameAs(expected);

            var set = (Guid[]) actual;

            set.Should().BeEmpty();
        }

        [Fact]
        public void PopulateThrowsExceptionWithNullInstance()
        {
            var strategy = Substitute.For<IExecuteStrategy>();

            var sut = new ArrayTypeCreator();

            Action action = () => sut.Populate(strategy, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PopulateThrowsExceptionWithNullStrategy()
        {
            var instance = new List<string>();

            var sut = new ArrayTypeCreator();

            Action action = () => sut.Populate(null!, instance);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PopulateThrowsExceptionWithUnsupportedType()
        {
            var instance = new Lazy<bool>(() => true);

            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new ArrayTypeCreator();

            Action action = () => sut.Populate(executeStrategy, instance);

            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void PriorityReturnsHigherThanDefaultTypeCreator()
        {
            var sut = new ArrayTypeCreator();
            var other = new DefaultTypeCreator();

            sut.Priority.Should().BeGreaterThan(other.Priority);
        }

        private class ArrayTypeCreatorWrapper : ArrayTypeCreator
        {
            public void CreateItem(Type type, IExecuteStrategy executeStrategy, object item)
            {
                CreateChildItem(type, executeStrategy, item);
            }

            public void RunCreateInstance(Type type, string referenceName, IExecuteStrategy executeStrategy,
                params object?[]? args)
            {
                base.CreateInstance(executeStrategy, type, referenceName, args);
            }

            public object RunPopulateInstance(object instance, IExecuteStrategy executeStrategy)
            {
                return base.PopulateInstance(executeStrategy, instance);
            }
        }
    }
}