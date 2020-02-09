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

            var action = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(buildConfiguration);
            action.IsMatch(buildConfiguration, buildChain, parameterInfo).Returns(true);
            action.Build(executeStrategy, parameterInfo).Returns(expected);

            var actions = new List<IBuildAction>
            {
                action
            };

            var sut = new BuildProcessor(actions);

            var actual = sut.Build(parameterInfo, executeStrategy);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForParameterReturnsValueFromActionWithHighestPriority()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var expected = new Person();

            var firstAction = Substitute.For<IBuildAction>();
            var secondAction = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(buildConfiguration);
            firstAction.Priority.Returns(int.MinValue);
            firstAction.IsMatch(buildConfiguration, buildChain, parameterInfo).Returns(true);
            secondAction.Priority.Returns(int.MaxValue);
            secondAction.IsMatch(buildConfiguration, buildChain, parameterInfo).Returns(true);
            secondAction.Build(executeStrategy, parameterInfo).Returns(expected);

            var actions = new List<IBuildAction>
            {
                firstAction,
                secondAction
            };

            var sut = new BuildProcessor(actions);

            var actual = sut.Build(parameterInfo, executeStrategy);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForParameterReturnsValueFromMatchingAction()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var expected = new Person();

            var firstAction = Substitute.For<IBuildAction>();
            var secondAction = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(buildConfiguration);
            firstAction.Priority.Returns(int.MaxValue);
            firstAction.IsMatch(buildConfiguration, buildChain, parameterInfo).Returns(false);
            secondAction.Priority.Returns(int.MinValue);
            secondAction.IsMatch(buildConfiguration, buildChain, parameterInfo).Returns(true);
            secondAction.Build(executeStrategy, parameterInfo).Returns(expected);

            var actions = new List<IBuildAction>
            {
                firstAction,
                secondAction
            };

            var sut = new BuildProcessor(actions);

            var actual = sut.Build(parameterInfo, executeStrategy);

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

            Action action = () => sut.Build(parameterInfo, executeStrategy);

            action.Should().Throw<BuildException>();
        }

        [Fact]
        public void BuildForParameterThrowsExceptionWithNullExecuteStrategy()
        {
            var actions = Array.Empty<IBuildAction>();
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            var sut = new BuildProcessor(actions);

            Action action = () => sut.Build(parameterInfo, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForParameterThrowsExceptionWithNullParameter()
        {
            var actions = Array.Empty<IBuildAction>();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new BuildProcessor(actions);

            Action action = () => sut.Build((ParameterInfo) null, executeStrategy);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForPropertyReturnsActionValue()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var expected = new Person();

            var action = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(buildConfiguration);
            action.IsMatch(buildConfiguration, buildChain, propertyInfo).Returns(true);
            action.Build(executeStrategy, propertyInfo).Returns(expected);

            var actions = new List<IBuildAction>
            {
                action
            };

            var sut = new BuildProcessor(actions);

            var actual = sut.Build(propertyInfo, executeStrategy);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForPropertyReturnsValueFromActionWithHighestPriority()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var expected = new Person();

            var firstAction = Substitute.For<IBuildAction>();
            var secondAction = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(buildConfiguration);
            firstAction.Priority.Returns(int.MinValue);
            firstAction.IsMatch(buildConfiguration, buildChain, propertyInfo).Returns(true);
            secondAction.Priority.Returns(int.MaxValue);
            secondAction.IsMatch(buildConfiguration, buildChain, propertyInfo).Returns(true);
            secondAction.Build(executeStrategy, propertyInfo).Returns(expected);

            var actions = new List<IBuildAction>
            {
                firstAction,
                secondAction
            };

            var sut = new BuildProcessor(actions);

            var actual = sut.Build(propertyInfo, executeStrategy);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForPropertyReturnsValueFromMatchingAction()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var expected = new Person();

            var firstAction = Substitute.For<IBuildAction>();
            var secondAction = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(buildConfiguration);
            firstAction.Priority.Returns(int.MaxValue);
            firstAction.IsMatch(buildConfiguration, buildChain, propertyInfo).Returns(false);
            secondAction.Priority.Returns(int.MinValue);
            secondAction.IsMatch(buildConfiguration, buildChain, propertyInfo).Returns(true);
            secondAction.Build(executeStrategy, propertyInfo).Returns(expected);

            var actions = new List<IBuildAction>
            {
                firstAction,
                secondAction
            };

            var sut = new BuildProcessor(actions);

            var actual = sut.Build(propertyInfo, executeStrategy);

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

            Action action = () => sut.Build(propertyInfo, executeStrategy);

            action.Should().Throw<BuildException>();
        }

        [Fact]
        public void BuildForPropertyThrowsExceptionWithNullExecuteStrategy()
        {
            var actions = Array.Empty<IBuildAction>();
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));

            var sut = new BuildProcessor(actions);

            Action action = () => sut.Build(propertyInfo, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForPropertyThrowsExceptionWithNullProperty()
        {
            var actions = Array.Empty<IBuildAction>();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new BuildProcessor(actions);

            Action action = () => sut.Build((PropertyInfo) null, executeStrategy);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForTypeReturnsActionValue()
        {
            var type = typeof(Person);
            var expected = new Person();

            var action = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(buildConfiguration);
            action.IsMatch(buildConfiguration, buildChain, type).Returns(true);
            action.Build(executeStrategy, type).Returns(expected);

            var actions = new List<IBuildAction>
            {
                action
            };

            var sut = new BuildProcessor(actions);

            var actual = sut.Build(type, executeStrategy);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForTypeReturnsValueFromActionWithHighestPriority()
        {
            var type = typeof(Person);
            var expected = new Person();

            var firstAction = Substitute.For<IBuildAction>();
            var secondAction = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(buildConfiguration);
            firstAction.Priority.Returns(int.MinValue);
            firstAction.IsMatch(buildConfiguration, buildChain, type).Returns(true);
            secondAction.Priority.Returns(int.MaxValue);
            secondAction.IsMatch(buildConfiguration, buildChain, type).Returns(true);
            secondAction.Build(executeStrategy, type).Returns(expected);

            var actions = new List<IBuildAction>
            {
                firstAction,
                secondAction
            };

            var sut = new BuildProcessor(actions);

            var actual = sut.Build(type, executeStrategy);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForTypeReturnsValueFromMatchingAction()
        {
            var type = typeof(Person);
            var expected = new Person();

            var firstAction = Substitute.For<IBuildAction>();
            var secondAction = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(buildConfiguration);
            firstAction.Priority.Returns(int.MaxValue);
            firstAction.IsMatch(buildConfiguration, buildChain, type).Returns(false);
            secondAction.Priority.Returns(int.MinValue);
            secondAction.IsMatch(buildConfiguration, buildChain, type).Returns(true);
            secondAction.Build(executeStrategy, type).Returns(expected);

            var actions = new List<IBuildAction>
            {
                firstAction,
                secondAction
            };

            var sut = new BuildProcessor(actions);

            var actual = sut.Build(type, executeStrategy);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForTypeThrowsExceptionWhenNoBuildActionFound()
        {
            var actions = Array.Empty<IBuildAction>();
            var type = typeof(Person);

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new BuildProcessor(actions);

            Action action = () => sut.Build(type, executeStrategy);

            action.Should().Throw<BuildException>();
        }

        [Fact]
        public void BuildForTypeThrowsExceptionWithNullExecuteStrategy()
        {
            var actions = Array.Empty<IBuildAction>();
            var type = typeof(Person);

            var sut = new BuildProcessor(actions);

            Action action = () => sut.Build(type, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForTypeThrowsExceptionWithNullType()
        {
            var actions = Array.Empty<IBuildAction>();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new BuildProcessor(actions);

            Action action = () => sut.Build((Type) null, executeStrategy);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void CanCreateInstance()
        {
            Action action = () => new BuildProcessor();

            action.Should().NotThrow();
        }

        [Fact]
        public void ThrowsExceptionWithNullActions()
        {
            Action action = () => new BuildProcessor(null);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}