namespace ModelBuilder.UnitTests.TypeCreators
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Net.NetworkInformation;
    using System.Reflection;
    using FluentAssertions;
    using ModelBuilder.TypeCreators;
    using ModelBuilder.UnitTests.Models;
    using NSubstitute;
    using Xunit;

    public class EnumerableTypeCreatorTests
    {
        [Fact]
        public void AutoDetectConstructorReturnsFalse()
        {
            var sut = new EnumerableTypeCreator();

            sut.AutoDetectConstructor.Should().BeFalse();
        }

        [Fact]
        public void AutoPopulateReturnsFalse()
        {
            var sut = new EnumerableTypeCreator();

            sut.AutoPopulate.Should().BeFalse();
        }

        [Theory]
        [InlineData(typeof(string), false)]
        [InlineData(typeof(Stream), false)]
        [InlineData(typeof(int), false)]
        [InlineData(typeof(int?), false)]
        [InlineData(typeof(Tuple<string, bool>), false)]
        [InlineData(typeof(ICollection), false)]
        [InlineData(typeof(IList), false)]
        [InlineData(typeof(ArrayList), false)]
        [InlineData(typeof(AbstractCollection<Person>), false)]
        [InlineData(typeof(ArraySegment<string>), false)]
        [InlineData(typeof(IPAddressCollection), false)]
        [InlineData(typeof(GatewayIPAddressInformationCollection), false)]
        [InlineData(typeof(IPAddressInformationCollection), false)]
        [InlineData(typeof(MulticastIPAddressInformationCollection), false)]
        [InlineData(typeof(UnicastIPAddressInformationCollection), false)]
        [InlineData(typeof(Dictionary<,>.KeyCollection), false)]
        [InlineData(typeof(Dictionary<,>.ValueCollection), false)]
        [InlineData(typeof(ReadOnlyDictionary<string, int>), false)]
        [InlineData(typeof(ReadOnlyCollection<int>), false)]
        [InlineData(typeof(SortedDictionary<,>.KeyCollection), false)]
        [InlineData(typeof(SortedDictionary<,>.ValueCollection), false)]
        [InlineData(typeof(IInterfaceCollection<Person>), false)]
        [InlineData(typeof(IEnumerable<string>), true)]
        [InlineData(typeof(ICollection<string>), true)]
        [InlineData(typeof(IList<string>), true)]
        [InlineData(typeof(IList<KeyValuePair<string, Guid>>), true)]
        [InlineData(typeof(List<KeyValuePair<string, Guid>>), true)]
        [InlineData(typeof(List<MultipleGenericArguments<string, Guid>>), true)]
        [InlineData(typeof(IList<MultipleGenericArguments<string, Guid>>), true)]
        [InlineData(typeof(Collection<KeyValuePair<string, Guid>>), true)]
        [InlineData(typeof(IReadOnlyCollection<int>), true)]
        [InlineData(typeof(IReadOnlyList<int>), true)]
        [InlineData(typeof(IDictionary<string, int>), true)]
        [InlineData(typeof(Collection<string>), true)]
        [InlineData(typeof(List<string>), true)]
        [InlineData(typeof(HashSet<string>), true)]
        [InlineData(typeof(Dictionary<string, int>), true)]
        [InlineData(typeof(LinkedList<string>), true)]
        [InlineData(typeof(InheritedGenericCollection), true)]
        public void CanCreateReturnsWhetherTypeIsSupportedTest(Type type, bool supported)
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var configuration = Substitute.For<IBuildConfiguration>();

            executeStrategy.Configuration.Returns(configuration);

            var sut = new EnumerableTypeCreator();

            var actual = sut.CanCreate(configuration, null!, type);

            actual.Should().Be(supported);
        }

        [Fact]
        public void CanCreateThrowsExceptionWithNullParameter()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var configuration = Substitute.For<IBuildConfiguration>();

            executeStrategy.Configuration.Returns(configuration);

            var sut = new EnumerableTypeCreator();

            Action action = () => sut.CanCreate(configuration, null!, (ParameterInfo) null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CanCreateThrowsExceptionWithNullProperty()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var configuration = Substitute.For<IBuildConfiguration>();

            executeStrategy.Configuration.Returns(configuration);

            var sut = new EnumerableTypeCreator();

            Action action = () => sut.CanCreate(configuration, null!, (PropertyInfo) null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CanCreateThrowsExceptionWithNullType()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var configuration = Substitute.For<IBuildConfiguration>();

            executeStrategy.Configuration.Returns(configuration);

            var sut = new EnumerableTypeCreator();

            Action action = () => sut.CanCreate(configuration, null!, (Type) null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(typeof(string), false)]
        [InlineData(typeof(Stream), false)]
        [InlineData(typeof(int), false)]
        [InlineData(typeof(ReadOnlyCollection<int>), false)]
        [InlineData(typeof(Tuple<string, bool>), false)]
        [InlineData(typeof(IReadOnlyCollection<int>), false)]
        [InlineData(typeof(IReadOnlyList<int>), false)]
        [InlineData(typeof(IEnumerable<string>), false)]
        [InlineData(typeof(ArraySegment<string>), false)]
        [InlineData(typeof(IPAddressCollection), false)]
        [InlineData(typeof(GatewayIPAddressInformationCollection), false)]
        [InlineData(typeof(IPAddressInformationCollection), false)]
        [InlineData(typeof(MulticastIPAddressInformationCollection), false)]
        [InlineData(typeof(UnicastIPAddressInformationCollection), false)]
        [InlineData(typeof(Dictionary<,>.KeyCollection), false)]
        [InlineData(typeof(Dictionary<,>.ValueCollection), false)]
        [InlineData(typeof(SortedDictionary<,>.KeyCollection), false)]
        [InlineData(typeof(SortedDictionary<,>.ValueCollection), false)]
        [InlineData(typeof(AbstractCollection<Person>), true)]
        [InlineData(typeof(IDictionary<string, int>), true)]
        [InlineData(typeof(Dictionary<string, int>), true)]
        [InlineData(typeof(ICollection<string>), true)]
        [InlineData(typeof(Collection<string>), true)]
        [InlineData(typeof(IList<string>), true)]
        [InlineData(typeof(List<string>), true)]
        [InlineData(typeof(HashSet<string>), true)]
        [InlineData(typeof(LinkedList<string>), true)]
        [InlineData(typeof(InheritedGenericCollection), true)]
        [InlineData(typeof(IList<KeyValuePair<string, Guid>>), true)]
        [InlineData(typeof(List<KeyValuePair<string, Guid>>), true)]
        [InlineData(typeof(List<MultipleGenericArguments<string, Guid>>), true)]
        [InlineData(typeof(IList<MultipleGenericArguments<string, Guid>>), true)]
        [InlineData(typeof(Collection<KeyValuePair<string, Guid>>), true)]
        public void CanPopulateReturnsWhetherTypeIsSupportedTest(Type type, bool supported)
        {
            var configuration = Substitute.For<IBuildConfiguration>();

            var sut = new EnumerableTypeCreator();

            var actual = sut.CanPopulate(configuration, null!, type);

            actual.Should().Be(supported);
        }

        [Fact]
        public void CanPopulateThrowsExceptionWithNullConfiguration()
        {
            var sut = new EnumerableTypeCreator();

            Action action = () => sut.CanPopulate(null!, null!, typeof(string));

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CanPopulateThrowsExceptionWithNullParameter()
        {
            var sut = new EnumerableTypeCreator();
            var configuration = Substitute.For<IBuildConfiguration>();

            Action action = () => sut.CanPopulate(configuration, null!, (ParameterInfo) null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CanPopulateThrowsExceptionWithNullProperty()
        {
            var sut = new EnumerableTypeCreator();
            var configuration = Substitute.For<IBuildConfiguration>();

            Action action = () => sut.CanPopulate(configuration, null!, (PropertyInfo) null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CanPopulateThrowsExceptionWithNullType()
        {
            var sut = new EnumerableTypeCreator();
            var configuration = Substitute.For<IBuildConfiguration>();

            Action action = () => sut.CanPopulate(configuration, null!, (Type) null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateByNameThrowsExceptionWithNullExecuteStrategy()
        {
            var sut = new EnumerableTypeCreatorWrapper();

            Action action = () => sut.CreateByName(null!, typeof(string), null!, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateByNameThrowsExceptionWithNullType()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new EnumerableTypeCreatorWrapper();

            Action action = () => sut.CreateByName(executeStrategy, null!, null!, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateChildItemThrowsExceptionWithNullExecuteStrategy()
        {
            var person = new Person();

            var sut = new EnumerableTypeCreatorWrapper();

            Action action = () => sut.CreateItem(typeof(Person), null!, person);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateInstanceThrowsExceptionWithNullType()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new EnumerableTypeCreatorWrapper();

            Action action = () => sut.CreateValue(executeStrategy, null!, null!, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(typeof(IEnumerable<string>), typeof(List<string>))]
        [InlineData(typeof(ICollection<string>), typeof(List<string>))]
        [InlineData(typeof(Collection<string>), typeof(Collection<string>))]
        [InlineData(typeof(IList<string>), typeof(List<string>))]
        [InlineData(typeof(List<string>), typeof(List<string>))]
        [InlineData(typeof(LinkedList<string>), typeof(LinkedList<string>))]
        [InlineData(typeof(InheritedGenericCollection), typeof(InheritedGenericCollection))]
        [InlineData(typeof(HashSet<string>), typeof(HashSet<string>))]
        [InlineData(typeof(IList<KeyValuePair<string, Guid>>), typeof(List<KeyValuePair<string, Guid>>))]
        [InlineData(typeof(List<KeyValuePair<string, Guid>>), typeof(List<KeyValuePair<string, Guid>>))]
        [InlineData(typeof(List<MultipleGenericArguments<string, Guid>>),
            typeof(List<MultipleGenericArguments<string, Guid>>))]
        [InlineData(typeof(IList<MultipleGenericArguments<string, Guid>>),
            typeof(List<MultipleGenericArguments<string, Guid>>))]
        [InlineData(typeof(Collection<KeyValuePair<string, Guid>>), typeof(Collection<KeyValuePair<string, Guid>>))]
        [InlineData(typeof(ICollection<KeyValuePair<string, Guid>>), typeof(Dictionary<string, Guid>))]
        [InlineData(typeof(IReadOnlyCollection<int>), typeof(List<int>))]
        [InlineData(typeof(IReadOnlyList<int>), typeof(List<int>))]
        [InlineData(typeof(IDictionary<string, Person>), typeof(Dictionary<string, Person>))]
        [InlineData(typeof(Dictionary<string, Person>), typeof(Dictionary<string, Person>))]
        public void CreateReturnsInstanceOfMostAppropriateTypeTest(Type requestedType, Type expectedType)
        {
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var configuration = Substitute.For<IBuildConfiguration>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(configuration);

            var sut = new EnumerableTypeCreator();

            var actual = sut.Create(executeStrategy, requestedType);

            actual.Should().NotBeNull();
            actual.Should().BeOfType(expectedType);
            actual.Should().BeAssignableTo(requestedType);
            actual.As<IEnumerable>().Should().BeEmpty();
        }

        [Fact]
        public void PopulateAddsItemsToCollectionFromExecuteStrategy()
        {
            var expected = new Collection<Guid>();
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var configuration = Substitute.For<IBuildConfiguration>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(configuration);

            executeStrategy.Create(typeof(Guid)).Returns(Guid.NewGuid());

            var sut = new EnumerableTypeCreator
            {
                MinCount = 5,
                MaxCount = 15
            };

            var actual = sut.Populate(executeStrategy, expected);

            actual.Should().BeSameAs(expected);

            var set = (Collection<Guid>) actual;

            set.Count.Should().BeGreaterOrEqualTo(sut.MinCount);
            set.Count.Should().BeLessOrEqualTo(sut.MaxCount);
            set.All(x => x != Guid.Empty).Should().BeTrue();
        }

        [Theory]
        [InlineData(typeof(Dictionary<string, int>))]
        [InlineData(typeof(IEnumerable<string>))]
        [InlineData(typeof(ICollection<string>))]
        [InlineData(typeof(Collection<string>))]
        [InlineData(typeof(IList<string>))]
        [InlineData(typeof(List<string>))]
        [InlineData(typeof(LinkedList<string>))]
        [InlineData(typeof(InheritedGenericCollection))]
        public void PopulateAddsItemsToInstancesTest(Type type)
        {
            var configuration = Model.UsingDefaultConfiguration();

            var sut = new EnumerableTypeCreator();

            var executeStrategy = new DefaultExecuteStrategy();

            executeStrategy.Initialize(configuration);

            var actual = sut.Create(executeStrategy, type)!;

            sut.Populate(executeStrategy, actual);

            var converted = (IEnumerable) actual;

            converted.Should().NotBeEmpty();
        }

        [Fact]
        public void PopulateAddsItemsToListFromExecuteStrategy()
        {
            var expected = new List<Guid>();

            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var configuration = Substitute.For<IBuildConfiguration>();

            executeStrategy.Configuration.Returns(configuration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Create(typeof(Guid)).Returns(Guid.NewGuid());

            var sut = new EnumerableTypeCreator();

            var actual = sut.Populate(executeStrategy, expected);

            actual.Should().BeSameAs(expected);

            var set = (List<Guid>) actual;

            set.Count.Should().BeGreaterOrEqualTo(sut.MinCount);
            set.Count.Should().BeLessOrEqualTo(sut.MaxCount);
            set.All(x => x != Guid.Empty).Should().BeTrue();
        }

        [Fact]
        public void PopulateCanAddItemsBasedOnPreviousItem()
        {
            var actual = new List<int>();
            var executeStrategy = Model.UsingDefaultConfiguration()
                .UsingExecuteStrategy<DefaultExecuteStrategy<List<int>>>();

            var sut = new IncrementingEnumerableTypeCreator();

            var result = (List<int>) sut.Populate(executeStrategy, actual);

            var baseValue = result[0];
            var expected = new List<int>(result.Count);

            for (var index = 0; index < result.Count; index++)
            {
                expected.Add(baseValue + index);
            }

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void PopulateThrowsExceptionWithUnsupportedType()
        {
            var instance = new Lazy<bool>(() => true);

            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var configuration = Substitute.For<IBuildConfiguration>();

            executeStrategy.Configuration.Returns(configuration);
            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new EnumerableTypeCreator();

            Action action = () => sut.Populate(executeStrategy, instance);

            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void PriorityReturnsHigherThanDefaultTypeCreator()
        {
            var sut = new EnumerableTypeCreator();
            var other = new DefaultTypeCreator();

            sut.Priority.Should().BeGreaterThan(other.Priority);
        }

        private class EnumerableTypeCreatorWrapper : EnumerableTypeCreator
        {
            public object? CreateByName(IExecuteStrategy executeStrategy, Type type, string? referenceName,
                params object?[]? args)
            {
                return Create(executeStrategy, type, referenceName, args);
            }

            public void CreateItem(Type type, IExecuteStrategy executeStrategy, object item)
            {
                CreateChildItem(type, executeStrategy, item);
            }

            public object? CreateValue(IExecuteStrategy executeStrategy, Type type, string? referenceName,
                params object?[]? args)
            {
                return CreateInstance(executeStrategy, type, referenceName, args);
            }
        }
    }
}