﻿namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using FluentAssertions;
    using Xunit;

    public class RelativeValueGeneratorTests
    {
        [Fact]
        public void GetSourceValueReturnsNullFromSourcePropertyTest()
        {
            var context = new Person();

            var target = new GeneratorWrapper(PropertyExpression.FirstName, PropertyExpression.LastName);

            var actual = target.ReadSourceValue(context);

            actual.Should().BeNull();
        }

        [Fact]
        public void GetSourceValueReturnsNullWhenPropertyNotFoundTest()
        {
            var context = new SlimModel();

            var target = new GeneratorWrapper(PropertyExpression.FirstName, PropertyExpression.LastName);

            var actual = target.ReadSourceValue(context);

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

            var actual = target.ReadSourceValue(context);

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

            var actual = target.ReadSourceValue(context);

            actual.Should().Be(context.LastName);
        }

        [Fact]
        public void GetSourceValueThrowsExceptionWhenNoSourceExpressionProvidedTest()
        {
            var context = new Person();

            var target = new GeneratorWrapper(PropertyExpression.FirstName, (Regex)null, (Type)null);

            Action action = () => target.ReadSourceValue(context);

            action.ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void GetSourceValueThrowsExceptionWithNullContextTest()
        {
            var target = new GeneratorWrapper(PropertyExpression.FirstName, PropertyExpression.LastName);

            Action action = () => target.ReadSourceValue(null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void GetValueThrowsExceptionWithNullContextTest()
        {
            var target = new GeneratorWrapper(PropertyExpression.FirstName, PropertyExpression.FirstName, (Type)null);

            Action action = () => target.ReadValue(PropertyExpression.LastName, null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void GetValueThrowsExceptionWithNullExpressionTest()
        {
            var context = new Person();

            var target = new GeneratorWrapper(PropertyExpression.FirstName, PropertyExpression.FirstName, (Type)null);

            Action action = () => target.ReadValue(null, context);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Theory]
        [InlineData(typeof(bool), null, null, false)]
        [InlineData(typeof(string), null, null, false)]
        [InlineData(typeof(string), "FirstName", null, false)]
        [InlineData(typeof(string), "FirstName", typeof(List<string>), false)]
        [InlineData(typeof(string), "stuff", typeof(Person), false)]
        [InlineData(typeof(string), "FirstName", typeof(Person), true)]
        public void IsSupportedReturnsFalseForUnsupportedScenariosTest(
            Type type,
            string referenceName,
            Type contextType,
            bool expected)
        {
            var buildChain = new LinkedList<object>();

            if (contextType != null)
            {
                var context = Activator.CreateInstance(contextType);

                buildChain.AddFirst(context);
            }

            var target = new GeneratorWrapper(PropertyExpression.FirstName, PropertyExpression.Gender);

            var actual = target.IsSupported(type, referenceName, buildChain);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsSupportedReturnsFalseWithNullBuildChainTest()
        {
            var context = new SlimModel();
            var buildChain = new LinkedList<object>();

            buildChain.AddFirst(context);

            var target = new GeneratorWrapper(PropertyExpression.FirstName, typeof(string));

            var actual = target.IsSupported(typeof(string), "FirstName", null);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsSupportedReturnsTrueWhenSourceExpressionIsNullAndTargetExpressionMatchesReferenceNameTest()
        {
            var context = new SlimModel();
            var buildChain = new LinkedList<object>();

            buildChain.AddFirst(context);

            var target = new GeneratorWrapper(PropertyExpression.FirstName, (Regex)null);

            var actual = target.IsSupported(typeof(string), "FirstName", buildChain);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsSupportedReturnsTrueWhenTargetExpressionMatchesReferenceNameTest()
        {
            var context = new SlimModel();
            var buildChain = new LinkedList<object>();

            buildChain.AddFirst(context);

            var target = new GeneratorWrapper(PropertyExpression.FirstName, typeof(string));

            var actual = target.IsSupported(typeof(string), "FirstName", buildChain);

            actual.Should().BeTrue();
        }

        [Fact]
        public void ThrowsExceptionWithNullTargetExpressionAndTypesTest()
        {
            Action action = () => new GeneratorWrapper(null, PropertyExpression.FirstName, typeof(string));

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
            public GeneratorWrapper(Regex targetNameExpression, params Type[] types) : base(targetNameExpression, types)
            {
            }

            public GeneratorWrapper(Regex targetNameExpression, Regex sourceNameExpression, params Type[] types)
                : base(targetNameExpression, sourceNameExpression, types)
            {
            }

            public GeneratorWrapper(Regex targetNameExpression, Regex sourceNameExpression)
                : base(targetNameExpression, sourceNameExpression, typeof(string))
            {
            }

            public string ReadSourceValue(object context)
            {
                return GetSourceValue<string>(context);
            }

            public string ReadValue(Regex expression, object context)
            {
                return GetValue<string>(expression, context);
            }

            protected override object GenerateValue(Type type, string referenceName, LinkedList<object> buildChain)
            {
                throw new NotImplementedException();
            }
        }
    }
}