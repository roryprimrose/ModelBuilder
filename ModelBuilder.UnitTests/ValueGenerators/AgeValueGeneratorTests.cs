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
        [Fact]
        public void AllowNullReturnsFalseByDefault()
        {
            var sut = new AgeValueGenerator();

            sut.AllowNull.Should().BeFalse();
        }

        [Theory]
        [InlineData(5, 0)]
        [InlineData(364, 0)]
        [InlineData(365, 1)]
        [InlineData(400, 1)]
        [InlineData(365 * 99 + 10, 99)]
        public void CanAssignAgeFromDob(int daysOld, int expectedYears)
        {
            var dob = DateTime.Now.AddDays(-daysOld);

            var model = new AgeFromDob
            {
                DateOfBirth = dob
            };

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var buildChain = new BuildHistory();

            buildChain.Push(model);

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new Wrapper {MinAge = 15, MaxAge = 30};

            var actual = (int) sut.RunGenerate(typeof(int), "age", executeStrategy)!;

            actual.Should().Be(expectedYears);
        }

        [Theory]
        [InlineData(false, 100, false)]
        [InlineData(true, 100, true)]
        [InlineData(true, 50, true)]
        [InlineData(true, 10, true)]
        [InlineData(true, 0, false)]
        public void GenerateReturnsNullBasedOnAllowNullAndPercentageChance(bool allowNull, int percentageChance,
            bool expected)
        {
            var buildChain = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new AgeValueGenerator
            {
                AllowNull = allowNull,
                NullPercentageChance = percentageChance
            };

            var nullFound = false;

            for (var index = 0; index < 1000; index++)
            {
                var actual = (int?) sut.Generate(executeStrategy, typeof(int?));

                if (actual == null!)
                {
                    nullFound = true;
                    break;
                }
            }

            nullFound.Should().Be(expected);
        }

        [Fact]
        public void GenerateReturnsRandomValueWhenDobInFuture()
        {
            var dob = DateTime.Now.AddDays(1);

            var model = new AgeFromDob
            {
                DateOfBirth = dob
            };

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var buildChain = new BuildHistory();

            buildChain.Push(model);

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new Wrapper {MinAge = 15, MaxAge = 30};

            var actual = (int) sut.RunGenerate(typeof(int), "age", executeStrategy)!;

            actual.Should().BeGreaterOrEqualTo(15);
            actual.Should().BeLessOrEqualTo(30);
        }

        [Theory]
        [ClassData(typeof(NumericTypeDataSource))]
        public void GenerateReturnsValuesBetweenMinAndMaxTest(Type type, bool typeSupported)
        {
            if (typeSupported == false)
            {
                // Ignore this test
                return;
            }

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var buildChain = new BuildHistory();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new Wrapper {MinAge = 15, MaxAge = 30};

            for (var index = 0; index < 1000; index++)
            {
                var value = sut.RunGenerate(type, "Age", executeStrategy);

                var evaluateType = type;

                if (type.IsNullable())
                {
                    evaluateType = type.GenericTypeArguments[0];
                }

                value.Should().BeOfType(evaluateType);

                var convertedValue = Convert.ToDouble(value, CultureInfo.InvariantCulture);

                convertedValue.Should().BeGreaterOrEqualTo(sut.MinAge);
                convertedValue.Should().BeLessOrEqualTo(sut.MaxAge);
            }
        }

        [Fact]
        public void GenerateThrowsExceptionWithNullExecuteStrategy()
        {
            var sut = new Wrapper();

            Action action = () => sut.RunGenerate(typeof(string), "Age", null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GenerateThrowsExceptionWithNullType()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new Wrapper();

            Action action = () => sut.RunGenerate(null!, "Age", executeStrategy);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [ClassData(typeof(NumericTypeDataSource))]
        public void IsMatchEvaluatesRequestedTypeTest(Type type, bool typeSupported)
        {
            var buildChain = Substitute.For<IBuildChain>();

            var sut = new Wrapper();

            var actual = sut.RunIsMatch(type, "Age", buildChain);

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

            var sut = new Wrapper();

            var actual = sut.RunIsMatch(type, null!, buildChain);

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

            var sut = new Wrapper();

            var actual = sut.RunIsMatch(type, "Stuff", buildChain);

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

            var sut = new Wrapper();

            var actual = sut.RunIsMatch(type, "age", buildChain);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsMatchThrowsExceptionWithNullExecuteStrategy()
        {
            var sut = new Wrapper();

            Action action = () => sut.RunIsMatch(typeof(string), "Age", null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchThrowsExceptionWithNullType()
        {
            var buildChain = Substitute.For<IBuildChain>();

            var sut = new Wrapper();

            Action action = () => sut.RunIsMatch(null!, "Age", buildChain);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void MaxAgeDefaultsTo100()
        {
            var sut = new AgeValueGenerator();

            sut.MaxAge.Should().Be(100);
        }

        [Fact]
        public void MinAgeDefaultsTo1()
        {
            var sut = new AgeValueGenerator();

            sut.MinAge.Should().Be(1);
        }

        [Fact]
        public void NullPercentageChanceReturns10ByDefault()
        {
            var sut = new AgeValueGenerator();

            sut.NullPercentageChance.Should().Be(10);
        }

        [Fact]
        public void PriorityReturnsValueHigherThanNumericValueGenerator()
        {
            var sut = new AgeValueGenerator();
            var other = new NumericValueGenerator();

            sut.Priority.Should().BeGreaterThan(other.Priority);
        }

        private class Wrapper : AgeValueGenerator
        {
            public object? RunGenerate(Type type, string referenceName, IExecuteStrategy executeStrategy)
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