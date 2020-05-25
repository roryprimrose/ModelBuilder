namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
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
            Action action = () => ((Type) null!).IsNullable();

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
            var values = Model.Create<List<int>>()!;

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
            Action action = () => CommonExtensions.Next<int>(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetRunsActionAgainstInstance()
        {
            var value = Guid.NewGuid();
            var sut = new Person();

            var actual = sut.Set(x => x.Id = value);

            actual.Should().BeSameAs(sut);
            actual.Id.Should().Be(value);
        }

        [Fact]
        public void SetThrowsExceptionWithNullAction()
        {
            var sut = new Person();

            Action action = () => sut.Set(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetThrowsExceptionWithNullInstance()
        {
            Action action = () => ((object) null!).Set(x => { });

            action.Should().Throw<ArgumentNullException>();
        }
    }
}