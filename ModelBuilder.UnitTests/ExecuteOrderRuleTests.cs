namespace ModelBuilder.UnitTests
{
    using System;
    using System.Text.RegularExpressions;
    using FluentAssertions;
    using Xunit;

    public class ExecuteOrderRuleTests
    {
        [Fact]
        public void IsMatchPropertyTypeOnlyReturnsWhetherEvaluatorMatchesTest()
        {
            var target = new ExecuteOrderRule((type, name) => false, 1000);

            var actual = target.IsMatch(typeof(string), "Stuff");

            actual.Should().BeFalse();
        }

        [Theory]
        [InlineData(typeof(string), "stuff", typeof(int), "no", false)]
        [InlineData(typeof(string), "stuff", typeof(string), "no", false)]
        [InlineData(typeof(string), "stuff", typeof(int), "stuff", false)]
        [InlineData(typeof(string), "stuff", typeof(string), "stuff", true)]
        public void IsMatchPropertyTypeOnlyReturnsWhetherTypeAndNameMatchTest(
            Type type,
            string name,
            Type matchType,
            string matchName,
            bool expected)
        {
            var priority = Environment.TickCount;

            var target = new ExecuteOrderRule(type, name, priority);

            var actual = target.IsMatch(matchType, matchName);

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(typeof(string), "stuff", typeof(int), "no", false)]
        [InlineData(typeof(string), "stuff", typeof(string), "no", false)]
        [InlineData(typeof(string), "stuff", typeof(int), "stuff", false)]
        [InlineData(typeof(string), "stuff", typeof(string), "stuff", true)]
        [InlineData(typeof(string), "First.+", typeof(string), "FirstName", true)]
        [InlineData(typeof(string), "(F|f)irst[_]?(N|n)ame", typeof(string), "FirstName", true)]
        [InlineData(typeof(string), "(F|f)irst[_]?(N|n)ame", typeof(string), "First_Name", true)]
        [InlineData(typeof(string), "(F|f)irst[_]?(N|n)ame", typeof(string), "First_name", true)]
        [InlineData(typeof(string), "(F|f)irst[_]?(N|n)ame", typeof(string), "Firstname", true)]
        [InlineData(typeof(string), "(F|f)irst[_]?(N|n)ame", typeof(string), "first_name", true)]
        [InlineData(typeof(string), "(F|f)irst[_]?(N|n)ame", typeof(string), "first_Name", true)]
        [InlineData(typeof(string), "(F|f)irst[_]?(N|n)ame", typeof(string), "firstname", true)]
        public void IsMatchPropertyTypeOnlyReturnsWhetherTypeAndRegularExpressionMatchTest(
            Type type,
            string expression,
            Type matchType,
            string matchName,
            bool expected)
        {
            var priority = Environment.TickCount;
            var regex = new Regex(expression);

            var target = new ExecuteOrderRule(type, regex, priority);

            var actual = target.IsMatch(matchType, matchName);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsMatchReturnsWhetherEvaluatorMatchesTest()
        {
            var target = new ExecuteOrderRule((declaringType, propertyType, name) => false, 1000);

            var actual = target.IsMatch(typeof(Person), typeof(string), "Stuff");

            actual.Should().BeFalse();
        }

        [Theory]
        [InlineData(typeof(Person), typeof(string), "stuff", typeof(Person), typeof(int), "no", false)]
        [InlineData(typeof(Person), typeof(string), "stuff", typeof(Person), typeof(string), "no", false)]
        [InlineData(typeof(Person), typeof(string), "stuff", typeof(Person), typeof(int), "stuff", false)]
        [InlineData(typeof(Person), typeof(string), "stuff", typeof(Company), typeof(string), "stuff", false)]
        [InlineData(typeof(Person), typeof(string), "stuff", typeof(Person), typeof(string), "stuff", true)]
        public void IsMatchReturnsWhetherTypeAndNameMatchTest(
            Type declaringType,
            Type propertyType,
            string name,
            Type matchDeclaringType,
            Type matchPropertyType,
            string matchName,
            bool expected)
        {
            var priority = Environment.TickCount;

            var target = new ExecuteOrderRule(declaringType, propertyType, name, priority);

            var actual = target.IsMatch(matchDeclaringType, matchPropertyType, matchName);

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(typeof(Person), typeof(string), "stuff", typeof(Person), typeof(int), "no", false)]
        [InlineData(typeof(Person), typeof(string), "stuff", typeof(Person), typeof(string), "no", false)]
        [InlineData(typeof(Person), typeof(string), "stuff", typeof(Person), typeof(int), "stuff", false)]
        [InlineData(typeof(Person), typeof(string), "stuff", typeof(Company), typeof(string), "stuff", false)]
        [InlineData(typeof(Person), typeof(string), "First.+", typeof(Company), typeof(string), "FirstName", false)]
        [InlineData(
            typeof(Person),
            typeof(string),
            "(F|f)irst[_]?(N|n)ame",
            typeof(Company),
            typeof(string),
            "FirstName",
            false)]
        [InlineData(
            typeof(Person),
            typeof(string),
            "(F|f)irst[_]?(N|n)ame",
            typeof(Company),
            typeof(string),
            "First_Name",
            false)]
        [InlineData(
            typeof(Person),
            typeof(string),
            "(F|f)irst[_]?(N|n)ame",
            typeof(Company),
            typeof(string),
            "First_name",
            false)]
        [InlineData(
            typeof(Person),
            typeof(string),
            "(F|f)irst[_]?(N|n)ame",
            typeof(Company),
            typeof(string),
            "Firstname",
            false)]
        [InlineData(
            typeof(Person),
            typeof(string),
            "(F|f)irst[_]?(N|n)ame",
            typeof(Company),
            typeof(string),
            "first_name",
            false)]
        [InlineData(
            typeof(Person),
            typeof(string),
            "(F|f)irst[_]?(N|n)ame",
            typeof(Company),
            typeof(string),
            "first_Name",
            false)]
        [InlineData(
            typeof(Person),
            typeof(string),
            "(F|f)irst[_]?(N|n)ame",
            typeof(Company),
            typeof(string),
            "firstname",
            false)]
        [InlineData(typeof(Person), typeof(string), "stuff", typeof(Person), typeof(string), "stuff", true)]
        [InlineData(typeof(Person), typeof(string), "First.+", typeof(Person), typeof(string), "FirstName", true)]
        [InlineData(
            typeof(Person),
            typeof(string),
            "(F|f)irst[_]?(N|n)ame",
            typeof(Person),
            typeof(string),
            "FirstName",
            true)]
        [InlineData(
            typeof(Person),
            typeof(string),
            "(F|f)irst[_]?(N|n)ame",
            typeof(Person),
            typeof(string),
            "First_Name",
            true)]
        [InlineData(
            typeof(Person),
            typeof(string),
            "(F|f)irst[_]?(N|n)ame",
            typeof(Person),
            typeof(string),
            "First_name",
            true)]
        [InlineData(
            typeof(Person),
            typeof(string),
            "(F|f)irst[_]?(N|n)ame",
            typeof(Person),
            typeof(string),
            "Firstname",
            true)]
        [InlineData(
            typeof(Person),
            typeof(string),
            "(F|f)irst[_]?(N|n)ame",
            typeof(Person),
            typeof(string),
            "first_name",
            true)]
        [InlineData(
            typeof(Person),
            typeof(string),
            "(F|f)irst[_]?(N|n)ame",
            typeof(Person),
            typeof(string),
            "first_Name",
            true)]
        [InlineData(
            typeof(Person),
            typeof(string),
            "(F|f)irst[_]?(N|n)ame",
            typeof(Person),
            typeof(string),
            "firstname",
            true)]
        public void IsMatchReturnsWhetherTypeAndRegularExpressionMatchTest(
            Type declaringType,
            Type propertyType,
            string expression,
            Type matchDeclaringType,
            Type matchPropertyType,
            string matchName,
            bool expected)
        {
            var priority = Environment.TickCount;
            var regex = new Regex(expression);

            var target = new ExecuteOrderRule(declaringType, propertyType, regex, priority);

            var actual = target.IsMatch(matchDeclaringType, matchPropertyType, matchName);

            actual.Should().Be(expected);
        }

        [Fact]
        public void ReturnsConstructorValuesPropertyTypeOnlyTest()
        {
            var type = typeof(string);
            var name = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var target = new ExecuteOrderRule(type, name, priority);

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
        public void ThrowsExceptionWhenCreatedWithNullFunctionTest()
        {
            var priority = Environment.TickCount;

            Action action = () => new ExecuteOrderRule((Func<Type, Type, string, bool>)null, priority);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullDeclaringTypePropertyTypeAndNameTest()
        {
            var priority = Environment.TickCount;

            Action action = () => new ExecuteOrderRule(null, null, (string)null, priority);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullDeclaringTypePropertyTypeAndRegularExpressionTest()
        {
            var priority = Environment.TickCount;

            Action action = () => new ExecuteOrderRule(null, null, (Regex)null, priority);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullPropertyTypeAndNameTest()
        {
            var priority = Environment.TickCount;

            Action action = () => new ExecuteOrderRule(null, (string)null, priority);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullPropertyTypeAndRegularExpressionTest()
        {
            var priority = Environment.TickCount;

            Action action = () => new ExecuteOrderRule(null, (Regex)null, priority);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullPropertyTypeOnlyFunctionTest()
        {
            var priority = Environment.TickCount;

            Action action = () => new ExecuteOrderRule((Func<Type, string, bool>)null, priority);

            action.ShouldThrow<ArgumentNullException>();
        }
    }
}