﻿namespace ModelBuilder.UnitTests.TypeCreators
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using System.Net.NetworkInformation;
    using FluentAssertions;
    using ModelBuilder.TypeCreators;
    using ModelBuilder.UnitTests.Models;
    using NSubstitute;
    using Xunit;

    public class EnumerableTypeCreatorTests
    {
        [Fact]
        public void AutoDetectConstructorReturnsFalseTest()
        {
            var target = new EnumerableTypeCreator();

            target.AutoDetectConstructor.Should().BeFalse();
        }

        [Fact]
        public void AutoPopulateReturnsFalseTest()
        {
            var target = new EnumerableTypeCreator();

            target.AutoPopulate.Should().BeFalse();
        }

        [Theory]
        [InlineData(typeof(string), false)]
        [InlineData(typeof(Stream), false)]
        [InlineData(typeof(int), false)]
        [InlineData(typeof(ReadOnlyCollection<int>), false)]
        [InlineData(typeof(IDictionary<string, int>), false)]
        [InlineData(typeof(Tuple<string, bool>), false)]
        [InlineData(typeof(AbstractCollection<Person>), false)]
        [InlineData(typeof(IReadOnlyCollection<int>), false)]
        [InlineData(typeof(IReadOnlyList<int>), false)]
        [InlineData(typeof(ArraySegment<string>), false)]
        [InlineData(typeof(IPAddressCollection), false)]
        [InlineData(typeof(GatewayIPAddressInformationCollection), false)]
        [InlineData(typeof(IPAddressInformationCollection), false)]
        [InlineData(typeof(MulticastIPAddressInformationCollection), false)]
        [InlineData(typeof(UnicastIPAddressInformationCollection), false)]
        [InlineData(typeof(Dictionary<,>.KeyCollection), false)]
        [InlineData(typeof(Dictionary<,>.ValueCollection), false)]
        [InlineData(typeof(Dictionary<string, int>), true)]
        [InlineData(typeof(SortedDictionary<,>.KeyCollection), false)]
        [InlineData(typeof(SortedDictionary<,>.ValueCollection), false)]
        [InlineData(typeof(IEnumerable<string>), true)]
        [InlineData(typeof(ICollection<string>), true)]
        [InlineData(typeof(Collection<string>), true)]
        [InlineData(typeof(IList<string>), true)]
        [InlineData(typeof(List<string>), true)]
        [InlineData(typeof(HashSet<string>), true)]
        [InlineData(typeof(LinkedList<string>), true)]
        [InlineData(typeof(InheritedGenericCollection), true)]
        public void CanCreateReturnsWhetherTypeIsSupportedTest(Type type, bool supported)
        {
            var target = new EnumerableTypeCreator();

            var actual = target.CanCreate(type, null, null);

            actual.Should().Be(supported);
        }

        [Fact]
        public void CanCreateThrowsExceptionWithNullTypeTest()
        {
            var target = new EnumerableTypeCreator();

            Action action = () => target.CanCreate(null, null, null);

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
        public void CanPopulateReturnsWhetherTypeIsSupportedTest(Type type, bool supported)
        {
            var target = new EnumerableTypeCreator();

            var actual = target.CanPopulate(type, null, null);

            actual.Should().Be(supported);
        }

        [Fact]
        public void CanPopulateThrowsExceptionWithNullTypeTest()
        {
            var target = new EnumerableTypeCreator();

            Action action = () => target.CanPopulate(null, null, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateChildItemThrowsExceptionWithNullExecuteStrategyTest()
        {
            var person = new Person();

            var target = new EnumerableTypeCreatorWrapper();

            Action action = () => target.CreateItem(typeof(Person), null, person);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateDoesNotPopulateListTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new IncrementingEnumerableTypeCreator();

            var result = (IList<int>) target.Create(typeof(IList<int>), null, executeStrategy);

            result.Should().BeEmpty();
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
        public void CreateReturnsInstanceTest(Type type)
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new EnumerableTypeCreator();

            var actual = target.Create(type, null, executeStrategy);

            actual.Should().NotBeNull();
        }

        [Theory]
        [InlineData(typeof(IEnumerable<int>))]
        [InlineData(typeof(ICollection<int>))]
        [InlineData(typeof(IList<int>))]
        public void CreateReturnsNewListOfSpecifiedTypeTest(Type targetType)
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new EnumerableTypeCreator();

            var actual = target.Create(targetType, null, executeStrategy);

            actual.Should().BeOfType<List<int>>();
            actual.As<List<int>>().Should().BeEmpty();
        }

        [Theory]
        [InlineData(typeof(string), false)]
        [InlineData(typeof(Stream), false)]
        [InlineData(typeof(int), false)]
        [InlineData(typeof(ReadOnlyCollection<int>), false)]
        [InlineData(typeof(IDictionary<string, int>), false)]
        [InlineData(typeof(Tuple<string, bool>), false)]
        [InlineData(typeof(AbstractCollection<Person>), false)]
        [InlineData(typeof(IReadOnlyCollection<int>), false)]
        [InlineData(typeof(IReadOnlyList<int>), false)]
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
        [InlineData(typeof(Dictionary<string, int>), true)]
        [InlineData(typeof(IEnumerable<string>), true)]
        [InlineData(typeof(ICollection<string>), true)]
        [InlineData(typeof(Collection<string>), true)]
        [InlineData(typeof(IList<string>), true)]
        [InlineData(typeof(List<string>), true)]
        [InlineData(typeof(LinkedList<string>), true)]
        [InlineData(typeof(HashSet<string>), true)]
        [InlineData(typeof(InheritedGenericCollection), true)]
        public void CreateValidatesWhetherTypeIsSupportedTest(Type type, bool supported)
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new EnumerableTypeCreator();

            Action action = () => target.Create(type, null, executeStrategy);

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
        public void PopulateAddsItemsToCollectionFromExecuteStrategyTest()
        {
            var expected = new Collection<Guid>();

            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Create(typeof(Guid)).Returns(Guid.NewGuid());

            var target = new EnumerableTypeCreator
            {
                AutoPopulateCount = 15
            };

            var actual = target.Populate(expected, executeStrategy);

            actual.Should().BeSameAs(expected);

            var set = (Collection<Guid>) actual;

            set.Should().HaveCount(target.AutoPopulateCount);
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

            var target = new EnumerableTypeCreator();

            var executeStrategy = new DefaultExecuteStrategy();

            executeStrategy.Initialize(configuration);

            var actual = target.Create(type, null, executeStrategy);

            target.Populate(actual, executeStrategy);

            var converted = (IEnumerable) actual;

            converted.Should().NotBeEmpty();
        }

        [Fact]
        public void PopulateAddsItemsToListFromExecuteStrategyTest()
        {
            var expected = new List<Guid>();

            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Create(typeof(Guid)).Returns(Guid.NewGuid());

            var target = new EnumerableTypeCreator
            {
                AutoPopulateCount = 15
            };

            var actual = target.Populate(expected, executeStrategy);

            actual.Should().BeSameAs(expected);

            var set = (List<Guid>) actual;

            set.Should().HaveCount(target.AutoPopulateCount);
            set.All(x => x != Guid.Empty).Should().BeTrue();
        }

        [Fact]
        public void PopulateCanAddItemsBasedOnPreviousItemTest()
        {
            var actual = new List<int>();
            var executeStrategy = Model.UsingDefaultConfiguration().UsingExecuteStrategy<DefaultExecuteStrategy<List<int>>>();

            var target = new IncrementingEnumerableTypeCreator();

            var result = (List<int>) target.Populate(actual, executeStrategy);

            var baseValue = result[0];
            var expected = new List<int>(target.AutoPopulateCount);

            for (var index = 0; index < target.AutoPopulateCount; index++)
            {
                expected.Add(baseValue + index);
            }

            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void PopulateThrowsExceptionWithUnsupportedTypeTest()
        {
            var instance = new Lazy<bool>(() => true);

            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new EnumerableTypeCreator();

            Action action = () => target.Populate(instance, executeStrategy);

            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void PriorityReturnsHigherThanDefaultTypeCreatorTest()
        {
            var target = new EnumerableTypeCreator();
            var other = new DefaultTypeCreator();

            target.Priority.Should().BeGreaterThan(other.Priority);
        }

        [Fact]
        public void SettingAutoPopulateCountShouldNotChangeDefaultAutoPopulateCountTest()
        {
            var target = new EnumerableTypeCreator
            {
                AutoPopulateCount = Environment.TickCount
            };

            EnumerableTypeCreator.DefaultAutoPopulateCount.Should().NotBe(target.AutoPopulateCount);
        }

        [Fact]
        public void SettingDefaultAutoPopulateCountOnlyAffectsNewInstancesTest()
        {
            var expected = EnumerableTypeCreator.DefaultAutoPopulateCount;

            try
            {
                var first = new EnumerableTypeCreator();

                EnumerableTypeCreator.DefaultAutoPopulateCount = 11;

                var second = new EnumerableTypeCreator();

                first.AutoPopulateCount.Should().Be(expected);
                second.AutoPopulateCount.Should().Be(11);
            }
            finally
            {
                EnumerableTypeCreator.DefaultAutoPopulateCount = expected;
            }
        }

        private class EnumerableTypeCreatorWrapper : EnumerableTypeCreator
        {
            public void CreateItem(Type type, IExecuteStrategy executeStrategy, object item)
            {
                CreateChildItem(type, executeStrategy, item);
            }
        }
    }
}