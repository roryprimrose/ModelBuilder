namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder.UnitTests.Models;
    using Xunit;

    public class SetEachByIndexExtensionsTests
    {
        [Fact]
        public void SetEachByIndexCollectionRunsActionAgainstInstance()
        {
            var sut = new Collection<Person>
            {
                new(),
                new()
            };

            var actual = sut.SetEach(
                (index, x) => { x.Priority = index; });

            actual.Should().BeSameAs(sut);
            actual.Count.Should().Be(2);
            actual.First().Priority.Should().Be(0);
            actual.Skip(1).First().Priority.Should().Be(1);
        }

        [Fact]
        public void SetEachByIndexCollectionThrowsExceptionWithNullAction()
        {
            var sut = new Collection<string>();

            Action action = () => sut.SetEach((Action<int, string>)null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachByIndexCollectionThrowsExceptionWithNullInstance()
        {
            Action action = () => ((Collection<string>)null!).SetEach((_, _) => { });

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachByIndexDictionaryRunsActionAgainstInstance()
        {
            var sut = new Dictionary<Guid, Person>
            {
                {
                    Guid.NewGuid(), new Person()
                },
                {
                    Guid.NewGuid(), new Person()
                }
            };

            var actual = sut.SetEach((_, x) => { x.Value.Id = x.Key; });

            actual.Should().BeSameAs(sut);
            actual.Count.Should().Be(2);
            actual.First().Key.Should().Be(actual.First().Value.Id);
            actual.Skip(1).First().Key.Should().Be(actual.Skip(1).First().Value.Id);
        }

        [Fact]
        public void SetEachByIndexDictionaryThrowsExceptionWithNullAction()
        {
            var sut = new Dictionary<Guid, Person>();

            Action action = () => sut.SetEach((Action<int, KeyValuePair<Guid, Person>>)null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachByIndexDictionaryThrowsExceptionWithNullInstance()
        {
            Action action = () => ((Dictionary<Guid, Person>)null!).SetEach((_, _) => { });

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachByIndexICollectionRunsActionAgainstInstance()
        {
            var source = new List<Person>
            {
                new(),
                new()
            };
            ICollection<Person> sut = new Collection<Person>(source);

            var actual = sut.SetEach(
                (index, x) => { x.Priority = index; });

            actual.Should().BeSameAs(sut);
            actual.Count.Should().Be(2);
            actual.First().Priority.Should().Be(0);
            actual.Skip(1).First().Priority.Should().Be(1);
        }

        [Fact]
        public void SetEachByIndexICollectionThrowsExceptionWithNullAction()
        {
            var data = new List<Person>();

            ICollection<Person> sut = new Collection<Person>(data);

            Action action = () => sut.SetEach((Action<int, Person>)null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachByIndexICollectionThrowsExceptionWithNullInstance()
        {
            Action action = () => ((ICollection<Person>)null!).SetEach((_, _) => { });

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachByIndexIDictionaryRunsActionAgainstInstance()
        {
            IDictionary<Guid, Person> sut = new Dictionary<Guid, Person>
            {
                {
                    Guid.NewGuid(), new Person()
                },
                {
                    Guid.NewGuid(), new Person()
                }
            };

            var actual = sut.SetEach((_, x) => { x.Value.Id = x.Key; });

            actual.Should().BeSameAs(sut);
            actual.Count.Should().Be(2);
            actual.First().Key.Should().Be(actual.First().Value.Id);
            actual.Skip(1).First().Key.Should().Be(actual.Skip(1).First().Value.Id);
        }

        [Fact]
        public void SetEachByIndexIDictionaryThrowsExceptionWithNullAction()
        {
            IDictionary<Guid, Person> sut = new Dictionary<Guid, Person>();

            Action action = () => sut.SetEach((Action<int, KeyValuePair<Guid, Person>>)null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachByIndexIDictionaryThrowsExceptionWithNullInstance()
        {
            Action action = () => ((IDictionary<Guid, Person>)null!).SetEach((_, _) => { });

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachByIndexIEnumerableRunsActionAgainstInstance()
        {
            var sut = BuildPeople();

            var actual = sut.SetEach(
                (index, x) => { x.Priority = index; });

            actual.Count.Should().Be(2);
            actual[0].Priority.Should().Be(0);
            actual[1].Priority.Should().Be(1);
        }

        [Fact]
        public void SetEachByIndexIEnumerableThrowsExceptionWithNullAction()
        {
            IEnumerable<string> sut = new Collection<string>();

            Action action = () => sut.SetEach((Action<int, string>)null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachByIndexIEnumerableThrowsExceptionWithNullInstance()
        {
            Action action = () => ((IEnumerable<string>)null!).SetEach((_, _) => { });

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachByIndexIListRunsActionAgainstInstance()
        {
            IList<Person> sut = new List<Person>
            {
                new(),
                new()
            };

            var actual = sut.SetEach(
                (index, x) => { x.Priority = index; });

            actual.Should().BeSameAs(sut);
            actual.Count.Should().Be(2);
            actual.First().Priority.Should().Be(0);
            actual.Skip(1).First().Priority.Should().Be(1);
        }

        [Fact]
        public void SetEachByIndexIListThrowsExceptionWithNullAction()
        {
            IList<string> sut = new List<string>();

            Action action = () => sut.SetEach((Action<int, string>)null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachByIndexIListThrowsExceptionWithNullInstance()
        {
            Action action = () => ((IList<string>)null!).SetEach((_, _) => { });

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachByIndexIReadOnlyCollectionRunsActionAgainstInstance()
        {
            var source = new List<Person>
            {
                new(),
                new()
            };
            IReadOnlyCollection<Person> sut = new ReadOnlyCollection<Person>(source);

            var actual = sut.SetEach(
                (index, x) => { x.Priority = index; });

            actual.Should().BeSameAs(sut);
            actual.Count.Should().Be(2);
            actual.First().Priority.Should().Be(0);
            actual.Skip(1).First().Priority.Should().Be(1);
        }

        [Fact]
        public void SetEachByIndexIReadOnlyCollectionThrowsExceptionWithNullAction()
        {
            var data = new List<Person>();

            IReadOnlyCollection<Person> sut = new ReadOnlyCollection<Person>(data);

            Action action = () => sut.SetEach((Action<int, Person>)null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachByIndexIReadOnlyCollectionThrowsExceptionWithNullInstance()
        {
            Action action = () => ((IReadOnlyCollection<Person>)null!).SetEach((_, _) => { });

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachByIndexIReadOnlyDictionaryRunsActionAgainstInstance()
        {
            IReadOnlyDictionary<Guid, Person> sut = new Dictionary<Guid, Person>
            {
                {
                    Guid.NewGuid(), new Person()
                },
                {
                    Guid.NewGuid(), new Person()
                }
            };

            var actual = sut.SetEach((_, x) => { x.Value.Id = x.Key; });

            actual.Should().BeSameAs(sut);
            actual.Count.Should().Be(2);
            actual.First().Key.Should().Be(actual.First().Value.Id);
            actual.Skip(1).First().Key.Should().Be(actual.Skip(1).First().Value.Id);
        }

        [Fact]
        public void SetEachByIndexIReadOnlyDictionaryThrowsExceptionWithNullAction()
        {
            IReadOnlyDictionary<Guid, Person> sut = new Dictionary<Guid, Person>();

            Action action = () => sut.SetEach((Action<int, KeyValuePair<Guid, Person>>)null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachByIndexIReadOnlyDictionaryThrowsExceptionWithNullInstance()
        {
            Action action = () => ((IReadOnlyDictionary<Guid, Person>)null!).SetEach((_, _) => { });

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachByIndexIReadOnlyListRunsActionAgainstInstance()
        {
            var source = new List<Person>
            {
                new(),
                new()
            };
            IReadOnlyList<Person> sut = new ReadOnlyCollection<Person>(source);

            var actual = sut.SetEach(
                (index, x) => { x.Priority = index; });

            actual.Should().BeSameAs(sut);
            actual.Count.Should().Be(2);
            actual[0].Priority.Should().Be(0);
            actual[1].Priority.Should().Be(1);
        }

        [Fact]
        public void SetEachByIndexIReadOnlyListThrowsExceptionWithNullAction()
        {
            var data = new List<Person>();

            IReadOnlyList<Person> sut = new ReadOnlyCollection<Person>(data);

            Action action = () => sut.SetEach((Action<int, Person>)null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachByIndexIReadOnlyListThrowsExceptionWithNullInstance()
        {
            Action action = () => ((IReadOnlyList<Person>)null!).SetEach((_, _) => { });

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachByIndexListAsIEnumerableRunsActionAgainstInstance()
        {
            var sut = new List<Person>
            {
                new(),
                new()
            };

            var actual = sut.SetEach(
                (index, x) => { x.Priority = index; });

            actual.Should().BeSameAs(sut);
            actual.Count.Should().Be(2);
            actual[0].Priority.Should().Be(0);
            actual[1].Priority.Should().Be(1);
        }

        [Fact]
        public void SetEachByIndexListThrowsExceptionWithNullAction()
        {
            var sut = new List<string>();

            Action action = () => sut.SetEach((Action<int, string>)null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachByIndexListThrowsExceptionWithNullInstance()
        {
            Action action = () => ((List<Person>)null!).SetEach((_, _) => { });

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachByIndexReadOnlyCollectionRunsActionAgainstInstance()
        {
            var source = new List<Person>
            {
                new(),
                new()
            };
            var sut = new ReadOnlyCollection<Person>(source);

            var actual = sut.SetEach(
                (index, x) => { x.Priority = index; });

            actual.Should().BeSameAs(sut);
            actual.Count.Should().Be(2);
            actual.First().Priority.Should().Be(0);
            actual.Skip(1).First().Priority.Should().Be(1);
        }

        [Fact]
        public void SetEachByIndexReadOnlyCollectionThrowsExceptionWithNullAction()
        {
            var data = new List<Person>();

            var sut = new ReadOnlyCollection<Person>(data);

            Action action = () => sut.SetEach((Action<int, Person>)null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachByIndexReadOnlyCollectionThrowsExceptionWithNullInstance()
        {
            Action action = () => ((ReadOnlyCollection<Person>)null!).SetEach((_, _) => { });

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachByIndexReadOnlyDictionaryRunsActionAgainstInstance()
        {
            var data = new Dictionary<Guid, Person>
            {
                {
                    Guid.NewGuid(), new Person()
                },
                {
                    Guid.NewGuid(), new Person()
                }
            };

            var sut = new ReadOnlyDictionary<Guid, Person>(data);

            var actual = sut.SetEach((_, x) => { x.Value.Id = x.Key; });

            actual.Should().BeSameAs(sut);
            actual.Count.Should().Be(2);
            actual.First().Key.Should().Be(actual.First().Value.Id);
            actual.Skip(1).First().Key.Should().Be(actual.Skip(1).First().Value.Id);
        }

        [Fact]
        public void SetEachByIndexReadOnlyDictionaryThrowsExceptionWithNullAction()
        {
            var data = new Dictionary<Guid, Person>
            {
                {
                    Guid.NewGuid(), new Person()
                },
                {
                    Guid.NewGuid(), new Person()
                }
            };

            var sut = new ReadOnlyDictionary<Guid, Person>(data);

            Action action = () => sut.SetEach((Action<int, KeyValuePair<Guid, Person>>)null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachByIndexReadOnlyDictionaryThrowsExceptionWithNullInstance()
        {
            Action action = () => ((ReadOnlyDictionary<Guid, Person>)null!).SetEach((_, _) => { });

            action.Should().Throw<ArgumentNullException>();
        }

        private static IEnumerable<Person> BuildPeople()
        {
            yield return new()
            {
                Id = Guid.NewGuid()
            };
            yield return new()
            {
                Id = Guid.NewGuid()
            };
        }
    }
}