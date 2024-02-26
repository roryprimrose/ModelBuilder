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
        [Fact]
        public void CreatesWithDefaultMaxCount()
        {
            var sut = new Wrapper();

            sut.MaxCount.Should().BeGreaterThan(0);
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

            var sut = new Wrapper();

            var value = sut.RunGenerate(type, "Count", executeStrategy);

            if (type.IsNullable()
                && value == null!)
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

            convertedValue.Should().BeLessOrEqualTo(sut.MaxCount);
            convertedValue.Should().BeGreaterOrEqualTo(1);
        }

        [Fact]
        public void GenerateReturnsValueBetweenDefaultMinAndMaxValues()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new Wrapper();

            for (var index = 0; index < 1000; index++)
            {
                var value = (int) sut.RunGenerate(typeof(int), "Count", executeStrategy);

                value.Should().BeGreaterOrEqualTo(1);
                value.Should().BeLessOrEqualTo(sut.MaxCount);
            }
        }

        [Fact]
        public void GenerateReturnsValueBetweenDefinedMinAndMaxValues()
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new Wrapper {MaxCount = 10};

            for (var index = 0; index < 1000; index++)
            {
                var value = (int) sut.RunGenerate(typeof(int), "Count", executeStrategy);

                value.Should().BeGreaterOrEqualTo(1);
                value.Should().BeLessOrEqualTo(sut.MaxCount);
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
        public void IsMatchEvaluatesRequestedReferenceNameTest(string? referenceName, bool isSupported)
        {
            var buildChain = Substitute.For<IBuildChain>();

            var sut = new Wrapper();

            var actual = sut.RunIsMatch(typeof(int), referenceName!, buildChain);

            actual.Should().Be(isSupported);
        }

        [Theory]
        [ClassData(typeof(NumericTypeDataSource))]
        public void IsMatchEvaluatesRequestedTypeTest(Type type, bool isSupported)
        {
            var buildChain = Substitute.For<IBuildChain>();

            var sut = new Wrapper();

            var actual = sut.RunIsMatch(type, "Count", buildChain);

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

            var sut = new Wrapper();

            var actual = sut.RunIsMatch(type, null!, buildChain);

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

            var sut = new Wrapper();

            var actual = sut.RunIsMatch(type, "Stuff", buildChain);

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

            var sut = new Wrapper();

            var actual = sut.RunIsMatch(type, "Count", buildChain);

            actual.Should().BeTrue();
        }

        [Fact]
        public void MaxCountReturnsDefaultValue()
        {
            var sut = new CountValueGenerator();

            sut.MaxCount.Should().Be(30);
        }

        [Fact]
        public void PriorityReturnsGreaterThanNumericValueGenerator()
        {
            var sut = new Wrapper();

            var generator = new NumericValueGenerator();

            sut.Priority.Should().BeGreaterThan(generator.Priority);
        }

        private class Wrapper : CountValueGenerator
        {
            public object RunGenerate(Type type, string referenceName, IExecuteStrategy executeStrategy)
            {
                return Generate(executeStrategy, type, referenceName)!;
            }

            public bool RunIsMatch(Type type, string referenceName, IBuildChain buildChain)
            {
                return IsMatch(buildChain, type, referenceName);
            }
        }
    }
}