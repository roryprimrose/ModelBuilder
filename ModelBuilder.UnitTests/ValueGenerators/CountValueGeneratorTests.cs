namespace ModelBuilder.UnitTests.ValueGenerators
{
    using System;
    using System.Globalization;
    using FluentAssertions;
    using ModelBuilder.ValueGenerators;
    using NSubstitute;
    using Xunit;

    public class CountValueGeneratorTests
    {
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

            var target = new Wrapper();

            var value = target.RunGenerate(type, "Count", executeStrategy);

            if (type.IsNullable()
                && value == null)
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
        public void GenerateReturnsValueBetweenDefinedMinAndMaxValues()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new Wrapper();

            for (var index = 0; index < 1000; index++)
            {
                var value = (int) target.RunGenerate(typeof(int), "Count", executeStrategy);

                value.Should().BeGreaterOrEqualTo(1);
                value.Should().BeLessOrEqualTo(target.MaxCount);
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
        public void IsMatchEvaluatesRequestedReferenceNameTest(string referenceName, bool isSupported)
        {
            var buildChain = Substitute.For<IBuildChain>();

            var target = new Wrapper();

            var actual = target.RunIsMatch(typeof(int), referenceName, buildChain);

            actual.Should().Be(isSupported);
        }

        [Theory]
        [ClassData(typeof(NumericTypeDataSource))]
        public void IsMatchEvaluatesRequestedTypeTest(Type type, bool isSupported)
        {
            var buildChain = Substitute.For<IBuildChain>();

            var target = new Wrapper();

            var actual = target.RunIsMatch(type, "Count", buildChain);

            actual.Should().Be(isSupported);
        }

        [Theory]
        [ClassData(typeof(NumericTypeDataSource))]
        public void IsMatchReturnsFalseWhenReferenceNameIsNullTest(Type type, bool isSupported)
        {
            if (isSupported == false)
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
        public void IsMatchReturnsFalseWhenReferenceNameNotCountTest(Type type, bool isSupported)
        {
            if (isSupported == false)
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
        public void IsMatchReturnsTrueWhenReferenceNameIsCountTest(Type type, bool isSupported)
        {
            if (isSupported == false)
            {
                // Ignore this test
                return;
            }

            var buildChain = Substitute.For<IBuildChain>();

            var target = new Wrapper();

            var actual = target.RunIsMatch(type, "Count", buildChain);

            actual.Should().BeTrue();
        }

        [Fact]
        public void PriorityReturnsGreaterThanNumericValueGeneratorTest()
        {
            var target = new Wrapper();
            
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

        private class Wrapper : CountValueGenerator
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