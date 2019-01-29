namespace ModelBuilder.UnitTests
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using FluentAssertions;
    using Xunit;

    public class ExpressionExtensionTests
    {
        private class StaticGetter
        {
            public static string Value
            {
                get
                {
                    return Guid.NewGuid().ToString();
                }
            }
        }

        [Fact]
        public void GetPropertyReturnsPropertyInfoOfExpressionTest()
        {
            var actual = Wrapper<Person>(x => x.Priority);

            actual.Name.Should().Be("Priority");
            actual.PropertyType.Should().Be<int>();
        }

        [Fact]
        public void GetPropertyThrowsExceptionForStaticPropertyTest()
        {
            Action action = () => Wrapper<WithStatic>(x => WithStatic.Second);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GetPropertyThrowsExceptionForStaticReadOnlyPropertyTest()
        {
            Action action = () => Wrapper<WithStatic>(x => StaticGetter.Value);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GetPropertyThrowsExceptionWhenPropertyNotOnTargetTypeTest()
        {
            Action action = () => Wrapper<Person>(x => x.Priority.ToString().Length);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GetPropertyThrowsExceptionWithFieldExpressionTest()
        {
            Action action = () => Wrapper<Person>(x => x.MinAge);

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GetPropertyThrowsExceptionWithMethodExpressionTest()
        {
            Action action = () => Wrapper<Person>(x => x.DoSomething());

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void GetPropertyThrowsExceptionWithNullExpressionTest()
        {
            Action action = () => Wrapper<Person>(null);

            action.Should().Throw<ArgumentNullException>();
        }

        private PropertyInfo Wrapper<T>(Expression<Func<T, object>> expression)
        {
            return expression.GetProperty();
        }
    }
}