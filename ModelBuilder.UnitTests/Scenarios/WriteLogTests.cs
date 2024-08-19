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
        public void LoggingIsDisabledByDefaultInGenericStrategy()
        {
            var strategy = Model.UsingExecuteStrategy<DefaultExecuteStrategy<Person>>();

            strategy.Log.IsEnabled.Should().BeFalse();
        }

        [Fact]
        public void LoggingIsDisabledByDefaultInNonGenericStrategy()
        {
            var strategy = new DefaultExecuteStrategy();

            strategy.Log.IsEnabled.Should().BeFalse();
        }

        [Fact]
        public void LoggingIsEnabledOnWriteLogForGenericStrategy()
        {
            var strategy = Model.UsingExecuteStrategy<DefaultExecuteStrategy<Person>>().WriteLog(_output.WriteLine);

            strategy.Log.IsEnabled.Should().BeTrue();
        }

        [Fact]
        public void LoggingIsEnabledOnWriteLogForNonGenericStrategy()
        {
            var strategy = new DefaultExecuteStrategy().WriteLog(_output.WriteLine);

            strategy.Log.IsEnabled.Should().BeTrue();
        }

        [Fact]
        public void WriteLogRendersLogFromGenericConfigurationCreate()
        {
            var strategy = Model.Ignoring<Person>(x => x.FirstName).WriteLog<Person>(_output.WriteLine);

            var actual = strategy.Create();

            var log = strategy.Log.Output;

            _output.WriteLine(log);

            log.Should().NotBeNullOrWhiteSpace();

            actual.Should().NotBeNull();
        }

        [Fact]
        public void WriteLogRendersLogFromGenericConfigurationPopulate()
        {
            var person = new Person();

            var strategy = Model.Ignoring<Person>(x => x.FirstName).WriteLog<Person>(_output.WriteLine);

            var actual = strategy.Populate(person);

            var log = strategy.Log.Output;

            _output.WriteLine(log);

            log.Should().NotBeNullOrWhiteSpace();

            actual.Should().NotBeNull();
        }

        [Fact]
        public void WriteLogRendersLogFromGenericExecuteStrategyCreate()
        {
            var strategy = Model.Ignoring<Person>(x => x.FirstName)
                .UsingExecuteStrategy<DefaultExecuteStrategy<Person>>().WriteLog(_output.WriteLine);

            var actual = strategy.Create();

            var log = strategy.Log.Output;

            _output.WriteLine(log);

            log.Should().NotBeNullOrWhiteSpace();

            actual.Should().NotBeNull();
        }

        [Fact]
        public void WriteLogRendersLogFromGenericExecuteStrategyPopulate()
        {
            var person = new Person();

            var strategy = Model.Ignoring<Person>(x => x.FirstName)
                .UsingExecuteStrategy<DefaultExecuteStrategy<Person>>().WriteLog(_output.WriteLine);

            var actual = strategy.Populate(person);

            var log = strategy.Log.Output;

            _output.WriteLine(log);

            log.Should().NotBeNullOrWhiteSpace();

            actual.Should().NotBeNull();
        }

        [Fact]
        public void WriteLogRendersLogFromGenericModelCreate()
        {
            var strategy = Model.WriteLog<Names>(_output.WriteLine);

            var actual = strategy.Create();

            var log = strategy.Log.Output;

            _output.WriteLine(log);

            log.Should().NotBeNullOrWhiteSpace();

            actual.Should().NotBeNull();
        }

        [Fact]
        public void WriteLogRendersLogFromGenericModelPopulate()
        {
            var person = new Person();

            var strategy = Model.WriteLog<Names>(_output.WriteLine);

            var actual = strategy.Populate(person);

            var log = strategy.Log.Output;

            _output.WriteLine(log);

            log.Should().NotBeNullOrWhiteSpace();

            actual.Should().NotBeNull();
        }

        [Fact]
        public void WriteLogRendersLogFromNonGenericConfigurationCreate()
        {
            var strategy = Model.UsingDefaultConfiguration().WriteLog(_output.WriteLine);

            var actual = strategy.Create(typeof(Person));

            var log = strategy.Log.Output;

            _output.WriteLine(log);

            log.Should().NotBeNullOrWhiteSpace();

            actual.Should().NotBeNull();
        }

        [Fact]
        public void WriteLogRendersLogFromNonGenericConfigurationPopulate()
        {
            var person = new Person();

            var strategy = Model.UsingDefaultConfiguration().WriteLog(_output.WriteLine);

            var actual = strategy.Populate(person);

            var log = strategy.Log.Output;

            _output.WriteLine(log);

            log.Should().NotBeNullOrWhiteSpace();

            actual.Should().NotBeNull();
        }

        [Fact]
        public void WriteLogRendersLogFromNonGenericExecuteStrategyCreate()
        {
            var strategy = Model.UsingExecuteStrategy<DefaultExecuteStrategy>().WriteLog(_output.WriteLine);

            var actual = strategy.Create(typeof(Person));

            var log = strategy.Log.Output;

            _output.WriteLine(log);

            log.Should().NotBeNullOrWhiteSpace();

            actual.Should().NotBeNull();
        }

        [Fact]
        public void WriteLogRendersLogFromNonGenericExecuteStrategyPopulate()
        {
            var person = new Person();

            var strategy = Model.UsingExecuteStrategy<DefaultExecuteStrategy>().WriteLog(_output.WriteLine);

            var actual = strategy.Populate(person);

            var log = strategy.Log.Output;

            _output.WriteLine(log);

            log.Should().NotBeNullOrWhiteSpace();

            actual.Should().NotBeNull();
        }

        [Fact]
        public void WriteLogRendersLogFromNonGenericModelCreate()
        {
            var strategy = Model.WriteLog(_output.WriteLine);

            var actual = strategy.Create(typeof(Person));

            var log = strategy.Log.Output;

            _output.WriteLine(log);

            log.Should().NotBeNullOrWhiteSpace();

            actual.Should().NotBeNull();
        }

        [Fact]
        public void WriteLogRendersLogFromNonGenericModelPopulate()
        {
            var person = new Person();

            var strategy = Model.WriteLog(_output.WriteLine);

            var actual = strategy.Populate(person);

            var log = strategy.Log.Output;

            _output.WriteLine(log);

            log.Should().NotBeNullOrWhiteSpace();

            actual.Should().NotBeNull();
        }

        [Fact]
        public void WriteLogRendersLogWhenGenericCreateFails()
        {
            var actual = string.Empty;
            var strategy = Model.WriteLog<StructConstructorException>(x => actual = x);

            var action = () => strategy.Create();

            action.Should().Throw<BuildException>();

            _output.WriteLine(actual);

            strategy.Log.Output.Should().NotBeNullOrWhiteSpace();
            actual.Should().Be(strategy.Log.Output);

            actual.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void WriteLogRendersLogWhenGenericPopulateFails()
        {
            var value = new PropertySetFailure();
            var actual = string.Empty;
            var strategy = Model.WriteLog<StructConstructorException>(x => actual = x);

            var action = () => strategy.Populate(value);

            action.Should().Throw<BuildException>();

            _output.WriteLine(actual);

            strategy.Log.Output.Should().NotBeNullOrWhiteSpace();
            actual.Should().Be(strategy.Log.Output);

            actual.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void WriteLogRendersLogWhenNonGenericCreateFails()
        {
            var actual = string.Empty;
            var strategy = Model.WriteLog(x => actual = x);

            var action = () => strategy.Create(typeof(StructConstructorException));

            action.Should().Throw<BuildException>();

            _output.WriteLine(actual);

            strategy.Log.Output.Should().NotBeNullOrWhiteSpace();
            actual.Should().Be(strategy.Log.Output);

            actual.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void WriteLogRendersLogWhenNonGenericPopulateFails()
        {
            var value = new PropertySetFailure();
            var actual = string.Empty;
            var strategy = Model.WriteLog(x => actual = x);

            var action = () => strategy.Populate(value);

            action.Should().Throw<BuildException>();

            _output.WriteLine(actual);

            strategy.Log.Output.Should().NotBeNullOrWhiteSpace();
            actual.Should().Be(strategy.Log.Output);

            actual.Should().NotBeNullOrWhiteSpace();
        }
    }
}