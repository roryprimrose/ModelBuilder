namespace ModelBuilder.UnitTests
{
    using System;
    using System.Globalization;
    using FluentAssertions;
    using NSubstitute;
    using Xunit;

    public class CountValueGeneratorTests
    {
        [Theory]
        [ClassData(typeof(NumericTypeRangeDataSource))]
        public void GenerateCanEvaluateManyTimesTest(Type type, bool isSupported, double min, double max)
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            if (isSupported == false)
            {
                // Ignore this test
                return;
            }

            var target = new CountValueGenerator();

            for (var index = 0; index < 1000; index++)
            {
                var value = target.Generate(type, "Count", executeStrategy);

                if (type.IsNullable() &&
                    value == null)
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

                var convertedValue = Convert.ToDouble(value, CultureInfo.InvariantCulture);

                convertedValue.Should().BeGreaterOrEqualTo(min);
                convertedValue.Should().BeLessOrEqualTo(max);
            }
        }

        [Fact]
        public void GenerateCanReturnNullAndNonNullValuesTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var nullFound = false;
            var valueFound = false;

            var target = new CountValueGenerator();

            for (var index = 0; index < 1000; index++)
            {
                var value = (int?)target.Generate(typeof(int?), "Count", executeStrategy);

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
        public void GenerateReturnsNewValueTest(Type type, bool isSupported)
        {
            if (isSupported == false)
            {
                // Ignore this test
                return;
            }

            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new CountValueGenerator();

            var value = target.Generate(type, "Count", executeStrategy);

            if (type.IsNullable() &&
                value == null)
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

            var convertedValue = Convert.ToDouble(value, CultureInfo.InvariantCulture);

            convertedValue.Should().BeLessOrEqualTo(target.MaxCount);
            convertedValue.Should().BeGreaterOrEqualTo(1);
        }

        [Fact]
        public void GenerateThrowsExceptionWhenReferenceNotCountTest()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new CountValueGenerator();

            Action action = () => target.Generate(typeof(int), "Stuff", executeStrategy);

            action.Should().Throw<NotSupportedException>();
        }

        [Theory]
        [ClassData(typeof(NumericTypeDataSource))]
        public void GenerateValidatesRequestedTypeTest(Type type, bool isSupported)
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new CountValueGenerator();

            Action action = () => target.Generate(type, "Count", executeStrategy);

            if (isSupported)
            {
                action.Should().NotThrow();
            }
            else
            {
                action.Should().Throw<NotSupportedException>();
            }
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("", false)]
        [InlineData("other", false)]
        [InlineData("someCount", false)]
        [InlineData("Counter", false)]
        [InlineData("length", true)]
        [InlineData("Length", true)]
        [InlineData("count", true)]
        [InlineData("Count", true)]
        public void IsSupportedEvaluatesRequestedReferenceNameTest(string referenceName, bool isSupported)
        {
            var target = new CountValueGenerator();

            var actual = target.IsSupported(typeof(int), referenceName, null);

            actual.Should().Be(isSupported);
        }

        [Theory]
        [ClassData(typeof(NumericTypeDataSource))]
        public void IsSupportedEvaluatesRequestedTypeTest(Type type, bool isSupported)
        {
            var target = new CountValueGenerator();

            var actual = target.IsSupported(type, "Count", null);

            actual.Should().Be(isSupported);
        }

        [Theory]
        [ClassData(typeof(NumericTypeDataSource))]
        public void IsSupportedReturnsFalseWhenReferenceNameIsNullTest(Type type, bool isSupported)
        {
            if (isSupported == false)
            {
                // Ignore this test
                return;
            }

            var target = new CountValueGenerator();

            var actual = target.IsSupported(type, null, null);

            actual.Should().BeFalse();
        }

        [Theory]
        [ClassData(typeof(NumericTypeDataSource))]
        public void IsSupportedReturnsFalseWhenReferenceNameNotCountTest(Type type, bool isSupported)
        {
            if (isSupported == false)
            {
                // Ignore this test
                return;
            }

            var target = new CountValueGenerator();

            var actual = target.IsSupported(type, "Stuff", null);

            actual.Should().BeFalse();
        }

        [Theory]
        [ClassData(typeof(NumericTypeDataSource))]
        public void IsSupportedReturnsTrueWhenReferenceNameIsCountTest(Type type, bool isSupported)
        {
            if (isSupported == false)
            {
                // Ignore this test
                return;
            }

            var target = new CountValueGenerator();

            var actual = target.IsSupported(type, "Count", null);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsSupportedThrowsExceptionWithNullTypeTest()
        {
            var target = new CountValueGenerator();

            Action action = () => target.IsSupported(null, null, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PriorityReturnsGreaterThanNumericValueGeneratorTest()
        {
            var target = new CountValueGenerator();
            var generator = new NumericValueGenerator();

            target.Priority.Should().BeGreaterThan(generator.Priority);
        }

        [Fact]
        public void SettingDefaultMaxCountOnlyAffectsNewInstancesTest()
        {
            var expected = CountValueGenerator.DefaultMaxCount;

            try
            {
                var first = new CountValueGenerator();

                CountValueGenerator.DefaultMaxCount = 11;

                var second = new CountValueGenerator();

                first.MaxCount.Should().Be(expected);
                second.MaxCount.Should().Be(11);
            }
            finally
            {
                CountValueGenerator.DefaultMaxCount = expected;
            }
        }

        [Fact]
        public void SettingMaxCountShouldNotChangeDefaultMaxCountTest()
        {
            var target = new CountValueGenerator
            {
                MaxCount = Environment.TickCount
            };

            CountValueGenerator.DefaultMaxCount.Should().NotBe(target.MaxCount);
        }
    }
}