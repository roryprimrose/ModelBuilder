namespace ModelBuilder.UnitTests.ValueGenerators
{
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;
    using FluentAssertions;
    using ModelBuilder.UnitTests.Models;
    using ModelBuilder.ValueGenerators;
    using NSubstitute;
    using Xunit;

    public class RegexTypeNameValueGeneratorTests
    {
        [Fact]
        public void GenerateForParameterReturnsValue()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var expected = Guid.NewGuid().ToString();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new Wrapper(PropertyExpression.FirstName, typeof(string), expected);

            var actual = sut.Generate(parameterInfo, executeStrategy);

            actual.Should().Be(expected);
        }

        [Fact]
        public void GenerateForPropertyReturnsValue()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var expected = Guid.NewGuid().ToString();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new Wrapper(PropertyExpression.FirstName, typeof(string), expected);

            var actual = sut.Generate(propertyInfo, executeStrategy);

            actual.Should().Be(expected);
        }

        [Fact]
        public void GenerateForTypeThrowsNotSupportedException()
        {
            var value = Guid.NewGuid().ToString();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new Wrapper(PropertyExpression.FirstName, typeof(string), value);

            Action action = () => sut.Generate(typeof(string), executeStrategy);

            action.Should().Throw<NotSupportedException>();
        }

        [Theory]
        [InlineData(typeof(string), "FirstName", true)]
        [InlineData(typeof(string), "LastName", false)]
        public void IsSupportedForReturnsWhetherNameMatchesExpression(Type type, string expression, bool expected)
        {
            var nameRegex = new Regex(expression);
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var value = Guid.NewGuid().ToString();

            var buildChain = Substitute.For<IBuildChain>();

            var sut = new Wrapper(nameRegex, type, value);

            var actual = sut.IsSupported(propertyInfo, buildChain);

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(typeof(string), "FirstName", true)]
        [InlineData(typeof(int), "FirstName", false)]
        public void IsSupportedForReturnsWhetherTypeMatches(Type type, string expression, bool expected)
        {
            var nameRegex = new Regex(expression);
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var value = Guid.NewGuid().ToString();

            var buildChain = Substitute.For<IBuildChain>();

            var sut = new Wrapper(nameRegex, type, value);

            var actual = sut.IsSupported(propertyInfo, buildChain);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsSupportedForTargetTypeReturnsFalse()
        {
            var value = Guid.NewGuid().ToString();

            var buildChain = Substitute.For<IBuildChain>();

            var sut = new Wrapper(PropertyExpression.FirstName, typeof(string), value);

            var actual = sut.IsSupported(typeof(string), buildChain);

            actual.Should().BeFalse();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void IsSupportedReturnsFalseForUnsupportedReferenceName(string referenceName)
        {
            var nameRegex = PropertyExpression.FirstName;
            var value = Guid.NewGuid().ToString();

            var buildChain = Substitute.For<IBuildChain>();

            var sut = new Wrapper(nameRegex, typeof(string), value);

            var actual = sut.IsSupported(typeof(string), referenceName, buildChain);

            actual.Should().BeFalse();
        }

        [Fact]
        public void ThrowsExceptionWithNullExpression()
        {
            var value = Guid.NewGuid().ToString();

            Action action = () => new Wrapper(null, typeof(string), value);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWithNullType()
        {
            var value = Guid.NewGuid().ToString();

            Action action = () => new Wrapper(PropertyExpression.FirstName, null, value);

            action.Should().Throw<ArgumentNullException>();
        }

        private class Wrapper : RegexTypeNameValueGenerator
        {
            private readonly object _value;

            public Wrapper(Regex nameExpression, Type type, object value) : base(nameExpression, type)
            {
                _value = value;
            }

            public override object Generate(Type type, string referenceName, IExecuteStrategy executeStrategy)
            {
                return _value;
            }

            public override int Priority => Environment.TickCount;
        }
    }
}