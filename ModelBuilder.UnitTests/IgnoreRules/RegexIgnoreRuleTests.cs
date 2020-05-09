namespace ModelBuilder.UnitTests.IgnoreRules
{
    using System;
    using System.Text.RegularExpressions;
    using FluentAssertions;
    using ModelBuilder.IgnoreRules;
    using ModelBuilder.UnitTests.Models;
    using Xunit;

    public class RegexIgnoreRuleTests
    {
        [Fact]
        public void IsMatchReturnsFalseWhenPropertyDoesNotMatch()
        {
            var property = typeof(Person).GetProperty(nameof(Person.FirstName));

            var sut = new RegexIgnoreRule(NameExpression.LastName);

            var actual = sut.IsMatch(property);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchReturnsTrueWhenPropertyMatches()
        {
            var property = typeof(Person).GetProperty(nameof(Person.FirstName));

            var sut = new RegexIgnoreRule(NameExpression.FirstName);

            var actual = sut.IsMatch(property);

            actual.Should().BeTrue();
        }

        [Theory]
        [InlineData("First", true)]
        [InlineData("Last", true)]
        public void IsMatchReturnsWhenPropertyMatchesExpression(string expression, bool expected)
        {
            var property = typeof(Person).GetProperty(nameof(Person.FirstName));

            var sut = new RegexIgnoreRule(expression);

            var actual = sut.IsMatch(property);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsMatchThrowsExceptionWithNullProperty()
        {
            var sut = new RegexIgnoreRule(NameExpression.FirstName);

            Action action = () => sut.IsMatch(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ThrowsExceptionWhenCreatedWithInvalidExpression(string expression)
        {
            Action action = () => new RegexIgnoreRule(expression);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullExpression()
        {
            Action action = () => new RegexIgnoreRule((Regex) null);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}