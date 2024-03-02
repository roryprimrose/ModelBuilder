namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder.UnitTests.Models;
    using Xunit;

    public class SetEachExtensionsTests
    {
        [Fact]
        public void SetEachCollectionRunsActionAgainstInstance()
        {
            var index = 0;
            var sut = new Collection<Person>
            {
                new Person(),
                new Person()
            };

            var actual = sut.SetEach(
                x =>
                {
                    index++;
                    x.Priority = index;
                });

            actual.Should().BeSameAs(sut);
            actual.Count.Should().Be(2);
            actual.First().Priority.Should().Be(1);
            actual.Skip(1).First().Priority.Should().Be(2);
        }

        [Fact]
        public void SetEachCollectionThrowsExceptionWithNullAction()
        {
            var sut = new Collection<string>();

            Action action = () => sut.SetEach(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachCollectionThrowsExceptionWithNullInstance()
        {
            Action action = () => ((Collection<string>)null!).SetEach(x => { });

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachDictionaryRunsActionAgainstInstance()
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

            var actual = sut.SetEach(x => { x.Value.Id = x.Key; });

            actual.Should().BeSameAs(sut);
            actual.Count.Should().Be(2);
            actual.First().Key.Should().Be(actual.First().Value.Id);
            actual.Skip(1).First().Key.Should().Be(actual.Skip(1).First().Value.Id);
        }

        [Fact]
        public void SetEachDictionaryThrowsExceptionWithNullAction()
        {
            var sut = new Dictionary<Guid, Person>();

            Action action = () => sut.SetEach(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachDictionaryThrowsExceptionWithNullInstance()
        {
            Action action = () => ((Dictionary<Guid, Person>)null!).SetEach(x => { });

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachICollectionRunsActionAgainstInstance()
        {
            var index = 0;
            var source = new List<Person>
            {
                new Person(),
                new Person()
            };
            ICollection<Person> sut = new Collection<Person>(source);

            var actual = sut.SetEach(
                x =>
                {
                    index++;
                    x.Priority = index;
                });

            actual.Should().BeSameAs(sut);
            actual.Count.Should().Be(2);
            actual.First().Priority.Should().Be(1);
            actual.Skip(1).First().Priority.Should().Be(2);
        }

        [Fact]
        public void SetEachICollectionThrowsExceptionWithNullAction()
        {
            var data = new List<Person>();

            ICollection<Person> sut = new Collection<Person>(data);

            Action action = () => sut.SetEach(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachICollectionThrowsExceptionWithNullInstance()
        {
            Action action = () => ((ICollection<Person>)null!).SetEach(x => { });

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachIDictionaryRunsActionAgainstInstance()
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

            var actual = sut.SetEach(x => { x.Value.Id = x.Key; });

            actual.Should().BeSameAs(sut);
            actual.Count.Should().Be(2);
            actual.First().Key.Should().Be(actual.First().Value.Id);
            actual.Skip(1).First().Key.Should().Be(actual.Skip(1).First().Value.Id);
        }

        [Fact]
        public void SetEachIDictionaryThrowsExceptionWithNullAction()
        {
            IDictionary<Guid, Person> sut = new Dictionary<Guid, Person>();

            Action action = () => sut.SetEach(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachIDictionaryThrowsExceptionWithNullInstance()
        {
            Action action = () => ((IDictionary<Guid, Person>)null!).SetEach(x => { });

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachIEnumerableRunsActionAgainstInstance()
        {
            var index = 0;
            var sut = BuildPeople();

            var actual = sut.SetEach(
                x =>
                {
                    index++;
                    x.Priority = index;
                });

            actual.Count.Should().Be(2);
            actual[0].Priority.Should().Be(1);
            actual[1].Priority.Should().Be(2);
        }

        [Fact]
        public void SetEachIEnumerableThrowsExceptionWithNullAction()
        {
            IEnumerable<string> sut = new Collection<string>();

            Action action = () => sut.SetEach(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachIEnumerableThrowsExceptionWithNullInstance()
        {
            Action action = () => ((IEnumerable<string>)null!).SetEach(x => { });

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachIListRunsActionAgainstInstance()
        {
            var index = 0;
            IList<Person> sut = new List<Person>
            {
                new Person(),
                new Person()
            };

            var actual = sut.SetEach(
                x =>
                {
                    index++;
                    x.Priority = index;
                });

            actual.Should().BeSameAs(sut);
            actual.Count.Should().Be(2);
            actual.First().Priority.Should().Be(1);
            actual.Skip(1).First().Priority.Should().Be(2);
        }

        [Fact]
        public void SetEachIListThrowsExceptionWithNullAction()
        {
            IList<string> sut = new List<string>();

            Action action = () => sut.SetEach(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachIListThrowsExceptionWithNullInstance()
        {
            Action action = () => ((IList<string>)null!).SetEach(x => { });

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachIReadOnlyCollectionRunsActionAgainstInstance()
        {
            var index = 0;
            var source = new List<Person>
            {
                new Person(),
                new Person()
            };
            IReadOnlyCollection<Person> sut = new ReadOnlyCollection<Person>(source);

            var actual = sut.SetEach(
                x =>
                {
                    index++;
                    x.Priority = index;
                });

            actual.Should().BeSameAs(sut);
            actual.Count.Should().Be(2);
            actual.First().Priority.Should().Be(1);
            actual.Skip(1).First().Priority.Should().Be(2);
        }

        [Fact]
        public void SetEachIReadOnlyCollectionThrowsExceptionWithNullAction()
        {
            var data = new List<Person>();

            IReadOnlyCollection<Person> sut = new ReadOnlyCollection<Person>(data);

            Action action = () => sut.SetEach(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachIReadOnlyCollectionThrowsExceptionWithNullInstance()
        {
            Action action = () => ((IReadOnlyCollection<Person>)null!).SetEach(x => { });

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachIReadOnlyDictionaryRunsActionAgainstInstance()
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

            var actual = sut.SetEach(x => { x.Value.Id = x.Key; });

            actual.Should().BeSameAs(sut);
            actual.Count.Should().Be(2);
            actual.First().Key.Should().Be(actual.First().Value.Id);
            actual.Skip(1).First().Key.Should().Be(actual.Skip(1).First().Value.Id);
        }

        [Fact]
        public void SetEachIReadOnlyDictionaryThrowsExceptionWithNullAction()
        {
            IReadOnlyDictionary<Guid, Person> sut = new Dictionary<Guid, Person>();

            Action action = () => sut.SetEach(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachIReadOnlyDictionaryThrowsExceptionWithNullInstance()
        {
            Action action = () => ((IReadOnlyDictionary<Guid, Person>)null!).SetEach(x => { });

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachIReadOnlyListRunsActionAgainstInstance()
        {
            var index = 0;
            var source = new List<Person>
            {
                new Person(),
                new Person()
            };
            IReadOnlyList<Person> sut = new ReadOnlyCollection<Person>(source);

            var actual = sut.SetEach(
                x =>
                {
                    index++;
                    x.Priority = index;
                });

            actual.Should().BeSameAs(sut);
            actual.Count.Should().Be(2);
            actual[0].Priority.Should().Be(1);
            actual[1].Priority.Should().Be(2);
        }

        [Fact]
        public void SetEachIReadOnlyListThrowsExceptionWithNullAction()
        {
            var data = new List<Person>();

            IReadOnlyList<Person> sut = new ReadOnlyCollection<Person>(data);

            Action action = () => sut.SetEach(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachIReadOnlyListThrowsExceptionWithNullInstance()
        {
            Action action = () => ((IReadOnlyList<Person>)null!).SetEach(x => { });

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachListAsIEnumerableRunsActionAgainstInstance()
        {
            var index = 0;
            var sut = new List<Person>
            {
                new Person(),
                new Person()
            };

            var actual = sut.SetEach(
                x =>
                {
                    index++;
                    x.Priority = index;
                });

            actual.Should().BeSameAs(sut);
            actual.Count.Should().Be(2);
            actual[0].Priority.Should().Be(1);
            actual[1].Priority.Should().Be(2);
        }

        [Fact]
        public void SetEachListThrowsExceptionWithNullAction()
        {
            var sut = new List<string>();

            Action action = () => sut.SetEach(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachListThrowsExceptionWithNullInstance()
        {
            Action action = () => ((List<Person>)null!).SetEach(x => { });

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachReadOnlyCollectionRunsActionAgainstInstance()
        {
            var index = 0;
            var source = new List<Person>
            {
                new Person(),
                new Person()
            };
            var sut = new ReadOnlyCollection<Person>(source);

            var actual = sut.SetEach(
                x =>
                {
                    index++;
                    x.Priority = index;
                });

            actual.Should().BeSameAs(sut);
            actual.Count.Should().Be(2);
            actual.First().Priority.Should().Be(1);
            actual.Skip(1).First().Priority.Should().Be(2);
        }

        [Fact]
        public void SetEachReadOnlyCollectionThrowsExceptionWithNullAction()
        {
            var data = new List<Person>();

            var sut = new ReadOnlyCollection<Person>(data);

            Action action = () => sut.SetEach(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachReadOnlyCollectionThrowsExceptionWithNullInstance()
        {
            Action action = () => ((ReadOnlyCollection<Person>)null!).SetEach(x => { });

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachReadOnlyDictionaryRunsActionAgainstInstance()
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

            var actual = sut.SetEach(x => { x.Value.Id = x.Key; });

            actual.Should().BeSameAs(sut);
            actual.Count.Should().Be(2);
            actual.First().Key.Should().Be(actual.First().Value.Id);
            actual.Skip(1).First().Key.Should().Be(actual.Skip(1).First().Value.Id);
        }

        [Fact]
        public void SetEachReadOnlyDictionaryThrowsExceptionWithNullAction()
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

            Action action = () => sut.SetEach(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachReadOnlyDictionaryThrowsExceptionWithNullInstance()
        {
            Action action = () => ((ReadOnlyDictionary<Guid, Person>)null!).SetEach(x => { });

            action.Should().Throw<ArgumentNullException>();
        }

        private static IEnumerable<Person> BuildPeople()
        {
            yield return new Person
            {
                Id = Guid.NewGuid()
            };
            yield return new Person
            {
                Id = Guid.NewGuid()
            };
        }
    }
}