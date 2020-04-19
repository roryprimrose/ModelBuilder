namespace ModelBuilder.UnitTests.IgnoreRules
{
    using System;
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

        [Fact]
        public void IsMatchThrowsExceptionWithNullProperty()
        {
            var sut = new RegexIgnoreRule(NameExpression.FirstName);

            Action action = () => sut.IsMatch(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullExpression()
        {
            Action action = () => new RegexIgnoreRule(null);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}