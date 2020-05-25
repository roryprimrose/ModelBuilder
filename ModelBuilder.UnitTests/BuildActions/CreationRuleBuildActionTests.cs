namespace ModelBuilder.UnitTests.BuildActions
{
    using System;
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using ModelBuilder.BuildActions;
    using ModelBuilder.CreationRules;
    using ModelBuilder.UnitTests.Models;
    using NSubstitute;
    using NSubstitute.ExceptionExtensions;
    using Xunit;
    using Xunit.Abstractions;

    public class CreationRuleBuildActionTests
    {
        private readonly OutputBuildLog _buildLog;
        private readonly ITestOutputHelper _output;

        public CreationRuleBuildActionTests(ITestOutputHelper output)
        {
            _output = output;
            _buildLog = new OutputBuildLog(output);
        }

        [Fact]
        public void BuildForParameterInfoReturnsNullWhenNoMatchingRuleFound()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildConfiguration = new BuildConfiguration();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var rule = Substitute.For<ICreationRule>();

            buildConfiguration.CreationRules.Add(rule);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.Log.Returns(_buildLog);

            var sut = new CreationRuleBuildAction();

            var actual = sut.Build(executeStrategy, parameterInfo);

            actual.Should().BeNull();
        }

        [Fact]
        public void BuildForParameterInfoReturnsNullWhenNoRulesExist()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildConfiguration = new BuildConfiguration();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.Log.Returns(_buildLog);

            var sut = new CreationRuleBuildAction();

            var actual = sut.Build(executeStrategy, parameterInfo);

            actual.Should().BeNull();
        }

        [Fact]
        public void BuildForParameterInfoReturnsRuleValueWhenMatchingRuleFound()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildConfiguration = new BuildConfiguration();
            var expected = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var rule = Substitute.For<ICreationRule>();

            buildConfiguration.CreationRules.Add(rule);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.Log.Returns(_buildLog);
            rule.IsMatch(parameterInfo).Returns(true);
            rule.Create(executeStrategy, parameterInfo).Returns(expected);

            var sut = new CreationRuleBuildAction();

            var actual = sut.Build(executeStrategy, parameterInfo);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForParameterInfoReturnsValueFromRuleWithHighestPriority()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildConfiguration = new BuildConfiguration();
            var expected = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var firstRule = Substitute.For<ICreationRule>();
            var secondRule = Substitute.For<ICreationRule>();

            buildConfiguration.CreationRules.Add(firstRule);
            buildConfiguration.CreationRules.Add(secondRule);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.Log.Returns(_buildLog);
            firstRule.Priority.Returns(10);
            firstRule.IsMatch(parameterInfo).Returns(true);
            secondRule.Priority.Returns(20);
            secondRule.IsMatch(parameterInfo).Returns(true);
            secondRule.Create(executeStrategy, parameterInfo).Returns(expected);

            var sut = new CreationRuleBuildAction();

            var actual = sut.Build(executeStrategy, parameterInfo);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForParameterInfoThrowsExceptionWithNullExecuteStrategy()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            var sut = new CreationRuleBuildAction();

            Action action = () => sut.Build(null!, parameterInfo);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForParameterInfoThrowsExceptionWithNullParameterInfo()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new CreationRuleBuildAction();

            Action action = () => sut.Build(executeStrategy, (ParameterInfo) null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForPropertyInfoRethrowsBuildException()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName))!;
            var buildConfiguration = new BuildConfiguration();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var rule = Substitute.For<ICreationRule>();

            buildConfiguration.CreationRules.Add(rule);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.Log.Returns(_buildLog);
            rule.IsMatch(propertyInfo!).Returns(true);
            rule.Create(executeStrategy, propertyInfo!).Throws<BuildException>();

            var sut = new CreationRuleBuildAction();

            Action action = () => sut.Build(executeStrategy, propertyInfo!);

            var exception = action.Should().Throw<BuildException>().Which;

            _output.WriteLine(exception.Message);
        }

        [Fact]
        public void BuildForPropertyInfoReturnsNullWhenNoMatchingRuleFound()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName))!;
            var buildConfiguration = new BuildConfiguration();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var rule = Substitute.For<ICreationRule>();

            buildConfiguration.CreationRules.Add(rule);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.Log.Returns(_buildLog);

            var sut = new CreationRuleBuildAction();

            var actual = sut.Build(executeStrategy, propertyInfo!);

            actual.Should().BeNull();
        }

        [Fact]
        public void BuildForPropertyInfoReturnsNullWhenNoRulesExist()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName))!;
            var buildConfiguration = new BuildConfiguration();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.Log.Returns(_buildLog);

            var sut = new CreationRuleBuildAction();

            var actual = sut.Build(executeStrategy, propertyInfo!);

            actual.Should().BeNull();
        }

        [Fact]
        public void BuildForPropertyInfoReturnsRuleValueWhenMatchingRuleFound()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName))!;
            var buildConfiguration = new BuildConfiguration();
            var expected = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var rule = Substitute.For<ICreationRule>();

            buildConfiguration.CreationRules.Add(rule);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.Log.Returns(_buildLog);
            rule.IsMatch(propertyInfo!).Returns(true);
            rule.Create(executeStrategy, propertyInfo!).Returns(expected);

            var sut = new CreationRuleBuildAction();

            var actual = sut.Build(executeStrategy, propertyInfo!);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForPropertyInfoReturnsValueFromRuleWithHighestPriority()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName))!;
            var buildConfiguration = new BuildConfiguration();
            var expected = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var firstRule = Substitute.For<ICreationRule>();
            var secondRule = Substitute.For<ICreationRule>();

            buildConfiguration.CreationRules.Add(firstRule);
            buildConfiguration.CreationRules.Add(secondRule);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.Log.Returns(_buildLog);
            firstRule.Priority.Returns(10);
            firstRule.IsMatch(propertyInfo).Returns(true);
            secondRule.Priority.Returns(20);
            secondRule.IsMatch(propertyInfo).Returns(true);
            secondRule.Create(executeStrategy, propertyInfo).Returns(expected);

            var sut = new CreationRuleBuildAction();

            var actual = sut.Build(executeStrategy, propertyInfo);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForPropertyInfoThrowsExceptionWithNullExecuteStrategy()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName))!;

            var sut = new CreationRuleBuildAction();

            Action action = () => sut.Build(null!, propertyInfo);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForPropertyInfoThrowsExceptionWithNullPropertyInfo()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new CreationRuleBuildAction();

            Action action = () => sut.Build(executeStrategy, (PropertyInfo) null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForTypeReturnsNullWhenNoMatchingRuleFound()
        {
            var type = typeof(Person);
            var buildConfiguration = new BuildConfiguration();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var rule = Substitute.For<ICreationRule>();

            buildConfiguration.CreationRules.Add(rule);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.Log.Returns(_buildLog);

            var sut = new CreationRuleBuildAction();

            var actual = sut.Build(executeStrategy, type);

            actual.Should().BeNull();
        }

        [Fact]
        public void BuildForTypeReturnsNullWhenNoRulesExist()
        {
            var type = typeof(Person);
            var buildConfiguration = new BuildConfiguration();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.Log.Returns(_buildLog);

            var sut = new CreationRuleBuildAction();

            var actual = sut.Build(executeStrategy, type);

            actual.Should().BeNull();
        }

        [Fact]
        public void BuildForTypeReturnsRuleValueWhenMatchingRuleFound()
        {
            var type = typeof(Person);
            var buildConfiguration = new BuildConfiguration();
            var expected = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var rule = Substitute.For<ICreationRule>();

            buildConfiguration.CreationRules.Add(rule);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.Log.Returns(_buildLog);
            rule.IsMatch(type).Returns(true);
            rule.Create(executeStrategy, type).Returns(expected);

            var sut = new CreationRuleBuildAction();

            var actual = sut.Build(executeStrategy, type);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForTypeReturnsValueFromRuleWithHighestPriority()
        {
            var type = typeof(Person);
            var buildConfiguration = new BuildConfiguration();
            var expected = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var firstRule = Substitute.For<ICreationRule>();
            var secondRule = Substitute.For<ICreationRule>();

            buildConfiguration.CreationRules.Add(firstRule);
            buildConfiguration.CreationRules.Add(secondRule);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.Log.Returns(_buildLog);
            firstRule.Priority.Returns(10);
            firstRule.IsMatch(type).Returns(true);
            secondRule.Priority.Returns(20);
            secondRule.IsMatch(type).Returns(true);
            secondRule.Create(executeStrategy, type).Returns(expected);

            var sut = new CreationRuleBuildAction();

            var actual = sut.Build(executeStrategy, type);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForTypeThrowsExceptionWithNullExecuteStrategy()
        {
            var type = typeof(Person);

            var sut = new CreationRuleBuildAction();

            Action action = () => sut.Build(null!, type);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForTypeThrowsExceptionWithNullType()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new CreationRuleBuildAction();

            Action action = () => sut.Build(executeStrategy, (Type) null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildLogsValueCreation()
        {
            var type = typeof(Person);
            var buildConfiguration = new BuildConfiguration();
            var expected = new Person();

            var buildLog = Substitute.For<IBuildLog>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var rule = Substitute.For<ICreationRule>();

            buildConfiguration.CreationRules.Add(rule);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.Log.Returns(buildLog);
            rule.IsMatch(type).Returns(true);
            rule.Create(executeStrategy, type).Returns(expected);

            var sut = new CreationRuleBuildAction();

            sut.Build(executeStrategy, type);

            buildLog.Received().CreatingValue(type, rule.GetType(), null!);
        }

        [Fact]
        public void BuildRethrowsBuildException()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildConfiguration = new BuildConfiguration();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var rule = Substitute.For<ICreationRule>();

            buildConfiguration.CreationRules.Add(rule);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.Log.Returns(_buildLog);
            rule.IsMatch(parameterInfo).Returns(true);
            rule.Create(executeStrategy, parameterInfo).Throws<BuildException>();

            var sut = new CreationRuleBuildAction();

            Action action = () => sut.Build(executeStrategy, parameterInfo);

            var exception = action.Should().Throw<BuildException>().Which;

            _output.WriteLine(exception.Message);
        }

        [Fact]
        public void BuildThrowsExceptionWhenRuleFails()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName))!;
            var buildConfiguration = new BuildConfiguration();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var rule = Substitute.For<ICreationRule>();

            buildConfiguration.CreationRules.Add(rule);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.Log.Returns(_buildLog);
            rule.IsMatch(propertyInfo).Returns(true);
            rule.Create(executeStrategy, propertyInfo).Throws<TimeoutException>();

            var sut = new CreationRuleBuildAction();

            Action action = () => sut.Build(executeStrategy, propertyInfo);

            var exception = action.Should().Throw<BuildException>().Which;

            exception.InnerException.Should().BeOfType<TimeoutException>();

            _output.WriteLine(exception.Message);
        }

        [Fact]
        public void GetBuildCapabilityForParameterInfoReturnsNullWhenNoMatchingRuleFound()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var rule = Substitute.For<ICreationRule>();

            buildConfiguration.CreationRules.Add(rule);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.Log.Returns(_buildLog);

            var sut = new CreationRuleBuildAction();

            var actual = sut.GetBuildCapability(buildConfiguration, buildChain, parameterInfo);

            actual.Should().BeNull();
        }

        [Fact]
        public void GetBuildCapabilityForParameterInfoReturnsNullWhenNoRulesExist()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.Log.Returns(_buildLog);

            var sut = new CreationRuleBuildAction();

            var actual = sut.GetBuildCapability(buildConfiguration, buildChain, parameterInfo);

            actual.Should().BeNull();
        }

        [Fact]
        public void GetBuildCapabilityForParameterInfoReturnsCapabilityWhenMatchingRuleFound()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var expected = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var rule = Substitute.For<ICreationRule>();

            buildConfiguration.CreationRules.Add(rule);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.Log.Returns(_buildLog);
            rule.IsMatch(parameterInfo).Returns(true);
            rule.Create(executeStrategy, parameterInfo).Returns(expected);

            var sut = new CreationRuleBuildAction();

            var actual = sut.GetBuildCapability(buildConfiguration, buildChain, parameterInfo)!;

            actual.Should().NotBeNull();
            actual.SupportsCreate.Should().BeTrue();
            actual.SupportsPopulate.Should().BeFalse();
            actual.AutoDetectConstructor.Should().BeFalse();
            actual.AutoPopulate.Should().BeFalse();
            actual.ImplementedByType.Should().Be(rule.GetType());
        }

        [Fact]
        public void GetBuildCapabilityForParameterInfoThrowsExceptionWithNullBuildChain()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildConfiguration = new BuildConfiguration();

            var sut = new CreationRuleBuildAction();

            Action action = () => sut.GetBuildCapability(buildConfiguration, null!, parameterInfo);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildCapabilityForParameterInfoThrowsExceptionWithNullBuildConfiguration()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildChain = new BuildHistory();

            var sut = new CreationRuleBuildAction();

            Action action = () => sut.GetBuildCapability(null!, buildChain, parameterInfo);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildCapabilityForParameterInfoThrowsExceptionWithNullParameterInfo()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var sut = new CreationRuleBuildAction();

            Action action = () => sut.GetBuildCapability(buildConfiguration, buildChain, (ParameterInfo) null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildCapabilityForPropertyInfoReturnsNullWhenNoMatchingRuleFound()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName))!;
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var rule = Substitute.For<ICreationRule>();

            buildConfiguration.CreationRules.Add(rule);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.Log.Returns(_buildLog);

            var sut = new CreationRuleBuildAction();

            var actual = sut.GetBuildCapability(buildConfiguration, buildChain, propertyInfo);

            actual.Should().BeNull();
        }

        [Fact]
        public void GetBuildCapabilityForPropertyInfoReturnsNullWhenNoRulesExist()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName))!;
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.Log.Returns(_buildLog);

            var sut = new CreationRuleBuildAction();

            var actual = sut.GetBuildCapability(buildConfiguration, buildChain, propertyInfo);

            actual.Should().BeNull();
        }

        [Fact]
        public void GetBuildCapabilityForPropertyInfoReturnsCapabilityWhenMatchingRuleFound()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName))!;
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var expected = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var rule = Substitute.For<ICreationRule>();

            buildConfiguration.CreationRules.Add(rule);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.Log.Returns(_buildLog);
            rule.IsMatch(propertyInfo).Returns(true);
            rule.Create(executeStrategy, propertyInfo).Returns(expected);

            var sut = new CreationRuleBuildAction();

            var actual = sut.GetBuildCapability(buildConfiguration, buildChain, propertyInfo);

            actual.Should().NotBeNull();
            actual!.SupportsCreate.Should().BeTrue();
            actual.SupportsPopulate.Should().BeFalse();
            actual.AutoDetectConstructor.Should().BeFalse();
            actual.AutoPopulate.Should().BeFalse();
            actual.ImplementedByType.Should().Be(rule.GetType());
        }

        [Fact]
        public void GetBuildCapabilityForPropertyInfoThrowsExceptionWithNullBuildChain()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName))!;
            var buildConfiguration = new BuildConfiguration();

            var sut = new CreationRuleBuildAction();

            Action action = () => sut.GetBuildCapability(buildConfiguration, null!, propertyInfo);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildCapabilityForPropertyInfoThrowsExceptionWithNullBuildConfiguration()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName))!;
            var buildChain = new BuildHistory();

            var sut = new CreationRuleBuildAction();

            Action action = () => sut.GetBuildCapability(null!, buildChain, propertyInfo);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildCapabilityForPropertyInfoThrowsExceptionWithNullPropertyInfo()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var sut = new CreationRuleBuildAction();

            Action action = () => sut.GetBuildCapability(buildConfiguration, buildChain, (PropertyInfo) null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildCapabilityForTypeReturnsNullWhenNoMatchingRuleFound()
        {
            var type = typeof(Person);
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var rule = Substitute.For<ICreationRule>();

            buildConfiguration.CreationRules.Add(rule);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.Log.Returns(_buildLog);

            var sut = new CreationRuleBuildAction();

            var actual = sut.GetBuildCapability(buildConfiguration, buildChain, type);

            actual.Should().BeNull();
        }

        [Fact]
        public void GetBuildCapabilityForTypeReturnsNullWhenNoRulesExist()
        {
            var type = typeof(Person);
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.Log.Returns(_buildLog);

            var sut = new CreationRuleBuildAction();

            var actual = sut.GetBuildCapability(buildConfiguration, buildChain, type);

            actual.Should().BeNull();
        }

        [Fact]
        public void GetBuildCapabilityForTypeReturnsCapabilityWhenMatchingRuleFound()
        {
            var type = typeof(Person);
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var expected = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var rule = Substitute.For<ICreationRule>();

            buildConfiguration.CreationRules.Add(rule);

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.Log.Returns(_buildLog);
            rule.IsMatch(type).Returns(true);
            rule.Create(executeStrategy, type).Returns(expected);

            var sut = new CreationRuleBuildAction();

            var actual = sut.GetBuildCapability(buildConfiguration, buildChain, type);

            actual.Should().NotBeNull();
            actual!.SupportsCreate.Should().BeTrue();
            actual.SupportsPopulate.Should().BeFalse();
            actual.AutoDetectConstructor.Should().BeFalse();
            actual.AutoPopulate.Should().BeFalse();
            actual.ImplementedByType.Should().Be(rule.GetType());
        }

        [Fact]
        public void GetBuildCapabilityForTypeThrowsExceptionWithNullBuildChain()
        {
            var type = typeof(Person);
            var buildConfiguration = new BuildConfiguration();

            var sut = new CreationRuleBuildAction();

            Action action = () => sut.GetBuildCapability(buildConfiguration, null!, type);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildCapabilityForTypeThrowsExceptionWithNullBuildConfiguration()
        {
            var type = typeof(Person);
            var buildChain = new BuildHistory();

            var sut = new CreationRuleBuildAction();

            Action action = () => sut.GetBuildCapability(null!, buildChain, type);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildCapabilityForTypeThrowsExceptionWithNullType()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var sut = new CreationRuleBuildAction();

            Action action = () => sut.GetBuildCapability(buildConfiguration, buildChain, (Type) null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PopulateThrowsException()
        {
            var sut = new CreationRuleBuildAction();

            Action action = () => sut.Populate(null!, null!);

            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void PriorityReturnsLowerThanCircularReferencePriority()
        {
            var circularReferenceStep = new CircularReferenceBuildAction();

            var sut = new CreationRuleBuildAction();

            sut.Priority.Should().BeLessThan(circularReferenceStep.Priority);
        }
    }
}