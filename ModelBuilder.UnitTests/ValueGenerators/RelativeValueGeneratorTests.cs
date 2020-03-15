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
        public void GetSourceValueReturnsNullFromSourceNullableIntProperty()
        {
            var context = new RelativeNullableInt
            {
                YearStarted = null
            };

            var sut = new Wrapper<int?>(PropertyExpression.FirstName, new Regex("YearStarted"));

            var actual = sut.ReadSourceValue(context);

            actual.Should().NotHaveValue();
        }

        [Fact]
        public void GetSourceValueReturnsNullFromSourceProperty()
        {
            var context = new Person();

            var sut = new Wrapper<string>(PropertyExpression.FirstName, PropertyExpression.LastName);

            var actual = sut.ReadSourceValue(context);

            actual.Should().BeNull();
        }

        [Fact]
        public void GetSourceValueReturnsNullWhenPropertyNotFound()
        {
            var context = new SlimModel();

            var sut = new Wrapper<string>(PropertyExpression.FirstName, PropertyExpression.LastName);

            var actual = sut.ReadSourceValue(context);

            actual.Should().BeNull();
        }

        [Fact]
        public void GetSourceValueReturnsValueFromSourceIntProperty()
        {
            var context = new Person
            {
                Priority = Environment.TickCount
            };

            var sut = new Wrapper<int>(PropertyExpression.FirstName, new Regex("Priority"));

            var actual = sut.ReadSourceValue(context);

            actual.Should().Be(context.Priority);
        }

        [Fact]
        public void GetSourceValueReturnsValueFromSourceNullableIntProperty()
        {
            var context = new RelativeNullableInt
            {
                YearStarted = Environment.TickCount
            };

            var sut = new Wrapper<int?>(PropertyExpression.FirstName, new Regex("YearStarted"));

            var actual = sut.ReadSourceValue(context);

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

            var sut = new Wrapper<string>(PropertyExpression.FirstName, PropertyExpression.Gender);

            var actual = sut.ReadSourceValue(context);

            actual.Should().Be(expected);
        }

        [Fact]
        public void GetSourceValueReturnsValueFromSourceStringProperty()
        {
            var context = new Person
            {
                LastName = Guid.NewGuid().ToString()
            };

            var sut = new Wrapper<string>(PropertyExpression.FirstName, PropertyExpression.LastName);

            var actual = sut.ReadSourceValue(context);

            actual.Should().Be(context.LastName);
        }

        [Fact]
        public void GetSourceValueThrowsExceptionWhenNoSourceExpressionProvided()
        {
            var context = new Person();

            var sut = new Wrapper<string>(PropertyExpression.FirstName, (Regex) null, (Type) null);

            Action action = () => sut.ReadSourceValue(context);

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void GetSourceValueThrowsExceptionWithNullContext()
        {
            var sut = new Wrapper<string>(PropertyExpression.FirstName, PropertyExpression.LastName);

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Action action = () => sut.ReadSourceValue(null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetValueThrowsExceptionWithNullContext()
        {
            var sut = new Wrapper<string>(
                PropertyExpression.FirstName,
                PropertyExpression.FirstName,
                (Type) null);

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Action action = () => sut.ReadValue(PropertyExpression.LastName, null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetValueThrowsExceptionWithNullExpression()
        {
            var context = new Person();

            var sut = new Wrapper<string>(
                PropertyExpression.FirstName,
                PropertyExpression.FirstName,
                (Type) null);

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Action action = () => sut.ReadValue(null, context);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMaleReturnsRandomValueWhenNoContextFound()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new Wrapper<string>(
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
        public void IsMaleReturnsRandomValueWhenNoGenderPropertyFound()
        {
            var person = new PersonWithoutGender();
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            buildChain.Push(person);

            var sut = new Wrapper<string>(
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

            var sut = new Wrapper<string>(
                PropertyExpression.FirstName,
                PropertyExpression.Email,
                typeof(string));

            var actual = sut.GetIsMale(executeStrategy);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsMaleThrowsExceptionWithNullExecuteStrategy()
        {
            var sut = new Wrapper<string>(
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
        public void IsMatchReturnsFalseForUnsupportedScenariosTest(
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

            var sut = new Wrapper<string>(PropertyExpression.FirstName, PropertyExpression.Gender);

            var actual = sut.RunIsMatch(type, referenceName, buildChain);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsMatchReturnsTrueWhenSourceExpressionIsNullAndTargetExpressionMatchesReferenceName()
        {
            var context = new SlimModel();
            var buildChain = new BuildHistory();

            buildChain.Push(context);

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            var sut = new Wrapper<string>(PropertyExpression.FirstName, (Regex) null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            var actual = sut.RunIsMatch(typeof(string), "FirstName", buildChain);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsMatchReturnsTrueWhenTargetExpressionMatchesReferenceName()
        {
            var context = new SlimModel();
            var buildChain = new BuildHistory();

            buildChain.Push(context);

            var sut = new Wrapper<string>(PropertyExpression.FirstName, typeof(string));

            var actual = sut.RunIsMatch(typeof(string), "FirstName", buildChain);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsMatchThrowsExceptionWithNullBuildChain()
        {
            var sut = new Wrapper<string>(PropertyExpression.FirstName, typeof(string));

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Action action = () => sut.RunIsMatch(typeof(string), "FirstName", null);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchThrowsExceptionWithNullType()
        {
            var context = new SlimModel();
            var buildChain = new BuildHistory();

            buildChain.Push(context);

            var sut = new Wrapper<string>(PropertyExpression.FirstName, typeof(string));

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Action action = () => sut.RunIsMatch(null, "FirstName", buildChain);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWithNullTargetExpressionAndTypes()
        {
            // ReSharper disable once ObjectCreationAsStatement
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Action action = () => new Wrapper<string>(null, PropertyExpression.FirstName, typeof(string));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ThrowsExceptionWithNullTargetExpression()
        {
            // ReSharper disable once ObjectCreationAsStatement
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Action action = () => new Wrapper<string>(null, PropertyExpression.Gender);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            action.Should().Throw<ArgumentException>();
        }

        private class Wrapper<T> : RelativeValueGenerator
        {
            public Wrapper(Regex targetNameExpression, params Type[] types) : base(targetNameExpression, types)
            {
            }

            public Wrapper(Regex targetNameExpression, Regex sourceNameExpression, params Type[] types) : base(
                targetNameExpression,
                sourceNameExpression,
                types)
            {
            }

            public Wrapper(Regex targetNameExpression, Regex sourceNameExpression) : base(
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

            public bool RunIsMatch(Type type, string referenceName, IBuildChain buildChain)
            {
                return IsMatch(buildChain, type, referenceName);
            }

            protected override object Generate(IExecuteStrategy executeStrategy, Type type, string referenceName)
            {
                throw new NotImplementedException();
            }
        }
    }
}