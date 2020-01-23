namespace ModelBuilder.UnitTests.ExecuteOrderRules
{
    using System;
    using FluentAssertions;
    using ModelBuilder.ExecuteOrderRules;
    using ModelBuilder.UnitTests.Models;
    using Xunit;

    public class RegexExecuteOrderRuleTests
    {
        [Fact]
        public void IsMatchReturnsFalseWhenPropertyDoesNotMatch()
        {
            var priority = Environment.TickCount;
            var property = typeof(Person).GetProperty(nameof(Person.FirstName));

            var sut = new RegexExecuteOrderRule(PropertyExpression.LastName, priority);

            var actual = sut.IsMatch(property);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchReturnsTrueWhenPropertyMatches()
        {
            var priority = Environment.TickCount;
            var property = typeof(Person).GetProperty(nameof(Person.FirstName));

            var sut = new RegexExecuteOrderRule(PropertyExpression.FirstName, priority);

            var actual = sut.IsMatch(property);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsMatchThrowsExceptionWithNullPropertyTest()
        {
            var priority = Environment.TickCount;

            var sut = new RegexExecuteOrderRule(PropertyExpression.FirstName, priority);

            Action action = () => sut.IsMatch(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PriorityReturnsConstructorValue()
        {
            var priority = Environment.TickCount;

            var sut = new RegexExecuteOrderRule(PropertyExpression.LastName, priority);

            sut.Priority.Should().Be(priority);
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullExpressionTest()
        {
            var priority = Environment.TickCount;

            Action action = () => new RegexExecuteOrderRule(null, priority);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}