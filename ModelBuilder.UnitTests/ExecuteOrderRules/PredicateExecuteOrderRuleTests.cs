namespace ModelBuilder.UnitTests.ExecuteOrderRules
{
    using System;
    using FluentAssertions;
    using ModelBuilder.ExecuteOrderRules;
    using ModelBuilder.UnitTests.Models;
    using Xunit;

    public class PredicateExecuteOrderRuleTests
    {
        [Fact]
        public void IsMatchReturnsFalseWhenPropertyDeclaringTypeDoesNotMatch()
        {
            var priority = Environment.TickCount;
            var property = typeof(Person).GetProperty(nameof(Person.LastName));

            var sut = new PredicateExecuteOrderRule(x => x.DeclaringType == typeof(string), priority);

            var actual = sut.IsMatch(property);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchReturnsFalseWhenPropertyNameDoesNotMatch()
        {
            var priority = Environment.TickCount;
            var property = typeof(Person).GetProperty(nameof(Person.FirstName));

            var sut = new PredicateExecuteOrderRule(x => x.Name == nameof(Person.LastName), priority);

            var actual = sut.IsMatch(property);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchReturnsFalseWhenPropertyTypeDoesNotMatch()
        {
            var priority = Environment.TickCount;
            var property = typeof(Person).GetProperty(nameof(Person.LastName));

            var sut = new PredicateExecuteOrderRule(x => x.PropertyType == typeof(DateTimeOffset), priority);

            var actual = sut.IsMatch(property);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchReturnsTrueWhenInheritedPropertyMatchesPropertyOnDeclaredType()
        {
            var priority = Environment.TickCount;
            var property = typeof(Person).GetProperty(nameof(Person.Id));

            var sut = new PredicateExecuteOrderRule(x =>
                x.ReflectedType == typeof(Person) && x.Name == nameof(Person.Id) && x.PropertyType == typeof(Guid), priority);

            var actual = sut.IsMatch(property);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsMatchReturnsTrueWhenPropertyMatches()
        {
            var priority = Environment.TickCount;
            var property = typeof(Person).GetProperty(nameof(Person.FirstName));

            var sut = new PredicateExecuteOrderRule(x => x.Name == nameof(Person.FirstName), priority);

            var actual = sut.IsMatch(property);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsMatchReturnsTrueWhenPropertyMatchesBaseTypeProperty()
        {
            var priority = Environment.TickCount;
            var property = typeof(Person).GetProperty(nameof(Entity.Id));

            var sut = new PredicateExecuteOrderRule(x => x.ReflectedType == typeof(Person), priority);

            var actual = sut.IsMatch(property);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsMatchReturnsTrueWhenPropertyMatchesDerivedTypeProperty()
        {
            var priority = Environment.TickCount;
            var property = typeof(Person).GetProperty(nameof(Entity.Id));

            var sut = new PredicateExecuteOrderRule(x => x.DeclaringType == typeof(Entity), priority);

            var actual = sut.IsMatch(property);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsMatchThrowsExceptionWithNullProperty()
        {
            var priority = Environment.TickCount;
            var sut = new PredicateExecuteOrderRule(x => x.Name == nameof(Person.FirstName), priority);

            Action action = () => sut.IsMatch(null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullExpression()
        {
            var priority = Environment.TickCount;

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new PredicateExecuteOrderRule(null, priority);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}
