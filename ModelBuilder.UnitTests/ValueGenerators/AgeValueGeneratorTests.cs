namespace ModelBuilder.UnitTests.ValueGenerators
{
    using System;
    using System.Globalization;
    using FluentAssertions;
    using ModelBuilder.ValueGenerators;
    using NSubstitute;
    using Xunit;

    public class AgeValueGeneratorTests
    {
        [Theory]
        [ClassData(typeof(NumericTypeRangeDataSource))]
        public void GenerateReturnsValuesBetweenMinAndMaxTest(Type type, bool typeSupported, double min, double max)
        {
            if (typeSupported == false)
            {
                // Ignore this test
                return;
            }

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var buildChain = new BuildHistory();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new Wrapper {MinAge = 15, MaxAge = 30};

            for (var index = 0; index < 1000; index++)
            {
                var value = target.RunGenerate(type, "Age", executeStrategy);

                if (type.IsNullable()
                    && value == null)
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

                convertedValue.Should().BeGreaterOrEqualTo(target.MinAge);
                convertedValue.Should().BeLessOrEqualTo(target.MaxAge);
            }
        }

        [Theory]
        [ClassData(typeof(NumericTypeDataSource))]
        public void IsMatchEvaluatesRequestedTypeTest(Type type, bool typeSupported)
        {
            var buildChain = Substitute.For<IBuildChain>();

            var target = new Wrapper();

            var actual = target.RunIsMatch(type, "Age", buildChain);

            actual.Should().Be(typeSupported);
        }

        [Theory]
        [ClassData(typeof(NumericTypeDataSource))]
        public void IsMatchReturnsFalseWhenReferenceNameIsNullTest(Type type, bool typeSupported)
        {
            if (typeSupported == false)
            {
                // Ignore this test
                return;
            }

            var buildChain = Substitute.For<IBuildChain>();

            var target = new Wrapper();

            var actual = target.RunIsMatch(type, null, buildChain);

            actual.Should().BeFalse();
        }

        [Theory]
        [ClassData(typeof(NumericTypeDataSource))]
        public void IsMatchReturnsFalseWhenReferenceNameNotAgeTest(Type type, bool typeSupported)
        {
            if (typeSupported == false)
            {
                // Ignore this test
                return;
            }

            var buildChain = Substitute.For<IBuildChain>();

            var target = new Wrapper();

            var actual = target.RunIsMatch(type, "Stuff", buildChain);

            actual.Should().BeFalse();
        }

        [Theory]
        [ClassData(typeof(NumericTypeDataSource))]
        public void IsMatchReturnsWhetherTypeIsSupported(Type type, bool typeSupported)
        {
            if (typeSupported == false)
            {
                // Ignore this test
                return;
            }

            var buildChain = Substitute.For<IBuildChain>();

            var target = new Wrapper();

            var actual = target.RunIsMatch(type, "age", buildChain);

            actual.Should().BeTrue();
        }

        [Fact]
        public void MaxAgeDefaultsTo100()
        {
            var target = new AgeValueGenerator();

            target.MaxAge.Should().Be(100);
        }

        [Fact]
        public void MinAgeDefaultsTo1()
        {
            var target = new AgeValueGenerator();

            target.MinAge.Should().Be(1);
        }

        [Fact]
        public void PriorityReturnsValueHigherThanNumericValueGenerator()
        {
            var target = new AgeValueGenerator();
            var other = new NumericValueGenerator();

            target.Priority.Should().BeGreaterThan(other.Priority);
        }

        private class Wrapper : AgeValueGenerator
        {
            public object RunGenerate(Type type, string referenceName, IExecuteStrategy executeStrategy)
            {
                return Generate(executeStrategy, type, referenceName);
            }

            public bool RunIsMatch(Type type, string referenceName, IBuildChain buildChain)
            {
                return IsMatch(buildChain, type, referenceName);
            }
        }
    }
}