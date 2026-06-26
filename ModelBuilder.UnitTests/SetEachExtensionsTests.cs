namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder;
    using Xunit;

    public class SetEachExtensionsTests
    {
        [Fact]
        public void SetEachAppliesActionToEachItemWhenSourceIsCollection()
        {
            var source = new Collection<Person> { new(), new() };

            source.SetEach(x => x.Age = 5);

            source.Should().OnlyContain(x => x.Age == 5);
        }

        [Fact]
        public void SetEachAppliesActionToEachItemWhenSourceIsDictionary()
        {
            var source = new Dictionary<string, Person> { ["a"] = new(), ["b"] = new() };

            source.SetEach(x => x.Value.Age = 7);

            source.Values.Should().OnlyContain(x => x.Age == 7);
        }

        [Fact]
        public void SetEachAppliesActionToEachItemWhenSourceIsList()
        {
            var source = new List<Person> { new(), new() };

            source.SetEach(x => x.Age = 3);

            source.Should().OnlyContain(x => x.Age == 3);
        }

        [Fact]
        public void SetEachAppliesActionToEachItemWhenSourceIsReadOnlyCollection()
        {
            var source = new ReadOnlyCollection<Person>(new List<Person> { new(), new() });

            source.SetEach(x => x.Age = 9);

            source.Should().OnlyContain(x => x.Age == 9);
        }

        [Fact]
        public void SetEachAppliesActionToEachItemWhenSourceIsReadOnlyDictionary()
        {
            var source = new ReadOnlyDictionary<string, Person>(
                new Dictionary<string, Person> { ["a"] = new(), ["b"] = new() });

            source.SetEach(x => x.Value.Age = 11);

            source.Values.Should().OnlyContain(x => x.Age == 11);
        }

        [Fact]
        public void SetEachConvertsNonListEnumerableBeforeApplyingAction()
        {
            var first = new Person();
            var second = new Person();
            IEnumerable<Person> source = new[] { first, second }.Where(x => x != null);

            var actual = source.SetEach(x => x.Age = 8);

            actual.Should().BeOfType<List<Person>>();
            first.Age.Should().Be(8);
            second.Age.Should().Be(8);
        }

        [Fact]
        public void SetEachOnEnumerableReturnsUnderlyingListWhenSourceIsAlreadyList()
        {
            var list = new List<Person> { new(), new() };
            IEnumerable<Person> source = list;

            var actual = source.SetEach(x => x.Age = 4);

            actual.Should().BeSameAs(list);
        }

        [Fact]
        public void SetEachReturnsSameInstanceWhenSourceIsCollection()
        {
            var source = new Collection<Person> { new() };

            var actual = source.SetEach(x => x.Age = 1);

            actual.Should().BeSameAs(source);
        }

        [Fact]
        public void SetEachReturnsSameInstanceWhenSourceIsICollection()
        {
            ICollection<Person> source = new List<Person> { new() };

            var actual = source.SetEach(x => x.Age = 1);

            actual.Should().BeSameAs(source);
        }

        [Fact]
        public void SetEachReturnsSameInstanceWhenSourceIsIDictionary()
        {
            IDictionary<string, Person> source = new Dictionary<string, Person> { ["a"] = new() };

            var actual = source.SetEach(x => x.Value.Age = 1);

            actual.Should().BeSameAs(source);
        }

        [Fact]
        public void SetEachReturnsSameInstanceWhenSourceIsIList()
        {
            IList<Person> source = new List<Person> { new() };

            var actual = source.SetEach(x => x.Age = 1);

            actual.Should().BeSameAs(source);
        }

        [Fact]
        public void SetEachReturnsSameInstanceWhenSourceIsIReadOnlyCollection()
        {
            IReadOnlyCollection<Person> source = new List<Person> { new() };

            var actual = source.SetEach(x => x.Age = 1);

            actual.Should().BeSameAs(source);
        }

        [Fact]
        public void SetEachReturnsSameInstanceWhenSourceIsIReadOnlyDictionary()
        {
            IReadOnlyDictionary<string, Person> source = new Dictionary<string, Person> { ["a"] = new() };

            var actual = source.SetEach(x => x.Value.Age = 1);

            actual.Should().BeSameAs(source);
        }

        [Fact]
        public void SetEachReturnsSameInstanceWhenSourceIsIReadOnlyList()
        {
            IReadOnlyList<Person> source = new List<Person> { new() };

            var actual = source.SetEach(x => x.Age = 1);

            actual.Should().BeSameAs(source);
        }

        [Fact]
        public void SetEachReturnsSameInstanceWhenSourceIsList()
        {
            var source = new List<Person> { new() };

            var actual = source.SetEach(x => x.Age = 1);

            actual.Should().BeSameAs(source);
        }

        [Fact]
        public void SetEachExplicitAppliesActionToEachItem()
        {
            var source = new List<Person> { new(), new() };

            var actual = source.SetEachExplicit((Person x) => x.Age = 6);

            actual.Should().BeSameAs(source);
            source.Should().OnlyContain(x => x.Age == 6);
        }

        [Fact]
        public void SetEachExplicitAppliesActionToEachDictionaryEntry()
        {
            var source = new Dictionary<string, Person> { ["a"] = new(), ["b"] = new() };

            var actual = source.SetEachExplicit((KeyValuePair<string, Person> x) => x.Value.Age = 6);

            actual.Should().BeSameAs(source);
            source.Values.Should().OnlyContain(x => x.Age == 6);
        }

        [Fact]
        public void SetEachExplicitThrowsWithNullAction()
        {
            var source = new List<Person>();

            Action action = () => source.SetEachExplicit((Action<Person>)null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachExplicitThrowsWithNullInstances()
        {
            List<Person> source = null!;

            Action action = () => source.SetEachExplicit((Person x) => x.Age = 1);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachThrowsWithNullActionWhenSourceIsDictionary()
        {
            var source = new Dictionary<string, Person>();

            Action action = () => source.SetEach((Action<KeyValuePair<string, Person>>)null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachThrowsWithNullActionWhenSourceIsEnumerable()
        {
            IEnumerable<Person> source = new List<Person>();

            Action action = () => source.SetEach((Action<Person>)null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachThrowsWithNullActionWhenSourceIsList()
        {
            var source = new List<Person>();

            Action action = () => source.SetEach((Action<Person>)null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachThrowsWithNullInstancesWhenSourceIsDictionary()
        {
            Dictionary<string, Person> source = null!;

            Action action = () => source.SetEach(x => x.Value.Age = 1);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachThrowsWithNullInstancesWhenSourceIsEnumerable()
        {
            IEnumerable<Person> source = null!;

            Action action = () => source.SetEach(x => x.Age = 1);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachThrowsWithNullInstancesWhenSourceIsList()
        {
            List<Person> source = null!;

            Action action = () => source.SetEach(x => x.Age = 1);

            action.Should().Throw<ArgumentNullException>();
        }

        private class Person
        {
            public int Age { get; set; }
        }
    }
}
