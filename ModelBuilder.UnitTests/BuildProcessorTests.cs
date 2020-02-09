namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using ModelBuilder.BuildActions;
    using ModelBuilder.UnitTests.Models;
    using NSubstitute;
    using Xunit;

    public class BuildProcessorTests
    {
        [Fact]
        public void BuildForParameterReturnsActionValue()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var expected = new Person();
            var match = new MatchResult {IsMatch = true};

            var action = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(buildConfiguration);
            action.IsMatch(buildConfiguration, buildChain, parameterInfo).Returns(match);
            action.Build(executeStrategy, parameterInfo).Returns(expected);

            var actions = new List<IBuildAction>
            {
                action
            };

            var sut = new BuildProcessor(actions);

            var actual = sut.Build(executeStrategy, parameterInfo);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForParameterReturnsValueFromActionWithHighestPriority()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var expected = new Person();
            var match = new MatchResult {IsMatch = true};

            var firstAction = Substitute.For<IBuildAction>();
            var secondAction = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(buildConfiguration);
            firstAction.Priority.Returns(int.MinValue);
            firstAction.IsMatch(buildConfiguration, buildChain, parameterInfo).Returns(match);
            secondAction.Priority.Returns(int.MaxValue);
            secondAction.IsMatch(buildConfiguration, buildChain, parameterInfo).Returns(match);
            secondAction.Build(executeStrategy, parameterInfo).Returns(expected);

            var actions = new List<IBuildAction>
            {
                firstAction,
                secondAction
            };

            var sut = new BuildProcessor(actions);

            var actual = sut.Build(executeStrategy, parameterInfo);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForParameterReturnsValueFromMatchingAction()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var expected = new Person();
            var match = new MatchResult {IsMatch = true};

            var firstAction = Substitute.For<IBuildAction>();
            var secondAction = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(buildConfiguration);
            firstAction.Priority.Returns(int.MaxValue);
            firstAction.IsMatch(buildConfiguration, buildChain, parameterInfo).Returns(MatchResult.NoMatch);
            secondAction.Priority.Returns(int.MinValue);
            secondAction.IsMatch(buildConfiguration, buildChain, parameterInfo).Returns(match);
            secondAction.Build(executeStrategy, parameterInfo).Returns(expected);

            var actions = new List<IBuildAction>
            {
                firstAction,
                secondAction
            };

            var sut = new BuildProcessor(actions);

            var actual = sut.Build(executeStrategy, parameterInfo);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForParameterThrowsExceptionWhenNoBuildActionFound()
        {
            var actions = Array.Empty<IBuildAction>();
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new BuildProcessor(actions);

            Action action = () => sut.Build(executeStrategy, parameterInfo);

            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void BuildForParameterThrowsExceptionWithNullExecuteStrategy()
        {
            var actions = Array.Empty<IBuildAction>();
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            var sut = new BuildProcessor(actions);

            Action action = () => sut.Build(null, parameterInfo);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForParameterThrowsExceptionWithNullParameter()
        {
            var actions = Array.Empty<IBuildAction>();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new BuildProcessor(actions);

            Action action = () => sut.Build(executeStrategy, (ParameterInfo) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForPropertyReturnsActionValue()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var expected = new Person();
            var match = new MatchResult {IsMatch = true};

            var action = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(buildConfiguration);
            action.IsMatch(buildConfiguration, buildChain, propertyInfo).Returns(match);
            action.Build(executeStrategy, propertyInfo).Returns(expected);

            var actions = new List<IBuildAction>
            {
                action
            };

            var sut = new BuildProcessor(actions);

            var actual = sut.Build(executeStrategy, propertyInfo);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForPropertyReturnsValueFromActionWithHighestPriority()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var expected = new Person();
            var match = new MatchResult {IsMatch = true};

            var firstAction = Substitute.For<IBuildAction>();
            var secondAction = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(buildConfiguration);
            firstAction.Priority.Returns(int.MinValue);
            firstAction.IsMatch(buildConfiguration, buildChain, propertyInfo).Returns(match);
            secondAction.Priority.Returns(int.MaxValue);
            secondAction.IsMatch(buildConfiguration, buildChain, propertyInfo).Returns(match);
            secondAction.Build(executeStrategy, propertyInfo).Returns(expected);

            var actions = new List<IBuildAction>
            {
                firstAction,
                secondAction
            };

            var sut = new BuildProcessor(actions);

            var actual = sut.Build(executeStrategy, propertyInfo);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForPropertyReturnsValueFromMatchingAction()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var expected = new Person();
            var match = new MatchResult {IsMatch = true};

            var firstAction = Substitute.For<IBuildAction>();
            var secondAction = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(buildConfiguration);
            firstAction.Priority.Returns(int.MaxValue);
            firstAction.IsMatch(buildConfiguration, buildChain, propertyInfo).Returns(MatchResult.NoMatch);
            secondAction.Priority.Returns(int.MinValue);
            secondAction.IsMatch(buildConfiguration, buildChain, propertyInfo).Returns(match);
            secondAction.Build(executeStrategy, propertyInfo).Returns(expected);

            var actions = new List<IBuildAction>
            {
                firstAction,
                secondAction
            };

            var sut = new BuildProcessor(actions);

            var actual = sut.Build(executeStrategy, propertyInfo);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForPropertyThrowsExceptionWhenNoBuildActionFound()
        {
            var context = new Person();
            var actions = Array.Empty<IBuildAction>();
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var buildChain = new BuildHistory();

            buildChain.Push(context);

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new BuildProcessor(actions);

            Action action = () => sut.Build(executeStrategy, propertyInfo);

            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void BuildForPropertyThrowsExceptionWithNullExecuteStrategy()
        {
            var actions = Array.Empty<IBuildAction>();
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));

            var sut = new BuildProcessor(actions);

            Action action = () => sut.Build(null, propertyInfo);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForPropertyThrowsExceptionWithNullProperty()
        {
            var actions = Array.Empty<IBuildAction>();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new BuildProcessor(actions);

            Action action = () => sut.Build(executeStrategy, (PropertyInfo) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForTypeReturnsActionValue()
        {
            var type = typeof(Person);
            var expected = new Person();
            var match = new MatchResult {IsMatch = true};

            var action = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(buildConfiguration);
            action.IsMatch(buildConfiguration, buildChain, type).Returns(match);
            action.Build(executeStrategy, type).Returns(expected);

            var actions = new List<IBuildAction>
            {
                action
            };

            var sut = new BuildProcessor(actions);

            var actual = sut.Build(executeStrategy, type);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForTypeReturnsValueFromActionWithHighestPriority()
        {
            var type = typeof(Person);
            var expected = new Person();
            var match = new MatchResult {IsMatch = true};

            var firstAction = Substitute.For<IBuildAction>();
            var secondAction = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(buildConfiguration);
            firstAction.Priority.Returns(int.MinValue);
            firstAction.IsMatch(buildConfiguration, buildChain, type).Returns(match);
            secondAction.Priority.Returns(int.MaxValue);
            secondAction.IsMatch(buildConfiguration, buildChain, type).Returns(match);
            secondAction.Build(executeStrategy, type).Returns(expected);

            var actions = new List<IBuildAction>
            {
                firstAction,
                secondAction
            };

            var sut = new BuildProcessor(actions);

            var actual = sut.Build(executeStrategy, type);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForTypeReturnsValueFromMatchingAction()
        {
            var type = typeof(Person);
            var expected = new Person();
            var match = new MatchResult {IsMatch = true};

            var firstAction = Substitute.For<IBuildAction>();
            var secondAction = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(buildConfiguration);
            firstAction.Priority.Returns(int.MaxValue);
            firstAction.IsMatch(buildConfiguration, buildChain, type).Returns(MatchResult.NoMatch);
            secondAction.Priority.Returns(int.MinValue);
            secondAction.IsMatch(buildConfiguration, buildChain, type).Returns(match);
            secondAction.Build(executeStrategy, type).Returns(expected);

            var actions = new List<IBuildAction>
            {
                firstAction,
                secondAction
            };

            var sut = new BuildProcessor(actions);

            var actual = sut.Build(executeStrategy, type);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForTypeThrowsExceptionWhenNoBuildActionFound()
        {
            var actions = Array.Empty<IBuildAction>();
            var type = typeof(Person);

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new BuildProcessor(actions);

            Action action = () => sut.Build(executeStrategy, type);

            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void BuildForTypeThrowsExceptionWithNullExecuteStrategy()
        {
            var actions = Array.Empty<IBuildAction>();
            var type = typeof(Person);

            var sut = new BuildProcessor(actions);

            Action action = () => sut.Build(null, type);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForTypeThrowsExceptionWithNullType()
        {
            var actions = Array.Empty<IBuildAction>();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new BuildProcessor(actions);

            Action action = () => sut.Build(executeStrategy, (Type) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CanCreateInstance()
        {
            Action action = () => new BuildProcessor();

            action.Should().NotThrow();
        }

        [Fact]
        public void GetBuildPlanForParameterReturnsBuildPlan()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var match = new MatchResult {IsMatch = true};

            var action = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();

            action.IsMatch(buildConfiguration, buildChain, parameterInfo).Returns(match);

            var actions = new List<IBuildAction>
            {
                action
            };

            var sut = new BuildProcessor(actions);

            var actual = sut.GetBuildPlan(buildConfiguration, buildChain, parameterInfo);

            actual.Should().Be(match);
        }

        [Fact]
        public void GetBuildPlanForParameterReturnsPlanFromActionWithHighestPriority()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var firstMatch = new MatchResult {IsMatch = true};
            var secondMatch = new MatchResult
                {IsMatch = true, SupportsPopulate = true, AutoPopulate = true, RequiresActivator = true};

            var firstAction = Substitute.For<IBuildAction>();
            var secondAction = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(buildConfiguration);
            firstAction.Priority.Returns(int.MinValue);
            firstAction.IsMatch(buildConfiguration, buildChain, parameterInfo).Returns(firstMatch);
            secondAction.Priority.Returns(int.MaxValue);
            secondAction.IsMatch(buildConfiguration, buildChain, parameterInfo).Returns(secondMatch);

            var actions = new List<IBuildAction>
            {
                firstAction,
                secondAction
            };

            var sut = new BuildProcessor(actions);

            var actual = sut.GetBuildPlan(buildConfiguration, buildChain, parameterInfo);

            actual.Should().BeEquivalentTo(secondMatch);
        }

        [Fact]
        public void GetBuildPlanForParameterReturnsValueFromMatchingAction()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var firstMatch = new MatchResult {IsMatch = false};
            var secondMatch = new MatchResult
                {IsMatch = true, SupportsPopulate = true, AutoPopulate = true, RequiresActivator = true};

            var firstAction = Substitute.For<IBuildAction>();
            var secondAction = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(buildConfiguration);
            firstAction.Priority.Returns(int.MaxValue);
            firstAction.IsMatch(buildConfiguration, buildChain, parameterInfo).Returns(firstMatch);
            secondAction.Priority.Returns(int.MinValue);
            secondAction.IsMatch(buildConfiguration, buildChain, parameterInfo).Returns(secondMatch);

            var actions = new List<IBuildAction>
            {
                firstAction,
                secondAction
            };

            var sut = new BuildProcessor(actions);

            var actual = sut.GetBuildPlan(buildConfiguration, buildChain, parameterInfo);

            actual.Should().BeEquivalentTo(secondMatch);
        }

        [Fact]
        public void GetBuildPlanForParameterThrowsExceptionWithNullBuildChain()
        {
            var actions = Array.Empty<IBuildAction>();
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            var buildConfiguration = Substitute.For<IBuildConfiguration>();

            var sut = new BuildProcessor(actions);

            Action action = () => sut.GetBuildPlan(buildConfiguration, null, parameterInfo);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildPlanForParameterThrowsExceptionWithNullBuildConfiguration()
        {
            var actions = Array.Empty<IBuildAction>();
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            var buildChain = Substitute.For<IBuildChain>();

            var sut = new BuildProcessor(actions);

            Action action = () => sut.GetBuildPlan(null, buildChain, parameterInfo);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildPlanForParameterThrowsExceptionWithNullParameter()
        {
            var actions = Array.Empty<IBuildAction>();

            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();

            var sut = new BuildProcessor(actions);

            Action action = () => sut.GetBuildPlan(buildConfiguration, buildChain, (ParameterInfo) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildPlanForPropertyReturnsBuildPlan()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var match = new MatchResult {IsMatch = true};

            var action = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();

            action.IsMatch(buildConfiguration, buildChain, propertyInfo).Returns(match);

            var actions = new List<IBuildAction>
            {
                action
            };

            var sut = new BuildProcessor(actions);

            var actual = sut.GetBuildPlan(buildConfiguration, buildChain, propertyInfo);

            actual.Should().Be(match);
        }

        [Fact]
        public void GetBuildPlanForPropertyReturnsPlanFromActionWithHighestPriority()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var firstMatch = new MatchResult {IsMatch = true};
            var secondMatch = new MatchResult
                {IsMatch = true, SupportsPopulate = true, AutoPopulate = true, RequiresActivator = true};

            var firstAction = Substitute.For<IBuildAction>();
            var secondAction = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(buildConfiguration);
            firstAction.Priority.Returns(int.MinValue);
            firstAction.IsMatch(buildConfiguration, buildChain, propertyInfo).Returns(firstMatch);
            secondAction.Priority.Returns(int.MaxValue);
            secondAction.IsMatch(buildConfiguration, buildChain, propertyInfo).Returns(secondMatch);

            var actions = new List<IBuildAction>
            {
                firstAction,
                secondAction
            };

            var sut = new BuildProcessor(actions);

            var actual = sut.GetBuildPlan(buildConfiguration, buildChain, propertyInfo);

            actual.Should().BeEquivalentTo(secondMatch);
        }

        [Fact]
        public void GetBuildPlanForPropertyReturnsValueFromMatchingAction()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var firstMatch = new MatchResult {IsMatch = false};
            var secondMatch = new MatchResult
                {IsMatch = true, SupportsPopulate = true, AutoPopulate = true, RequiresActivator = true};

            var firstAction = Substitute.For<IBuildAction>();
            var secondAction = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(buildConfiguration);
            firstAction.Priority.Returns(int.MaxValue);
            firstAction.IsMatch(buildConfiguration, buildChain, propertyInfo).Returns(firstMatch);
            secondAction.Priority.Returns(int.MinValue);
            secondAction.IsMatch(buildConfiguration, buildChain, propertyInfo).Returns(secondMatch);

            var actions = new List<IBuildAction>
            {
                firstAction,
                secondAction
            };

            var sut = new BuildProcessor(actions);

            var actual = sut.GetBuildPlan(buildConfiguration, buildChain, propertyInfo);

            actual.Should().BeEquivalentTo(secondMatch);
        }

        [Fact]
        public void GetBuildPlanForPropertyThrowsExceptionWithNullBuildChain()
        {
            var actions = Array.Empty<IBuildAction>();
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));

            var buildConfiguration = Substitute.For<IBuildConfiguration>();

            var sut = new BuildProcessor(actions);

            Action action = () => sut.GetBuildPlan(buildConfiguration, null, propertyInfo);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildPlanForPropertyThrowsExceptionWithNullBuildConfiguration()
        {
            var actions = Array.Empty<IBuildAction>();
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));

            var buildChain = Substitute.For<IBuildChain>();

            var sut = new BuildProcessor(actions);

            Action action = () => sut.GetBuildPlan(null, buildChain, propertyInfo);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildPlanForPropertyThrowsExceptionWithNullProperty()
        {
            var actions = Array.Empty<IBuildAction>();

            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();

            var sut = new BuildProcessor(actions);

            Action action = () => sut.GetBuildPlan(buildConfiguration, buildChain, (PropertyInfo) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildPlanForTypeReturnsBuildPlan()
        {
            var type = typeof(Person);
            var match = new MatchResult {IsMatch = true};

            var action = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();

            action.IsMatch(buildConfiguration, buildChain, type).Returns(match);

            var actions = new List<IBuildAction>
            {
                action
            };

            var sut = new BuildProcessor(actions);

            var actual = sut.GetBuildPlan(buildConfiguration, buildChain, type);

            actual.Should().Be(match);
        }

        [Fact]
        public void GetBuildPlanForTypeReturnsPlanFromActionWithHighestPriority()
        {
            var type = typeof(Person);
            var firstMatch = new MatchResult {IsMatch = true};
            var secondMatch = new MatchResult
                {IsMatch = true, SupportsPopulate = true, AutoPopulate = true, RequiresActivator = true};

            var firstAction = Substitute.For<IBuildAction>();
            var secondAction = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(buildConfiguration);
            firstAction.Priority.Returns(int.MinValue);
            firstAction.IsMatch(buildConfiguration, buildChain, type).Returns(firstMatch);
            secondAction.Priority.Returns(int.MaxValue);
            secondAction.IsMatch(buildConfiguration, buildChain, type).Returns(secondMatch);

            var actions = new List<IBuildAction>
            {
                firstAction,
                secondAction
            };

            var sut = new BuildProcessor(actions);

            var actual = sut.GetBuildPlan(buildConfiguration, buildChain, type);

            actual.Should().BeEquivalentTo(secondMatch);
        }

        [Fact]
        public void GetBuildPlanForTypeReturnsValueFromMatchingAction()
        {
            var type = typeof(Person);
            var firstMatch = new MatchResult {IsMatch = false};
            var secondMatch = new MatchResult
                {IsMatch = true, SupportsPopulate = true, AutoPopulate = true, RequiresActivator = true};

            var firstAction = Substitute.For<IBuildAction>();
            var secondAction = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(buildConfiguration);
            firstAction.Priority.Returns(int.MaxValue);
            firstAction.IsMatch(buildConfiguration, buildChain, type).Returns(firstMatch);
            secondAction.Priority.Returns(int.MinValue);
            secondAction.IsMatch(buildConfiguration, buildChain, type).Returns(secondMatch);

            var actions = new List<IBuildAction>
            {
                firstAction,
                secondAction
            };

            var sut = new BuildProcessor(actions);

            var actual = sut.GetBuildPlan(buildConfiguration, buildChain, type);

            actual.Should().BeEquivalentTo(secondMatch);
        }

        [Fact]
        public void GetBuildPlanForTypeThrowsExceptionWithNullBuildChain()
        {
            var actions = Array.Empty<IBuildAction>();
            var type = typeof(Person);

            var buildConfiguration = Substitute.For<IBuildConfiguration>();

            var sut = new BuildProcessor(actions);

            Action action = () => sut.GetBuildPlan(buildConfiguration, null, type);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildPlanForTypeThrowsExceptionWithNullBuildConfiguration()
        {
            var actions = Array.Empty<IBuildAction>();
            var type = typeof(Person);

            var buildChain = Substitute.For<IBuildChain>();

            var sut = new BuildProcessor(actions);

            Action action = () => sut.GetBuildPlan(null, buildChain, type);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildPlanForTypeThrowsExceptionWithNullType()
        {
            var actions = Array.Empty<IBuildAction>();

            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();

            var sut = new BuildProcessor(actions);

            Action action = () => sut.GetBuildPlan(buildConfiguration, buildChain, (Type) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWithNullActions()
        {
            Action action = () => new BuildProcessor(null);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}