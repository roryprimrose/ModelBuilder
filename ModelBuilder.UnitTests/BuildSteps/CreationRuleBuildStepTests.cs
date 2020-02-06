﻿namespace ModelBuilder.UnitTests.BuildSteps
{
    using System;
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using ModelBuilder.BuildSteps;
    using ModelBuilder.CreationRules;
    using ModelBuilder.UnitTests.Models;
    using NSubstitute;
    using Xunit;

    public class CreationRuleBuildStepTests
    {
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

            var sut = new CreationRuleBuildStep();

            var actual = sut.Build(parameterInfo, executeStrategy);

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

            var sut = new CreationRuleBuildStep();

            var actual = sut.Build(parameterInfo, executeStrategy);

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
            rule.IsMatch(parameterInfo.ParameterType, parameterInfo.Name).Returns(true);
            rule.Create(parameterInfo.ParameterType, parameterInfo.Name, executeStrategy).Returns(expected);

            var sut = new CreationRuleBuildStep();

            var actual = sut.Build(parameterInfo, executeStrategy);

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
            firstRule.Priority.Returns(10);
            firstRule.IsMatch(parameterInfo.ParameterType, parameterInfo.Name).Returns(true);
            secondRule.Priority.Returns(20);
            secondRule.IsMatch(parameterInfo.ParameterType, parameterInfo.Name).Returns(true);
            secondRule.Create(parameterInfo.ParameterType, parameterInfo.Name, executeStrategy).Returns(expected);

            var sut = new CreationRuleBuildStep();

            var actual = sut.Build(parameterInfo, executeStrategy);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForParameterInfoThrowsExceptionWithNullExecuteStrategy()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            var sut = new CreationRuleBuildStep();

            Action action = () => sut.Build(parameterInfo, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForParameterInfoThrowsExceptionWithNullParameterInfo()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new CreationRuleBuildStep();

            Action action = () => sut.Build((ParameterInfo) null, executeStrategy);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForPropertyInfoReturnsNullWhenNoMatchingRuleFound()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var buildConfiguration = new BuildConfiguration();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var rule = Substitute.For<ICreationRule>();

            buildConfiguration.CreationRules.Add(rule);

            executeStrategy.Configuration.Returns(buildConfiguration);

            var sut = new CreationRuleBuildStep();

            var actual = sut.Build(propertyInfo, executeStrategy);

            actual.Should().BeNull();
        }

        [Fact]
        public void BuildForPropertyInfoReturnsNullWhenNoRulesExist()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var buildConfiguration = new BuildConfiguration();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.Configuration.Returns(buildConfiguration);

            var sut = new CreationRuleBuildStep();

            var actual = sut.Build(propertyInfo, executeStrategy);

            actual.Should().BeNull();
        }

        [Fact]
        public void BuildForPropertyInfoReturnsRuleValueWhenMatchingRuleFound()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var buildConfiguration = new BuildConfiguration();
            var expected = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var rule = Substitute.For<ICreationRule>();

            buildConfiguration.CreationRules.Add(rule);

            executeStrategy.Configuration.Returns(buildConfiguration);
            rule.IsMatch(propertyInfo.PropertyType, propertyInfo.Name).Returns(true);
            rule.Create(propertyInfo.PropertyType, propertyInfo.Name, executeStrategy).Returns(expected);

            var sut = new CreationRuleBuildStep();

            var actual = sut.Build(propertyInfo, executeStrategy);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForPropertyInfoReturnsValueFromRuleWithHighestPriority()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var buildConfiguration = new BuildConfiguration();
            var expected = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy>();
            var firstRule = Substitute.For<ICreationRule>();
            var secondRule = Substitute.For<ICreationRule>();

            buildConfiguration.CreationRules.Add(firstRule);
            buildConfiguration.CreationRules.Add(secondRule);

            executeStrategy.Configuration.Returns(buildConfiguration);
            firstRule.Priority.Returns(10);
            firstRule.IsMatch(propertyInfo.PropertyType, propertyInfo.Name).Returns(true);
            secondRule.Priority.Returns(20);
            secondRule.IsMatch(propertyInfo.PropertyType, propertyInfo.Name).Returns(true);
            secondRule.Create(propertyInfo.PropertyType, propertyInfo.Name, executeStrategy).Returns(expected);

            var sut = new CreationRuleBuildStep();

            var actual = sut.Build(propertyInfo, executeStrategy);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForPropertyInfoThrowsExceptionWithNullExecuteStrategy()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));

            var sut = new CreationRuleBuildStep();

            Action action = () => sut.Build(propertyInfo, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForPropertyInfoThrowsExceptionWithNullPropertyInfo()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new CreationRuleBuildStep();

            Action action = () => sut.Build((PropertyInfo) null, executeStrategy);

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

            var sut = new CreationRuleBuildStep();

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

            var sut = new CreationRuleBuildStep();

            var actual = sut.Build(type, executeStrategy);

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
            rule.IsMatch(type, null).Returns(true);
            rule.Create(type, null, executeStrategy).Returns(expected);

            var sut = new CreationRuleBuildStep();

            var actual = sut.Build(type, executeStrategy);

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
            firstRule.Priority.Returns(10);
            firstRule.IsMatch(type, null).Returns(true);
            secondRule.Priority.Returns(20);
            secondRule.IsMatch(type, null).Returns(true);
            secondRule.Create(type, null, executeStrategy).Returns(expected);

            var sut = new CreationRuleBuildStep();

            var actual = sut.Build(type, executeStrategy);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForTypeThrowsExceptionWithNullExecuteStrategy()
        {
            var type = typeof(Person);

            var sut = new CreationRuleBuildStep();

            Action action = () => sut.Build(type, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForTypeThrowsExceptionWithNullType()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new CreationRuleBuildStep();

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
            var rule = Substitute.For<ICreationRule>();

            buildConfiguration.CreationRules.Add(rule);

            executeStrategy.Configuration.Returns(buildConfiguration);

            var sut = new CreationRuleBuildStep();

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

            var sut = new CreationRuleBuildStep();

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
            var rule = Substitute.For<ICreationRule>();

            buildConfiguration.CreationRules.Add(rule);

            executeStrategy.Configuration.Returns(buildConfiguration);
            rule.IsMatch(parameterInfo.ParameterType, parameterInfo.Name).Returns(true);
            rule.Create(parameterInfo.ParameterType, parameterInfo.Name, executeStrategy).Returns(expected);

            var sut = new CreationRuleBuildStep();

            var actual = sut.IsMatch(parameterInfo, buildConfiguration, buildChain);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsMatchForParameterInfoThrowsExceptionWithNullBuildChain()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildConfiguration = new BuildConfiguration();

            var sut = new CreationRuleBuildStep();

            Action action = () => sut.IsMatch(parameterInfo, buildConfiguration, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForParameterInfoThrowsExceptionWithNullBuildConfiguration()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildChain = new BuildHistory();

            var sut = new CreationRuleBuildStep();

            Action action = () => sut.IsMatch(parameterInfo, null, buildChain);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForParameterInfoThrowsExceptionWithNullParameterInfo()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var sut = new CreationRuleBuildStep();

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
            var rule = Substitute.For<ICreationRule>();

            buildConfiguration.CreationRules.Add(rule);

            executeStrategy.Configuration.Returns(buildConfiguration);

            var sut = new CreationRuleBuildStep();

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

            var sut = new CreationRuleBuildStep();

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
            var rule = Substitute.For<ICreationRule>();

            buildConfiguration.CreationRules.Add(rule);

            executeStrategy.Configuration.Returns(buildConfiguration);
            rule.IsMatch(propertyInfo.PropertyType, propertyInfo.Name).Returns(true);
            rule.Create(propertyInfo.PropertyType, propertyInfo.Name, executeStrategy).Returns(expected);

            var sut = new CreationRuleBuildStep();

            var actual = sut.IsMatch(propertyInfo, buildConfiguration, buildChain);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsMatchForPropertyInfoThrowsExceptionWithNullBuildChain()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var buildConfiguration = new BuildConfiguration();

            var sut = new CreationRuleBuildStep();

            Action action = () => sut.IsMatch(propertyInfo, buildConfiguration, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForPropertyInfoThrowsExceptionWithNullBuildConfiguration()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var buildChain = new BuildHistory();

            var sut = new CreationRuleBuildStep();

            Action action = () => sut.IsMatch(propertyInfo, null, buildChain);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForPropertyInfoThrowsExceptionWithNullPropertyInfo()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var sut = new CreationRuleBuildStep();

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
            var rule = Substitute.For<ICreationRule>();

            buildConfiguration.CreationRules.Add(rule);

            executeStrategy.Configuration.Returns(buildConfiguration);

            var sut = new CreationRuleBuildStep();

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

            var sut = new CreationRuleBuildStep();

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
            var rule = Substitute.For<ICreationRule>();

            buildConfiguration.CreationRules.Add(rule);

            executeStrategy.Configuration.Returns(buildConfiguration);
            rule.IsMatch(type, null).Returns(true);
            rule.Create(type, null, executeStrategy).Returns(expected);

            var sut = new CreationRuleBuildStep();

            var actual = sut.IsMatch(type, buildConfiguration, buildChain);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsMatchForTypeThrowsExceptionWithNullBuildChain()
        {
            var type = typeof(Person);
            var buildConfiguration = new BuildConfiguration();

            var sut = new CreationRuleBuildStep();

            Action action = () => sut.IsMatch(type, buildConfiguration, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForTypeThrowsExceptionWithNullBuildConfiguration()
        {
            var type = typeof(Person);
            var buildChain = new BuildHistory();

            var sut = new CreationRuleBuildStep();

            Action action = () => sut.IsMatch(type, null, buildChain);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForTypeThrowsExceptionWithNullType()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var sut = new CreationRuleBuildStep();

            Action action = () => sut.IsMatch((Type) null, buildConfiguration, buildChain);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PriorityReturnsLowerThanCircularReferencePriority()
        {
            var circularReferenceStep = new CreationRuleBuildStep();

            var sut = new CreationRuleBuildStep();

            sut.Priority.Should().BeLessThan(circularReferenceStep.Priority);
        }
    }
}