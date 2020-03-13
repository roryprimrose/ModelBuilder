namespace ModelBuilder.UnitTests.CreationRules
{
    using System;
    using System.Reflection;
    using FluentAssertions;
    using ModelBuilder.CreationRules;
    using ModelBuilder.UnitTests.Models;
    using Xunit;

    public class ExpressionCreationRuleTests
    {
        [Fact]
        public void CreateForParameterThrowsException()
        {
            var sut = new ExpressionCreationRule<Person>(x => x.FirstName, (object) null, Environment.TickCount);

            Action action = () => sut.Create((ParameterInfo) null, null);

            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void CreateForPropertyReturnsExplicitNullValue()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));

            var sut = new ExpressionCreationRule<Person>(x => x.FirstName, (object) null, Environment.TickCount);

            var actual = sut.Create(propertyInfo, null);

            actual.Should().BeNull();
        }

        [Fact]
        public void CreateForPropertyReturnsExplicitValue()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var expected = Guid.NewGuid().ToString();

            var sut = new ExpressionCreationRule<Person>(x => x.FirstName, expected, Environment.TickCount);

            var actual = sut.Create(propertyInfo, null);

            actual.Should().Be(expected);
        }

        [Fact]
        public void CreateForPropertyReturnsValueFromLamda()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var expected = Guid.NewGuid().ToString();

            var sut = new ExpressionCreationRule<Person>(x => x.FirstName, () => expected, Environment.TickCount);

            var actual = sut.Create(propertyInfo, null);

            actual.Should().Be(expected);
        }

        [Fact]
        public void CreateForTypeThrowsException()
        {
            var sut = new ExpressionCreationRule<Person>(x => x.FirstName, (object) null, Environment.TickCount);

            Action action = () => sut.Create(typeof(string), null);

            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void IsMatchForParameterReturnsFalse()
        {
            var priority = Environment.TickCount;

            var sut = new ExpressionCreationRule<Person>(x => x.FirstName, (object) null, priority);

            var actual = sut.IsMatch((ParameterInfo) null);

            actual.Should().BeFalse();
        }

        [Theory]
        [InlineData(typeof(Person), nameof(Person.FirstName), true)]
        [InlineData(typeof(Simple), nameof(Simple.FirstName), false)]
        [InlineData(typeof(Person), nameof(Person.LastName), false)]
        [InlineData(typeof(Person), nameof(Person.Age), false)]
        [InlineData(typeof(Company), nameof(Company.Name), false)]
        public void IsMatchForPropertyReturnsWhetherPropertyMatches(Type declaringType, string propertyName,
            bool expected)
        {
            var propertyInfo = declaringType.GetProperty(propertyName);
            var priority = Environment.TickCount;

            var sut = new ExpressionCreationRule<Person>(x => x.FirstName, (object) null, priority);

            var actual = sut.IsMatch(propertyInfo);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsMatchForTypeReturnsFalse()
        {
            var priority = Environment.TickCount;

            var sut = new ExpressionCreationRule<Person>(x => x.FirstName, (object) null, priority);

            var actual = sut.IsMatch(typeof(string));

            actual.Should().BeFalse();
        }

        [Fact]
        public void PriorityReturnsConstructorValue()
        {
            var priority = Environment.TickCount;

            var sut = new ExpressionCreationRule<Person>(x => x.FirstName, (object) null, priority);

            sut.Priority.Should().Be(priority);
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullExpression()
        {
            Action action = () => new ExpressionCreationRule<Person>(null, (object) null, Environment.TickCount);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullValueGenerator()
        {
            Action action = () =>
                new ExpressionCreationRule<Person>(x => x.FirstName, null, Environment.TickCount);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}