namespace ModelBuilder.UnitTests
{
    using System;
    using System.Text.RegularExpressions;
    using FluentAssertions;
    using Xunit;

    public class ValueGeneratorMatcherTests
    {
        [Theory]
        [InlineData(typeof(string), "Value|Other", "stuff", false)]
        [InlineData(typeof(bool), "Value|Other", "stuff", false)]
        [InlineData(typeof(string), "Value|Other", "Other", false)]
        [InlineData(typeof(bool), "Value|Other", null, false)]
        [InlineData(typeof(bool), "Value|Other", "Value", true)]
        [InlineData(typeof(bool), "Value|Other", "Other", true)]
        [InlineData(typeof(bool?), "Value|Other", "Value", true)]
        [InlineData(typeof(bool?), "Value|Other", "Other", true)]
        public void IsSupportedEvaluatesSpecifiedExpressionAndTypesTest(
            Type type,
            string expression,
            string referenceName,
            bool expected)
        {
            var regex = new Regex(expression);

            var target = new WrapperGenerator(regex, typeof(bool), typeof(bool?));

            var actual = target.IsSupported(type, referenceName, null);

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData("Value|Other", "stuff", false)]
        [InlineData("Value|Other", "Value", true)]
        [InlineData("Value|Other", "Other", true)]
        public void IsSupportedEvaluatesSpecifiedExpressionTest(string expression, string referenceName, bool expected)
        {
            var regex = new Regex(expression);

            var target = new WrapperGenerator(regex);

            var actual = target.IsSupported(typeof(Guid), referenceName, null);

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
        public void IsSupportedEvaluatesSpecifiedNameAndTypesTest(Type type, string referenceName, bool expected)
        {
            var target = new WrapperGenerator("Match", typeof(bool), typeof(bool?));

            var actual = target.IsSupported(type, referenceName, null);

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData("stuff", false)]
        [InlineData(null, false)]
        [InlineData("Match", true)]
        [InlineData("match", true)]
        public void IsSupportedEvaluatesSpecifiedNamesTest(string referenceName, bool expected)
        {
            var target = new WrapperGenerator("Match");

            var actual = target.IsSupported(typeof(Guid), referenceName, null);

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(typeof(string), false)]
        [InlineData(typeof(bool), true)]
        [InlineData(typeof(bool?), true)]
        public void IsSupportedEvaluatesSpecifiedTypesTest(Type type, bool expected)
        {
            var target = new WrapperGenerator(typeof(bool), typeof(bool?));

            var actual = target.IsSupported(type, null, null);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullTypeTest()
        {
            var buildChain = new BuildHistory();

            var target = new WrapperGenerator("Test");

            Action action = () => target.IsSupported(null, "Test", buildChain);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWithNullExpressionTest()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new WrapperGenerator((Regex)null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWithNullReferenceNameTest()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new WrapperGenerator((string)null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWithNullTypesTest()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new WrapperGenerator((Type[])null);

            action.Should().Throw<ArgumentNullException>();
        }

        private class WrapperGenerator : ValueGeneratorMatcher
        {
            public WrapperGenerator(params Type[] types) : base(types)
            {
            }

            public WrapperGenerator(string referenceName, params Type[] types) : base(referenceName, types)
            {
            }

            public WrapperGenerator(Regex expression, params Type[] types) : base(expression, types)
            {
            }

            protected override object GenerateValue(Type type, string referenceName, IExecuteStrategy executeStrategy)
            {
                throw new NotImplementedException();
            }
        }
    }
}