namespace ModelBuilder.UnitTests
{
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using FluentAssertions;
    using ModelBuilder.UnitTests.Models;
    using Xunit;

    public class ExecuteOrderRuleTests
    {
        [Theory]
        [InlineData(typeof(string), "stuff", false)]
        [InlineData(typeof(Stream), "First.+", false)]
        [InlineData(typeof(string), "First.+", true)]
        [InlineData(typeof(string), "(F|f)irst[_]?(N|n)ame", true)]
        public void IsMatchPropertyTypeOnlyReturnsWhetherTypeAndRegularExpressionMatchTest(
            Type type,
            string expression,
            bool expected)
        {
            var priority = Environment.TickCount;
            var regex = new Regex(expression);

            var target = new ExecuteOrderRule(null, type, regex, priority);

            var property = typeof(Person).GetProperty(nameof(Person.FirstName));

            var actual = target.IsMatch(property);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsMatchReturnsWhetherEvaluatorMatchesTest()
        {
            var target = new ExecuteOrderRule((declaringType, propertyType, name) => false, 1000);

            var property = typeof(Person).GetProperty(nameof(Person.FirstName));

            var actual = target.IsMatch(property);

            actual.Should().BeFalse();
        }

        [Theory]
        [InlineData(typeof(Person), typeof(string), "stuff", false)]
        [InlineData(typeof(Person), typeof(Stream), "FirstName", false)]
        [InlineData(typeof(Stream), typeof(string), "stuff", false)]
        [InlineData(typeof(Person), typeof(string), "FirstName", true)]
        [InlineData(null, typeof(string), "FirstName", true)]
        public void IsMatchReturnsWhetherTypeAndNameMatchTest(
            Type declaringType,
            Type propertyType,
            string name,
            bool expected)
        {
            var priority = Environment.TickCount;

            var target = new ExecuteOrderRule(declaringType, propertyType, name, priority);

            var property = typeof(Person).GetProperty(nameof(Person.FirstName));

            var actual = target.IsMatch(property);

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(typeof(Stream), typeof(string), "FirstName", false)]
        [InlineData(typeof(Person), typeof(int), "FirstName", false)]
        [InlineData(typeof(Person), typeof(string), "stuff", false)]
        [InlineData(typeof(Person), typeof(string), "FirstName", true)]
        [InlineData(typeof(Person), typeof(string), "First.+", true)]
        [InlineData(typeof(Person), typeof(string), "(F|f)irst[_]?(N|n)ame", true)]
        public void IsMatchReturnsWhetherTypeAndRegularExpressionMatchTest(
            Type declaringType,
            Type propertyType,
            string expression,
            bool expected)
        {
            var priority = Environment.TickCount;
            var regex = new Regex(expression);

            var target = new ExecuteOrderRule(declaringType, propertyType, regex, priority);

            var property = typeof(Person).GetProperty(nameof(Person.FirstName));

            var actual = target.IsMatch(property);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsMatchThrowsExceptionWithNullProperty()
        {
            var target = new ExecuteOrderRule((declaringType, propertyType, name) => false, 1000);

            Action action = () => target.IsMatch(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ReturnsConstructorValuesPropertyTypeOnlyTest()
        {
            var declaringType = typeof(Person);
            var propertyType = typeof(string);
            var name = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var target = new ExecuteOrderRule(declaringType, propertyType, name, priority);

            target.Priority.Should().Be(priority);
        }

        [Fact]
        public void ReturnsConstructorValuesTest()
        {
            var declaringType = typeof(Person);
            var propertyType = typeof(string);
            var name = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var target = new ExecuteOrderRule(declaringType, propertyType, name, priority);

            target.Priority.Should().Be(priority);
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullDeclaringTypePropertyTypeAndNameTest()
        {
            var priority = Environment.TickCount;

            Action action = () => new ExecuteOrderRule(null, null, (string)null, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullDeclaringTypePropertyTypeAndRegularExpressionTest()
        {
            var priority = Environment.TickCount;

            Action action = () => new ExecuteOrderRule(null, null, (Regex)null, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullFunctionTest()
        {
            var priority = Environment.TickCount;

            Action action = () => new ExecuteOrderRule(null, priority);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}