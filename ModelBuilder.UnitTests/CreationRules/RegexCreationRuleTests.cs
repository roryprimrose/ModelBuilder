namespace ModelBuilder.UnitTests.CreationRules
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using FluentAssertions;
    using ModelBuilder.CreationRules;
    using ModelBuilder.UnitTests.Models;
    using Xunit;

    public class RegexCreationRuleTests
    {
        [Fact]
        public void CanCreateWithNullExplicitValue()
        {
            var expression = new Regex("FirstName");
            var priority = Environment.TickCount;

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new RegexCreationRule(typeof(string), expression, (object)null!, priority);

            action.Should().NotThrow();
        }

        [Fact]
        public void CreateWithParameterReturnsLiteralValue()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var expression = new Regex("FirstName");
            var expected = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new RegexCreationRule(typeof(string), expression, expected, priority);

            var actual = sut.Create(null!, parameterInfo);

            actual.Should().Be(expected);
        }

        [Fact]
        public void CreateWithParameterReturnsValueFromValueGenerator()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var expression = new Regex("FirstName");
            var expected = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new RegexCreationRule(typeof(string), expression, () => expected, priority);

            var actual = sut.Create(null!, parameterInfo);

            actual.Should().Be(expected);
        }

        [Fact]
        public void CreateWithParameterThrowsExceptionWithNullParameter()
        {
            var expression = new Regex("FirstName");
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new RegexCreationRule(typeof(string), expression, value, priority);

            Action action = () => sut.Create(null!, (ParameterInfo)null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateWithPropertyReturnsLiteralValue()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName))!;
            var expression = new Regex("FirstName");
            var expected = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new RegexCreationRule(typeof(string), expression, expected, priority);

            var actual = sut.Create(null!, propertyInfo);

            actual.Should().Be(expected);
        }

        [Fact]
        public void CreateWithPropertyReturnsValueFromValueGenerator()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName))!;
            var expression = new Regex("FirstName");
            var expected = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new RegexCreationRule(typeof(string), expression, () => expected, priority);

            var actual = sut.Create(null!, propertyInfo);

            actual.Should().Be(expected);
        }

        [Fact]
        public void CreateWithPropertyThrowsExceptionWithNullProperty()
        {
            var expression = new Regex("FirstName");
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new RegexCreationRule(typeof(string), expression, value, priority);

            Action action = () => sut.Create(null!, (PropertyInfo)null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CreateWithTypeThrowsException()
        {
            var expression = new Regex("FirstName");
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new RegexCreationRule(typeof(string), expression, value, priority);

            Action action = () => sut.Create(null!, typeof(string));

            action.Should().Throw<NotSupportedException>();
        }

        [Theory]
        [InlineData(typeof(Person), "firstName", true)]
        [InlineData(typeof(Person), "lastName", false)]
        [InlineData(typeof(Person), "dob", false)]
        [InlineData(typeof(Copy), "source", false)]
        public void IsMatchForParameterReturnsWhetherParameterMatches(Type declaringType, string parameterName,
            bool expected)
        {
            var parameterInfo = declaringType.GetConstructors()
                .First(x => x.GetParameters().Any(y => y.Name == parameterName)).GetParameters()
                .First(x => x.Name == parameterName);
            var expression = new Regex("FirstName", RegexOptions.IgnoreCase);
            var priority = Environment.TickCount;

            var sut = new RegexCreationRule(typeof(string), expression, expected, priority);

            var actual = sut.IsMatch(parameterInfo);

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(typeof(Person), nameof(Person.FirstName), true)]
        [InlineData(typeof(Person), nameof(Person.LastName), false)]
        [InlineData(typeof(Person), nameof(Person.Age), false)]
        [InlineData(typeof(Company), nameof(Company.Name), false)]
        public void IsMatchForPropertyReturnsWhetherPropertyMatches(Type declaringType, string propertyName,
            bool expected)
        {
            var propertyInfo = declaringType.GetProperty(propertyName)!;
            var expression = new Regex("FirstName");
            var priority = Environment.TickCount;

            var sut = new RegexCreationRule(typeof(string), expression, expected, priority);

            var actual = sut.IsMatch(propertyInfo);

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData("^First", true)]
        [InlineData("FirstName", true)]
        [InlineData("LastName", false)]
        public void IsMatchForPropertyReturnsWhetherPropertyMatchesStringExpression(string expression,
            bool expected)
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName))!;
            var priority = Environment.TickCount;

            var sut = new RegexCreationRule(typeof(string), expression, expected, priority);

            var actual = sut.IsMatch(propertyInfo);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsMatchForTypeReturnsFalse()
        {
            var expression = new Regex("FirstName");
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new RegexCreationRule(typeof(string), expression, value, priority);

            var actual = sut.IsMatch(typeof(string));

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchWithParameterThrowsExceptionWithNullParameter()
        {
            var expression = new Regex("FirstName");
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new RegexCreationRule(typeof(string), expression, value, priority);

            Action action = () => sut.IsMatch((ParameterInfo)null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchWithPropertyThrowsExceptionWithNullProperty()
        {
            var expression = new Regex("FirstName");
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            var sut = new RegexCreationRule(typeof(string), expression, value, priority);

            Action action = () => sut.IsMatch((PropertyInfo)null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PriorityReturnsConstructorValue()
        {
            var priority = Environment.TickCount;
            var expression = new Regex("FirstName");
            var value = Guid.NewGuid().ToString();

            var sut = new RegexCreationRule(typeof(string), expression, value, priority);

            sut.Priority.Should().Be(priority);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void ThrowsExceptionWhenCreatedWithInvalidStringExpression(string? expression)
        {
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new RegexCreationRule(typeof(string), expression!, value, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullExpression()
        {
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new RegexCreationRule(typeof(string), (Regex)null!, value, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullType()
        {
            var expression = new Regex("FirstName");
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new RegexCreationRule(null!, expression, value, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenCreatedWithNullValueGenerator()
        {
            var expression = new Regex("FirstName");
            var priority = Environment.TickCount;

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new RegexCreationRule(typeof(string), expression, null!, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenStringExpressionCreatedWithNullType()
        {
            const string expression = "FirstName";
            var value = Guid.NewGuid().ToString();
            var priority = Environment.TickCount;

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new RegexCreationRule(null!, expression, value, priority);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWhenStringExpressionCreatedWithNullValueGenerator()
        {
            const string expression = "FirstName";
            var priority = Environment.TickCount;

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new RegexCreationRule(typeof(string), expression, null!, priority);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}