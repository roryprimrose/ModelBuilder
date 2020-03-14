namespace ModelBuilder.UnitTests.ValueGenerators
{
    using System;
    using FluentAssertions;
    using ModelBuilder.ValueGenerators;
    using NSubstitute;
    using Xunit;

    public class NumericValueGeneratorTests
    {
        [Theory]
        [ClassData(typeof(NumericTypeRangeDataSource))]
        public void GenerateForTypeReturnsNullAndNonNullValues(Type type, bool typeSupported, double min,
            double max)
        {
            if (typeSupported == false)
            {
                // Ignore this test
                return;
            }

            if (type.IsNullable() == false)
            {
                // Ignore this test
                return;
            }

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var target = new Wrapper();

            var nullFound = false;
            var valueFound = false;

            for (var index = 0; index < 1000; index++)
            {
                var nextValue = target.RunGenerate(type, null, executeStrategy);

                if (nextValue == null)
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
        [ClassData(typeof(NumericTypeRangeDataSource))]
        public void GenerateForTypeReturnsRandomValues(Type type, bool typeSupported, double min, double max)
        {
            if (typeSupported == false)
            {
                // Ignore this test
                return;
            }

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var target = new Wrapper();

            var randomValueFound = false;
            var firstValue = target.RunGenerate(type, null, executeStrategy);

            for (var index = 0; index < 1000; index++)
            {
                var nextValue = target.RunGenerate(type, null, executeStrategy);

                if (firstValue != nextValue)
                {
                    randomValueFound = true;
                }
            }

            randomValueFound.Should().BeTrue();
        }

        [Fact]
        public void GenerateThrowsExceptionWithNullExecuteStrategy()
        {
            var target = new Wrapper();

            Action action = () => target.RunGenerate(typeof(int), null, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GenerateThrowsExceptionWithNullType()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var target = new Wrapper();

            Action action = () => target.RunGenerate(null, null, executeStrategy);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [ClassData(typeof(NumericTypeDataSource))]
        public void IsMatchForTypeEvaluatesRequestedTypeTest(Type type, bool typeSupported)
        {
            var buildChain = Substitute.For<IBuildChain>();

            var target = new Wrapper();

            var actual = target.RunIsMatch(type, null, buildChain);

            actual.Should().Be(typeSupported);
        }

        [Fact]
        public void IsMatchThrowsExceptionWithNullBuildChain()
        {
            var target = new Wrapper();

            Action action = () => target.RunIsMatch(typeof(int), null, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchThrowsExceptionWithNullType()
        {
            var buildChain = Substitute.For<IBuildChain>();

            var target = new Wrapper();

            Action action = () => target.RunIsMatch(null, null, buildChain);

            action.Should().Throw<ArgumentNullException>();
        }

        private class Wrapper : NumericValueGenerator
        {
            public object RunGenerate(Type type, string referenceName, IExecuteStrategy executeStrategy)
            {
                return Generate(type, referenceName, executeStrategy);
            }

            public bool RunIsMatch(Type type, string referenceName, IBuildChain buildChain)
            {
                return IsMatch(type, referenceName, buildChain);
            }
        }
    }
}