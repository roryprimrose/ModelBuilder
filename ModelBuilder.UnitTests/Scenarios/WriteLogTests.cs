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