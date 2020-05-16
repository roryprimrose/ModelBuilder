namespace ModelBuilder.UnitTests
{
    using System;
    using System.Globalization;
    using System.Linq.Expressions;
    using System.Reflection;
    using FluentAssertions;
    using ModelBuilder.UnitTests.Models;
    using Xunit;

    public class ExpressionExtensionTests
    {
        [Fact]
        public void GetPropertyReturnsPropertyInfoOfExpression()
        {
            var actual = Wrapper<Person>(x => x.Priority);

            actual.Name.Should().Be("Priority");
            actual.PropertyType.Should().Be<int>();
        }

        [Fact]
        public void GetPropertyThrowsExceptionForStaticProperty()
        {
            Action action = () => Wrapper<WithStatic>(x => WithStatic.Second!);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GetPropertyThrowsExceptionForStaticReadOnlyProperty()
        {
            Action action = () => Wrapper<WithStatic>(x => StaticGetter.Value);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GetPropertyThrowsExceptionWhenPropertyNotOnTargetType()
        {
            Action action = () => Wrapper<Person>(x => x.Priority.ToString(CultureInfo.InvariantCulture).Length);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GetPropertyThrowsExceptionWithFieldExpression()
        {
            Action action = () => Wrapper<Person>(x => x.MinAge);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GetPropertyThrowsExceptionWithMethodExpression()
        {
            Action action = () => Wrapper<Person>(x => x.DoSomething()!);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GetPropertyThrowsExceptionWithNullExpression()
        {
            Action action = () => Wrapper<Person>(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        private PropertyInfo Wrapper<T>(Expression<Func<T, object>> expression)
        {
            return expression.GetProperty();
        }

        private class StaticGetter
        {
            public static string Value { get { return Guid.NewGuid().ToString(); } }
        }
    }
}