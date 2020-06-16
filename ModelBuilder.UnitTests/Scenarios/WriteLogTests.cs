namespace ModelBuilder.UnitTests.Scenarios
{
    using FluentAssertions;
    using ModelBuilder.UnitTests.Models;
    using Xunit;
    using Xunit.Abstractions;

    public class WriteLogTests
    {
        private readonly ITestOutputHelper _output;

        public WriteLogTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void LoggingIsDisabledByDefault()
        {
            var strategy = Model.UsingExecuteStrategy<DefaultExecuteStrategy<Person>>();

            var actual = strategy.Create();

            actual.Should().NotBeNull();

            var log = strategy.Log.Output;

            _output.WriteLine(log);

            log.Should().BeNullOrWhiteSpace();
        }

        [Fact]
        public void LoggingIsEnabledOnWriteLog()
        {
            var strategy = Model.UsingExecuteStrategy<DefaultExecuteStrategy<Person>>().WriteLog(_output.WriteLine);

            var actual = strategy.Create();

            actual.Should().NotBeNull();

            var log = strategy.Log.Output;

            _output.WriteLine(log);

            log.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void WriteLogRendersLogFromConfiguration()
        {
            var actual = Model.Ignoring<Person>(x => x.FirstName).WriteLog<Person>(_output.WriteLine).Create();

            actual.Should().NotBeNull();
        }

        [Fact]
        public void WriteLogRendersLogFromExecuteStrategy()
        {
            var actual = Model.Ignoring<Person>(x => x.FirstName)
                .UsingExecuteStrategy<DefaultExecuteStrategy<Person>>().WriteLog(_output.WriteLine).Create();

            actual.Should().NotBeNull();
        }

        [Fact]
        public void WriteLogRendersLogFromModel()
        {
            var actual = Model.WriteLog<Names>(_output.WriteLine).Create();

            actual.Should().NotBeNull();
        }
    }
}