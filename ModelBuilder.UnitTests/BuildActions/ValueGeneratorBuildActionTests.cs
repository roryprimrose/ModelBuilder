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

            var actual = sut.Build(executeStrategy, parameterInfo);

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

            var actual = sut.Build(executeStrategy, parameterInfo);

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

            var actual = sut.Build(executeStrategy, parameterInfo);

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

            var actual = sut.Build(executeStrategy, parameterInfo);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForParameterInfoThrowsExceptionWithNullExecuteStrategy()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            var sut = new ValueGeneratorBuildAction();

            Action action = () => sut.Build(null, parameterInfo);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForParameterInfoThrowsExceptionWithNullParameterInfo()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new ValueGeneratorBuildAction();

            Action action = () => sut.Build(executeStrategy, (ParameterInfo) null);

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

            var actual = sut.Build(executeStrategy, propertyInfo);

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

            var actual = sut.Build(executeStrategy, propertyInfo);

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

            var actual = sut.Build(executeStrategy, propertyInfo);

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

            var actual = sut.Build(executeStrategy, propertyInfo);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForPropertyInfoThrowsExceptionWithNullExecuteStrategy()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));

            var sut = new ValueGeneratorBuildAction();

            Action action = () => sut.Build(null, propertyInfo);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForPropertyInfoThrowsExceptionWithNullPropertyInfo()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new ValueGeneratorBuildAction();

            Action action = () => sut.Build(executeStrategy, (PropertyInfo) null);

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

            var actual = sut.Build(executeStrategy, type);

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

            var actual = sut.Build(executeStrategy, type);

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

            var actual = sut.Build(executeStrategy, type);

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

            var actual = sut.Build(executeStrategy, type);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForTypeThrowsExceptionWithNullExecuteStrategy()
        {
            var type = typeof(Person);

            var sut = new ValueGeneratorBuildAction();

            Action action = () => sut.Build(null, type);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForTypeThrowsExceptionWithNullType()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new ValueGeneratorBuildAction();

            Action action = () => sut.Build(executeStrategy, (Type) null);

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

            sut.Build(executeStrategy, type);

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

            Action action = () => sut.Build(executeStrategy, parameterInfo);

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

            Action action = () => sut.Build(executeStrategy, propertyInfo);

            var exception = action.Should().Throw<BuildException>().Which;

            exception.InnerException.Should().BeOfType<TimeoutException>();

            _output.WriteLine(exception.Message);
        }

        [Fact]
        public void GetBuildCapabilityForParameterInfoReturnsGeneratorValueWhenMatchingGeneratorFound()
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

            var actual = sut.GetBuildCapability(buildConfiguration, buildChain, parameterInfo);

            actual.SupportsCreate.Should().BeTrue();
        }

        [Fact]
        public void GetBuildCapabilityForParameterInfoReturnsNullWhenNoGeneratorsExist()
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

            var actual = sut.GetBuildCapability(buildConfiguration, buildChain, parameterInfo);

            actual.Should().BeNull();
        }

        [Fact]
        public void GetBuildCapabilityForParameterInfoReturnsNullWhenNoMatchingGeneratorFound()
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

            var actual = sut.GetBuildCapability(buildConfiguration, buildChain, parameterInfo);

            actual.Should().BeNull();
        }

        [Fact]
        public void GetBuildCapabilityForParameterInfoThrowsExceptionWithNullBuildChain()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildConfiguration = new BuildConfiguration();

            var sut = new ValueGeneratorBuildAction();

            Action action = () => sut.GetBuildCapability(buildConfiguration, null, parameterInfo);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildCapabilityForParameterInfoThrowsExceptionWithNullBuildConfiguration()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildChain = new BuildHistory();

            var sut = new ValueGeneratorBuildAction();

            Action action = () => sut.GetBuildCapability(null, buildChain, parameterInfo);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildCapabilityForParameterInfoThrowsExceptionWithNullParameterInfo()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var sut = new ValueGeneratorBuildAction();

            Action action = () => sut.GetBuildCapability(buildConfiguration, buildChain, (ParameterInfo) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildCapabilityForPropertyInfoReturnsCapabilityWhenMatchingGeneratorFound()
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

            var actual = sut.GetBuildCapability(buildConfiguration, buildChain, propertyInfo);

            actual.SupportsCreate.Should().BeTrue();
            actual.SupportsPopulate.Should().BeFalse();
            actual.AutoDetectConstructor.Should().BeFalse();
            actual.AutoPopulate.Should().BeFalse();
        }

        [Fact]
        public void GetBuildCapabilityForPropertyInfoReturnsNullWhenNoGeneratorsExist()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Log.Returns(_buildLog);

            var sut = new ValueGeneratorBuildAction();

            var actual = sut.GetBuildCapability(buildConfiguration, buildChain, propertyInfo);

            actual.Should().BeNull();
        }

        [Fact]
        public void GetBuildCapabilityForPropertyInfoReturnsNullWhenNoMatchingGeneratorFound()
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

            var actual = sut.GetBuildCapability(buildConfiguration, buildChain, propertyInfo);

            actual.Should().BeNull();
        }

        [Fact]
        public void GetBuildCapabilityForPropertyInfoThrowsExceptionWithNullBuildChain()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var buildConfiguration = new BuildConfiguration();

            var sut = new ValueGeneratorBuildAction();

            Action action = () => sut.GetBuildCapability(buildConfiguration, null, propertyInfo);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildCapabilityForPropertyInfoThrowsExceptionWithNullBuildConfiguration()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var buildChain = new BuildHistory();

            var sut = new ValueGeneratorBuildAction();

            Action action = () => sut.GetBuildCapability(null, buildChain, propertyInfo);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildCapabilityForPropertyInfoThrowsExceptionWithNullPropertyInfo()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var sut = new ValueGeneratorBuildAction();

            Action action = () => sut.GetBuildCapability(buildConfiguration, buildChain, (PropertyInfo) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildCapabilityForTypeReturnsCapabilityWhenMatchingGeneratorFound()
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

            var actual = sut.GetBuildCapability(buildConfiguration, buildChain, type);

            actual.SupportsCreate.Should().BeTrue();
            actual.SupportsPopulate.Should().BeFalse();
            actual.AutoDetectConstructor.Should().BeFalse();
            actual.AutoPopulate.Should().BeFalse();
        }

        [Fact]
        public void GetBuildCapabilityForTypeReturnsNullWhenNoGeneratorsExist()
        {
            var type = typeof(Person);
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new ValueGeneratorBuildAction();

            var actual = sut.GetBuildCapability(buildConfiguration, buildChain, type);

            actual.Should().BeNull();
        }

        [Fact]
        public void GetBuildCapabilityForTypeReturnsNullWhenNoMatchingGeneratorFound()
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

            var actual = sut.GetBuildCapability(buildConfiguration, buildChain, type);

            actual.Should().BeNull();
        }

        [Fact]
        public void GetBuildCapabilityForTypeThrowsExceptionWithNullBuildChain()
        {
            var type = typeof(Person);
            var buildConfiguration = new BuildConfiguration();

            var sut = new ValueGeneratorBuildAction();

            Action action = () => sut.GetBuildCapability(buildConfiguration, null, type);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildCapabilityForTypeThrowsExceptionWithNullBuildConfiguration()
        {
            var type = typeof(Person);
            var buildChain = new BuildHistory();

            var sut = new ValueGeneratorBuildAction();

            Action action = () => sut.GetBuildCapability(null, buildChain, type);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildCapabilityForTypeThrowsExceptionWithNullType()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var sut = new ValueGeneratorBuildAction();

            Action action = () => sut.GetBuildCapability(buildConfiguration, buildChain, (Type) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PopulateThrowsException()
        {
            var sut = new ValueGeneratorBuildAction();

            Action action = () => sut.Populate(null, null);

            action.Should().Throw<NotSupportedException>();
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