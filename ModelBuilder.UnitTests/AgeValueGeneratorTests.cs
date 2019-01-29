namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using NSubstitute;
    using Xunit;

    public class AgeValueGeneratorTests
    {
        [Theory]
        [ClassData(typeof(NumericTypeDataSource))]
        public void GenerateCanEvalutateManyTimesTest(Type type, bool typeSupported, double min, double max)
        {
            if (typeSupported == false)
            {
                // Ignore this test
                return;
            }

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var buildChain = new LinkedList<object>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new AgeValueGenerator();

            for (var index = 0; index < 1000; index++)
            {
                var value = target.Generate(type, "Age", executeStrategy);

                if (type.IsNullable() &&
                    (value == null))
                {
                    // Nullable values could be returned so nothing more to assert
                    return;
                }

                var evaluateType = type;

                if (type.IsNullable())
                {
                    evaluateType = type.GenericTypeArguments[0];
                }

                value.Should().BeOfType(evaluateType);

                var convertedValue = Convert.ToDouble(value);

                convertedValue.Should().BeGreaterOrEqualTo(min);
                convertedValue.Should().BeLessOrEqualTo(max);
            }
        }

        [Fact]
        public void GenerateCanReturnNullAndNonNullValuesTest()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var buildChain = new LinkedList<object>();

            executeStrategy.BuildChain.Returns(buildChain);

            var nullFound = false;
            var valueFound = false;

            var target = new AgeValueGenerator();

            for (var index = 0; index < 1000; index++)
            {
                var value = (int?)target.Generate(typeof(int?), "Age", executeStrategy);

                if (value == null)
                {
                    nullFound = true;
                }
                else
                {
                    valueFound = true;
                }

                if (nullFound && valueFound)
                {
                    break;
                }
            }

            nullFound.Should().BeTrue();
            valueFound.Should().BeTrue();
        }

        [Theory]
        [ClassData(typeof(NumericTypeDataSource))]
        public void GenerateReturnsNewValueTest(Type type, bool typeSupported, double min, double max)
        {
            if (typeSupported == false)
            {
                // Ignore this test
                return;
            }

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var buildChain = new LinkedList<object>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new AgeValueGenerator();

            var value = target.Generate(type, "Age", executeStrategy);

            if (type.IsNullable() &&
                (value == null))
            {
                // We can't run the assertions because null is a valid outcome
                return;
            }

            var evaluateType = type;

            if (type.IsNullable())
            {
                evaluateType = type.GenericTypeArguments[0];
            }

            value.Should().BeOfType(evaluateType);

            var convertedValue = Convert.ToDouble(value);

            convertedValue.Should().BeLessOrEqualTo(target.MaxAge);
            convertedValue.Should().BeGreaterOrEqualTo(1);
        }

        [Theory]
        [ClassData(typeof(NumericTypeDataSource))]
        public void GenerateThrowsExceptionWhenReferenceNotAgeTest(
            Type type,
            bool typeSupported,
            double min,
            double max)
        {
            var buildChain = new LinkedList<object>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new AgeValueGenerator();

            Action action = () => target.Generate(type, "Stuff", executeStrategy);

            action.Should().Throw<NotSupportedException>();
        }
        
        [Theory]
        [ClassData(typeof(NumericTypeDataSource))]
        public void GenerateValidatesRequestedTypeTest(Type type, bool typeSupported, double min, double max)
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var buildChain = new LinkedList<object>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new AgeValueGenerator();

            Action action = () => target.Generate(type, "Age", executeStrategy);

            if (typeSupported)
            {
                action.Should().NotThrow();
            }
            else
            {
                action.Should().Throw<NotSupportedException>();
            }
        }

        [Fact]
        public void HasHigherPriorityThanNumericValueGeneratorTest()
        {
            var target = new AgeValueGenerator();
            var other = new NumericValueGenerator();

            target.Priority.Should().BeGreaterThan(other.Priority);
        }

        [Theory]
        [ClassData(typeof(NumericTypeDataSource))]
        public void IsSupportedEvaluatesRequestedTypeTest(Type type, bool typeSupported, double min, double max)
        {
            var target = new AgeValueGenerator();

            var actual = target.IsSupported(type, "Age", null);

            actual.Should().Be(typeSupported);
        }

        [Theory]
        [ClassData(typeof(NumericTypeDataSource))]
        public void IsSupportedReturnsFalseWhenReferenceNameIsNullTest(
            Type type,
            bool typeSupported,
            double min,
            double max)
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
        [ClassData(typeof(NumericTypeDataSource))]
        public void IsSupportedReturnsFalseWhenReferenceNameNotAgeTest(
            Type type,
            bool typeSupported,
            double min,
            double max)
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
        [ClassData(typeof(NumericTypeDataSource))]
        public void IsSupportedReturnsTrueWhenReferenceNameIncludesAgeTest(
            Type type,
            bool typeSupported,
            double min,
            double max)
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

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void SettingDefaultMaxAgeOnlyAffectsNewInstancesTest()
        {
            var expected = AgeValueGenerator.DefaultMaxAge;

            try
            {
                var first = new AgeValueGenerator();

                AgeValueGenerator.DefaultMaxAge = 11;

                var second = new AgeValueGenerator();

                first.MaxAge.Should().Be(expected);
                second.MaxAge.Should().Be(11);
            }
            finally
            {
                AgeValueGenerator.DefaultMaxAge = expected;
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