namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using FluentAssertions;
    using ModelBuilder.UnitTests.Models;
    using Xunit;

    public class RelativeValueGeneratorTests
    {
        [Fact]
        public void GetSourceValueReturnsNullFromSourceNullableIntPropertyTest()
        {
            var context = new RelativeNullableInt
            {
                YearStarted = null
            };

            var target = new GeneratorWrapper<int?>(PropertyExpression.FirstName, new Regex("YearStarted"));

            var actual = target.ReadSourceValue(context);

            actual.Should().NotHaveValue();
        }

        [Fact]
        public void GetSourceValueReturnsNullFromSourcePropertyTest()
        {
            var context = new Person();

            var target = new GeneratorWrapper<string>(PropertyExpression.FirstName, PropertyExpression.LastName);

            var actual = target.ReadSourceValue(context);

            actual.Should().BeNull();
        }

        [Fact]
        public void GetSourceValueReturnsNullWhenPropertyNotFoundTest()
        {
            var context = new SlimModel();

            var target = new GeneratorWrapper<string>(PropertyExpression.FirstName, PropertyExpression.LastName);

            var actual = target.ReadSourceValue(context);

            actual.Should().BeNull();
        }

        [Fact]
        public void GetSourceValueReturnsValueFromSourceIntPropertyTest()
        {
            var context = new Person
            {
                Priority = Environment.TickCount
            };

            var target = new GeneratorWrapper<int>(PropertyExpression.FirstName, new Regex("Priority"));

            var actual = target.ReadSourceValue(context);

            actual.Should().Be(context.Priority);
        }

        [Fact]
        public void GetSourceValueReturnsValueFromSourceNullableIntPropertyTest()
        {
            var context = new RelativeNullableInt
            {
                YearStarted = Environment.TickCount
            };

            var target = new GeneratorWrapper<int?>(PropertyExpression.FirstName, new Regex("YearStarted"));

            var actual = target.ReadSourceValue(context);

            actual.Should().Be(context.YearStarted);
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

            var target = new GeneratorWrapper<string>(PropertyExpression.FirstName, PropertyExpression.Gender);

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

            var target = new GeneratorWrapper<string>(PropertyExpression.FirstName, PropertyExpression.LastName);

            var actual = target.ReadSourceValue(context);

            actual.Should().Be(context.LastName);
        }

        [Fact]
        public void GetSourceValueThrowsExceptionWhenNoSourceExpressionProvidedTest()
        {
            var context = new Person();

            var target = new GeneratorWrapper<string>(PropertyExpression.FirstName, (Regex)null, (Type)null);

            Action action = () => target.ReadSourceValue(context);

            action.ShouldThrow<InvalidOperationException>();
        }

        [Fact]
        public void GetSourceValueThrowsExceptionWithNullContextTest()
        {
            var target = new GeneratorWrapper<string>(PropertyExpression.FirstName, PropertyExpression.LastName);

            Action action = () => target.ReadSourceValue(null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void GetValueThrowsExceptionWithNullContextTest()
        {
            var target = new GeneratorWrapper<string>(
                PropertyExpression.FirstName,
                PropertyExpression.FirstName,
                (Type)null);

            Action action = () => target.ReadValue(PropertyExpression.LastName, null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void GetValueThrowsExceptionWithNullExpressionTest()
        {
            var context = new Person();

            var target = new GeneratorWrapper<string>(
                PropertyExpression.FirstName,
                PropertyExpression.FirstName,
                (Type)null);

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

            var target = new GeneratorWrapper<string>(PropertyExpression.FirstName, PropertyExpression.Gender);

            var actual = target.IsSupported(type, referenceName, buildChain);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsSupportedReturnsFalseWithNullBuildChainTest()
        {
            var context = new SlimModel();
            var buildChain = new LinkedList<object>();

            buildChain.AddFirst(context);

            var target = new GeneratorWrapper<string>(PropertyExpression.FirstName, typeof(string));

            var actual = target.IsSupported(typeof(string), "FirstName", null);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsSupportedReturnsTrueWhenSourceExpressionIsNullAndTargetExpressionMatchesReferenceNameTest()
        {
            var context = new SlimModel();
            var buildChain = new LinkedList<object>();

            buildChain.AddFirst(context);

            var target = new GeneratorWrapper<string>(PropertyExpression.FirstName, (Regex)null);

            var actual = target.IsSupported(typeof(string), "FirstName", buildChain);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsSupportedReturnsTrueWhenTargetExpressionMatchesReferenceNameTest()
        {
            var context = new SlimModel();
            var buildChain = new LinkedList<object>();

            buildChain.AddFirst(context);

            var target = new GeneratorWrapper<string>(PropertyExpression.FirstName, typeof(string));

            var actual = target.IsSupported(typeof(string), "FirstName", buildChain);

            actual.Should().BeTrue();
        }

        [Fact]
        public void ThrowsExceptionWithNullTargetExpressionAndTypesTest()
        {
            Action action = () => new GeneratorWrapper<string>(null, PropertyExpression.FirstName, typeof(string));

            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void ThrowsExceptionWithNullTargetExpressionTest()
        {
            Action action = () => new GeneratorWrapper<string>(null, PropertyExpression.Gender);

            action.ShouldThrow<ArgumentException>();
        }

        private class GeneratorWrapper<T> : RelativeValueGenerator
        {
            public GeneratorWrapper(Regex targetNameExpression, params Type[] types) : base(targetNameExpression, types)
            {
            }

            public GeneratorWrapper(Regex targetNameExpression, Regex sourceNameExpression, params Type[] types) : base(
                targetNameExpression,
                sourceNameExpression,
                types)
            {
            }

            public GeneratorWrapper(Regex targetNameExpression, Regex sourceNameExpression) : base(
                targetNameExpression,
                sourceNameExpression,
                typeof(string))
            {
            }

            public T ReadSourceValue(object context)
            {
                return GetSourceValue<T>(context);
            }

            public T ReadValue(Regex expression, object context)
            {
                return GetValue<T>(expression, context);
            }

            protected override object GenerateValue(Type type, string referenceName, IExecuteStrategy executeStrategy)
            {
                throw new NotImplementedException();
            }
        }
    }
}