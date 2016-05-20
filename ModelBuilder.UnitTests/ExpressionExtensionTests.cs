using System;
using System.Linq.Expressions;
using System.Reflection;
using FluentAssertions;
using Xunit;

namespace ModelBuilder.UnitTests
{
    public class ExpressionExtensionTests
    {
        [Fact]
        public void GetPropertyReturnsPropertyInfoOfExpressionTest()
        {
            var actual = Wrapper<Person>(x => x.Priority);

            actual.Name.Should().Be("Priority");
            actual.PropertyType.Should().Be<int>();
        }

        [Fact]
        public void GetPropertyThrowsExceptionWhenPropertyNotOnTargetTypeTest()
        {
            Action action = () => Wrapper<Person>(x => x.Priority.ToString().Length);

            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void GetPropertyThrowsExceptionWithFieldExpressionTest()
        {
            Action action = () => Wrapper<Person>(x => x.MinAge);

            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void GetPropertyThrowsExceptionWithMethodExpressionTest()
        {
            Action action = () => Wrapper<Person>(x => x.DoSomething());

            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void GetPropertyThrowsExceptionWithNullExpressionTest()
        {
            Action action = () => Wrapper<Person>(null);

            action.ShouldThrow<ArgumentNullException>();
        }

        private PropertyInfo Wrapper<T>(Expression<Func<T, object>> expression)
        {
            return expression.GetProperty();
        }
    }
}