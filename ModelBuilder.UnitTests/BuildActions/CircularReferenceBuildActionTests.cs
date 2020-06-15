namespace ModelBuilder.UnitTests.BuildActions
{
    using System;
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using ModelBuilder.BuildActions;
    using ModelBuilder.UnitTests.Models;
    using NSubstitute;
    using Xunit;

    public class CircularReferenceBuildActionTests
    {
        [Fact]
        public void BuildForParameterReturnsNullWhenNoMatchingTypeFound()
        {
            var buildChain = new BuildHistory();
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            buildChain.Push(Guid.NewGuid());
            buildChain.Push(DateTimeOffset.UtcNow);

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new CircularReferenceBuildAction();

            var actual = sut.Build(executeStrategy, parameterInfo);

            actual.Should().BeNull();
        }

        [Fact]
        public void BuildForParameterReturnsValueMatchingType()
        {
            var buildChain = new BuildHistory();
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var expected = Guid.NewGuid().ToString();

            buildChain.Push(Guid.NewGuid());
            buildChain.Push(expected);
            buildChain.Push(DateTimeOffset.UtcNow);

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new CircularReferenceBuildAction();

            var actual = sut.Build(executeStrategy, parameterInfo);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForParameterThrowsExceptionWithNullExecuteStrategy()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            var sut = new CircularReferenceBuildAction();

            Action action = () => sut.Build(null!, parameterInfo);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForParameterThrowsExceptionWithNullType()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new CircularReferenceBuildAction();

            Action action = () => sut.Build(executeStrategy, (ParameterInfo) null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForPropertyReturnsNullWhenNoMatchingTypeFound()
        {
            var buildChain = new BuildHistory();
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName))!;

            buildChain.Push(Guid.NewGuid());
            buildChain.Push(DateTimeOffset.UtcNow);

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new CircularReferenceBuildAction();

            var actual = sut.Build(executeStrategy, propertyInfo);

            actual.Should().BeNull();
        }

        [Fact]
        public void BuildForPropertyReturnsValueMatchingType()
        {
            var buildChain = new BuildHistory();
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName))!;
            var expected = Guid.NewGuid().ToString();

            buildChain.Push(Guid.NewGuid());
            buildChain.Push(expected);
            buildChain.Push(DateTimeOffset.UtcNow);

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new CircularReferenceBuildAction();

            var actual = sut.Build(executeStrategy, propertyInfo);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForPropertyThrowsExceptionWithNullExecuteStrategy()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName))!;

            var sut = new CircularReferenceBuildAction();

            Action action = () => sut.Build(null!, propertyInfo);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForPropertyThrowsExceptionWithNullType()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new CircularReferenceBuildAction();

            Action action = () => sut.Build(executeStrategy, (PropertyInfo) null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForTypeReturnsNullWhenNoMatchingTypeFound()
        {
            var buildChain = new BuildHistory();
            var type = typeof(string);

            buildChain.Push(Guid.NewGuid());
            buildChain.Push(DateTimeOffset.UtcNow);

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new CircularReferenceBuildAction();

            var actual = sut.Build(executeStrategy, type);

            actual.Should().BeNull();
        }

        [Fact]
        public void BuildForTypeReturnsValueMatchingType()
        {
            var buildChain = new BuildHistory();
            var type = typeof(string);
            var expected = Guid.NewGuid().ToString();

            buildChain.Push(Guid.NewGuid());
            buildChain.Push(expected);
            buildChain.Push(DateTimeOffset.UtcNow);

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new CircularReferenceBuildAction();

            var actual = sut.Build(executeStrategy, type);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForTypeThrowsExceptionWithNullExecuteStrategy()
        {
            var type = typeof(string);

            var sut = new CircularReferenceBuildAction();

            Action action = () => sut.Build(null!, type);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForTypeThrowsExceptionWithNullType()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new CircularReferenceBuildAction();

            Action action = () => sut.Build(executeStrategy, (Type) null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildCapabilityForParameterReturnsCapabilityWhenBuildChainContainsMatchingType()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            buildChain.Push(Guid.NewGuid());
            buildChain.Push(Guid.NewGuid().ToString());
            buildChain.Push(DateTimeOffset.UtcNow);

            var sut = new CircularReferenceBuildAction();

            var actual = sut.GetBuildCapability(buildConfiguration, buildChain, parameterInfo)!;

            actual.Should().NotBeNull();
            actual.SupportsCreate.Should().BeTrue();
            actual.SupportsPopulate.Should().BeFalse();
            actual.AutoPopulate.Should().BeFalse();
            actual.ImplementedByType.Should().BeAssignableTo<IBuildCapability>();
        }

        [Fact]
        public void GetBuildCapabilityForParameterReturnsNullWhenBuildChainDoesNotContainMatchingType()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            buildChain.Push(Guid.NewGuid());

            var sut = new CircularReferenceBuildAction();

            var actual = sut.GetBuildCapability(buildConfiguration, buildChain, parameterInfo);

            actual.Should().BeNull();
        }

        [Fact]
        public void GetBuildCapabilityForParameterReturnsNullWhenBuildChainIsEmpty()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            var sut = new CircularReferenceBuildAction();

            var actual = sut.GetBuildCapability(buildConfiguration, buildChain, parameterInfo);

            actual.Should().BeNull();
        }

        [Fact]
        public void GetBuildCapabilityForParameterThrowsExceptionWithNullBuildChain()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildConfiguration = new BuildConfiguration();

            var sut = new CircularReferenceBuildAction();

            Action action = () => sut.GetBuildCapability(buildConfiguration, null!, parameterInfo);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildCapabilityForParameterThrowsExceptionWithNullType()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var sut = new CircularReferenceBuildAction();

            Action action = () => sut.GetBuildCapability(buildConfiguration, buildChain, (ParameterInfo) null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildCapabilityForPropertyReturnsCapabilityWhenBuildChainContainsMatchingType()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName))!;

            buildChain.Push(Guid.NewGuid());
            buildChain.Push(Guid.NewGuid().ToString());
            buildChain.Push(DateTimeOffset.UtcNow);

            var sut = new CircularReferenceBuildAction();

            var actual = sut.GetBuildCapability(buildConfiguration, buildChain, propertyInfo);

            actual.Should().NotBeNull();
            actual!.SupportsCreate.Should().BeTrue();
            actual.SupportsPopulate.Should().BeFalse();
            actual.AutoPopulate.Should().BeFalse();
            actual.ImplementedByType.Should().BeAssignableTo<IBuildCapability>();
        }

        [Fact]
        public void GetBuildCapabilityForPropertyReturnsNullWhenBuildChainDoesNotContainMatchingType()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName))!;

            buildChain.Push(Guid.NewGuid());

            var sut = new CircularReferenceBuildAction();

            var actual = sut.GetBuildCapability(buildConfiguration, buildChain, propertyInfo);

            actual.Should().BeNull();
        }

        [Fact]
        public void GetBuildCapabilityForPropertyReturnsNullWhenBuildChainIsEmpty()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName))!;

            var sut = new CircularReferenceBuildAction();

            var actual = sut.GetBuildCapability(buildConfiguration, buildChain, propertyInfo);

            actual.Should().BeNull();
        }

        [Fact]
        public void GetBuildCapabilityForPropertyThrowsExceptionWithNullBuildChain()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName))!;
            var buildConfiguration = new BuildConfiguration();

            var sut = new CircularReferenceBuildAction();

            Action action = () => sut.GetBuildCapability(buildConfiguration, null!, propertyInfo);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildCapabilityForPropertyThrowsExceptionWithNullType()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var sut = new CircularReferenceBuildAction();

            Action action = () => sut.GetBuildCapability(buildConfiguration, buildChain, (PropertyInfo) null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildCapabilityForTypeReturnsCapabilityWhenBuildChainContainsMatchingType()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var type = typeof(string);

            buildChain.Push(Guid.NewGuid());
            buildChain.Push(Guid.NewGuid().ToString());
            buildChain.Push(DateTimeOffset.UtcNow);

            var sut = new CircularReferenceBuildAction();

            var actual = sut.GetBuildCapability(buildConfiguration, buildChain, type);

            actual.Should().NotBeNull();
            actual!.SupportsCreate.Should().BeTrue();
            actual.SupportsPopulate.Should().BeFalse();
            actual.AutoPopulate.Should().BeFalse();
            actual.ImplementedByType.Should().BeAssignableTo<IBuildCapability>();
        }

        [Fact]
        public void GetBuildCapabilityForTypeReturnsNullWhenBuildChainDoesNotContainMatchingType()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var type = typeof(string);

            buildChain.Push(Guid.NewGuid());

            var sut = new CircularReferenceBuildAction();

            var actual = sut.GetBuildCapability(buildConfiguration, buildChain, type);

            actual.Should().BeNull();
        }

        [Fact]
        public void GetBuildCapabilityForTypeReturnsNullWhenBuildChainIsEmpty()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var type = typeof(string);

            var sut = new CircularReferenceBuildAction();

            var actual = sut.GetBuildCapability(buildConfiguration, buildChain, type);

            actual.Should().BeNull();
        }

        [Fact]
        public void GetBuildCapabilityForTypeThrowsExceptionWithNullBuildChain()
        {
            var type = typeof(string);
            var buildConfiguration = new BuildConfiguration();

            var sut = new CircularReferenceBuildAction();

            Action action = () => sut.GetBuildCapability(buildConfiguration, null!, type);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildCapabilityForTypeThrowsExceptionWithNullType()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var sut = new CircularReferenceBuildAction();

            Action action = () => sut.GetBuildCapability(buildConfiguration, buildChain, (Type) null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetBuildCapabilityReturnsCapabilityWithoutPopulateSupport()
        {
            var value = new Person();
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var type = typeof(string);

            buildChain.Push(Guid.NewGuid());
            buildChain.Push(Guid.NewGuid().ToString());
            buildChain.Push(DateTimeOffset.UtcNow);

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new CircularReferenceBuildAction();

            var actual = sut.GetBuildCapability(buildConfiguration, buildChain, type)!;

            Action action = () => actual.Populate(executeStrategy, value);

            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void PopulateThrowsException()
        {
            var sut = new CircularReferenceBuildAction();

            Action action = () => sut.Populate(null!, null!);

            action.Should().Throw<NotSupportedException>();
        }

        [Fact]
        public void PriorityReturnsMaximumValue()
        {
            var sut = new CircularReferenceBuildAction();

            sut.Priority.Should().Be(int.MaxValue);
        }
    }
}