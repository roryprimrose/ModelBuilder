namespace ModelBuilder.UnitTests.IgnoreRules
{
    using System;
    using FluentAssertions;
    using ModelBuilder.IgnoreRules;
    using ModelBuilder.UnitTests.Models;
    using Xunit;

    public class ExpressionIgnoreRuleTests
    {
        [Fact]
        public void IsMatchReturnsFalseWhenPropertyDoesNotMatch()
        {
            var property = typeof(Person).GetProperty(nameof(Person.FirstName));

            var sut = new ExpressionIgnoreRule<Person>(x => x.LastName);

            var actual = sut.IsMatch(property);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchReturnsTrueWhenPropertyMatches()
        {
            var property = typeof(Person).GetProperty(nameof(Person.FirstName));

            var sut = new ExpressionIgnoreRule<Person>(x => x.FirstName);

            var actual = sut.IsMatch(property);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsMatchThrowsExceptionWithNullPropertyTest()
        {
            var sut = new ExpressionIgnoreRule<Person>(x => x.FirstName);

            Action action = () => sut.IsMatch(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullExpressionTest()
        {
            Action action = () => new ExpressionIgnoreRule<Person>(null);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}