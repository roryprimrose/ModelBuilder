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
            Action action = () => ((Type)null!).IsNullable();

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
        public void SetExpressionAutoInitSetter()
        {
            var sut = new PropertySetters();
            var expected = Model.Create<ConsoleColor>();

            var actual = sut.Set(x => x.AutoInit, expected);

            actual.AutoInit.Should().Be(expected);
            sut.AutoInit.Should().Be(expected);
        }

        [Fact]
        public void SetExpressionAutoInternalSetter()
        {
            var sut = new PropertySetters();
            var expected = Model.Create<DateTimeOffset>();

            var actual = sut.Set(x => x.AutoInternal, expected);

            actual.AutoInternal.Should().Be(expected);
            sut.AutoInternal.Should().Be(expected);
        }

        [Fact]
        public void SetExpressionAutoPrivateInternalSetter()
        {
            var sut = new PropertySetters();
            var expected = Model.Create<PropertySetters>();

            var actual = sut.Set(x => x.AutoPrivateInternal, expected);

            actual.AutoPrivateInternal.Should().BeEquivalentTo(expected);
            sut.AutoPrivateInternal.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void SetExpressionAutoPrivateSetter()
        {
            var sut = new PropertySetters();
            var expected = Model.Create<int>();

            var actual = sut.Set(x => x.AutoPrivate, expected);

            actual.AutoPrivate.Should().Be(expected);
            sut.AutoPrivate.Should().Be(expected);
        }

        [Fact]
        public void SetExpressionAutoProtectedInternalSetter()
        {
            var sut = new PropertySetters();
            var expected = Model.Create<Uri>();

            var actual = sut.Set(x => x.AutoProtectedInternal, expected);

            actual.AutoProtectedInternal.Should().BeEquivalentTo(expected);
            sut.AutoProtectedInternal.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void SetExpressionAutoProtectedSetter()
        {
            var sut = new PropertySetters();
            var expected = Model.Create<decimal>();

            var actual = sut.Set(x => x.AutoProtected, expected);

            actual.AutoProtected.Should().Be(expected);
            sut.AutoProtected.Should().Be(expected);
        }

        [Fact]
        public void SetExpressionAutoPublicSetter()
        {
            var sut = new PropertySetters();
            var expected = Guid.NewGuid();

            var actual = sut.Set(x => x.AutoPublic, expected);

            actual.AutoPublic.Should().Be(expected);
            sut.AutoPublic.Should().Be(expected);
        }

        [Fact]
        public void SetExpressionAutoReadonlySetter()
        {
            var sut = new PropertySetters();

            var actual = sut.Set(x => x.AutoReadonly, null);

            actual.AutoReadonly.Should().BeNull();
            sut.AutoReadonly.Should().BeNull();
        }

        [Fact]
        public void SetExpressionBackingField()
        {
            var sut = new PropertySetters();
            var expected = Model.Create<float>();

            var actual = sut.Set(x => x._backingField, expected);

            actual._backingField.Should().Be(expected);
            sut._backingField.Should().Be(expected);
        }

        [Fact]
        public void SetExpressionComplexExpressionThrowsException()
        {
            var sut = new PropertySetters();

            Action action = () => sut.Set(x => x.AutoPublic.ToString(), string.Empty);

            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void SetExpressionMethodThrowsException()
        {
            var sut = new PropertySetters();

            Action action = () => sut.Set(x => x.BackingFieldMethod(), default(float));

            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void SetExpressionPrivateBackingFieldSetter()
        {
            var sut = new PropertySetters();
            var expected = Model.Create<float>();

            var actual = sut.Set(x => x.PrivateBackingField, expected);

            actual.PrivateBackingField.Should().Be(expected);
            sut.PrivateBackingField.Should().Be(expected);
        }

        [Fact]
        public void SetExpressionPublicBackingFieldSetter()
        {
            var sut = new PropertySetters();
            var expected = Model.Create<float>();

            var actual = sut.Set(x => x.PublicBackingField, expected);

            actual.PublicBackingField.Should().Be(expected);
            sut.PublicBackingField.Should().Be(expected);
        }

        [Fact]
        public void SetExpressionReadonlySetterThrowsException()
        {
            var sut = new PropertySetters();

            Action action = () => sut.Set(x => x.Readonly, null);

            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void SetExpressionThrowsExceptionWithNullAction()
        {
            var sut = new PropertySetters();

            Action action = () => sut.Set(null!, true);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SetExpressionThrowsExceptionWithNullInstance()
        {
            var sut = new PropertySetters();

            Action action = () => ((PropertySetters)null!).Set(x => x.AutoPublic, Guid.Empty);

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
            Action action = () => ((object)null!).Set(x => { });

            action.Should().Throw<ArgumentNullException>();
        }
    }
}