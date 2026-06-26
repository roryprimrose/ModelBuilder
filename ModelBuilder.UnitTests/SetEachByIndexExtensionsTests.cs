namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder;
    using Xunit;

    public class SetEachByIndexExtensionsTests
    {
        [Fact]
        public void SetEachAppliesIndexToEachItemWhenSourceIsCollection()
        {
            var source = new Collection<Person> { new(), new(), new() };

            source.SetEach((index, x) => x.Age = index);

            source.Select(x => x.Age).Should().ContainInOrder(0, 1, 2);
        }

        [Fact]
        public void SetEachAppliesIndexToEachItemWhenSourceIsDictionary()
        {
            var source = new Dictionary<string, Person> { ["a"] = new(), ["b"] = new() };

            source.SetEach((index, x) => x.Value.Age = index);

            source.Values.Select(x => x.Age).Should().BeEquivalentTo(new[] { 0, 1 });
        }

        [Fact]
        public void SetEachAppliesIndexToEachItemWhenSourceIsList()
        {
            var source = new List<Person> { new(), new(), new() };

            source.SetEach((index, x) => x.Age = index);

            source.Select(x => x.Age).Should().ContainInOrder(0, 1, 2);
        }

        [Fact]
        public void SetEachAppliesIndexToEachItemWhenSourceIsReadOnlyCollection()
        {
            var source = new ReadOnlyCollection<Person>(new List<Person> { new(), new() });

            source.SetEach((index, x) => x.Age = index);

            source.Select(x => x.Age).Should().ContainInOrder(0, 1);
        }

        [Fact]
        public void SetEachAppliesIndexToEachItemWhenSourceIsReadOnlyDictionary()
        {
            var source = new ReadOnlyDictionary<string, Person>(
                new Dictionary<string, Person> { ["a"] = new(), ["b"] = new() });

            source.SetEach((index, x) => x.Value.Age = index);

            source.Values.Select(x => x.Age).Should().BeEquivalentTo(new[] { 0, 1 });
        }

        [Fact]
        public void SetEachConvertsNonListEnumerableBeforeApplyingAction()
        {
            var first = new Person();
            var second = new Person();
            IEnumerable<Person> source = new[] { first, second }.Where(x => x != null);

            var actual = source.SetEach((index, x) => x.Age = index);

            actual.Should().BeOfType<List<Person>>();
            first.Age.Should().Be(0);
            second.Age.Should().Be(1);
        }

        [Fact]
        public void SetEachOnEnumerableReturnsUnderlyingListWhenSourceIsAlreadyList()
        {
            var list = new List<Person> { new(), new() };
            IEnumerable<Person> source = list;

            var actual = source.SetEach((index, x) => x.Age = index);

            actual.Should().BeSameAs(list);
        }

        [Fact]
        public void SetEachReturnsSameInstanceWhenSourceIsCollection()
        {
            var source = new Collection<Person> { new() };

            var actual = source.SetEach((index, x) => x.Age = index);

            actual.Should().BeSameAs(source);
        }

        [Fact]
        public void SetEachReturnsSameInstanceWhenSourceIsICollection()
        {
            ICollection<Person> source = new List<Person> { new() };

            var actual = source.SetEach((index, x) => x.Age = index);

            actual.Should().BeSameAs(source);
        }

        [Fact]
        public void SetEachReturnsSameInstanceWhenSourceIsIDictionary()
        {
            IDictionary<string, Person> source = new Dictionary<string, Person> { ["a"] = new() };

            var actual = source.SetEach((index, x) => x.Value.Age = index);

            actual.Should().BeSameAs(source);
        }

        [Fact]
        public void SetEachReturnsSameInstanceWhenSourceIsIList()
        {
            IList<Person> source = new List<Person> { new() };

            var actual = source.SetEach((index, x) => x.Age = index);

            actual.Should().BeSameAs(source);
        }

        [Fact]
        public void SetEachReturnsSameInstanceWhenSourceIsIReadOnlyCollection()
        {
            IReadOnlyCollection<Person> source = new List<Person> { new() };

            var actual = source.SetEach((index, x) => x.Age = index);

            actual.Should().BeSameAs(source);
        }

        [Fact]
        public void SetEachReturnsSameInstanceWhenSourceIsIReadOnlyDictionary()
        {
            IReadOnlyDictionary<string, Person> source = new Dictionary<string, Person> { ["a"] = new() };

            var actual = source.SetEach((index, x) => x.Value.Age = index);

            actual.Should().BeSameAs(source);
        }

        [Fact]
        public void SetEachReturnsSameInstanceWhenSourceIsIReadOnlyList()
        {
            IReadOnlyList<Person> source = new List<Person> { new() };

            var actual = source.SetEach((index, x) => x.Age = index);

            actual.Should().BeSameAs(source);
        }

        [Fact]
        public void SetEachReturnsSameInstanceWhenSourceIsList()
        {
            var source = new List<Person> { new() };

            var actual = source.SetEach((index, x) => x.Age = index);

            actual.Should().BeSameAs(source);
        }

        [Fact]
        public void SetEachExplicitAppliesIndexToEachItem()
        {
            var source = new List<Person> { new(), new(), new() };

            var actual = source.SetEachExplicit((int index, Person x) => x.Age = index);

            actual.Should().BeSameAs(source);
            source.Select(x => x.Age).Should().ContainInOrder(0, 1, 2);
        }

        [Fact]
        public void SetEachExplicitAppliesIndexToEachDictionaryEntry()
        {
            var source = new Dictionary<string, Person> { ["a"] = new(), ["b"] = new() };

            var actual = source.SetEachExplicit((int index, KeyValuePair<string, Person> x) => x.Value.Age = index);

            actual.Should().BeSameAs(source);
            source.Values.Select(x => x.Age).Should().BeEquivalentTo(new[] { 0, 1 });
        }

        [Fact]
        public void SetEachExplicitThrowsWithNullAction()
        {
            var source = new List<Person>();

            Action action = () => source.SetEachExplicit((Action<int, Person>)null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachExplicitThrowsWithNullInstances()
        {
            List<Person> source = null!;

            Action action = () => source.SetEachExplicit((int index, Person x) => x.Age = index);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachThrowsWithNullActionWhenSourceIsDictionary()
        {
            var source = new Dictionary<string, Person>();

            Action action = () => source.SetEach((Action<int, KeyValuePair<string, Person>>)null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachThrowsWithNullActionWhenSourceIsEnumerable()
        {
            IEnumerable<Person> source = new List<Person>();

            Action action = () => source.SetEach((Action<int, Person>)null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachThrowsWithNullActionWhenSourceIsList()
        {
            var source = new List<Person>();

            Action action = () => source.SetEach((Action<int, Person>)null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachThrowsWithNullInstancesWhenSourceIsDictionary()
        {
            Dictionary<string, Person> source = null!;

            Action action = () => source.SetEach((index, x) => x.Value.Age = index);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachThrowsWithNullInstancesWhenSourceIsEnumerable()
        {
            IEnumerable<Person> source = null!;

            Action action = () => source.SetEach((index, x) => x.Age = index);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachThrowsWithNullInstancesWhenSourceIsList()
        {
            List<Person> source = null!;

            Action action = () => source.SetEach((index, x) => x.Age = index);

            action.Should().Throw<ArgumentNullException>();
        }

        private class Person
        {
            public int Age { get; set; }
        }
    }
}
