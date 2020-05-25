namespace ModelBuilder.UnitTests.ValueGenerators
{
    using System;
    using System.Text.RegularExpressions;
    using FluentAssertions;
    using ModelBuilder.ValueGenerators;
    using NSubstitute;
    using Xunit;

    public class RegexTypeNameValueGeneratorTests
    {
        [Theory]
        [InlineData(typeof(string), "FirstName", true)]
        [InlineData(typeof(string), "LastName", false)]
        public void IsMatchForReturnsWhetherNameMatchesExpression(Type type, string expression, bool expected)
        {
            var nameRegex = new Regex(expression);
            var value = Guid.NewGuid().ToString();

            var buildChain = Substitute.For<IBuildChain>();

            var sut = new Wrapper(nameRegex, type, value);

            var actual = sut.RunIsMatch(typeof(string), "FirstName", buildChain);

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(typeof(string), "FirstName", true)]
        [InlineData(typeof(int), "FirstName", false)]
        public void IsMatchForReturnsWhetherTypeMatches(Type type, string expression, bool expected)
        {
            var nameRegex = new Regex(expression);
            var value = Guid.NewGuid().ToString();

            var buildChain = Substitute.For<IBuildChain>();

            var sut = new Wrapper(nameRegex, type, value);

            var actual = sut.RunIsMatch(typeof(string), "FirstName", buildChain);

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(null!)]
        [InlineData("")]
        [InlineData("  ")]
        public void IsMatchReturnsFalseForUnsupportedReferenceName(string referenceName)
        {
            var nameRegex = NameExpression.FirstName;
            var value = Guid.NewGuid().ToString();

            var buildChain = Substitute.For<IBuildChain>();

            var sut = new Wrapper(nameRegex, typeof(string), value);

            var actual = sut.RunIsMatch(typeof(string), referenceName, buildChain);

            actual.Should().BeFalse();
        }

        [Fact]
        public void ThrowsExceptionWithNullExpression()
        {
            var value = Guid.NewGuid().ToString();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new Wrapper(null!, typeof(string), value);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWithNullType()
        {
            var value = Guid.NewGuid().ToString();

            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new Wrapper(NameExpression.FirstName, null!, value);

            action.Should().Throw<ArgumentNullException>();
        }

        private class Wrapper : RegexTypeNameValueGenerator
        {
            private readonly object _value;

            public Wrapper(Regex nameExpression, Type type, object value) : base(nameExpression, type)
            {
                _value = value;
            }

            public bool RunIsMatch(Type type, string referenceName, IBuildChain buildChain)
            {
                return IsMatch(buildChain, type, referenceName);
            }

            protected override object? Generate(IExecuteStrategy executeStrategy, Type type, string? referenceName)
            {
                return _value;
            }

            public override int Priority => Environment.TickCount;
        }
    }
}