namespace ModelBuilder.UnitTests.ValueGenerators
{
    using System;
    using System.Globalization;
    using FluentAssertions;
    using ModelBuilder.TypeCreators;
    using ModelBuilder.ValueGenerators;
    using NSubstitute;
    using Xunit;

    public class AgeValueGeneratorTests
    {
        [Fact]
        public void CanCreateParameters()
        {
            var config = BuildConfigurationFactory.CreateEmpty().AddTypeCreator<DefaultTypeCreator>()
                .AddValueGenerator<AgeValueGenerator>();

            var actual = config.Create<ParameterTest>();

            actual.Age.Should().NotBe(0);
            actual.MyAge.Should().NotBe(0);
            actual.AgeOfRelic.Should().NotBe(0);
        }

        [Fact]
        public void CanPopulateProperties()
        {
            var config = BuildConfigurationFactory.CreateEmpty().AddTypeCreator<DefaultTypeCreator>()
                .AddValueGenerator<AgeValueGenerator>();

            var actual = config.Create<PropertyTest>();

            actual.Age.Should().NotBe(0);
            actual.MyAge.Should().NotBe(0);
            actual.AgeOfRelic.Should().NotBe(0);
        }

        [Theory]
        [ClassData(typeof(NumericTypeRangeDataSource))]
        public void GenerateCanEvaluateManyTimesTest(Type type, bool typeSupported, double min, double max)
        {
            if (typeSupported == false)
            {
                // Ignore this test
                return;
            }

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var buildChain = new BuildHistory();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new AgeValueGenerator();

            for (var index = 0; index < 1000; index++)
            {
                var value = target.Generate(type, "Age", executeStrategy);

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

                convertedValue.Should().BeGreaterOrEqualTo(min);
                convertedValue.Should().BeLessOrEqualTo(max);
            }
        }

        [Fact]
        public void GenerateCanReturnNullAndNonNullValuesTest()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var buildChain = new BuildHistory();

            executeStrategy.BuildChain.Returns(buildChain);

            var nullFound = false;
            var valueFound = false;

            var target = new AgeValueGenerator();

            for (var index = 0; index < 1000; index++)
            {
                var value = (int?) target.Generate(typeof(int?), "Age", executeStrategy);

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
        public void GenerateReturnsNewValueTest(Type type, bool typeSupported)
        {
            if (typeSupported == false)
            {
                // Ignore this test
                return;
            }

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var buildChain = new BuildHistory();

            executeStrategy.BuildChain.Returns(buildChain);

            var target = new AgeValueGenerator();

            var value = target.Generate(type, "Age", executeStrategy);

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

            convertedValue.Should().BeLessOrEqualTo(target.MaxAge);
            convertedValue.Should().BeGreaterOrEqualTo(1);
        }

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

            var target = new AgeValueGenerator();

            target.MinAge = 15;
            target.MaxAge = 30;

            for (var index = 0; index < 1000; index++)
            {
                var value = target.Generate(type, "Age", executeStrategy);

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
        public void GenerateValidatesRequestedTypeTest(Type type, bool typeSupported)
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var buildChain = new BuildHistory();

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

        [Theory]
        [ClassData(typeof(NumericTypeDataSource))]
        public void IsMatchEvaluatesRequestedTypeTest(Type type, bool typeSupported)
        {
            var buildChain = Substitute.For<IBuildChain>();

            var target = new AgeValueGenerator();

            var actual = target.IsMatch(type, "Age", buildChain);

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

            var target = new AgeValueGenerator();

            var actual = target.IsMatch(type, null, buildChain);

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

            var target = new AgeValueGenerator();

            var actual = target.IsMatch(type, "Stuff", buildChain);

            actual.Should().BeFalse();
        }

        [Theory]
        [ClassData(typeof(NumericTypeDataSource))]
        public void IsMatchReturnsTrueWhenReferenceNameIncludesAgeTest(Type type, bool typeSupported)
        {
            if (typeSupported == false)
            {
                // Ignore this test
                return;
            }

            var buildChain = Substitute.For<IBuildChain>();

            var target = new AgeValueGenerator();

            var actual = target.IsMatch(type, "SomeAgeValue", buildChain);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsMatchThrowsExceptionWithNullTypeTest()
        {
            var target = new AgeValueGenerator();

            Action action = () => target.IsMatch(null, null, null);

            action.Should().Throw<ArgumentNullException>();
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
        public void PriorityReturnsValueHigherThanNumericValueGeneratorTest()
        {
            var target = new AgeValueGenerator();
            var other = new NumericValueGenerator();

            target.Priority.Should().BeGreaterThan(other.Priority);
        }

        private class ParameterTest
        {
            public ParameterTest(int age, byte myAge, long ageOfRelic)
            {
                Age = age;
                MyAge = myAge;
                AgeOfRelic = ageOfRelic;
            }

            public int Age { get; }

            public long AgeOfRelic { get; }

            public byte MyAge { get; }
        }

        private class PropertyTest
        {
            public int Age { get; set; }

            public long AgeOfRelic { get; set; }

            public byte MyAge { get; set; }
        }
    }
}