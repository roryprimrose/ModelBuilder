namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder.UnitTests.Models;
    using Xunit;

    public class ExtensionsTests
    {
        [Theory]
        [InlineData(typeof(string), false)]
        [InlineData(typeof(bool), false)]
        [InlineData(typeof(int), false)]
        [InlineData(typeof(List<int>), false)]
        [InlineData(typeof(bool?), true)]
        [InlineData(typeof(int?), true)]
        [InlineData(typeof(byte?), true)]
        [InlineData(typeof(Guid?), true)]
        public void IsNullableReturnsWhetherTypeIsNullableTest(Type type, bool expected)
        {
            var actual = type.IsNullable();

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsNullableThrowsExceptionWithNullType()
        {
            Action action = () => ((Type) null).IsNullable();

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void NextReturnsDefaultValueWhenListIsEmpty()
        {
            var values = new List<Location>();

            var actual = values.Next();

            actual.Should().BeNull();
        }

        [Fact]
        public void NextReturnsRandomValue()
        {
            var values = Model.Create<List<int>>();

            var first = values.Next();
            var second = first;

            for (var index = 0; index < 1000; index++)
            {
                second = values.Next();

                if (first != second)
                {
                    break;
                }
            }

            first.Should().NotBe(second);
        }

        [Fact]
        public void NextThrowsExceptionWithNullSource()
        {
            Action action = () => ModelBuilder.Extensions.Next<int>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachCollectionRunsActionAgainstInstance()
        {
            var index = 0;
            var target = new Collection<Person>
            {
                new Person(),
                new Person()
            };

            var actual = target.SetEach(
                x =>
                {
                    index++;
                    x.Priority = index;
                });

            actual.Should().BeSameAs(target);
            actual.Count.Should().Be(2);
            actual.First().Priority.Should().Be(1);
            actual.Skip(1).First().Priority.Should().Be(2);
        }

        [Fact]
        public void SetEachCollectionThrowsExceptionWithNullAction()
        {
            var target = new Collection<string>();

            Action action = () => target.SetEach(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachCollectionThrowsExceptionWithNullInstance()
        {
            Action action = () => ((Collection<string>) null).SetEach(x => { });

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachDictionaryRunsActionAgainstInstance()
        {
            var target = new Dictionary<Guid, Person>
            {
                {
                    Guid.NewGuid(), new Person()
                },
                {
                    Guid.NewGuid(), new Person()
                }
            };

            var actual = target.SetEach(x => { x.Value.Id = x.Key; });

            actual.Should().BeSameAs(target);
            actual.Count.Should().Be(2);
            actual.First().Key.Should().Be(actual.First().Value.Id);
            actual.Skip(1).First().Key.Should().Be(actual.Skip(1).First().Value.Id);
        }

        [Fact]
        public void SetEachEnumerableThrowsExceptionWithNullAction()
        {
            IEnumerable<string> target = new Collection<string>();

            Action action = () => target.SetEach(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachIEnumerableRunsActionAgainstInstance()
        {
            var index = 0;
            var target = BuildPeople();

            var actual = target.SetEach(
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
        public void SetEachIEnumerableThrowsExceptionWithNullInstance()
        {
            Action action = () => ((IEnumerable<string>) null).SetEach(x => { });

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachListAsIEnumerableRunsActionAgainstInstance()
        {
            var index = 0;
            var target = new List<Person>
            {
                new Person(),
                new Person()
            };

            var actual = target.SetEach(
                x =>
                {
                    index++;
                    x.Priority = index;
                });

            actual.Should().BeSameAs(target);
            actual.Count.Should().Be(2);
            actual[0].Priority.Should().Be(1);
            actual[1].Priority.Should().Be(2);
        }

        [Fact]
        public void SetEachListThrowsExceptionWithNullAction()
        {
            var target = new List<string>();

            Action action = () => target.SetEach(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetEachListThrowsExceptionWithNullInstance()
        {
            Action action = () => ((List<string>) null).SetEach(x => { });

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
            var target = new ReadOnlyCollection<Person>(source);

            var actual = target.SetEach(
                x =>
                {
                    index++;
                    x.Priority = index;
                });

            actual.Should().BeSameAs(target);
            actual.Count.Should().Be(2);
            actual.First().Priority.Should().Be(1);
            actual.Skip(1).First().Priority.Should().Be(2);
        }

        [Fact]
        public void SetRunsActionAgainstInstance()
        {
            var value = Guid.NewGuid();
            var target = new Person();

            var actual = target.Set(x => x.Id = value);

            actual.Should().BeSameAs(target);
            actual.Id.Should().Be(value);
        }

        [Fact]
        public void SetThrowsExceptionWithNullAction()
        {
            var target = new Person();

            Action action = () => target.Set(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetThrowsExceptionWithNullInstance()
        {
            Action action = () => ((object) null).Set(x => { });

            action.Should().Throw<ArgumentNullException>();
        }

        private IEnumerable<Person> BuildPeople()
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