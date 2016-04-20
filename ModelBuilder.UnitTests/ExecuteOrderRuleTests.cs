using System;
using FluentAssertions;
using Xunit;

namespace ModelBuilder.UnitTests
{
    public class ExecuteOrderRuleTests
    {
        [Theory]
        [InlineData(typeof(string), "stuff", typeof(int), "no", false)]
        [InlineData(typeof(string), "stuff", typeof(string), "no", false)]
        [InlineData(typeof(string), "stuff", typeof(int), "stuff", false)]
        [InlineData(typeof(string), "stuff", typeof(string), "stuff", true)]
        public void IsMatchReturnsWhetherTypeAndNameMatchTest(Type type, string name, Type matchType,
            string matchName, bool expected)
        {
            var priority = Environment.TickCount;

            var target = new ExecuteOrderRule(type, name, priority);

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

            Action action = () => new ExecuteOrderRule(null, null, priority);

            action.ShouldThrow<ArgumentNullException>();
        }
    }
}