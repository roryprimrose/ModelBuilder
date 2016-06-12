namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.IO;
    using System.Linq;
    using FluentAssertions;
    using NSubstitute;
    using Xunit;

    public class ArrayTypeCreatorTests
    {
        [Fact]
        public void CreateChildItemThrowsExceptionWithNullExecuteStrategyTest()
        {
            var person = new Person();

            var target = new ArrayTypeCreatorWrapper();

            Action action = () => target.CreateItem(typeof(Person[]), null, person);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Theory]
        [InlineData(typeof(byte[]))]
        [InlineData(typeof(int[]))]
        [InlineData(typeof(Guid[]))]
        [InlineData(typeof(Person[]))]
        public void CreateReturnsInstanceTest(Type type)
        {
            var target = new ArrayTypeCreator();

            var actual = target.Create(type, null, null);

            actual.Should().NotBeNull();
        }

        [Fact]
        public void CreateThrowsExceptionWithNullTypeTest()
        {
            var target = new ArrayTypeCreator();

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
            var target = new ArrayTypeCreator();

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

        [Fact]
        public void DefaultMaxCountIsPositiveTest()
        {
            ArrayTypeCreator.DefaultMaxCount.Should().BeGreaterThan(0);
        }

        [Fact]
        public void DisablesAutoConstructorDetectionTest()
        {
            var target = new ArrayTypeCreatorWrapper();

            target.AutoDetectConstructor.Should().BeFalse();
        }

        [Fact]
        public void DisablesAutoPopulateTest()
        {
            var target = new ArrayTypeCreatorWrapper();

            target.AutoPopulate.Should().BeFalse();
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
        public void IsSupportedReturnsWhetherTypeIsSupportedTest(Type type, bool supported)
        {
            var target = new ArrayTypeCreator();

            var actual = target.IsSupported(type, null, null);

            actual.Should().Be(supported);
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullTypeTest()
        {
            var target = new ArrayTypeCreator();

            Action action = () => target.IsSupported(null, null, null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void PopulateAddsItemsToCollectionFromExecuteStrategyTest()
        {
            var expected = new Guid[15];

            var strategy = Substitute.For<IExecuteStrategy>();

            strategy.CreateWith(typeof(Guid)).Returns(Guid.NewGuid());

            var target = new ArrayTypeCreator
            {
                MaxCount = 15
            };

            var actual = target.Populate(expected, strategy);

            actual.Should().BeSameAs(expected);

            var set = (Guid[])actual;

            set.Should().HaveCount(target.MaxCount);
            set.All(x => x != Guid.Empty).Should().BeTrue();
        }

        [Fact]
        public void PopulateAddsItemsToListFromExecuteStrategyTest()
        {
            var expected = new Guid[15];

            var strategy = Substitute.For<IExecuteStrategy>();

            strategy.CreateWith(typeof(Guid)).Returns(Guid.NewGuid());

            var target = new ArrayTypeCreator
            {
                MaxCount = 15
            };

            var actual = target.Populate(expected, strategy);

            actual.Should().BeSameAs(expected);

            var set = (Guid[])actual;

            set.Should().HaveCount(target.MaxCount);
            set.All(x => x != Guid.Empty).Should().BeTrue();
        }

        [Fact]
        public void PopulateCanAddItemsBasedOnPreviousItemTest()
        {
            var actual = new int[15];
            var executeStrategy = Model.BuildStrategy.GetExecuteStrategy<List<int>>();

            var target = new IncrementingArrayTypeCreator();

            var result = (int[])target.Populate(actual, executeStrategy);

            var baseValue = result[0];
            var expected = new int[actual.Length];

            for (var index = 0; index < expected.Length; index++)
            {
                expected[index] = baseValue + index;
            }

            result.ShouldAllBeEquivalentTo(expected);
        }

        [Fact]
        public void PopulateInferesItemTypeByArrayTypeWhenFirstItemIsNullTest()
        {
            var actual = new Person[15];
            var executeStrategy = Model.BuildStrategy.GetExecuteStrategy<List<int>>();

            var target = new ArrayTypeCreator();

            var result = (Person[])target.Populate(actual, executeStrategy);

            result.All(x => x != null).Should().BeTrue();
        }

        [Fact]
        public void PopulateReturnsEmptyArrayWhenSourceHasZeroLengthTest()
        {
            var expected = new Guid[0];

            var strategy = Substitute.For<IExecuteStrategy>();

            strategy.CreateWith(typeof(Guid)).Returns(Guid.NewGuid());

            var target = new ArrayTypeCreator
            {
                MaxCount = 15
            };

            var actual = target.Populate(expected, strategy);

            actual.Should().BeSameAs(expected);

            var set = (Guid[])actual;

            set.Should().BeEmpty();
        }

        [Fact]
        public void PopulateThrowsExceptionWithNullInstanceTest()
        {
            var strategy = Substitute.For<IExecuteStrategy>();

            var target = new ArrayTypeCreator();

            Action action = () => target.Populate(null, strategy);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void PopulateThrowsExceptionWithNullStrategyTest()
        {
            var instance = new List<string>();

            var target = new ArrayTypeCreator();

            Action action = () => target.Populate(instance, null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void PopulateThrowsExceptionWithUnsupportedTypeTest()
        {
            var instance = new Lazy<bool>(() => true);

            var strategy = Substitute.For<IExecuteStrategy>();

            var target = new ArrayTypeCreator();

            Action action = () => target.Populate(instance, strategy);

            action.ShouldThrow<NotSupportedException>();
        }

        [Fact]
        public void ProrityReturnsHigherThanDefaultTypeCreatorTest()
        {
            var target = new ArrayTypeCreator();
            var other = new DefaultTypeCreator();

            target.Priority.Should().BeGreaterThan(other.Priority);
        }

        [Fact]
        public void SettingDefaultMaxCountOnlyAffectsNewInstancesTest()
        {
            var expected = ArrayTypeCreator.DefaultMaxCount;

            try
            {
                var first = new ArrayTypeCreator();

                ArrayTypeCreator.DefaultMaxCount = 11;

                var second = new ArrayTypeCreator();

                first.MaxCount.Should().Be(expected);
                second.MaxCount.Should().Be(11);
            }
            finally
            {
                ArrayTypeCreator.DefaultMaxCount = expected;
            }
        }

        [Fact]
        public void SettingMaxCountShouldNotChangeDefaultMaxCountTest()
        {
            var target = new ArrayTypeCreator
            {
                MaxCount = Environment.TickCount
            };

            ArrayTypeCreator.DefaultMaxCount.Should().NotBe(target.MaxCount);
        }

        private class ArrayTypeCreatorWrapper : ArrayTypeCreator
        {
            public void CreateItem(Type type, IExecuteStrategy executeStrategy, object item)
            {
                CreateChildItem(type, executeStrategy, item);
            }
        }
    }
}