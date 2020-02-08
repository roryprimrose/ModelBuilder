namespace ModelBuilder.UnitTests.BuildActions
{
    using System;
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using ModelBuilder.BuildActions;
    using ModelBuilder.UnitTests.Models;
    using ModelBuilder.ValueGenerators;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using Xunit;
    using Xunit.Abstractions;

    public class ValueGeneratorBuildActionTests
    {
        private readonly OutputBuildLog _buildLog;
        private readonly ITestOutputHelper _output;

        public ValueGeneratorBuildActionTests(ITestOutputHelper output)
        {
            _output = output;
            _buildLog = new OutputBuildLog(output);
        }

        [Fact]
        public void BuildForParameterInfoReturnsGeneratorValueWhenMatchingGeneratorFound()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var expected = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var generator = Substitute.For<IValueGenerator>();

            buildConfiguration.ValueGenerators.Add(generator);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);

            executeStrategy.Configuration.Returns(buildConfiguration);
            generator.IsMatch(parameterInfo.ParameterType, parameterInfo.Name, buildChain).Returns(true);
            generator.Generate(parameterInfo.ParameterType, parameterInfo.Name, executeStrategy).Returns(expected);

            var sut = new ValueGeneratorBuildAction();

            var actual = sut.Build(parameterInfo, executeStrategy);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForParameterInfoReturnsNullWhenNoGeneratorsExist()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);

            var sut = new ValueGeneratorBuildAction();

            var actual = sut.Build(parameterInfo, executeStrategy);

            actual.Should().BeNull();
        }

        [Fact]
        public void BuildForParameterInfoReturnsNullWhenNoMatchingGeneratorFound()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var generator = Substitute.For<IValueGenerator>();

            buildConfiguration.ValueGenerators.Add(generator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);

            var sut = new ValueGeneratorBuildAction();

            var actual = sut.Build(parameterInfo, executeStrategy);

            actual.Should().BeNull();
        }

        [Fact]
        public void BuildForParameterInfoReturnsValueFromGeneratorWithHighestPriority()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var expected = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var firstGenerator = Substitute.For<IValueGenerator>();
            var secondGenerator = Substitute.For<IValueGenerator>();

            buildConfiguration.ValueGenerators.Add(firstGenerator);
            buildConfiguration.ValueGenerators.Add(secondGenerator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);
            firstGenerator.Priority.Returns(10);
            firstGenerator.IsMatch(parameterInfo.ParameterType, parameterInfo.Name, buildChain).Returns(true);
            secondGenerator.Priority.Returns(20);
            secondGenerator.IsMatch(parameterInfo.ParameterType, parameterInfo.Name, buildChain).Returns(true);
            secondGenerator.Generate(parameterInfo.ParameterType, parameterInfo.Name, executeStrategy)
                .Returns(expected);

            var sut = new ValueGeneratorBuildAction();

            var actual = sut.Build(parameterInfo, executeStrategy);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForParameterInfoThrowsExceptionWithNullExecuteStrategy()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            var sut = new ValueGeneratorBuildAction();

            Action action = () => sut.Build(parameterInfo, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForParameterInfoThrowsExceptionWithNullParameterInfo()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new ValueGeneratorBuildAction();

            Action action = () => sut.Build((ParameterInfo) null, executeStrategy);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForPropertyInfoReturnsGeneratorValueWhenMatchingGeneratorFound()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var expected = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var generator = Substitute.For<IValueGenerator>();

            buildConfiguration.ValueGenerators.Add(generator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);
            generator.IsMatch(propertyInfo.PropertyType, propertyInfo.Name, buildChain).Returns(true);
            generator.Generate(propertyInfo.PropertyType, propertyInfo.Name, executeStrategy).Returns(expected);

            var sut = new ValueGeneratorBuildAction();

            var actual = sut.Build(propertyInfo, executeStrategy);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForPropertyInfoReturnsNullWhenNoGeneratorsExist()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);

            var sut = new ValueGeneratorBuildAction();

            var actual = sut.Build(propertyInfo, executeStrategy);

            actual.Should().BeNull();
        }

        [Fact]
        public void BuildForPropertyInfoReturnsNullWhenNoMatchingGeneratorFound()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var generator = Substitute.For<IValueGenerator>();

            buildConfiguration.ValueGenerators.Add(generator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);

            var sut = new ValueGeneratorBuildAction();

            var actual = sut.Build(propertyInfo, executeStrategy);

            actual.Should().BeNull();
        }

        [Fact]
        public void BuildForPropertyInfoReturnsValueFromGeneratorWithHighestPriority()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var expected = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var firstGenerator = Substitute.For<IValueGenerator>();
            var secondGenerator = Substitute.For<IValueGenerator>();

            buildConfiguration.ValueGenerators.Add(firstGenerator);
            buildConfiguration.ValueGenerators.Add(secondGenerator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);
            firstGenerator.Priority.Returns(10);
            firstGenerator.IsMatch(propertyInfo.PropertyType, propertyInfo.Name, buildChain).Returns(true);
            secondGenerator.Priority.Returns(20);
            secondGenerator.IsMatch(propertyInfo.PropertyType, propertyInfo.Name, buildChain).Returns(true);
            secondGenerator.Generate(propertyInfo.PropertyType, propertyInfo.Name, executeStrategy).Returns(expected);

            var sut = new ValueGeneratorBuildAction();

            var actual = sut.Build(propertyInfo, executeStrategy);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForPropertyInfoThrowsExceptionWithNullExecuteStrategy()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));

            var sut = new ValueGeneratorBuildAction();

            Action action = () => sut.Build(propertyInfo, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForPropertyInfoThrowsExceptionWithNullPropertyInfo()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new ValueGeneratorBuildAction();

            Action action = () => sut.Build((PropertyInfo) null, executeStrategy);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForTypeReturnsGeneratorValueWhenMatchingGeneratorFound()
        {
            var type = typeof(Person);
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var expected = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var generator = Substitute.For<IValueGenerator>();

            buildConfiguration.ValueGenerators.Add(generator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);
            generator.IsMatch(type, null, buildChain).Returns(true);
            generator.Generate(type, null, executeStrategy).Returns(expected);

            var sut = new ValueGeneratorBuildAction();

            var actual = sut.Build(type, executeStrategy);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForTypeReturnsNullWhenNoGeneratorsExist()
        {
            var type = typeof(Person);
            var buildConfiguration = new BuildConfiguration();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.Configuration.Returns(buildConfiguration);

            var sut = new ValueGeneratorBuildAction();

            var actual = sut.Build(type, executeStrategy);

            actual.Should().BeNull();
        }

        [Fact]
        public void BuildForTypeReturnsNullWhenNoMatchingGeneratorFound()
        {
            var type = typeof(Person);
            var buildConfiguration = new BuildConfiguration();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var generator = Substitute.For<IValueGenerator>();

            buildConfiguration.ValueGenerators.Add(generator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.Log.Returns(_buildLog);

            var sut = new ValueGeneratorBuildAction();

            var actual = sut.Build(type, executeStrategy);

            actual.Should().BeNull();
        }

        [Fact]
        public void BuildForTypeReturnsValueFromGeneratorWithHighestPriority()
        {
            var type = typeof(Person);
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var expected = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var firstGenerator = Substitute.For<IValueGenerator>();
            var secondGenerator = Substitute.For<IValueGenerator>();

            buildConfiguration.ValueGenerators.Add(firstGenerator);
            buildConfiguration.ValueGenerators.Add(secondGenerator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);
            firstGenerator.Priority.Returns(10);
            firstGenerator.IsMatch(type, null, buildChain).Returns(true);
            secondGenerator.Priority.Returns(20);
            secondGenerator.IsMatch(type, null, buildChain).Returns(true);
            secondGenerator.Generate(type, null, executeStrategy).Returns(expected);

            var sut = new ValueGeneratorBuildAction();

            var actual = sut.Build(type, executeStrategy);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForTypeThrowsExceptionWithNullExecuteStrategy()
        {
            var type = typeof(Person);

            var sut = new ValueGeneratorBuildAction();

            Action action = () => sut.Build(type, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForTypeThrowsExceptionWithNullType()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new ValueGeneratorBuildAction();

            Action action = () => sut.Build((Type) null, executeStrategy);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildLogsValueCreation()
        {
            var type = typeof(string);
            var buildConfiguration = new BuildConfiguration();
            var expected = Guid.NewGuid().ToString();

            var buildLog = Substitute.For<IBuildLog>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var generator = Substitute.For<IValueGenerator>();

            buildConfiguration.ValueGenerators.Add(generator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(buildLog);
            generator.IsMatch(type, null, buildChain).Returns(true);
            generator.Generate(type, null, executeStrategy).Returns(expected);

            var sut = new ValueGeneratorBuildAction();

            sut.Build(type, executeStrategy);

            buildLog.Received().CreatingValue(type, generator.GetType(), null);
        }

        [Fact]
        public void BuildRethrowsBuildException()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildConfiguration = new BuildConfiguration();

            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var generator = Substitute.For<IValueGenerator>();

            buildConfiguration.ValueGenerators.Add(generator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);
            generator.IsMatch(parameterInfo.ParameterType, parameterInfo.Name, buildChain).Returns(true);
            generator.Generate(parameterInfo.ParameterType, parameterInfo.Name, executeStrategy)
                .Throws<BuildException>();

            var sut = new ValueGeneratorBuildAction();

            Action action = () => sut.Build(parameterInfo, executeStrategy);

            var exception = action.Should().Throw<BuildException>().Which;

            _output.WriteLine(exception.Message);
        }

        [Fact]
        public void BuildThrowsExceptionWhenGeneratorFails()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var buildConfiguration = new BuildConfiguration();

            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var generator = Substitute.For<IValueGenerator>();

            buildConfiguration.ValueGenerators.Add(generator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);
            generator.IsMatch(propertyInfo.PropertyType, propertyInfo.Name, buildChain).Returns(true);
            generator.Generate(propertyInfo.PropertyType, propertyInfo.Name, executeStrategy)
                .Throws<TimeoutException>();

            var sut = new ValueGeneratorBuildAction();

            Action action = () => sut.Build(propertyInfo, executeStrategy);

            var exception = action.Should().Throw<BuildException>().Which;

            exception.InnerException.Should().BeOfType<TimeoutException>();

            _output.WriteLine(exception.Message);
        }

        [Fact]
        public void IsMatchForParameterInfoReturnsGeneratorValueWhenMatchingGeneratorFound()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var expected = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var generator = Substitute.For<IValueGenerator>();

            buildConfiguration.ValueGenerators.Add(generator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);
            generator.IsMatch(parameterInfo.ParameterType, parameterInfo.Name, buildChain).Returns(true);
            generator.Generate(parameterInfo.ParameterType, parameterInfo.Name, executeStrategy).Returns(expected);

            var sut = new ValueGeneratorBuildAction();

            var actual = sut.IsMatch(parameterInfo, buildConfiguration, buildChain);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsMatchForParameterInfoReturnsNullWhenNoGeneratorsExist()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);

            var sut = new ValueGeneratorBuildAction();

            var actual = sut.IsMatch(parameterInfo, buildConfiguration, buildChain);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchForParameterInfoReturnsNullWhenNoMatchingGeneratorFound()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var generator = Substitute.For<IValueGenerator>();

            buildConfiguration.ValueGenerators.Add(generator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);

            var sut = new ValueGeneratorBuildAction();

            var actual = sut.IsMatch(parameterInfo, buildConfiguration, buildChain);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchForParameterInfoThrowsExceptionWithNullBuildChain()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildConfiguration = new BuildConfiguration();

            var sut = new ValueGeneratorBuildAction();

            Action action = () => sut.IsMatch(parameterInfo, buildConfiguration, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForParameterInfoThrowsExceptionWithNullBuildConfiguration()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildChain = new BuildHistory();

            var sut = new ValueGeneratorBuildAction();

            Action action = () => sut.IsMatch(parameterInfo, null, buildChain);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForParameterInfoThrowsExceptionWithNullParameterInfo()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var sut = new ValueGeneratorBuildAction();

            Action action = () => sut.IsMatch((ParameterInfo) null, buildConfiguration, buildChain);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForPropertyInfoReturnsGeneratorValueWhenMatchingGeneratorFound()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var expected = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var generator = Substitute.For<IValueGenerator>();

            buildConfiguration.ValueGenerators.Add(generator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);
            generator.IsMatch(propertyInfo.PropertyType, propertyInfo.Name, buildChain).Returns(true);
            generator.Generate(propertyInfo.PropertyType, propertyInfo.Name, executeStrategy).Returns(expected);

            var sut = new ValueGeneratorBuildAction();

            var actual = sut.IsMatch(propertyInfo, buildConfiguration, buildChain);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsMatchForPropertyInfoReturnsNullWhenNoGeneratorsExist()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);

            var sut = new ValueGeneratorBuildAction();

            var actual = sut.IsMatch(propertyInfo, buildConfiguration, buildChain);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchForPropertyInfoReturnsNullWhenNoMatchingGeneratorFound()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var generator = Substitute.For<IValueGenerator>();

            buildConfiguration.ValueGenerators.Add(generator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);

            var sut = new ValueGeneratorBuildAction();

            var actual = sut.IsMatch(propertyInfo, buildConfiguration, buildChain);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchForPropertyInfoThrowsExceptionWithNullBuildChain()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var buildConfiguration = new BuildConfiguration();

            var sut = new ValueGeneratorBuildAction();

            Action action = () => sut.IsMatch(propertyInfo, buildConfiguration, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForPropertyInfoThrowsExceptionWithNullBuildConfiguration()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var buildChain = new BuildHistory();

            var sut = new ValueGeneratorBuildAction();

            Action action = () => sut.IsMatch(propertyInfo, null, buildChain);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForPropertyInfoThrowsExceptionWithNullPropertyInfo()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var sut = new ValueGeneratorBuildAction();

            Action action = () => sut.IsMatch((PropertyInfo) null, buildConfiguration, buildChain);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForTypeReturnsGeneratorValueWhenMatchingGeneratorFound()
        {
            var type = typeof(Person);
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var expected = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var generator = Substitute.For<IValueGenerator>();

            buildConfiguration.ValueGenerators.Add(generator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);
            generator.IsMatch(type, null, buildChain).Returns(true);
            generator.Generate(type, null, executeStrategy).Returns(expected);

            var sut = new ValueGeneratorBuildAction();

            var actual = sut.IsMatch(type, buildConfiguration, buildChain);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsMatchForTypeReturnsNullWhenNoGeneratorsExist()
        {
            var type = typeof(Person);
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new ValueGeneratorBuildAction();

            var actual = sut.IsMatch(type, buildConfiguration, buildChain);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchForTypeReturnsNullWhenNoMatchingGeneratorFound()
        {
            var type = typeof(Person);
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var generator = Substitute.For<IValueGenerator>();

            buildConfiguration.ValueGenerators.Add(generator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);

            var sut = new ValueGeneratorBuildAction();

            var actual = sut.IsMatch(type, buildConfiguration, buildChain);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchForTypeThrowsExceptionWithNullBuildChain()
        {
            var type = typeof(Person);
            var buildConfiguration = new BuildConfiguration();

            var sut = new ValueGeneratorBuildAction();

            Action action = () => sut.IsMatch(type, buildConfiguration, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForTypeThrowsExceptionWithNullBuildConfiguration()
        {
            var type = typeof(Person);
            var buildChain = new BuildHistory();

            var sut = new ValueGeneratorBuildAction();

            Action action = () => sut.IsMatch(type, null, buildChain);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForTypeThrowsExceptionWithNullType()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var sut = new ValueGeneratorBuildAction();

            Action action = () => sut.IsMatch((Type) null, buildConfiguration, buildChain);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PriorityReturnsLowerThanCreationRuleBuildActionPriority()
        {
            var otherStep = new CreationRuleBuildAction();

            var sut = new ValueGeneratorBuildAction();

            sut.Priority.Should().BeLessThan(otherStep.Priority);
        }
    }
}