namespace ModelBuilder.UnitTests.ValueGenerators
{
    using System;
    using System.Text.RegularExpressions;
    using FluentAssertions;
    using ModelBuilder.ValueGenerators;
    using NSubstitute;
    using Xunit;

    public class ValueGeneratorMatcherTests
    {
        [Theory]
        [InlineData(typeof(string), "Value|Other", "stuff", false)]
        [InlineData(typeof(bool), "Value|Other", "stuff", false)]
        [InlineData(typeof(string), "Value|Other", "Other", false)]
        [InlineData(typeof(bool), "Value|Other", null!, false)]
        [InlineData(typeof(bool), "Value|Other", "Value", true)]
        [InlineData(typeof(bool), "Value|Other", "Other", true)]
        [InlineData(typeof(bool?), "Value|Other", "Value", true)]
        [InlineData(typeof(bool?), "Value|Other", "Other", true)]
        public void IsMatchEvaluatesSpecifiedExpressionAndTypesTest(
            Type type,
            string expression,
            string referenceName,
            bool expected)
        {
            var regex = new Regex(expression);

            var buildChain = Substitute.For<IBuildChain>();

            var sut = new Wrapper(regex, typeof(bool), typeof(bool?));

            var actual = sut.RunIsMatch(type, referenceName, buildChain);

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData("Value|Other", "stuff", false)]
        [InlineData("Value|Other", "Value", true)]
        [InlineData("Value|Other", "Other", true)]
        public void IsMatchEvaluatesSpecifiedExpressionTest(string expression, string referenceName, bool expected)
        {
            var regex = new Regex(expression);

            var buildChain = Substitute.For<IBuildChain>();

            var sut = new Wrapper(regex);

            var actual = sut.RunIsMatch(typeof(Guid), referenceName, buildChain);

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(typeof(string), "stuff", false)]
        [InlineData(typeof(bool), "stuff", false)]
        [InlineData(typeof(bool?), "stuff", false)]
        [InlineData(typeof(string), "Match", false)]
        [InlineData(typeof(string), "match", false)]
        [InlineData(typeof(bool), "match", true)]
        [InlineData(typeof(bool), "Match", true)]
        [InlineData(typeof(bool?), "match", true)]
        [InlineData(typeof(bool?), "Match", true)]
        public void IsMatchEvaluatesSpecifiedNameAndTypesTest(Type type, string referenceName, bool expected)
        {
            var buildChain = Substitute.For<IBuildChain>();

            var sut = new Wrapper("Match", typeof(bool), typeof(bool?));

            var actual = sut.RunIsMatch(type, referenceName, buildChain);

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData("stuff", false)]
        [InlineData(null!, false)]
        [InlineData("Match", true)]
        [InlineData("match", true)]
        public void IsMatchEvaluatesSpecifiedNamesTest(string referenceName, bool expected)
        {
            var buildChain = Substitute.For<IBuildChain>();

            var sut = new Wrapper("Match");

            var actual = sut.RunIsMatch(typeof(Guid), referenceName, buildChain);

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(typeof(string), false)]
        [InlineData(typeof(bool), true)]
        [InlineData(typeof(bool?), true)]
        public void IsMatchEvaluatesSpecifiedTypesTest(Type type, bool expected)
        {
            var buildChain = Substitute.For<IBuildChain>();

            var sut = new Wrapper(typeof(bool), typeof(bool?));

            var actual = sut.RunIsMatch(type, null!, buildChain);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsMatchThrowsExceptionWithNullBuildChain()
        {
            var type = typeof(string);

            var sut = new Wrapper("Test");

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Action action = () => sut.RunIsMatch(type, "Test", null!);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchThrowsExceptionWithNullType()
        {
            var buildChain = new BuildHistory();

            var sut = new Wrapper("Test");

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Action action = () => sut.RunIsMatch(null!, "Test", buildChain);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWithNullExpression()
        {
            // ReSharper disable once ObjectCreationAsStatement
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Action action = () => new Wrapper((Regex)null!);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWithNullReferenceName()
        {
            // ReSharper disable once ObjectCreationAsStatement
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Action action = () => new Wrapper((string)null!);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWithNullTypes()
        {
            // ReSharper disable once ObjectCreationAsStatement
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Action action = () => new Wrapper((Type[])null!);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            action.Should().Throw<ArgumentNullException>();
        }

        private class Wrapper : ValueGeneratorMatcher
        {
            public Wrapper(params Type[] types) : base(types)
            {
            }

            public Wrapper(string referenceName, params Type[] types) : base(referenceName, types)
            {
            }

            public Wrapper(Regex expression, params Type[] types) : base(expression, types)
            {
            }

            protected override object? Generate(IExecuteStrategy executeStrategy, Type type, string? referenceName)
            {
                throw new NotImplementedException();
            }

            public bool RunIsMatch(Type type, string referenceName, IBuildChain buildChain)
            {
                return IsMatch(buildChain, type, referenceName);
            }
        }
    }
}