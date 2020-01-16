// ReSharper disable UnusedMethodReturnValue.Local
namespace ModelBuilder.UnitTests.ValueGenerators
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using FluentAssertions;
    using FluentAssertions.Execution;
    using ModelBuilder.UnitTests.Models;
    using ModelBuilder.ValueGenerators;
    using NSubstitute;
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

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void GetSourceValueThrowsExceptionWithNullContextTest()
        {
            var target = new GeneratorWrapper<string>(PropertyExpression.FirstName, PropertyExpression.LastName);

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Action action = () => target.ReadSourceValue(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetValueThrowsExceptionWithNullContextTest()
        {
            var target = new GeneratorWrapper<string>(
                PropertyExpression.FirstName,
                PropertyExpression.FirstName,
                (Type)null);

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Action action = () => target.ReadValue(PropertyExpression.LastName, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetValueThrowsExceptionWithNullExpressionTest()
        {
            var context = new Person();

            var target = new GeneratorWrapper<string>(
                PropertyExpression.FirstName,
                PropertyExpression.FirstName,
                (Type)null);

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Action action = () => target.ReadValue(null, context);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMaleReturnsRandomValueWhenNoContextFoundTest()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new GeneratorWrapper<string>(
                PropertyExpression.FirstName,
                PropertyExpression.Email,
                typeof(string));

            var maleFound = false;
            var nonMaleFound = false;

            for (var index = 0; index < 1000; index++)
            {
                var actual = sut.GetIsMale(executeStrategy);

                if (actual)
                {
                    maleFound = true;
                }
                else
                {
                    nonMaleFound = true;
                }

                if (maleFound && nonMaleFound)
                {
                    // We have found both random options
                    return;
                }
            }

            throw new AssertionFailedException(
                $"Did not find both values - Male found {maleFound} - Non-male found {nonMaleFound}");
        }

        [Fact]
        public void IsMaleReturnsRandomValueWhenNoGenderPropertyFoundTest()
        {
            var person = new PersonWithoutGender();
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(person);

            var sut = new GeneratorWrapper<string>(
                PropertyExpression.FirstName,
                PropertyExpression.Email,
                typeof(string));

            var maleFound = false;
            var nonMaleFound = false;

            for (var index = 0; index < 1000; index++)
            {
                var actual = sut.GetIsMale(executeStrategy);

                if (actual)
                {
                    maleFound = true;
                }
                else
                {
                    nonMaleFound = true;
                }

                if (maleFound && nonMaleFound)
                {
                    // We have found both random options
                    return;
                }
            }

            throw new AssertionFailedException(
                $"Did not find both values - Male found {maleFound} - Non-male found {nonMaleFound}");
        }

        [Theory]
        [InlineData(Gender.Female, false)]
        [InlineData(Gender.Male, true)]
        [InlineData(Gender.Unknown, false)]
        public void IsMaleReturnsValueBasedOnGenderPropertyTest(Gender gender, bool expected)
        {
            var person = new Person
            {
                Gender = gender
            };
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(person);

            var sut = new GeneratorWrapper<string>(
                PropertyExpression.FirstName,
                PropertyExpression.Email,
                typeof(string));

            var actual = sut.GetIsMale(executeStrategy);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsMaleThrowsExceptionWithNullExecuteStrategyTest()
        {
            var sut = new GeneratorWrapper<string>(
                PropertyExpression.FirstName,
                PropertyExpression.Email,
                typeof(string));

            Action action = () => sut.GetIsMale(null);

            action.Should().Throw<ArgumentNullException>();
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
            var buildChain = new BuildHistory();

            if (contextType != null)
            {
                var context = Activator.CreateInstance(contextType);

                buildChain.Push(context);
            }

            var target = new GeneratorWrapper<string>(PropertyExpression.FirstName, PropertyExpression.Gender);

            var actual = target.IsSupported(type, referenceName, buildChain);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsSupportedReturnsTrueWhenSourceExpressionIsNullAndTargetExpressionMatchesReferenceNameTest()
        {
            var context = new SlimModel();
            var buildChain = new BuildHistory();

            buildChain.Push(context);

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            var target = new GeneratorWrapper<string>(PropertyExpression.FirstName, (Regex)null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            var actual = target.IsSupported(typeof(string), "FirstName", buildChain);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsSupportedReturnsTrueWhenTargetExpressionMatchesReferenceNameTest()
        {
            var context = new SlimModel();
            var buildChain = new BuildHistory();

            buildChain.Push(context);

            var target = new GeneratorWrapper<string>(PropertyExpression.FirstName, typeof(string));

            var actual = target.IsSupported(typeof(string), "FirstName", buildChain);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullBuildChainTest()
        {
            var target = new GeneratorWrapper<string>(PropertyExpression.FirstName, typeof(string));

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Action action = () => target.IsSupported(typeof(string), "FirstName", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullTypeTest()
        {
            var context = new SlimModel();
            var buildChain = new BuildHistory();

            buildChain.Push(context);

            var target = new GeneratorWrapper<string>(PropertyExpression.FirstName, typeof(string));

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Action action = () => target.IsSupported(null, "FirstName", buildChain);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWithNullTargetExpressionAndTypesTest()
        {
            // ReSharper disable once ObjectCreationAsStatement
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Action action = () => new GeneratorWrapper<string>(null, PropertyExpression.FirstName, typeof(string));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ThrowsExceptionWithNullTargetExpressionTest()
        {
            // ReSharper disable once ObjectCreationAsStatement
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Action action = () => new GeneratorWrapper<string>(null, PropertyExpression.Gender);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            action.Should().Throw<ArgumentException>();
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

            public bool GetIsMale(IExecuteStrategy executeStrategy)
            {
                return IsMale(executeStrategy);
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