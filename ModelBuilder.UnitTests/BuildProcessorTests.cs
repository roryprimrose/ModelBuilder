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
        public void CanCreateInstance()
        {
            // ReSharper disable once ObjectCreationAsStatement
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
        public void GetBuildCapabilityForParameterReturnsBuildCapabilityWhenSupported(BuildRequirement requirement,
            bool canCreate,
            bool canPopulate, bool capabilityExists)
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            var capability = Substitute.For<IBuildCapability>();

            var buildAction = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            capability.SupportsCreate.Returns(canCreate);
            capability.SupportsPopulate.Returns(canPopulate);
            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);

            buildAction.GetBuildCapability(buildConfiguration, buildChain, parameterInfo).Returns(capability);

            var actions = new List<IBuildAction>
            {
                buildAction
            };

            var sut = new BuildProcessor(actions);

            if (capabilityExists)
            {
                var actual = sut.GetBuildCapability(executeStrategy, requirement, parameterInfo);

                actual.Should().Be(capability);
            }
            else
            {
                Action action = () => sut.GetBuildCapability(executeStrategy, requirement, parameterInfo);

                action.Should().Throw<BuildException>();
            }
        }

        [Fact]
        public void GetBuildCapabilityForParameterReturnsCapabilityFromAction()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            var firstCapability = Substitute.For<IBuildCapability>();
            var secondCapability = Substitute.For<IBuildCapability>();
            var firstAction = Substitute.For<IBuildAction>();
            var secondAction = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            secondCapability.SupportsCreate.Returns(true);
            secondCapability.SupportsPopulate.Returns(true);
            secondCapability.AutoPopulate.Returns(true);
            firstAction.Priority.Returns(int.MaxValue);
            firstAction.GetBuildCapability(buildConfiguration, buildChain, parameterInfo).Returns(firstCapability);
            secondAction.Priority.Returns(int.MinValue);
            secondAction.GetBuildCapability(buildConfiguration, buildChain, parameterInfo).Returns(secondCapability);

            var actions = new List<IBuildAction>
            {
                firstAction,
                secondAction
            };

            var sut = new BuildProcessor(actions);

            var actual = sut.GetBuildCapability(executeStrategy, BuildRequirement.Create, parameterInfo);

            actual.Should().BeEquivalentTo(secondCapability);
        }

        [Fact]
        public void GetBuildCapabilityForParameterReturnsCapabilityFromActionWithHighestPriority()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            var firstCapability = Substitute.For<IBuildCapability>();
            var secondCapability = Substitute.For<IBuildCapability>();
            var firstAction = Substitute.For<IBuildAction>();
            var secondAction = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            firstCapability.SupportsCreate.Returns(true);
            secondCapability.SupportsCreate.Returns(true);
            secondCapability.SupportsPopulate.Returns(true);
            secondCapability.AutoPopulate.Returns(true);
            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);
            firstAction.Priority.Returns(int.MinValue);
            firstAction.GetBuildCapability(buildConfiguration, buildChain, parameterInfo).Returns(firstCapability);
            secondAction.Priority.Returns(int.MaxValue);
            secondAction.GetBuildCapability(buildConfiguration, buildChain, parameterInfo).Returns(secondCapability);

            var actions = new List<IBuildAction>
            {
                firstAction,
                secondAction
            };

            var sut = new BuildProcessor(actions);

            var actual = sut.GetBuildCapability(executeStrategy, BuildRequirement.Create, parameterInfo);

            actual.Should().BeEquivalentTo(secondCapability);
        }

        [Fact]
        public void GetBuildCapabilityForParameterThrowsExceptionWithNullExecuteStrategy()
        {
            var actions = Array.Empty<IBuildAction>();
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            var sut = new BuildProcessor(actions);

            Action action = () =>
                sut.GetBuildCapability(null!, BuildRequirement.Create, parameterInfo);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildCapabilityForParameterThrowsExceptionWithNullParameter()
        {
            var actions = Array.Empty<IBuildAction>();

            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();
            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new BuildProcessor(actions);

            Action action = () =>
                sut.GetBuildCapability(executeStrategy, BuildRequirement.Create, (ParameterInfo)null!);

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
        public void GetBuildCapabilityForPropertyReturnsBuildCapabilityWhenSupported(BuildRequirement requirement,
            bool canCreate,
            bool canPopulate, bool capabilityExists)
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName))!;
            var capability = Substitute.For<IBuildCapability>();
            capability.SupportsCreate.Returns(canCreate);
            capability.SupportsPopulate.Returns(canPopulate);

            var buildAction = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();
            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);

            buildAction.GetBuildCapability(buildConfiguration, buildChain, propertyInfo).Returns(capability);

            var actions = new List<IBuildAction>
            {
                buildAction
            };

            var sut = new BuildProcessor(actions);

            if (capabilityExists)
            {
                var actual = sut.GetBuildCapability(executeStrategy, requirement, propertyInfo);

                actual.Should().Be(capability);
            }
            else
            {
                Action action = () => sut.GetBuildCapability(executeStrategy, requirement, propertyInfo);

                action.Should().Throw<BuildException>();
            }
        }

        [Fact]
        public void GetBuildCapabilityForPropertyReturnsCapabilityFromAction()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName))!;

            var firstCapability = Substitute.For<IBuildCapability>();
            var secondCapability = Substitute.For<IBuildCapability>();
            var firstAction = Substitute.For<IBuildAction>();
            var secondAction = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            secondCapability.SupportsCreate.Returns(true);
            secondCapability.SupportsPopulate.Returns(true);
            secondCapability.AutoPopulate.Returns(true);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(buildConfiguration);
            firstAction.Priority.Returns(int.MaxValue);
            firstAction.GetBuildCapability(buildConfiguration, buildChain, propertyInfo).Returns(firstCapability);
            secondAction.Priority.Returns(int.MinValue);
            secondAction.GetBuildCapability(buildConfiguration, buildChain, propertyInfo).Returns(secondCapability);

            var actions = new List<IBuildAction>
            {
                firstAction,
                secondAction
            };

            var sut = new BuildProcessor(actions);

            var actual = sut.GetBuildCapability(executeStrategy, BuildRequirement.Create, propertyInfo);

            actual.Should().BeEquivalentTo(secondCapability);
        }

        [Fact]
        public void GetBuildCapabilityForPropertyReturnsCapabilityFromActionWithHighestPriority()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName))!;
            var firstCapability = Substitute.For<IBuildCapability>();
            var secondCapability = Substitute.For<IBuildCapability>();

            var firstAction = Substitute.For<IBuildAction>();
            var secondAction = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            firstCapability.SupportsCreate.Returns(true);
            secondCapability.SupportsCreate.Returns(true);
            secondCapability.SupportsPopulate.Returns(true);
            secondCapability.AutoPopulate.Returns(true);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(buildConfiguration);
            firstAction.Priority.Returns(int.MinValue);
            firstAction.GetBuildCapability(buildConfiguration, buildChain, propertyInfo).Returns(firstCapability);
            secondAction.Priority.Returns(int.MaxValue);
            secondAction.GetBuildCapability(buildConfiguration, buildChain, propertyInfo).Returns(secondCapability);

            var actions = new List<IBuildAction>
            {
                firstAction,
                secondAction
            };

            var sut = new BuildProcessor(actions);

            var actual = sut.GetBuildCapability(executeStrategy, BuildRequirement.Create, propertyInfo);

            actual.Should().BeEquivalentTo(secondCapability);
        }

        [Fact]
        public void GetBuildCapabilityForPropertyThrowsExceptionWithNullExecuteStrategy()
        {
            var actions = Array.Empty<IBuildAction>();
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName))!;

            var sut = new BuildProcessor(actions);

            Action action = () => sut.GetBuildCapability(null!, BuildRequirement.Create, propertyInfo);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildCapabilityForPropertyThrowsExceptionWithNullProperty()
        {
            var actions = Array.Empty<IBuildAction>();

            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();
            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new BuildProcessor(actions);

            Action action = () =>
                sut.GetBuildCapability(executeStrategy, BuildRequirement.Create, (PropertyInfo)null!);

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
        public void GetBuildCapabilityForTypeReturnsBuildCapability(BuildRequirement requirement, bool canCreate,
            bool canPopulate, bool planExists)
        {
            var type = typeof(Person);

            var capability = Substitute.For<IBuildCapability>();
            var buildAction = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            capability.SupportsCreate.Returns(canCreate);
            capability.SupportsPopulate.Returns(canPopulate);
            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);

            buildAction.GetBuildCapability(buildConfiguration, buildChain, type).Returns(capability);

            var actions = new List<IBuildAction>
            {
                buildAction
            };

            var sut = new BuildProcessor(actions);

            if (planExists)
            {
                var actual = sut.GetBuildCapability(executeStrategy, requirement, type);

                actual.Should().Be(capability);
            }
            else
            {
                Action action = () => sut.GetBuildCapability(executeStrategy, requirement, type);

                action.Should().Throw<BuildException>();
            }
        }

        [Fact]
        public void GetBuildCapabilityForTypeReturnsCapabilityFromAction()
        {
            var type = typeof(Person);
            var firstCapability = Substitute.For<IBuildCapability>();
            var secondCapability = Substitute.For<IBuildCapability>();

            var firstAction = Substitute.For<IBuildAction>();
            var secondAction = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            secondCapability.SupportsCreate.Returns(true);
            secondCapability.SupportsPopulate.Returns(true);
            secondCapability.AutoPopulate.Returns(true);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(buildConfiguration);
            firstAction.Priority.Returns(int.MaxValue);
            firstAction.GetBuildCapability(buildConfiguration, buildChain, type).Returns(firstCapability);
            secondAction.Priority.Returns(int.MinValue);
            secondAction.GetBuildCapability(buildConfiguration, buildChain, type).Returns(secondCapability);

            var actions = new List<IBuildAction>
            {
                firstAction,
                secondAction
            };

            var sut = new BuildProcessor(actions);

            var actual = sut.GetBuildCapability(executeStrategy, BuildRequirement.Create, type);

            actual.Should().BeEquivalentTo(secondCapability);
        }

        [Fact]
        public void GetBuildCapabilityForTypeReturnsCapabilityFromActionWithHighestPriority()
        {
            var type = typeof(Person);
            var firstCapability = Substitute.For<IBuildCapability>();
            var secondCapability = Substitute.For<IBuildCapability>();

            var firstAction = Substitute.For<IBuildAction>();
            var secondAction = Substitute.For<IBuildAction>();
            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            firstCapability.SupportsCreate.Returns(true);
            secondCapability.SupportsCreate.Returns(true);
            secondCapability.SupportsPopulate.Returns(true);
            secondCapability.AutoPopulate.Returns(true);
            executeStrategy.BuildChain.Returns(buildChain);
            executeStrategy.Configuration.Returns(buildConfiguration);
            firstAction.Priority.Returns(int.MinValue);
            firstAction.GetBuildCapability(buildConfiguration, buildChain, type).Returns(firstCapability);
            secondAction.Priority.Returns(int.MaxValue);
            secondAction.GetBuildCapability(buildConfiguration, buildChain, type).Returns(secondCapability);

            var actions = new List<IBuildAction>
            {
                firstAction,
                secondAction
            };

            var sut = new BuildProcessor(actions);

            var actual = sut.GetBuildCapability(executeStrategy, BuildRequirement.Create, type);

            actual.Should().BeEquivalentTo(secondCapability);
        }

        [Fact]
        public void GetBuildCapabilityForTypeThrowsExceptionWithNullExecuteStrategy()
        {
            var actions = Array.Empty<IBuildAction>();
            var type = typeof(Person);

            var sut = new BuildProcessor(actions);

            Action action = () => sut.GetBuildCapability(null!, BuildRequirement.Create, type);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildCapabilityForTypeThrowsExceptionWithNullType()
        {
            var actions = Array.Empty<IBuildAction>();

            var buildConfiguration = Substitute.For<IBuildConfiguration>();
            var buildChain = Substitute.For<IBuildChain>();
            var executeStrategy = Substitute.For<IExecuteStrategy>();
            executeStrategy.Configuration.Returns(buildConfiguration);
            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new BuildProcessor(actions);

            Action action = () =>
                sut.GetBuildCapability(executeStrategy, BuildRequirement.Create, (Type)null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ThrowsExceptionWithNullActions()
        {
            // ReSharper disable once ObjectCreationAsStatement
            Action action = () => new BuildProcessor(null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}