namespace ModelBuilder.UnitTests.ExecuteOrderRules
{
    using System;
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using ModelBuilder.ExecuteOrderRules;
    using ModelBuilder.UnitTests.Models;
    using Xunit;

    public class ExpressionExecuteOrderRuleTests
    {
        [Fact]
        public void IsMatchReturnsFalseForParameter()
        {
            var priority = Environment.TickCount;
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            var sut = new ExpressionExecuteOrderRule<Simple>(x => x.LastName, priority);

            var actual = sut.IsMatch(parameterInfo);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchReturnsFalseWhenPropertyDeclaringTypeDoesNotMatch()
        {
            var priority = Environment.TickCount;
            var property = typeof(Person).GetProperty(nameof(Person.LastName))!;

            var sut = new ExpressionExecuteOrderRule<Simple>(x => x.LastName, priority);

            var actual = sut.IsMatch(property);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchReturnsFalseWhenPropertyDoesNotMatch()
        {
            var priority = Environment.TickCount;
            var property = typeof(Person).GetProperty(nameof(Person.FirstName))!;

            var sut = new ExpressionExecuteOrderRule<Person>(x => x.LastName, priority);

            var actual = sut.IsMatch(property);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchReturnsFalseWhenPropertyNameDoesNotMatch()
        {
            var priority = Environment.TickCount;
            var property = typeof(Person).GetProperty(nameof(Person.FirstName))!;

            var sut = new ExpressionExecuteOrderRule<Person>(x => x.LastName, priority);

            var actual = sut.IsMatch(property);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchReturnsTrueWhenInheritedPropertyMatchesPropertyOnDeclaredType()
        {
            var priority = Environment.TickCount;
            var property = typeof(Person).GetProperty(nameof(Person.Id))!;

            var sut = new ExpressionExecuteOrderRule<Person>(x => x.Id, priority);

            var actual = sut.IsMatch(property);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsMatchReturnsTrueWhenPropertyMatches()
        {
            var priority = Environment.TickCount;
            var property = typeof(Person).GetProperty(nameof(Person.FirstName))!;

            var sut = new ExpressionExecuteOrderRule<Person>(x => x.FirstName, priority);

            var actual = sut.IsMatch(property);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsMatchReturnsTrueWhenPropertyMatchesBaseTypeProperty()
        {
            var priority = Environment.TickCount;
            var property = typeof(Entity).GetProperty(nameof(Entity.Id))!;

            var sut = new ExpressionExecuteOrderRule<Person>(x => x.Id, priority);

            var actual = sut.IsMatch(property);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsMatchReturnsTrueWhenPropertyMatchesDerivedTypeProperty()
        {
            var priority = Environment.TickCount;
            var property = typeof(Person).GetProperty(nameof(Person.Id))!;

            var sut = new ExpressionExecuteOrderRule<Entity>(x => x.Id, priority);

            var actual = sut.IsMatch(property);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsMatchThrowsExceptionWithNullProperty()
        {
            var priority = Environment.TickCount;

            var sut = new ExpressionExecuteOrderRule<Person>(x => x.FirstName, priority);

            Action action = () => sut.IsMatch((PropertyInfo)null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PriorityReturnsConstructorValue()
        {
            var priority = Environment.TickCount;

            var sut = new ExpressionExecuteOrderRule<Person>(x => x.LastName, priority);

            sut.Priority.Should().Be(priority);
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullExpression()
        {
            var priority = Environment.TickCount;

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new ExpressionExecuteOrderRule<Person>(null!, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ToStringDoesNotReturnTypeName()
        {
            var sut = new ExpressionExecuteOrderRule<Person>(x => x.FirstName, Environment.TickCount);

            var actual = sut.ToString();

            actual.Should().NotBe(sut.GetType().ToString());
        }
    }
}