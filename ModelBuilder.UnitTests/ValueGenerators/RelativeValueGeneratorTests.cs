// ReSharper disable UnusedMethodReturnValue.Local

namespace ModelBuilder.UnitTests.ValueGenerators
{
    using System;
    using System.Dynamic;
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
        public void GetValueReturnsDefaultValueWhenDeclaredPropertyNotFound()
        {
            var context = new Person
            {
                LastName = Guid.NewGuid().ToString()
            };

            var sut = new Wrapper<string>(
                NameExpression.FirstName,
                typeof(string));

            var value = sut.ReadValue(NameExpression.Domain, context);

            value.Should().BeNull();
        }

        [Fact]
        public void GetValueReturnsDefaultValueWhenDynamicPropertyNotFound()
        {
            dynamic context = new ExpandoObject();

            context.LastName = Guid.NewGuid().ToString();

            var sut = new Wrapper<string>(
                NameExpression.FirstName,
                typeof(string));

            var value = (string)sut.ReadValue(NameExpression.Domain, context);

            value.Should().BeNull();
        }

        [Fact]
        public void GetValueReturnsValueFromDeclaredProperty()
        {
            var context = new Person
            {
                LastName = Guid.NewGuid().ToString()
            };

            var sut = new Wrapper<string>(
                NameExpression.FirstName,
                typeof(string));

            var value = sut.ReadValue(NameExpression.LastName, context);

            value.Should().Be(context.LastName);
        }

        [Fact]
        public void GetValueReturnsValueFromDynamicProperty()
        {
            dynamic context = new ExpandoObject();

            context.LastName = Guid.NewGuid().ToString();

            var sut = new Wrapper<string>(
                NameExpression.FirstName,
                typeof(string));

            var value = (string)sut.ReadValue(NameExpression.LastName, context);

            value.Should().Be(context.LastName);
        }

        [Theory]
        [InlineData(null!)]
        [InlineData(123)]
        public void GetValueReturnsValueFromNullableDeclaredProperty(int? expected)
        {
            var context = new RelativeNullableInt
            {
                YearLastUsed = expected
            };
            var expression = new Regex("YearLastUsed");

            var sut = new Wrapper<int?>(
                expression,
                typeof(int?));

            var actual = sut.ReadValue(expression, context);

            actual.Should().Be(expected);
        }

        [Theory]
        [InlineData(null!)]
        [InlineData(123)]
        public void GetValueReturnsValueFromNullableDynamicProperty(int? expected)
        {
            dynamic context = new ExpandoObject();

            context.YearLastUsed = expected!;

            var expression = new Regex("YearLastUsed");

            var sut = new Wrapper<int?>(
                expression,
                typeof(int?));

            var actual = (int?)sut.ReadValue(expression, context);

            actual.Should().Be(expected);
        }

        [Fact]
        public void GetValueThrowsExceptionWithNullContext()
        {
            var sut = new Wrapper<string>(
                NameExpression.FirstName,
                (Type) null!);

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Action action = () => sut.ReadValue(NameExpression.LastName, null!);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetValueThrowsExceptionWithNullExpression()
        {
            var context = new Person();

            var sut = new Wrapper<string>(
                NameExpression.FirstName,
                (Type) null!);

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Action action = () => sut.ReadValue(null!, context);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMaleReturnsRandomValueWhenNoContextFound()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new Wrapper<string>(
                NameExpression.FirstName,
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
                NameExpression.FirstName,
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
                NameExpression.FirstName,
                typeof(string));

            var actual = sut.GetIsMale(executeStrategy);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsMaleThrowsExceptionWithNullExecuteStrategy()
        {
            var sut = new Wrapper<string>(
                NameExpression.FirstName,
                typeof(string));

            Action action = () => sut.GetIsMale(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchReturnsTrueForMatchingExpandObject()
        {
            dynamic context = new ExpandoObject();
            var buildChain = new BuildHistory();

            context.FirstName = Guid.NewGuid().ToString();
            context.LastName = string.Empty;

            buildChain.Push(context);

            var sut = new Wrapper<string>(NameExpression.FirstName, typeof(string));

            var actual = sut.RunIsMatch(typeof(string), "FirstName", buildChain);

            actual.Should().BeTrue();
        }

        [Theory]
        [InlineData(typeof(bool), null, null, false)] // Type and name doesn't match
        [InlineData(typeof(string), null, null, false)] // Name is null
        [InlineData(typeof(string), "FirstName", null, false)] // No build context
        [InlineData(typeof(string), "stuff", typeof(Person),
            false)] // Name doesn't match but we have an object to check for properties
        [InlineData(typeof(string), "FirstName", typeof(Guid),
            false)] // Name matches but we don't have an object type to check for properties
        [InlineData(typeof(string), "FirstName", typeof(int),
            false)] // Name matches but we don't have an object type to check for properties
        [InlineData(typeof(string), "FirstName", typeof(string),
            false)] // Name matches but we don't have an object type to check for properties
        [InlineData(typeof(string), "FirstName", typeof(Person), true)]
        public void IsMatchReturnsWhetherScenarioSupported(
            Type type,
            string? referenceName,
            Type? contextType,
            bool expected)
        {
            var buildChain = new BuildHistory();

            if (contextType != null!)
            {
                object context;

                if (contextType == typeof(string))
                {
                    context = Guid.NewGuid().ToString();
                }
                else
                {
                    context = Activator.CreateInstance(contextType)!;
                }

                buildChain.Push(context);
            }

            var sut = new Wrapper<string>(NameExpression.FirstName);

            var actual = sut.RunIsMatch(type, referenceName!, buildChain);

            actual.Should().Be(expected);
        }

        [Fact]
        public void IsMatchThrowsExceptionWithNullBuildChain()
        {
            var sut = new Wrapper<string>(NameExpression.FirstName, typeof(string));

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Action action = () => sut.RunIsMatch(typeof(string), "FirstName", null!);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchThrowsExceptionWithNullType()
        {
            var context = new SlimModel();
            var buildChain = new BuildHistory();

            buildChain.Push(context);

            var sut = new Wrapper<string>(NameExpression.FirstName, typeof(string));

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Action action = () => sut.RunIsMatch(null!, "FirstName", buildChain);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWithNullTargetExpression()
        {
            // ReSharper disable once ObjectCreationAsStatement
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Action action = () => new Wrapper<string>(null!);
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            action.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void ThrowsExceptionWithNullTargetExpressionAndTypes()
        {
            // ReSharper disable once ObjectCreationAsStatement
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            Action action = () => new Wrapper<string>(null!, typeof(string));
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

            action.Should().Throw<ArgumentException>();
        }

        private class Wrapper<T> : RelativeValueGenerator
        {
            public Wrapper(Regex targetNameExpression, params Type[] types) : base(targetNameExpression, types)
            {
            }

            public bool GetIsMale(IExecuteStrategy executeStrategy)
            {
                return IsMale(executeStrategy);
            }

            public T ReadValue(Regex expression, object context)
            {
                return GetValue<T>(expression, context);
            }

            public bool RunIsMatch(Type type, string referenceName, IBuildChain buildChain)
            {
                return IsMatch(buildChain, type, referenceName);
            }

            protected override object? Generate(IExecuteStrategy executeStrategy, Type type, string? referenceName)
            {
                throw new NotImplementedException();
            }
        }
    }
}