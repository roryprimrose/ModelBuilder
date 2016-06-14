namespace ModelBuilder.UnitTests
{
    using System;
    using System.Text.RegularExpressions;
    using FluentAssertions;
    using Xunit;

    public class ExecuteOrderRuleTests
    {
        [Fact]
        public void IsMatchReturnsWhetherEvaluatorMatchesTest()
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
        public void IsMatchReturnsWhetherTypeAndNameMatchTest(
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
        public void IsMatchReturnsWhetherTypeAndRegularExpressionMatchTest(
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
        public void ReturnsConstructorValuesTest()
        {
            var type = typeof(string);
            var name = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var target = new ExecuteOrderRule(type, name, priority);

            target.Priority.Should().Be(priority);
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullFunctionTest()
        {
            var priority = Environment.TickCount;

            Action action = () => new ExecuteOrderRule(null, priority);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullTypeAndNameTest()
        {
            var priority = Environment.TickCount;

            Action action = () => new ExecuteOrderRule(null, (string)null, priority);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullTypeAndRegularExpressionTest()
        {
            var priority = Environment.TickCount;

            Action action = () => new ExecuteOrderRule(null, (Regex)null, priority);

            action.ShouldThrow<ArgumentNullException>();
        }
    }
}