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
        public void CreateReturnsNewListOfSpecfiedTypeTest()
        {
            var target = new EnumerableTypeCreator();

            var actual = target.Create(typeof (IEnumerable<int>), null, null);

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

        [Fact]
        public void IsSupportedReturnsFalseForNonGenericTypeTest()
        {
            var target = new EnumerableTypeCreator();

            var actual = target.IsSupported(typeof (MemoryStream), null, null);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsSupportedReturnsFalseForUnsupportedGenericTypeTest()
        {
            var target = new EnumerableTypeCreator();

            var actual = target.IsSupported(typeof (Tuple<string, bool>), null, null);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsSupportedReturnsFalseForValueTypeTest()
        {
            var target = new EnumerableTypeCreator();

            var actual = target.IsSupported(typeof (int), null, null);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsSupportedReturnsTrueForSupportedTypeTest()
        {
            var target = new EnumerableTypeCreator();

            var actual = target.IsSupported(typeof (IEnumerable<string>), null, null);

            actual.Should().BeTrue();
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

            strategy.CreateWith(typeof (Guid)).Returns(Guid.NewGuid());

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

            strategy.CreateWith(typeof (Guid)).Returns(Guid.NewGuid());

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
        public void PopulateThrowsExceptionWithWhenInstanceNotGenericCollectionBasedTest()
        {
            var instance = new Lazy<bool>(() => true);

            var strategy = Substitute.For<IExecuteStrategy>();

            var target = new EnumerableTypeCreator();

            Action action = () => target.Populate(instance, strategy);

            action.ShouldThrow<NotSupportedException>();
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
            var target = new EnumerableTypeCreator
            {
                AutoPopulateCount = Environment.TickCount
            };

            EnumerableTypeCreator.DefaultAutoPopulateCount.Should().NotBe(target.AutoPopulateCount);
        }
    }
}