namespace ModelBuilder.UnitTests.ExecuteOrderRules
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using ModelBuilder.ExecuteOrderRules;
    using ModelBuilder.UnitTests.Models;
    using Xunit;

    public class PropertyPredicateExecuteOrderRuleTests
    {
        [Fact]
        public void IsMatchReturnsFalseForParameter()
        {
            var priority = Environment.TickCount;
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            var sut = new PropertyPredicateExecuteOrderRule(x => x.DeclaringType == typeof(string), priority);

            var actual = sut.IsMatch(parameterInfo);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchReturnsFalseWhenPropertyDeclaringTypeDoesNotMatch()
        {
            var priority = Environment.TickCount;
            var property = typeof(Person).GetProperty(nameof(Person.LastName))!;

            var sut = new PropertyPredicateExecuteOrderRule(x => x.DeclaringType == typeof(string), priority);

            var actual = sut.IsMatch(property);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchReturnsFalseWhenPropertyNameDoesNotMatch()
        {
            var priority = Environment.TickCount;
            var property = typeof(Person).GetProperty(nameof(Person.FirstName))!;

            var sut = new PropertyPredicateExecuteOrderRule(x => x.Name == nameof(Person.LastName), priority);

            var actual = sut.IsMatch(property);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchReturnsFalseWhenPropertyTypeDoesNotMatch()
        {
            var priority = Environment.TickCount;
            var property = typeof(Person).GetProperty(nameof(Person.LastName))!;

            var sut = new PropertyPredicateExecuteOrderRule(x => x.PropertyType == typeof(DateTimeOffset), priority);

            var actual = sut.IsMatch(property);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchReturnsTrueWhenInheritedPropertyMatchesPropertyOnDeclaredType()
        {
            var priority = Environment.TickCount;
            var property = typeof(Person).GetProperty(nameof(Person.Id))!;

            var sut = new PropertyPredicateExecuteOrderRule(x =>
                    x.ReflectedType == typeof(Person) && x.Name == nameof(Person.Id) && x.PropertyType == typeof(Guid),
                priority);

            var actual = sut.IsMatch(property);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsMatchReturnsTrueWhenPropertyMatches()
        {
            var priority = Environment.TickCount;
            var property = typeof(Person).GetProperty(nameof(Person.FirstName))!;

            var sut = new PropertyPredicateExecuteOrderRule(x => x.Name == nameof(Person.FirstName), priority);

            var actual = sut.IsMatch(property);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsMatchReturnsTrueWhenPropertyMatchesBaseTypeProperty()
        {
            var priority = Environment.TickCount;
            var property = typeof(Person).GetProperty(nameof(Entity.Id))!;

            var sut = new PropertyPredicateExecuteOrderRule(x => x.ReflectedType == typeof(Person), priority);

            var actual = sut.IsMatch(property);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsMatchReturnsTrueWhenPropertyMatchesDerivedTypeProperty()
        {
            var priority = Environment.TickCount;
            var property = typeof(Person).GetProperty(nameof(Entity.Id))!;

            var sut = new PropertyPredicateExecuteOrderRule(x => x.DeclaringType == typeof(Entity), priority);

            var actual = sut.IsMatch(property);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsMatchThrowsExceptionWithNullProperty()
        {
            var priority = Environment.TickCount;
            var sut = new PropertyPredicateExecuteOrderRule(x => x.Name == nameof(Person.FirstName), priority);

            Action action = () => sut.IsMatch((PropertyInfo)null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PriorityReturnsConstructorValue()
        {
            var priority = Environment.TickCount;

            var sut = new PropertyPredicateExecuteOrderRule(x => x.DeclaringType == typeof(string), priority);

            sut.Priority.Should().Be(priority);
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullExpression()
        {
            var priority = Environment.TickCount;

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new PropertyPredicateExecuteOrderRule(null!, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ToStringDoesNotReturnTypeName()
        {
            var priority = Environment.TickCount;

            var sut = new PropertyPredicateExecuteOrderRule(x => x.DeclaringType == typeof(string), priority);

            var actual = sut.ToString();

            actual.ToString(CultureInfo.CurrentCulture).Should().NotBe(sut.GetType().FullName);
        }
    }
}