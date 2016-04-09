using System;
using FluentAssertions;
using Xunit;

namespace ModelBuilder.UnitTests
{
    public class AgeValueGeneratorTests
    {
        [Theory]
        [ClassData(typeof (NumericTypeDataSource))]
        public void GenerateReturnsNewValueTest(Type type, bool typeSupported)
        {
            if (typeSupported == false)
            {
                // Ignore this test
                return;
            }

            var target = new AgeValueGenerator();

            var value = target.Generate(type, "Age", null);

            value.Should().NotBeNull();
            value.Should().BeOfType(type);

            var convertedValue = Convert.ToInt32(value);

            convertedValue.Should().BeLessOrEqualTo(target.MaxAge);
            convertedValue.Should().BeGreaterOrEqualTo(0);
        }

        [Theory]
        [ClassData(typeof (NumericTypeDataSource))]
        public void GenerateThrowsExceptionWhenReferenceNotAgeTest(Type type, bool typeSupported)
        {
            var target = new AgeValueGenerator();

            Action action = () => target.Generate(type, "Stuff", null);

            action.ShouldThrow<NotSupportedException>();
        }

        [Theory]
        [ClassData(typeof (NumericTypeDataSource))]
        public void GenerateValidatesRequestedTypeTest(Type type, bool typeSupported)
        {
            var target = new AgeValueGenerator();

            Action action = () => target.Generate(type, "Age", null);

            if (typeSupported)
            {
                action.ShouldNotThrow();
            }
            else
            {
                action.ShouldThrow<NotSupportedException>();
            }
        }

        [Theory]
        [ClassData(typeof (NumericTypeDataSource))]
        public void IsSupportedEvaluatesRequestedTypeTest(Type type, bool typeSupported)
        {
            var target = new AgeValueGenerator();

            var actual = target.IsSupported(type, "Age", null);

            actual.Should().Be(typeSupported);
        }

        [Theory]
        [ClassData(typeof (NumericTypeDataSource))]
        public void IsSupportedReturnsFalseWhenReferenceNameIsNullTest(Type type, bool typeSupported)
        {
            if (typeSupported == false)
            {
                // Ignore this test
                return;
            }

            var target = new AgeValueGenerator();

            var actual = target.IsSupported(type, null, null);

            actual.Should().BeFalse();
        }

        [Theory]
        [ClassData(typeof (NumericTypeDataSource))]
        public void IsSupportedReturnsFalseWhenReferenceNameNotAgeTest(Type type, bool typeSupported)
        {
            if (typeSupported == false)
            {
                // Ignore this test
                return;
            }

            var target = new AgeValueGenerator();

            var actual = target.IsSupported(type, "Stuff", null);

            actual.Should().BeFalse();
        }

        [Theory]
        [ClassData(typeof (NumericTypeDataSource))]
        public void IsSupportedReturnsTrueWhenReferenceNameIncludesAgeTest(Type type, bool typeSupported)
        {
            if (typeSupported == false)
            {
                // Ignore this test
                return;
            }

            var target = new AgeValueGenerator();

            var actual = target.IsSupported(type, "SomeAgeValue", null);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullTypeTest()
        {
            var target = new AgeValueGenerator();

            Action action = () => target.IsSupported(null, null, null);

            action.ShouldThrow<ArgumentNullException>();
        }

        [Fact]
        public void SettingDefaultMaxAgeOnlyAffectsNewInstancesTest()
        {
            try
            {
                var first = new AgeValueGenerator();

                AgeValueGenerator.DefaultMaxAge = 11;

                var second = new AgeValueGenerator();

                first.MaxAge.Should().Be(100);
                second.MaxAge.Should().Be(11);
            }
            finally
            {
                AgeValueGenerator.DefaultMaxAge = 100;
            }
        }

        [Fact]
        public void SettingMaxAgeShouldNotChangeDefaultMaxAgeTest()
        {
            var target = new AgeValueGenerator
            {
                MaxAge = Environment.TickCount
            };

            AgeValueGenerator.DefaultMaxAge.Should().NotBe(target.MaxAge);
        }
    }
}