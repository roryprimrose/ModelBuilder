using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace ModelBuilder.UnitTests
{
    public class EnumerableTypeCreatorTests
    {
        [Fact]
        public void CreateChildItemThrowsExceptionWithNullExecuteStrategyTest()
        {
            var person = new Person();

            var target = new EnumerableTypeCreatorWrapper();

            Action action = () => target.CreateItem(typeof(Person), null, person);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void CreateDoesNotPopulateListTest()
        {
            var target = new IncrementingEnumerableTypeCreator();

            var result = (IList<int>) target.Create(typeof(IList<int>));

            result.Should().BeEmpty();
        }

        [Theory]
        [InlineData(typeof(Dictionary<string, int>))]
        [InlineData(typeof(IEnumerable<string>))]
        [InlineData(typeof(ICollection<string>))]
        [InlineData(typeof(IList<string>))]
        [InlineData(typeof(Collection<string>))]
        [InlineData(typeof(List<string>))]
        [InlineData(typeof(HashSet<string>))]
        [InlineData(typeof(LinkedList<string>))]
        [InlineData(typeof(ArraySegment<string>))]
        public void CreateReturnsInstanceTest(Type type)
        {
            var target = new EnumerableTypeCreator();

            var actual = target.Create(type, null, null);

            actual.Should().NotBeNull();
        }

        [Fact]
        public void CreateReturnsNewListOfSpecfiedTypeTest()
        {
            var target = new EnumerableTypeCreator();

            var actual = target.Create(typeof(IEnumerable<int>), null, null);

            actual.Should().BeOfType<List<int>>();
            actual.As<List<int>>().Should().BeEmpty();
        }

        [Fact]
        public void CreateThrowsExceptionWithNullTypeTest()
        {
            var target = new EnumerableTypeCreator();

            Action action = () => target.Create(null, null, null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Theory]
        [InlineData(typeof(string), false)]
        [InlineData(typeof(Stream), false)]
        [InlineData(typeof(int), false)]
        [InlineData(typeof(ReadOnlyCollection<int>), false)]
        [InlineData(typeof(IDictionary<string, int>), false)]
        [InlineData(typeof(Tuple<string, bool>), false)]
        [InlineData(typeof(Dictionary<string, int>), true)]
        [InlineData(typeof(IEnumerable<string>), true)]
        [InlineData(typeof(ICollection<string>), true)]
        [InlineData(typeof(Collection<string>), true)]
        [InlineData(typeof(IList<string>), true)]
        [InlineData(typeof(List<string>), true)]
        [InlineData(typeof(IReadOnlyCollection<int>), true)]
        [InlineData(typeof(IReadOnlyList<int>), true)]
        [InlineData(typeof(HashSet<string>), true)]
        [InlineData(typeof(LinkedList<string>), true)]
        [InlineData(typeof(ArraySegment<string>), true)]
        public void CreateValidatesWhetherTypeIsSupportedTest(Type type, bool supported)
        {
            var target = new EnumerableTypeCreator();

            Action action = () => target.Create(type, null, null);

            if (supported)
            {
                action.ShouldNotThrow();
            }
            else
            {
                action.ShouldThrow<NotSupportedException>();
            }
        }

        [Theory]
        [InlineData(typeof(string), false)]
        [InlineData(typeof(Stream), false)]
        [InlineData(typeof(int), false)]
        [InlineData(typeof(ReadOnlyCollection<int>), false)]
        [InlineData(typeof(IDictionary<string, int>), false)]
        [InlineData(typeof(Tuple<string, bool>), false)]
        [InlineData(typeof(Dictionary<string, int>), true)]
        [InlineData(typeof(IEnumerable<string>), true)]
        [InlineData(typeof(ICollection<string>), true)]
        [InlineData(typeof(Collection<string>), true)]
        [InlineData(typeof(IList<string>), true)]
        [InlineData(typeof(List<string>), true)]
        [InlineData(typeof(IReadOnlyCollection<int>), true)]
        [InlineData(typeof(IReadOnlyList<int>), true)]
        [InlineData(typeof(HashSet<string>), true)]
        [InlineData(typeof(LinkedList<string>), true)]
        [InlineData(typeof(ArraySegment<string>), true)]
        public void IsSupportedReturnsWhetherTypeIsSupportedTest(Type type, bool supported)
        {
            var target = new EnumerableTypeCreator();

            var actual = target.IsSupported(type, null, null);

            actual.Should().Be(supported);
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullTypeTest()
        {
            var target = new EnumerableTypeCreator();

            Action action = () => target.IsSupported(null, null, null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void PopulateAddsItemsToCollectionFromExecuteStrategyTest()
        {
            var expected = new Collection<Guid>();

            var strategy = Substitute.For<IExecuteStrategy>();

            strategy.CreateWith(typeof(Guid)).Returns(Guid.NewGuid());

            var target = new EnumerableTypeCreator
            {
                AutoPopulateCount = 15
            };

            var actual = target.Populate(expected, strategy);

            actual.Should().BeSameAs(expected);

            var set = (Collection<Guid>) actual;

            set.Should().HaveCount(target.AutoPopulateCount);
            set.All(x => x != Guid.Empty).Should().BeTrue();
        }

        [Fact]
        public void PopulateAddsItemsToListFromExecuteStrategyTest()
        {
            var expected = new List<Guid>();

            var strategy = Substitute.For<IExecuteStrategy>();

            strategy.CreateWith(typeof(Guid)).Returns(Guid.NewGuid());

            var target = new EnumerableTypeCreator
            {
                AutoPopulateCount = 15
            };

            var actual = target.Populate(expected, strategy);

            actual.Should().BeSameAs(expected);

            var set = (List<Guid>) actual;

            set.Should().HaveCount(target.AutoPopulateCount);
            set.All(x => x != Guid.Empty).Should().BeTrue();
        }

        [Fact]
        public void PopulateCanAddItemsBasedOnPreviousItemTest()
        {
            var actual = new List<int>();
            var executeStrategy = Model.BuildStrategy.GetExecuteStrategy<List<int>>();

            var target = new IncrementingEnumerableTypeCreator();

            var result = (List<int>) target.Populate(actual, executeStrategy);

            var baseValue = result[0];
            var expected = new List<int>(target.AutoPopulateCount);

            for (var index = 0; index < target.AutoPopulateCount; index++)
            {
                expected.Add(baseValue + index);
            }

            result.ShouldAllBeEquivalentTo(expected);
        }

        [Fact]
        public void PopulateThrowsExceptionWithNullInstanceTest()
        {
            var strategy = Substitute.For<IExecuteStrategy>();

            var target = new EnumerableTypeCreator();

            Action action = () => target.Populate(null, strategy);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void PopulateThrowsExceptionWithNullStrategyTest()
        {
            var instance = new List<string>();

            var target = new EnumerableTypeCreator();

            Action action = () => target.Populate(instance, null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void PopulateThrowsExceptionWithUnsupportedTypeTest()
        {
            var instance = new Lazy<bool>(() => true);

            var strategy = Substitute.For<IExecuteStrategy>();

            var target = new EnumerableTypeCreator();

            Action action = () => target.Populate(instance, strategy);

            action.ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public void ProrityReturnsHigherThanDefaultTypeCreatorTest()
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
            try
            {
                var first = new EnumerableTypeCreator();

                EnumerableTypeCreator.DefaultAutoPopulateCount = 11;

                var second = new EnumerableTypeCreator();

                first.AutoPopulateCount.Should().Be(10);
                second.AutoPopulateCount.Should().Be(11);
            }
            finally
            {
                EnumerableTypeCreator.DefaultAutoPopulateCount = 10;
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