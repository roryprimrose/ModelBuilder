namespace ModelBuilder.UnitTests.CreationRules
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder.CreationRules;
    using ModelBuilder.UnitTests.Models;
    using Xunit;

    public class TypePredicateCreationRuleTests
    {
        [Fact]
        public void CreateForParameterThrowsException()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new TypePredicateCreationRule(item => true, () => value, priority);

            Action action = () => sut.Create(null, parameterInfo);

            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void CreateForPropertyThrowsException()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new TypePredicateCreationRule(item => true, () => value, priority);

            Action action = () => sut.Create(null, propertyInfo);

            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void CreateForTypeReturnsExpressionValue()
        {
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new TypePredicateCreationRule(item => true, () => value, priority);

            var actual = sut.Create(null, typeof(string));

            actual.Should().Be(value);
        }

        [Fact]
        public void CreateForTypeReturnsLiteralValue()
        {
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new TypePredicateCreationRule(item => true, value, priority);

            var actual = sut.Create(null, typeof(string));

            actual.Should().Be(value);
        }

        [Fact]
        public void IsMatchForParameterReturnsFalse()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new TypePredicateCreationRule(item => true, () => value, priority);

            var actual = sut.IsMatch(parameterInfo);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchForPropertyReturnsFalse()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new TypePredicateCreationRule(item => true, () => value, priority);

            var actual = sut.IsMatch(propertyInfo);

            actual.Should().BeFalse();
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void IsMatchForTypeReturnsPredicateResult(bool isMatch)
        {
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new TypePredicateCreationRule(item => isMatch, () => value, priority);

            var actual = sut.IsMatch(typeof(string));

            actual.Should().Be(isMatch);
        }

        [Fact]
        public void IsMatchForTypeThrowsExceptionWithNullType()
        {
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new TypePredicateCreationRule(item => true, () => value, priority);

            Action action = () => sut.IsMatch((Type) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PriorityReturnsConstructorValue()
        {
            var priority = Environment.TickCount;

            var sut = new TypePredicateCreationRule(item => true, () => null, priority);

            sut.Priority.Should().Be(priority);
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullTypePredicate()
        {
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new TypePredicateCreationRule(null, value, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullValueGeneratorForType()
        {
            var priority = Environment.TickCount;

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new TypePredicateCreationRule(item => true, null, priority);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}