namespace ModelBuilder.UnitTests
{
    using System;
    using FluentAssertions;
    using NSubstitute;
    using Xunit;

    public class ValueGeneratorBaseTests
    {
        [Fact]
        public void GenerateReturnsGeneratedValueWhenIsSupportedReturnsTrueTest()
        {
            var type = typeof(string);
            var expected = Guid.NewGuid();
            var buildStrategy = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildStrategy);

            var target = new GenerateWrapper(true, expected);

            var actual = target.Generate(type, null, executeStrategy);

            actual.Should().Be(expected);
        }

        [Fact]
        public void GenerateThrowsExceptionWhenIsSupportedReturnsFalseTest()
        {
            var type = typeof(string);
            var expected = Guid.NewGuid();
            var buildStrategy = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildStrategy);

            var target = new GenerateWrapper(false, expected);

            Action action = () => target.Generate(type, null, executeStrategy);

            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void GenerateThrowsExceptionWithNullExecuteStrategyBuildChainTest()
        {
            var type = typeof(string);
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns((IBuildChain) null);

            var target = Substitute.ForPartsOf<ValueGeneratorBase>();

            Action action = () => target.Generate(type, null, executeStrategy);

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void GenerateThrowsExceptionWithNullExecuteStrategyTest()
        {
            var type = typeof(string);

            var target = Substitute.ForPartsOf<ValueGeneratorBase>();

            Action action = () => target.Generate(type, null, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GenerateThrowsExceptionWithNullTypeTest()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var target = Substitute.ForPartsOf<ValueGeneratorBase>();

            Action action = () => target.Generate(null, null, executeStrategy);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GeneratorReturnsCachedInstanceAcrossMultipleGeneratorsTest()
        {
            var first = new Wrapper();
            var second = new Wrapper();

            var firstActual = first.GetGenerator();
            var secondActual = second.GetGenerator();

            firstActual.Should().BeSameAs(secondActual);
        }

        [Fact]
        public void PriorityReturnsMinimumValueTest()
        {
            var buildStrategy = new BuildHistory();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildStrategy);

            var target = Substitute.ForPartsOf<ValueGeneratorBase>();

            var actual = target.Priority;

            actual.Should().Be(int.MinValue);
        }

        [Fact]
        public void VerifyGenerateRequestThrowsExceptionWithNullExecuteStrategyBuildChainTest()
        {
            var type = typeof(string);
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns((IBuildChain) null);

            var target = new Wrapper();

            Action action = () => target.RunVerifyGenerateRequest(type, null, executeStrategy);

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact]
        public void VerifyGenerateRequestThrowsExceptionWithNullExecuteStrategyTest()
        {
            var type = typeof(string);

            var target = new Wrapper();

            Action action = () => target.RunVerifyGenerateRequest(type, null, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void VerifyGenerateRequestThrowsExceptionWithNullTypeTest()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var target = new Wrapper();

            Action action = () => target.RunVerifyGenerateRequest(null, null, executeStrategy);

            action.Should().Throw<ArgumentNullException>();
        }

        private class GenerateWrapper : ValueGeneratorBase
        {
            private readonly bool _isSupported;
            private readonly object _value;

            public GenerateWrapper(bool isSupported, object value)
            {
                _isSupported = isSupported;
                _value = value;
            }

            public override bool IsSupported(Type type, string referenceName, IBuildChain buildChain)
            {
                return _isSupported;
            }

            public void RunVerifyGenerateRequest(Type type, string referenceName, IExecuteStrategy executeStrategy)
            {
                VerifyGenerateRequest(type, referenceName, executeStrategy);
            }

            protected override object GenerateValue(Type type, string referenceName, IExecuteStrategy executeStrategy)
            {
                return _value;
            }
        }

        private class Wrapper : ValueGeneratorBase
        {
            public IRandomGenerator GetGenerator()
            {
                return Generator;
            }

            public override bool IsSupported(Type type, string referenceName, IBuildChain buildChain)
            {
                throw new NotImplementedException();
            }

            public void RunVerifyGenerateRequest(Type type, string referenceName, IExecuteStrategy executeStrategy)
            {
                VerifyGenerateRequest(type, referenceName, executeStrategy);
            }

            protected override object GenerateValue(Type type, string referenceName, IExecuteStrategy executeStrategy)
            {
                throw new NotImplementedException();
            }
        }
    }
}