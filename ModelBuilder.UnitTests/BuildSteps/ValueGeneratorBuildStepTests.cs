namespace ModelBuilder.UnitTests.BuildSteps
{
    using System;
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using ModelBuilder.BuildSteps;
    using ModelBuilder.UnitTests.Models;
    using ModelBuilder.ValueGenerators;
    using NSubstitute;
    using Xunit;

    public class ValueGeneratorBuildStepTests
    {
        [Fact]
        public void BuildForParameterInfoReturnsNullWhenNoMatchingRuleFound()
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

            var sut = new ValueGeneratorBuildStep();

            var actual = sut.Build(parameterInfo, executeStrategy);

            actual.Should().BeNull();
        }

        [Fact]
        public void BuildForParameterInfoReturnsNullWhenNoRulesExist()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new ValueGeneratorBuildStep();

            var actual = sut.Build(parameterInfo, executeStrategy);

            actual.Should().BeNull();
        }

        [Fact]
        public void BuildForParameterInfoReturnsRuleValueWhenMatchingRuleFound()
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

            executeStrategy.Configuration.Returns(buildConfiguration);
            generator.IsSupported(parameterInfo.ParameterType, parameterInfo.Name, buildChain).Returns(true);
            generator.Generate(parameterInfo.ParameterType, parameterInfo.Name, executeStrategy).Returns(expected);

            var sut = new ValueGeneratorBuildStep();

            var actual = sut.Build(parameterInfo, executeStrategy);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForParameterInfoReturnsValueFromRuleWithHighestPriority()
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
            firstGenerator.Priority.Returns(10);
            firstGenerator.IsSupported(parameterInfo.ParameterType, parameterInfo.Name, buildChain).Returns(true);
            secondGenerator.Priority.Returns(20);
            secondGenerator.IsSupported(parameterInfo.ParameterType, parameterInfo.Name, buildChain).Returns(true);
            secondGenerator.Generate(parameterInfo.ParameterType, parameterInfo.Name, executeStrategy)
                .Returns(expected);

            var sut = new ValueGeneratorBuildStep();

            var actual = sut.Build(parameterInfo, executeStrategy);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForParameterInfoThrowsExceptionWithNullExecuteStrategy()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            var sut = new ValueGeneratorBuildStep();

            Action action = () => sut.Build(parameterInfo, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForParameterInfoThrowsExceptionWithNullParameterInfo()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new ValueGeneratorBuildStep();

            Action action = () => sut.Build((ParameterInfo) null, executeStrategy);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForPropertyInfoReturnsNullWhenNoMatchingRuleFound()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var generator = Substitute.For<IValueGenerator>();

            buildConfiguration.ValueGenerators.Add(generator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new ValueGeneratorBuildStep();

            var actual = sut.Build(propertyInfo, executeStrategy);

            actual.Should().BeNull();
        }

        [Fact]
        public void BuildForPropertyInfoReturnsNullWhenNoRulesExist()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new ValueGeneratorBuildStep();

            var actual = sut.Build(propertyInfo, executeStrategy);

            actual.Should().BeNull();
        }

        [Fact]
        public void BuildForPropertyInfoReturnsRuleValueWhenMatchingRuleFound()
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
            generator.IsSupported(propertyInfo.PropertyType, propertyInfo.Name, buildChain).Returns(true);
            generator.Generate(propertyInfo.PropertyType, propertyInfo.Name, executeStrategy).Returns(expected);

            var sut = new ValueGeneratorBuildStep();

            var actual = sut.Build(propertyInfo, executeStrategy);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForPropertyInfoReturnsValueFromRuleWithHighestPriority()
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
            firstGenerator.Priority.Returns(10);
            firstGenerator.IsSupported(propertyInfo.PropertyType, propertyInfo.Name, buildChain).Returns(true);
            secondGenerator.Priority.Returns(20);
            secondGenerator.IsSupported(propertyInfo.PropertyType, propertyInfo.Name, buildChain).Returns(true);
            secondGenerator.Generate(propertyInfo.PropertyType, propertyInfo.Name, executeStrategy).Returns(expected);

            var sut = new ValueGeneratorBuildStep();

            var actual = sut.Build(propertyInfo, executeStrategy);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForPropertyInfoThrowsExceptionWithNullExecuteStrategy()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));

            var sut = new ValueGeneratorBuildStep();

            Action action = () => sut.Build(propertyInfo, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForPropertyInfoThrowsExceptionWithNullPropertyInfo()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new ValueGeneratorBuildStep();

            Action action = () => sut.Build((PropertyInfo) null, executeStrategy);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForTypeReturnsNullWhenNoMatchingRuleFound()
        {
            var type = typeof(Person);
            var buildConfiguration = new BuildConfiguration();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var generator = Substitute.For<IValueGenerator>();

            buildConfiguration.ValueGenerators.Add(generator);

            executeStrategy.Configuration.Returns(buildConfiguration);

            var sut = new ValueGeneratorBuildStep();

            var actual = sut.Build(type, executeStrategy);

            actual.Should().BeNull();
        }

        [Fact]
        public void BuildForTypeReturnsNullWhenNoRulesExist()
        {
            var type = typeof(Person);
            var buildConfiguration = new BuildConfiguration();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.Configuration.Returns(buildConfiguration);

            var sut = new ValueGeneratorBuildStep();

            var actual = sut.Build(type, executeStrategy);

            actual.Should().BeNull();
        }

        [Fact]
        public void BuildForTypeReturnsRuleValueWhenMatchingRuleFound()
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
            generator.IsSupported(type, null, buildChain).Returns(true);
            generator.Generate(type, null, executeStrategy).Returns(expected);

            var sut = new ValueGeneratorBuildStep();

            var actual = sut.Build(type, executeStrategy);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForTypeReturnsValueFromRuleWithHighestPriority()
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
            firstGenerator.Priority.Returns(10);
            firstGenerator.IsSupported(type, null, buildChain).Returns(true);
            secondGenerator.Priority.Returns(20);
            secondGenerator.IsSupported(type, null, buildChain).Returns(true);
            secondGenerator.Generate(type, null, executeStrategy).Returns(expected);

            var sut = new ValueGeneratorBuildStep();

            var actual = sut.Build(type, executeStrategy);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForTypeThrowsExceptionWithNullExecuteStrategy()
        {
            var type = typeof(Person);

            var sut = new ValueGeneratorBuildStep();

            Action action = () => sut.Build(type, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForTypeThrowsExceptionWithNullType()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new ValueGeneratorBuildStep();

            Action action = () => sut.Build((Type) null, executeStrategy);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForParameterInfoReturnsNullWhenNoMatchingRuleFound()
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

            var sut = new ValueGeneratorBuildStep();

            var actual = sut.IsMatch(parameterInfo, buildConfiguration, buildChain);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchForParameterInfoReturnsNullWhenNoRulesExist()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new ValueGeneratorBuildStep();

            var actual = sut.IsMatch(parameterInfo, buildConfiguration, buildChain);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchForParameterInfoReturnsRuleValueWhenMatchingRuleFound()
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
            generator.IsSupported(parameterInfo.ParameterType, parameterInfo.Name, buildChain).Returns(true);
            generator.Generate(parameterInfo.ParameterType, parameterInfo.Name, executeStrategy).Returns(expected);

            var sut = new ValueGeneratorBuildStep();

            var actual = sut.IsMatch(parameterInfo, buildConfiguration, buildChain);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsMatchForParameterInfoThrowsExceptionWithNullBuildChain()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildConfiguration = new BuildConfiguration();

            var sut = new ValueGeneratorBuildStep();

            Action action = () => sut.IsMatch(parameterInfo, buildConfiguration, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForParameterInfoThrowsExceptionWithNullBuildConfiguration()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildChain = new BuildHistory();

            var sut = new ValueGeneratorBuildStep();

            Action action = () => sut.IsMatch(parameterInfo, null, buildChain);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForParameterInfoThrowsExceptionWithNullParameterInfo()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var sut = new ValueGeneratorBuildStep();

            Action action = () => sut.IsMatch((ParameterInfo) null, buildConfiguration, buildChain);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForPropertyInfoReturnsNullWhenNoMatchingRuleFound()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var generator = Substitute.For<IValueGenerator>();

            buildConfiguration.ValueGenerators.Add(generator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new ValueGeneratorBuildStep();

            var actual = sut.IsMatch(propertyInfo, buildConfiguration, buildChain);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchForPropertyInfoReturnsNullWhenNoRulesExist()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new ValueGeneratorBuildStep();

            var actual = sut.IsMatch(propertyInfo, buildConfiguration, buildChain);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchForPropertyInfoReturnsRuleValueWhenMatchingRuleFound()
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
            generator.IsSupported(propertyInfo.PropertyType, propertyInfo.Name, buildChain).Returns(true);
            generator.Generate(propertyInfo.PropertyType, propertyInfo.Name, executeStrategy).Returns(expected);

            var sut = new ValueGeneratorBuildStep();

            var actual = sut.IsMatch(propertyInfo, buildConfiguration, buildChain);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsMatchForPropertyInfoThrowsExceptionWithNullBuildChain()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var buildConfiguration = new BuildConfiguration();

            var sut = new ValueGeneratorBuildStep();

            Action action = () => sut.IsMatch(propertyInfo, buildConfiguration, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForPropertyInfoThrowsExceptionWithNullBuildConfiguration()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var buildChain = new BuildHistory();

            var sut = new ValueGeneratorBuildStep();

            Action action = () => sut.IsMatch(propertyInfo, null, buildChain);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForPropertyInfoThrowsExceptionWithNullPropertyInfo()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var sut = new ValueGeneratorBuildStep();

            Action action = () => sut.IsMatch((PropertyInfo) null, buildConfiguration, buildChain);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForTypeReturnsNullWhenNoMatchingRuleFound()
        {
            var type = typeof(Person);
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var generator = Substitute.For<IValueGenerator>();

            buildConfiguration.ValueGenerators.Add(generator);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new ValueGeneratorBuildStep();

            var actual = sut.IsMatch(type, buildConfiguration, buildChain);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchForTypeReturnsNullWhenNoRulesExist()
        {
            var type = typeof(Person);
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new ValueGeneratorBuildStep();

            var actual = sut.IsMatch(type, buildConfiguration, buildChain);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchForTypeReturnsRuleValueWhenMatchingRuleFound()
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
            generator.IsSupported(type, null, buildChain).Returns(true);
            generator.Generate(type, null, executeStrategy).Returns(expected);

            var sut = new ValueGeneratorBuildStep();

            var actual = sut.IsMatch(type, buildConfiguration, buildChain);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsMatchForTypeThrowsExceptionWithNullBuildChain()
        {
            var type = typeof(Person);
            var buildConfiguration = new BuildConfiguration();

            var sut = new ValueGeneratorBuildStep();

            Action action = () => sut.IsMatch(type, buildConfiguration, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForTypeThrowsExceptionWithNullBuildConfiguration()
        {
            var type = typeof(Person);
            var buildChain = new BuildHistory();

            var sut = new ValueGeneratorBuildStep();

            Action action = () => sut.IsMatch(type, null, buildChain);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForTypeThrowsExceptionWithNullType()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var sut = new ValueGeneratorBuildStep();

            Action action = () => sut.IsMatch((Type) null, buildConfiguration, buildChain);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PriorityReturnsLowerThanCreationRulePriority()
        {
            var otherStep = new CreationRuleBuildStep();

            var sut = new ValueGeneratorBuildStep();

            sut.Priority.Should().BeLessThan(otherStep.Priority);
        }
    }
}