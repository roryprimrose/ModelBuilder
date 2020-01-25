namespace ModelBuilder.UnitTests.ValueGenerators
{
    using System;
    using System.Linq;
    using System.Reflection;
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
        public void GenerateForTargetTypeThrowsNotSupportedException()
        {
            var value = Guid.NewGuid().ToString();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new Wrapper(PropertyExpression.FirstName, typeof(string), value);

            Action action = () => sut.Generate(typeof(string), executeStrategy);

            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void GenerateThrowsExceptionWithNullParameterInfo()
        {
            var expected = Guid.NewGuid().ToString();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new Wrapper(PropertyExpression.FirstName, typeof(string), expected);

            Action action = () => sut.Generate((ParameterInfo) null, executeStrategy);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GenerateThrowsExceptionWithNullPropertyInfo()
        {
            var expected = Guid.NewGuid().ToString();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new Wrapper(PropertyExpression.FirstName, typeof(string), expected);

            Action action = () => sut.Generate((PropertyInfo) null, executeStrategy);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(typeof(string), "firstName", true)]
        [InlineData(typeof(int), "firstName", false)]
        [InlineData(typeof(string), "lastName", false)]
        public void IsSupportedForParameterReturnsExpectedValue(Type type, string expression, bool expected)
        {
            var nameRegex = new Regex(expression);
            var parameterInfo = typeof(Person).GetConstructors()
                .Single(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var value = Guid.NewGuid().ToString();

            var buildChain = Substitute.For<IBuildChain>();

            var sut = new Wrapper(nameRegex, type, value);

            var actual = sut.IsSupported(parameterInfo, buildChain);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsSupportedForParameterThrowsExceptionWithNullBuildChain()
        {
            var value = Guid.NewGuid().ToString();
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            var sut = new Wrapper(PropertyExpression.FirstName, typeof(string), value);

            Action action = () => sut.IsSupported(parameterInfo, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsSupportedForParameterThrowsExceptionWithNullParameterInfo()
        {
            var value = Guid.NewGuid().ToString();

            var buildChain = Substitute.For<IBuildChain>();

            var sut = new Wrapper(PropertyExpression.FirstName, typeof(string), value);

            Action action = () => sut.IsSupported((ParameterInfo) null, buildChain);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(typeof(string), "FirstName", true)]
        [InlineData(typeof(int), "FirstName", false)]
        [InlineData(typeof(string), "LastName", false)]
        public void IsSupportedForPropertyReturnsExpectedValue(Type type, string expression, bool expected)
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
        public void IsSupportedForPropertyThrowsExceptionWithNullBuildChain()
        {
            var value = Guid.NewGuid().ToString();
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));

            var sut = new Wrapper(PropertyExpression.FirstName, typeof(string), value);

            Action action = () => sut.IsSupported(propertyInfo, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsSupportedForPropertyThrowsExceptionWithNullPropertyInfo()
        {
            var value = Guid.NewGuid().ToString();

            var buildChain = Substitute.For<IBuildChain>();

            var sut = new Wrapper(PropertyExpression.FirstName, typeof(string), value);

            Action action = () => sut.IsSupported((PropertyInfo) null, buildChain);

            action.Should().Throw<ArgumentNullException>();
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

            protected override object GenerateValue(Type type, string referenceName, IExecuteStrategy executeStrategy)
            {
                return _value;
            }

            public override int Priority => Environment.TickCount;
        }
    }
}