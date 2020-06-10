namespace ModelBuilder.UnitTests
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using ModelBuilder.UnitTests.Models;
    using NSubstitute;
    using Xunit;
    using Xunit.Abstractions;

    public class ExecuteStrategyExtensionsTests
    {
        private readonly ITestOutputHelper _output;

        public ExecuteStrategyExtensionsTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void BuildChainReturnsValueFromExecuteStrategy()
        {
            var expected = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(expected);

            var actual = executeStrategy.WriteLog(_output.WriteLine).BuildChain;

            actual.Should().BeSameAs(expected);
        }

        [Fact]
        public void ConfigurationReturnsValueFromExecuteStrategy()
        {
            var expected = Substitute.For<IBuildConfiguration>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.Configuration.Returns(expected);

            var actual = executeStrategy.WriteLog(_output.WriteLine).Configuration;

            actual.Should().BeSameAs(expected);
        }

        [Fact]
        public void LogReturnsValueFromExecuteStrategy()
        {
            var expected = Substitute.For<IBuildLog>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.Log.Returns(expected);

            var actual = executeStrategy.WriteLog(_output.WriteLine).Log;

            actual.Should().BeSameAs(expected);
        }

        [Fact]
        public void WriteLogCallsCreateParametersOnExecuteStrategy()
        {
            var method = typeof(Person).GetConstructors().First();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.WriteLog(_output.WriteLine).CreateParameters(method);

            executeStrategy.Received().CreateParameters(method);
        }

        [Fact]
        public void WriteLogCallsInitializeOnExecuteStrategy()
        {
            var configuration = Substitute.For<IBuildConfiguration>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.WriteLog(_output.WriteLine).Initialize(configuration);

            executeStrategy.Received().Initialize(configuration);
        }

        [Fact]
        public void WriteLogForExecuteStrategyThrowsExceptionWithNullAction()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            Action action = () => executeStrategy.WriteLog(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void WriteLogForExecuteStrategyThrowsExceptionWithNullExecuteStrategy()
        {
            Action action = () => ExecuteStrategyExtensions.WriteLog(null!, _output.WriteLine);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void WriteLogForExecuteStrategyTThrowsExceptionWithNullAction()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy<Person>>();

            Action action = () => executeStrategy.WriteLog(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void WriteLogForExecuteStrategyTThrowsExceptionWithNullExecuteStrategy()
        {
            Action action = () => ExecuteStrategyExtensions.WriteLog<Person>(null!, _output.WriteLine);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void WriteLogForExecuteStrategyTWritesLogOnCreate()
        {
            var count = 0;
            var expected = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy<Person>>();

            executeStrategy.Create().Returns(expected);

            var actual = executeStrategy.WriteLog(x =>
            {
                count++;
                _output.WriteLine(x);
            }).Create();

            actual.Should().Be(expected);
            count.Should().Be(1);
        }

        [Fact]
        public void WriteLogForExecuteStrategyTWritesLogOnPopulate()
        {
            var count = 0;
            var expected = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy<Person>>();

            executeStrategy.Populate(expected).Returns(expected);

            var actual = executeStrategy.WriteLog(x =>
            {
                count++;
                _output.WriteLine(x);
            }).Populate(expected);

            actual.Should().Be(expected);
            count.Should().Be(1);
        }

        [Fact]
        public void WriteLogForExecuteStrategyWritesLogOnCreate()
        {
            var count = 0;
            var expected = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.Create(typeof(Person)).Returns(expected);

            var actual = executeStrategy.WriteLog(x =>
            {
                count++;
                _output.WriteLine(x);
            }).Create(typeof(Person));

            actual.Should().Be(expected);
            count.Should().Be(1);
        }

        [Fact]
        public void WriteLogForExecuteStrategyWritesLogOnPopulate()
        {
            var count = 0;
            var expected = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.Populate(expected).Returns(expected);

            var actual = executeStrategy.WriteLog(x =>
            {
                count++;
                _output.WriteLine(x);
            }).Populate(expected);

            actual.Should().Be(expected);
            count.Should().Be(1);
        }
    }
}