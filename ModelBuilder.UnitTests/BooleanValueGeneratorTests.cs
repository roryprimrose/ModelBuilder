using System;
using FluentAssertions;
using Xunit;

namespace ModelBuilder.UnitTests
{
    public class BooleanValueGeneratorTests
    {
        [Fact]
        public void GenerateReturnsRandomValuesForBooleanTypeTest()
        {
            var target = new BooleanValueGenerator();

            var trueFound = false;
            var falseFound = false;

            for (var index = 0; index < 1000; index++)
            {
                var actual = (bool) target.Generate(typeof (bool), null, null);

                if (actual)
                {
                    trueFound = true;
                }
                else
                {
                    falseFound = true;
                }

                if (trueFound && falseFound)
                {
                    break;
                }
            }

            trueFound.Should().BeTrue();
            falseFound.Should().BeTrue();
        }

        [Fact]
        public void GenerateReturnsRandomValuesForNullabeBooleanTypeTest()
        {
            var target = new BooleanValueGenerator();

            var nullFound = false;
            var trueFound = false;
            var falseFound = false;

            for (var index = 0; index < 1000; index++)
            {
                var actual = (bool?) target.Generate(typeof (bool?), null, null);

                if (actual == null)
                {
                    nullFound = true;
                }
                else if (actual.Value)
                {
                    trueFound = true;
                }
                else
                {
                    falseFound = true;
                }

                if (nullFound &&
                    trueFound &&
                    falseFound)
                {
                    break;
                }
            }

            nullFound.Should().BeTrue();
            trueFound.Should().BeTrue();
            falseFound.Should().BeTrue();
        }

        [Fact]
        public void GenerateReturnsValueForBooleanTypeTest()
        {
            var target = new BooleanValueGenerator();

            var actual = target.Generate(typeof (bool), null, null);

            actual.Should().BeOfType<bool>();
        }

        [Fact]
        public void GenerateReturnsValueForNullableBooleanTypeTest()
        {
            var target = new BooleanValueGenerator();

            var actual = target.Generate(typeof (bool?), null, null);

            if (actual != null)
            {
                var converted = actual as bool?;

                converted.Should().HaveValue();
            }
        }

        [Fact]
        public void GenerateThrowsExceptionWithNullTypeTest()
        {
            var target = new BooleanValueGenerator();

            Action action = () => target.Generate(null, null, null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void IsSupportedReturnsFalseForUnsupportedTypeTest()
        {
            var target = new BooleanValueGenerator();

            var actual = target.IsSupported(typeof (string), null, null);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsSupportedReturnsTrueForBooleanTypeTest()
        {
            var target = new BooleanValueGenerator();

            var actual = target.IsSupported(typeof (bool), null, null);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsSupportedReturnsTrueForNullableBooleanTypeTest()
        {
            var target = new BooleanValueGenerator();

            var actual = target.IsSupported(typeof (bool?), null, null);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullTypeTest()
        {
            var target = new BooleanValueGenerator();

            Action action = () => target.IsSupported(null, null, null);

            action.ShouldThrow<ArgumentNullException>();
        }
    }
}