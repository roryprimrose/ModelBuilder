namespace ModelBuilder.UnitTests.IgnoreRules
{
    using System;
    using FluentAssertions;
    using ModelBuilder.IgnoreRules;
    using ModelBuilder.UnitTests.Models;
    using Xunit;

    public class PredicateIgnoreRuleTests
    {
        [Fact]
        public void IsMatchReturnsFalseWhenPropertyDeclaringTypeDoesNotMatch()
        {
            var property = typeof(Person).GetProperty(nameof(Person.LastName))!;

            var sut = new PredicateIgnoreRule(x => x.DeclaringType == typeof(string));

            var actual = sut.IsMatch(property);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchReturnsFalseWhenPropertyNameDoesNotMatch()
        {
            var property = typeof(Person).GetProperty(nameof(Person.FirstName))!;

            var sut = new PredicateIgnoreRule(x => x.Name == nameof(Person.LastName));

            var actual = sut.IsMatch(property);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchReturnsFalseWhenPropertyTypeDoesNotMatch()
        {
            var property = typeof(Person).GetProperty(nameof(Person.LastName))!;

            var sut = new PredicateIgnoreRule(x => x.PropertyType == typeof(DateTimeOffset));

            var actual = sut.IsMatch(property);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchReturnsTrueWhenInheritedPropertyMatchesPropertyOnDeclaredType()
        {
            var property = typeof(Person).GetProperty(nameof(Entity.Id))!;

            var sut = new PredicateIgnoreRule(x =>
                x.ReflectedType == typeof(Person) && x.Name == nameof(Entity.Id) && x.PropertyType == typeof(Guid));

            var actual = sut.IsMatch(property);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsMatchReturnsTrueWhenPropertyMatches()
        {
            var property = typeof(Person).GetProperty(nameof(Person.FirstName))!;

            var sut = new PredicateIgnoreRule(x => x.Name == nameof(Person.FirstName));

            var actual = sut.IsMatch(property);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsMatchReturnsTrueWhenPropertyMatchesBaseTypeProperty()
        {
            var property = typeof(Person).GetProperty(nameof(Entity.Id))!;

            var sut = new PredicateIgnoreRule(x => x.ReflectedType == typeof(Person));

            var actual = sut.IsMatch(property);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsMatchReturnsTrueWhenPropertyMatchesDerivedTypeProperty()
        {
            var property = typeof(Person).GetProperty(nameof(Entity.Id))!;

            var sut = new PredicateIgnoreRule(x => x.DeclaringType == typeof(Entity));

            var actual = sut.IsMatch(property);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsMatchThrowsExceptionWithNullProperty()
        {
            var sut = new PredicateIgnoreRule(x => x.Name == nameof(Person.FirstName));

            Action action = () => sut.IsMatch(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullExpression()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new PredicateIgnoreRule(null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}