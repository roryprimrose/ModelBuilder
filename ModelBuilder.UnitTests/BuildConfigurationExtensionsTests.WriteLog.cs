namespace ModelBuilder.UnitTests
{
    using System;
    using FluentAssertions;
    using ModelBuilder.UnitTests.Models;
    using NSubstitute;
    using Xunit;

    public partial class BuildConfigurationExtensionsTests
    {
        [Fact]
        public void WriteLogForExecuteStrategyReturnsExecuteStrategyWithLogging()
        {
            var configuration = Substitute.For<IBuildConfiguration>();

            var actual = configuration.WriteLog(_output.WriteLine);

            actual.Should().BeAssignableTo<IExecuteStrategy>();
            actual.Should().NotBeOfType<DefaultExecuteStrategy>();
        }

        [Fact]
        public void WriteLogForExecuteStrategyThrowsExceptionWithNullAction()
        {
            var configuration = Substitute.For<IBuildConfiguration>();

            Action action = () => configuration.WriteLog(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void WriteLogForExecuteStrategyThrowsExceptionWithNullConfiguration()
        {
            Action action = () => BuildConfigurationExtensions.WriteLog(null!, _output.WriteLine);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void WriteLogForExecuteStrategyTReturnsExecuteStrategyWithLogging()
        {
            var configuration = Substitute.For<IBuildConfiguration>();

            var actual = configuration.WriteLog<Person>(_output.WriteLine);

            actual.Should().BeAssignableTo<IExecuteStrategy<Person>>();
            actual.Should().NotBeOfType<DefaultExecuteStrategy<Person>>();
        }

        [Fact]
        public void WriteLogForExecuteStrategyTThrowsExceptionWithNullAction()
        {
            var configuration = Substitute.For<IBuildConfiguration>();

            Action action = () => configuration.WriteLog<Person>(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void WriteLogForExecuteStrategyTThrowsExceptionWithNullConfiguration()
        {
            Action action = () => BuildConfigurationExtensions.WriteLog<Person>(null!, _output.WriteLine);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}