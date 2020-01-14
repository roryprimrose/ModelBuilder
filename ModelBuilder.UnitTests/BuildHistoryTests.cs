namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Models;
    using Xunit;

    public class BuildHistoryTests
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void CountReturnsNumberOfItemsInHistory(int itemCount)
        {
            var sut = new BuildHistory();

            for (var index = 0; index < itemCount; index++)
            {
                var item = Model.Create<Person>();

                sut.Push(item);
            }

            sut.Count.Should().Be(itemCount);
        }

        [Fact]
        public void EnumerableReturnsAllItems()
        {
            var items = new List<object>();

            var sut = new BuildHistory();

            for (var index = 0; index < 10; index++)
            {
                var item = Model.Create<Person>();

                items.Add(item);

                sut.Push(item);
            }

            items.Reverse();

            foreach (var item in sut)
            {
                items.Should().Contain(item);
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void EnumerableReturnsFullHistory(int itemCount)
        {
            var items = new List<object>();

            var sut = new BuildHistory();

            for (var index = 0; index < itemCount; index++)
            {
                var item = Model.Create<Person>();

                items.Add(item);

                sut.Push(item);
            }

            items.Reverse();

            sut.Should().ContainInOrder(items);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void FirstAlwaysReturnsFirstValue(int itemCount)
        {
            var items = new List<object>();

            var sut = new BuildHistory();

            for (var index = 0; index < itemCount; index++)
            {
                var item = Model.Create<Person>();

                items.Add(item);

                sut.Push(item);
            }

            var expected = items.First();

            sut.First.Should().BeSameAs(expected);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public void FirstReturnsFirstValueAfterPoppingPreviousValues(int itemsToRemove)
        {
            var items = new List<object>();

            var sut = new BuildHistory();

            for (var index = 0; index < 5; index++)
            {
                var item = Model.Create<Person>();

                items.Add(item);

                sut.Push(item);
            }

            sut.First.Should().BeSameAs(items.First());

            for (var index = 0; index < itemsToRemove; index++)
            {
                sut.Pop();

                sut.First.Should().BeSameAs(items.First());
            }
        }

        [Fact]
        public void FirstReturnsNullAfterLastItemRemoved()
        {
            var sut = new BuildHistory();

            var item = Model.Create<Person>();

            sut.Push(item);

            sut.First.Should().BeSameAs(item);

            sut.Pop();

            sut.First.Should().BeNull();
        }

        [Fact]
        public void FirstReturnsNullWhenHistoryIsEmpty()
        {
            var sut = new BuildHistory();

            sut.First.Should().BeNull();
        }

        [Fact]
        public void FirstReturnsSingleValueInHistory()
        {
            var sut = new BuildHistory();

            var item = Model.Create<Person>();

            sut.Push(item);

            sut.First.Should().BeSameAs(item);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void LastAlwaysReturnsLastValue(int itemCount)
        {
            var items = new List<object>();

            var sut = new BuildHistory();

            for (var index = 0; index < itemCount; index++)
            {
                var item = Model.Create<Person>();

                items.Add(item);

                sut.Push(item);
            }

            var expected = items.Last();

            sut.Last.Should().BeSameAs(expected);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void LastReturnsLastValueAfterPoppingPreviousValues(int itemsToRemove)
        {
            var items = new List<object>();

            var sut = new BuildHistory();

            for (var index = 0; index < 5; index++)
            {
                var item = Model.Create<Person>();

                items.Add(item);

                sut.Push(item);
            }

            sut.Last.Should().BeSameAs(items.Last());

            for (var index = 0; index <= itemsToRemove; index++)
            {
                sut.Last.Should().BeSameAs(items[4 - index]);

                sut.Pop();

                sut.Last.Should().BeSameAs(items[3 - index]);
            }
        }

        [Fact]
        public void LastReturnsNullAfterLastItemRemoved()
        {
            var sut = new BuildHistory();

            var item = Model.Create<Person>();

            sut.Push(item);

            sut.Last.Should().BeSameAs(item);

            sut.Pop();

            sut.Last.Should().BeNull();
        }

        [Fact]
        public void LastReturnsNullWhenHistoryIsEmpty()
        {
            var sut = new BuildHistory();

            sut.Last.Should().BeNull();
        }

        [Fact]
        public void LastReturnsSingleValueInHistory()
        {
            var sut = new BuildHistory();

            var item = Model.Create<Person>();

            sut.Push(item);

            sut.Last.Should().BeSameAs(item);
        }

        [Fact]
        public void PopThrowsExceptionWhenHistoryIsEmpty()
        {
            var sut = new BuildHistory();

            Action action = () => sut.Pop();

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void PushThrowsExceptionWithNullInstance()
        {
            var sut = new BuildHistory();

            Action action = () => sut.Push(null);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}