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
            var match = new BuildCapability {SupportsCreate = true};

            var action = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(buildConfiguration);
            action.GetBuildCapability(buildConfiguration, buildChain, parameterInfo).Returns(match);
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
            var match = new BuildCapability {SupportsCreate = true};

            var firstAction = Substitute.For<IBuildAction>();
            var secondAction = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(buildConfiguration);
            firstAction.Priority.Returns(int.MinValue);
            firstAction.GetBuildCapability(buildConfiguration, buildChain, parameterInfo).Returns(match);
            secondAction.Priority.Returns(int.MaxValue);
            secondAction.GetBuildCapability(buildConfiguration, buildChain, parameterInfo).Returns(match);
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
            var match = new BuildCapability {SupportsCreate = true};

            var firstAction = Substitute.For<IBuildAction>();
            var secondAction = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(buildConfiguration);
            firstAction.Priority.Returns(int.MaxValue);
            secondAction.Priority.Returns(int.MinValue);
            secondAction.GetBuildCapability(buildConfiguration, buildChain, parameterInfo).Returns(match);
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
            var match = new BuildCapability {SupportsCreate = true};

            var action = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(buildConfiguration);
            action.GetBuildCapability(buildConfiguration, buildChain, propertyInfo).Returns(match);
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
            var match = new BuildCapability {SupportsCreate = true};

            var firstAction = Substitute.For<IBuildAction>();
            var secondAction = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(buildConfiguration);
            firstAction.Priority.Returns(int.MinValue);
            firstAction.GetBuildCapability(buildConfiguration, buildChain, propertyInfo).Returns(match);
            secondAction.Priority.Returns(int.MaxValue);
            secondAction.GetBuildCapability(buildConfiguration, buildChain, propertyInfo).Returns(match);
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
            var match = new BuildCapability {SupportsCreate = true};

            var firstAction = Substitute.For<IBuildAction>();
            var secondAction = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(buildConfiguration);
            firstAction.Priority.Returns(int.MaxValue);
            secondAction.Priority.Returns(int.MinValue);
            secondAction.GetBuildCapability(buildConfiguration, buildChain, propertyInfo).Returns(match);
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
            var match = new BuildCapability {SupportsCreate = true};

            var action = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(buildConfiguration);
            action.GetBuildCapability(buildConfiguration, buildChain, type).Returns(match);
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
            var match = new BuildCapability {SupportsCreate = true};

            var firstAction = Substitute.For<IBuildAction>();
            var secondAction = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(buildConfiguration);
            firstAction.Priority.Returns(int.MinValue);
            firstAction.GetBuildCapability(buildConfiguration, buildChain, type).Returns(match);
            secondAction.Priority.Returns(int.MaxValue);
            secondAction.GetBuildCapability(buildConfiguration, buildChain, type).Returns(match);
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
            var match = new BuildCapability {SupportsCreate = true};

            var firstAction = Substitute.For<IBuildAction>();
            var secondAction = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(buildConfiguration);
            firstAction.Priority.Returns(int.MaxValue);
            secondAction.Priority.Returns(int.MinValue);
            secondAction.GetBuildCapability(buildConfiguration, buildChain, type).Returns(match);
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

        [Theory]
        [InlineData(BuildRequirement.Create, true, true, true)]
        [InlineData(BuildRequirement.Create, true, false, true)]
        [InlineData(BuildRequirement.Create, false, true, false)]
        [InlineData(BuildRequirement.Create, false, false, false)]
        [InlineData(BuildRequirement.Populate, true, true, true)]
        [InlineData(BuildRequirement.Populate, true, false, false)]
        [InlineData(BuildRequirement.Populate, false, true, true)]
        [InlineData(BuildRequirement.Populate, false, false, false)]
        public void GetBuildCapabilityForParameterReturnsBuildPlan(BuildRequirement requirement, bool canCreate,
            bool canPopulate, bool planExists)
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var capability = new BuildCapability {SupportsCreate = canCreate, SupportsPopulate = canPopulate};

            var action = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();

            action.GetBuildCapability(buildConfiguration, buildChain, parameterInfo).Returns(capability);

            var actions = new List<IBuildAction>
            {
                action
            };

            var sut = new BuildProcessor(actions);

            var actual = sut.GetBuildCapability(buildConfiguration, buildChain, requirement, parameterInfo);

            if (planExists)
            {
                actual.Should().Be(capability);
            }
            else
            {
                actual.Should().BeNull();
            }
        }

        [Fact]
        public void GetBuildCapabilityForParameterReturnsPlanFromActionWithHighestPriority()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var firstMatch = new BuildCapability {SupportsCreate = true};
            var secondMatch = new BuildCapability
                {SupportsCreate = true, SupportsPopulate = true, AutoPopulate = true, AutoDetectConstructor = true};

            var firstAction = Substitute.For<IBuildAction>();
            var secondAction = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(buildConfiguration);
            firstAction.Priority.Returns(int.MinValue);
            firstAction.GetBuildCapability(buildConfiguration, buildChain, parameterInfo).Returns(firstMatch);
            secondAction.Priority.Returns(int.MaxValue);
            secondAction.GetBuildCapability(buildConfiguration, buildChain, parameterInfo).Returns(secondMatch);

            var actions = new List<IBuildAction>
            {
                firstAction,
                secondAction
            };

            var sut = new BuildProcessor(actions);

            var actual = sut.GetBuildCapability(buildConfiguration, buildChain, BuildRequirement.Create, parameterInfo);

            actual.Should().BeEquivalentTo(secondMatch);
        }

        [Fact]
        public void GetBuildCapabilityForParameterReturnsValueFromMatchingAction()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var firstMatch = new BuildCapability {SupportsCreate = false};
            var secondMatch = new BuildCapability
                {SupportsCreate = true, SupportsPopulate = true, AutoPopulate = true, AutoDetectConstructor = true};

            var firstAction = Substitute.For<IBuildAction>();
            var secondAction = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(buildConfiguration);
            firstAction.Priority.Returns(int.MaxValue);
            firstAction.GetBuildCapability(buildConfiguration, buildChain, parameterInfo).Returns(firstMatch);
            secondAction.Priority.Returns(int.MinValue);
            secondAction.GetBuildCapability(buildConfiguration, buildChain, parameterInfo).Returns(secondMatch);

            var actions = new List<IBuildAction>
            {
                firstAction,
                secondAction
            };

            var sut = new BuildProcessor(actions);

            var actual = sut.GetBuildCapability(buildConfiguration, buildChain, BuildRequirement.Create, parameterInfo);

            actual.Should().BeEquivalentTo(secondMatch);
        }

        [Fact]
        public void GetBuildCapabilityForParameterThrowsExceptionWithNullBuildChain()
        {
            var actions = Array.Empty<IBuildAction>();
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            var buildConfiguration = Substitute.For<IBuildConfiguration>();

            var sut = new BuildProcessor(actions);

            Action action = () =>
                sut.GetBuildCapability(buildConfiguration, null, BuildRequirement.Create, parameterInfo);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildCapabilityForParameterThrowsExceptionWithNullBuildConfiguration()
        {
            var actions = Array.Empty<IBuildAction>();
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            var buildChain = Substitute.For<IBuildChain>();

            var sut = new BuildProcessor(actions);

            Action action = () => sut.GetBuildCapability(null, buildChain, BuildRequirement.Create, parameterInfo);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildCapabilityForParameterThrowsExceptionWithNullParameter()
        {
            var actions = Array.Empty<IBuildAction>();

            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();

            var sut = new BuildProcessor(actions);

            Action action = () =>
                sut.GetBuildCapability(buildConfiguration, buildChain, BuildRequirement.Create, (ParameterInfo) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(BuildRequirement.Create, true, true, true)]
        [InlineData(BuildRequirement.Create, true, false, true)]
        [InlineData(BuildRequirement.Create, false, true, false)]
        [InlineData(BuildRequirement.Create, false, false, false)]
        [InlineData(BuildRequirement.Populate, true, true, true)]
        [InlineData(BuildRequirement.Populate, true, false, false)]
        [InlineData(BuildRequirement.Populate, false, true, true)]
        [InlineData(BuildRequirement.Populate, false, false, false)]
        public void GetBuildCapabilityForPropertyReturnsBuildPlan(BuildRequirement requirement, bool canCreate,
            bool canPopulate, bool planExists)
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var capability = new BuildCapability {SupportsCreate = canCreate, SupportsPopulate = canPopulate};

            var action = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();

            action.GetBuildCapability(buildConfiguration, buildChain, propertyInfo).Returns(capability);

            var actions = new List<IBuildAction>
            {
                action
            };

            var sut = new BuildProcessor(actions);

            var actual = sut.GetBuildCapability(buildConfiguration, buildChain, requirement, propertyInfo);

            if (planExists)
            {
                actual.Should().Be(capability);
            }
            else
            {
                actual.Should().BeNull();
            }
        }

        [Fact]
        public void GetBuildCapabilityForPropertyReturnsPlanFromActionWithHighestPriority()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var firstMatch = new BuildCapability {SupportsCreate = true};
            var secondMatch = new BuildCapability
                {SupportsCreate = true, SupportsPopulate = true, AutoPopulate = true, AutoDetectConstructor = true};

            var firstAction = Substitute.For<IBuildAction>();
            var secondAction = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(buildConfiguration);
            firstAction.Priority.Returns(int.MinValue);
            firstAction.GetBuildCapability(buildConfiguration, buildChain, propertyInfo).Returns(firstMatch);
            secondAction.Priority.Returns(int.MaxValue);
            secondAction.GetBuildCapability(buildConfiguration, buildChain, propertyInfo).Returns(secondMatch);

            var actions = new List<IBuildAction>
            {
                firstAction,
                secondAction
            };

            var sut = new BuildProcessor(actions);

            var actual = sut.GetBuildCapability(buildConfiguration, buildChain, BuildRequirement.Create, propertyInfo);

            actual.Should().BeEquivalentTo(secondMatch);
        }

        [Fact]
        public void GetBuildCapabilityForPropertyReturnsValueFromMatchingAction()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var firstMatch = new BuildCapability {SupportsCreate = false};
            var secondMatch = new BuildCapability
                {SupportsCreate = true, SupportsPopulate = true, AutoPopulate = true, AutoDetectConstructor = true};

            var firstAction = Substitute.For<IBuildAction>();
            var secondAction = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(buildConfiguration);
            firstAction.Priority.Returns(int.MaxValue);
            firstAction.GetBuildCapability(buildConfiguration, buildChain, propertyInfo).Returns(firstMatch);
            secondAction.Priority.Returns(int.MinValue);
            secondAction.GetBuildCapability(buildConfiguration, buildChain, propertyInfo).Returns(secondMatch);

            var actions = new List<IBuildAction>
            {
                firstAction,
                secondAction
            };

            var sut = new BuildProcessor(actions);

            var actual = sut.GetBuildCapability(buildConfiguration, buildChain, BuildRequirement.Create, propertyInfo);

            actual.Should().BeEquivalentTo(secondMatch);
        }

        [Fact]
        public void GetBuildCapabilityForPropertyThrowsExceptionWithNullBuildChain()
        {
            var actions = Array.Empty<IBuildAction>();
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));

            var buildConfiguration = Substitute.For<IBuildConfiguration>();

            var sut = new BuildProcessor(actions);

            Action action = () =>
                sut.GetBuildCapability(buildConfiguration, null, BuildRequirement.Create, propertyInfo);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildCapabilityForPropertyThrowsExceptionWithNullBuildConfiguration()
        {
            var actions = Array.Empty<IBuildAction>();
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));

            var buildChain = Substitute.For<IBuildChain>();

            var sut = new BuildProcessor(actions);

            Action action = () => sut.GetBuildCapability(null, buildChain, BuildRequirement.Create, propertyInfo);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildCapabilityForPropertyThrowsExceptionWithNullProperty()
        {
            var actions = Array.Empty<IBuildAction>();

            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();

            var sut = new BuildProcessor(actions);

            Action action = () =>
                sut.GetBuildCapability(buildConfiguration, buildChain, BuildRequirement.Create, (PropertyInfo) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [InlineData(BuildRequirement.Create, true, true, true)]
        [InlineData(BuildRequirement.Create, true, false, true)]
        [InlineData(BuildRequirement.Create, false, true, false)]
        [InlineData(BuildRequirement.Create, false, false, false)]
        [InlineData(BuildRequirement.Populate, true, true, true)]
        [InlineData(BuildRequirement.Populate, true, false, false)]
        [InlineData(BuildRequirement.Populate, false, true, true)]
        [InlineData(BuildRequirement.Populate, false, false, false)]
        public void GetBuildCapabilityForTypeReturnsBuildPlan(BuildRequirement requirement, bool canCreate,
            bool canPopulate, bool planExists)
        {
            var type = typeof(Person);
            var capability = new BuildCapability {SupportsCreate = canCreate, SupportsPopulate = canPopulate};

            var action = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();

            action.GetBuildCapability(buildConfiguration, buildChain, type).Returns(capability);

            var actions = new List<IBuildAction>
            {
                action
            };

            var sut = new BuildProcessor(actions);

            var actual = sut.GetBuildCapability(buildConfiguration, buildChain, requirement, type);

            if (planExists)
            {
                actual.Should().Be(capability);
            }
            else
            {
                actual.Should().BeNull();
            }
        }

        [Fact]
        public void GetBuildCapabilityForTypeReturnsPlanFromActionWithHighestPriority()
        {
            var type = typeof(Person);
            var firstMatch = new BuildCapability {SupportsCreate = true};
            var secondMatch = new BuildCapability
                {SupportsCreate = true, SupportsPopulate = true, AutoPopulate = true, AutoDetectConstructor = true};

            var firstAction = Substitute.For<IBuildAction>();
            var secondAction = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(buildConfiguration);
            firstAction.Priority.Returns(int.MinValue);
            firstAction.GetBuildCapability(buildConfiguration, buildChain, type).Returns(firstMatch);
            secondAction.Priority.Returns(int.MaxValue);
            secondAction.GetBuildCapability(buildConfiguration, buildChain, type).Returns(secondMatch);

            var actions = new List<IBuildAction>
            {
                firstAction,
                secondAction
            };

            var sut = new BuildProcessor(actions);

            var actual = sut.GetBuildCapability(buildConfiguration, buildChain, BuildRequirement.Create, type);

            actual.Should().BeEquivalentTo(secondMatch);
        }

        [Fact]
        public void GetBuildCapabilityForTypeReturnsValueFromMatchingAction()
        {
            var type = typeof(Person);
            var firstMatch = new BuildCapability {SupportsCreate = false};
            var secondMatch = new BuildCapability
                {SupportsCreate = true, SupportsPopulate = true, AutoPopulate = true, AutoDetectConstructor = true};

            var firstAction = Substitute.For<IBuildAction>();
            var secondAction = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(buildConfiguration);
            firstAction.Priority.Returns(int.MaxValue);
            firstAction.GetBuildCapability(buildConfiguration, buildChain, type).Returns(firstMatch);
            secondAction.Priority.Returns(int.MinValue);
            secondAction.GetBuildCapability(buildConfiguration, buildChain, type).Returns(secondMatch);

            var actions = new List<IBuildAction>
            {
                firstAction,
                secondAction
            };

            var sut = new BuildProcessor(actions);

            var actual = sut.GetBuildCapability(buildConfiguration, buildChain, BuildRequirement.Create, type);

            actual.Should().BeEquivalentTo(secondMatch);
        }

        [Fact]
        public void GetBuildCapabilityForTypeThrowsExceptionWithNullBuildChain()
        {
            var actions = Array.Empty<IBuildAction>();
            var type = typeof(Person);

            var buildConfiguration = Substitute.For<IBuildConfiguration>();

            var sut = new BuildProcessor(actions);

            Action action = () => sut.GetBuildCapability(buildConfiguration, null, BuildRequirement.Create, type);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildCapabilityForTypeThrowsExceptionWithNullBuildConfiguration()
        {
            var actions = Array.Empty<IBuildAction>();
            var type = typeof(Person);

            var buildChain = Substitute.For<IBuildChain>();

            var sut = new BuildProcessor(actions);

            Action action = () => sut.GetBuildCapability(null, buildChain, BuildRequirement.Create, type);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildCapabilityForTypeThrowsExceptionWithNullType()
        {
            var actions = Array.Empty<IBuildAction>();

            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();

            var sut = new BuildProcessor(actions);

            Action action = () =>
                sut.GetBuildCapability(buildConfiguration, buildChain, BuildRequirement.Create, (Type) null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PopulateReturnsActionValue()
        {
            var expected = new Person();
            var match = new BuildCapability {SupportsPopulate = true};

            var action = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(buildConfiguration);
            action.GetBuildCapability(buildConfiguration, buildChain, expected.GetType()).Returns(match);
            action.Populate(executeStrategy, expected).Returns(expected);

            var actions = new List<IBuildAction>
            {
                action
            };

            var sut = new BuildProcessor(actions);

            var actual = sut.Populate(executeStrategy, expected);

            actual.Should().Be(expected);
        }

        [Fact]
        public void PopulateReturnsValueFromActionWithHighestPriority()
        {
            var expected = new Person();
            var match = new BuildCapability { SupportsPopulate = true};

            var firstAction = Substitute.For<IBuildAction>();
            var secondAction = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(buildConfiguration);
            firstAction.Priority.Returns(int.MinValue);
            firstAction.GetBuildCapability(buildConfiguration, buildChain, expected.GetType()).Returns(match);
            secondAction.Priority.Returns(int.MaxValue);
            secondAction.GetBuildCapability(buildConfiguration, buildChain, expected.GetType()).Returns(match);
            secondAction.Populate(executeStrategy, expected).Returns(expected);

            var actions = new List<IBuildAction>
            {
                firstAction,
                secondAction
            };

            var sut = new BuildProcessor(actions);

            var actual = sut.Populate(executeStrategy, expected);

            actual.Should().Be(expected);
        }

        [Fact]
        public void PopulateReturnsValueFromMatchingAction()
        {
            var expected = new Person();
            var match = new BuildCapability { SupportsPopulate = true};

            var firstAction = Substitute.For<IBuildAction>();
            var secondAction = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(buildConfiguration);
            firstAction.Priority.Returns(int.MaxValue);
            firstAction.GetBuildCapability(buildConfiguration, buildChain, expected.GetType())
                .Returns((BuildCapability) null);
            secondAction.Priority.Returns(int.MinValue);
            secondAction.GetBuildCapability(buildConfiguration, buildChain, expected.GetType()).Returns(match);
            secondAction.Populate(executeStrategy, expected).Returns(expected);

            var actions = new List<IBuildAction>
            {
                firstAction,
                secondAction
            };

            var sut = new BuildProcessor(actions);

            var actual = sut.Populate(executeStrategy, expected);

            actual.Should().Be(expected);
        }

        [Fact]
        public void PopulateThrowsExceptionWhenNoBuildActionFound()
        {
            var actions = Array.Empty<IBuildAction>();
            var instance = new Person();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new BuildProcessor(actions);

            Action action = () => sut.Populate(executeStrategy, instance);

            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void PopulateThrowsExceptionWithNullExecuteStrategy()
        {
            var actions = Array.Empty<IBuildAction>();
            var instance = new Person();

            var sut = new BuildProcessor(actions);

            Action action = () => sut.Populate(null, instance);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PopulateThrowsExceptionWithNullInstance()
        {
            var actions = Array.Empty<IBuildAction>();

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new BuildProcessor(actions);

            Action action = () => sut.Populate(executeStrategy, null);

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