using System;
using System.Collections.Generic;
using FluentAssertions;
using Xunit;

namespace ModelBuilder.UnitTests
{
    public class RelativeValueGeneratorTests
    {
        [Fact]
        public void GetSourceValueReturnsNullFromSourcePropertyTest()
        {
            var context = new Person();

            var target = new GeneratorWrapper(PropertyExpression.FirstName, PropertyExpression.LastName);

            var actual = target.GetValue(context);

            actual.Should().BeNull();
        }

        [Theory]
        [InlineData(Gender.Unknown, "Unknown")]
        [InlineData(Gender.Male, "Male")]
        [InlineData(Gender.Female, "Female")]
        public void GetSourceValueReturnsValueFromSourcePropertyTest(Gender gender, string expected)
        {
            var context = new Person
            {
                Gender = gender
            };

            var target = new GeneratorWrapper(PropertyExpression.FirstName, PropertyExpression.Gender);

            var actual = target.GetValue(context);

            actual.Should().Be(expected);
        }

        [Fact]
        public void GetSourceValueReturnsValueFromSourceStringPropertyTest()
        {
            var context = new Person
            {
                LastName = Guid.NewGuid().ToString()
            };

            var target = new GeneratorWrapper(PropertyExpression.FirstName, PropertyExpression.LastName);

            var actual = target.GetValue(context);

            actual.Should().Be(context.LastName);
        }

        [Theory]
        [InlineData(typeof(bool), null, null, false)]
        [InlineData(typeof(string), null, null, false)]
        [InlineData(typeof(string), "FirstName", null, false)]
        [InlineData(typeof(string), "FirstName", typeof(List<string>), false)]
        [InlineData(typeof(string), "stuff", typeof(Person), false)]
        [InlineData(typeof(string), "FirstName", typeof(Person), true)]
        public void IsSupportedReturnsFalseForUnsupportedScenariosTest(Type type, string referenceName, Type contextType,
            bool expected)
        {
            object context = null;

            if (contextType != null)
            {
                context = Activator.CreateInstance(contextType);
            }

            var target = new GeneratorWrapper(PropertyExpression.FirstName, PropertyExpression.Gender);

            var actual = target.IsSupported(type, referenceName, context);

            actual.Should().Be(expected);
        }

        [Fact]
        public void ThrowsExceptionWithNullSourceExpressionTest()
        {
            Action action = () => new GeneratorWrapper(PropertyExpression.FirstName, null);

            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void ThrowsExceptionWithNullTargetExpressionTest()
        {
            Action action = () => new GeneratorWrapper(null, PropertyExpression.Gender);

            action.ShouldThrow<ArgumentException>();
        }

        private class GeneratorWrapper : RelativeValueGenerator
        {
            public GeneratorWrapper(string targetNameExpression, string sourceNameExpression)
                : base(targetNameExpression, sourceNameExpression)
            {
            }

            public string GetValue(object context)
            {
                return GetSourceValue(context);
            }
        }
    }
}