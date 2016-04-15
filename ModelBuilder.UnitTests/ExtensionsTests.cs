using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace ModelBuilder.UnitTests
{
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
        public void IsNullableThrowsExceptionWithNullTypeTest()
        {
            Type target = null;

            Action action = () => target.IsNullable();

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void SetEachCollectionRunsActionAgainstInstanceTest()
        {
            var index = 0;
            var target = new Collection<Person>
            {
                new Person(),
                new Person()
            };

            var actual = target.SetEach(x =>
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
        public void SetEachCollectionThrowsExceptionWithNullActionTest()
        {
            var target = new Collection<string>();

            Action action = () => target.SetEach(null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void SetEachCollectionThrowsExceptionWithNullInstanceTest()
        {
            Collection<string> target = null;

            Action action = () => target.SetEach(x => { });

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void SetEachDictionaryRunsActionAgainstInstanceTest()
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
        public void SetEachEnumerableThrowsExceptionWithNullActionTest()
        {
            IEnumerable<string> target = new Collection<string>();

            Action action = () => target.SetEach(null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void SetEachIEnumerableRunsActionAgainstInstanceTest()
        {
            var index = 0;
            var target = BuildPeople();

            var actual = target.SetEach(x =>
            {
                index++;
                x.Priority = index;
            });

            actual.Count.Should().Be(2);
            actual[0].Priority.Should().Be(1);
            actual[1].Priority.Should().Be(2);
        }

        [Fact]
        public void SetEachIEnumerableThrowsExceptionWithNullInstanceTest()
        {
            IEnumerable<string> target = null;

            Action action = () => target.SetEach(x => { });

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void SetEachListAsIEnumerableRunsActionAgainstInstanceTest()
        {
            var index = 0;
            var target = new List<Person>
            {
                new Person(),
                new Person()
            };

            var actual = target.SetEach(x =>
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
        public void SetEachListThrowsExceptionWithNullActionTest()
        {
            var target = new List<string>();

            Action action = () => target.SetEach(null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void SetEachListThrowsExceptionWithNullInstanceTest()
        {
            List<string> target = null;

            Action action = () => target.SetEach(x => { });

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void SetEachReadOnlyCollectionRunsActionAgainstInstanceTest()
        {
            var index = 0;
            var source = new List<Person>
            {
                new Person(),
                new Person()
            };
            var target = new ReadOnlyCollection<Person>(source);

            var actual = target.SetEach(x =>
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
        public void SetRunsActionAgainstInstanceTest()
        {
            var value = Guid.NewGuid();
            var target = new Person();

            var actual = target.Set(x => x.Id = value);

            actual.Should().BeSameAs(target);
            actual.Id.Should().Be(value);
        }

        [Fact]
        public void SetThrowsExceptionWithNullActionTest()
        {
            var target = new Person();

            Action action = () => target.Set(null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void SetThrowsExceptionWithNullInstanceTest()
        {
            object target = null;

            Action action = () => target.Set(x => { });

            action.ShouldThrow<ArgumentNullException>();
        }

        private IEnumerable<Person> BuildPeople()
        {
            yield return new Person {Id = Guid.NewGuid()};
            yield return new Person {Id = Guid.NewGuid()};
        }
    }
}