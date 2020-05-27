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
        [InlineData(true)]
        [InlineData(false)]
        public void AllowNullDeterminesWhetherNullCanBeReturned(bool allowNull)
        {
            var nullFound = false;

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var buildChain = new BuildHistory();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new NumericValueGenerator
            {
                AllowNull = allowNull
            };

            for (var index = 0; index < 10000; index++)
            {
                var value = sut.Generate(executeStrategy, typeof(int?));

                if (value == null)
                {
                    nullFound = true;

                    break;
                }
            }

            nullFound.Should().Be(allowNull);
        }

        [Fact]
        public void AllowNullReturnsFalseByDefault()
        {
            var sut = new NumericValueGenerator();

            sut.AllowNull.Should().BeFalse();
        }

        [Theory]
        [ClassData(typeof(NumericTypeDataSource))]
        public void GenerateForTypeReturnsRandomValues(Type type, bool typeSupported)
        {
            if (typeSupported == false)
            {
                // Ignore this test
                return;
            }

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new Wrapper();

            var randomValueFound = false;
            var firstValue = sut.RunGenerate(type, null!, executeStrategy);

            for (var index = 0; index < 1000; index++)
            {
                var nextValue = sut.RunGenerate(type, null!, executeStrategy);

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
            var sut = new Wrapper();

            Action action = () => sut.RunGenerate(typeof(int), null!, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GenerateThrowsExceptionWithNullType()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new Wrapper();

            Action action = () => sut.RunGenerate(null!, null!, executeStrategy);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [ClassData(typeof(NumericTypeDataSource))]
        public void IsMatchForTypeEvaluatesRequestedTypeTest(Type type, bool typeSupported)
        {
            var buildChain = Substitute.For<IBuildChain>();

            var sut = new Wrapper();

            var actual = sut.RunIsMatch(type, null!, buildChain);

            actual.Should().Be(typeSupported);
        }

        [Fact]
        public void IsMatchThrowsExceptionWithNullBuildChain()
        {
            var sut = new Wrapper();

            Action action = () => sut.RunIsMatch(typeof(int), null!, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchThrowsExceptionWithNullType()
        {
            var buildChain = Substitute.For<IBuildChain>();

            var sut = new Wrapper();

            Action action = () => sut.RunIsMatch(null!, null!, buildChain);

            action.Should().Throw<ArgumentNullException>();
        }

        private class Wrapper : NumericValueGenerator
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