namespace ModelBuilder.UnitTests.BuildSteps
{
    using System;
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using ModelBuilder.BuildSteps;
    using ModelBuilder.UnitTests.Models;
    using NSubstitute;
    using Xunit;

    public class CircularReferenceBuildStepTests
    {
        [Fact]
        public void BuildForParameterReturnsNullWhenNoMatchingTypeFound()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            buildChain.Push(Guid.NewGuid());
            buildChain.Push(DateTimeOffset.UtcNow);

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new CircularReferenceBuildStep();

            var actual = sut.Build(parameterInfo, executeStrategy);

            actual.Should().BeNull();
        }

        [Fact]
        public void BuildForParameterReturnsValueMatchingType()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var expected = Guid.NewGuid().ToString();

            buildChain.Push(Guid.NewGuid());
            buildChain.Push(expected);
            buildChain.Push(DateTimeOffset.UtcNow);

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new CircularReferenceBuildStep();

            var actual = sut.Build(parameterInfo, executeStrategy);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForParameterThrowsExceptionWithNullExecuteStrategy()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            var sut = new CircularReferenceBuildStep();

            Action action = () => sut.Build(parameterInfo, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForParameterThrowsExceptionWithNullType()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new CircularReferenceBuildStep();

            Action action = () => sut.Build((ParameterInfo) null, executeStrategy);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForPropertyReturnsNullWhenNoMatchingTypeFound()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));

            buildChain.Push(Guid.NewGuid());
            buildChain.Push(DateTimeOffset.UtcNow);

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new CircularReferenceBuildStep();

            var actual = sut.Build(propertyInfo, executeStrategy);

            actual.Should().BeNull();
        }

        [Fact]
        public void BuildForPropertyReturnsValueMatchingType()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var expected = Guid.NewGuid().ToString();

            buildChain.Push(Guid.NewGuid());
            buildChain.Push(expected);
            buildChain.Push(DateTimeOffset.UtcNow);

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new CircularReferenceBuildStep();

            var actual = sut.Build(propertyInfo, executeStrategy);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForPropertyThrowsExceptionWithNullExecuteStrategy()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));

            var sut = new CircularReferenceBuildStep();

            Action action = () => sut.Build(propertyInfo, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForPropertyThrowsExceptionWithNullType()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new CircularReferenceBuildStep();

            Action action = () => sut.Build((PropertyInfo) null, executeStrategy);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForTypeReturnsNullWhenNoMatchingTypeFound()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var type = typeof(string);

            buildChain.Push(Guid.NewGuid());
            buildChain.Push(DateTimeOffset.UtcNow);

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new CircularReferenceBuildStep();

            var actual = sut.Build(type, executeStrategy);

            actual.Should().BeNull();
        }

        [Fact]
        public void BuildForTypeReturnsValueMatchingType()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var type = typeof(string);
            var expected = Guid.NewGuid().ToString();

            buildChain.Push(Guid.NewGuid());
            buildChain.Push(expected);
            buildChain.Push(DateTimeOffset.UtcNow);

            var executeStrategy = Substitute.For<IExecuteStrategy>();

            executeStrategy.BuildChain.Returns(buildChain);

            var sut = new CircularReferenceBuildStep();

            var actual = sut.Build(type, executeStrategy);

            actual.Should().Be(expected);
        }

        [Fact]
        public void BuildForTypeThrowsExceptionWithNullExecuteStrategy()
        {
            var type = typeof(string);

            var sut = new CircularReferenceBuildStep();

            Action action = () => sut.Build(type, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void BuildForTypeThrowsExceptionWithNullType()
        {
            var executeStrategy = Substitute.For<IExecuteStrategy>();

            var sut = new CircularReferenceBuildStep();

            Action action = () => sut.Build((Type) null, executeStrategy);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForParameterReturnsFalseWhenBuildChainDoesNotContainMatchingType()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            buildChain.Push(Guid.NewGuid());

            var sut = new CircularReferenceBuildStep();

            var actual = sut.IsMatch(parameterInfo, buildConfiguration, buildChain);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchForParameterReturnsFalseWhenBuildChainIsEmpty()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();

            var sut = new CircularReferenceBuildStep();

            var actual = sut.IsMatch(parameterInfo, buildConfiguration, buildChain);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchForParameterReturnsTrueWhenBuildChainContainsMatchingType()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            buildChain.Push(Guid.NewGuid());
            buildChain.Push(Guid.NewGuid().ToString());
            buildChain.Push(DateTimeOffset.UtcNow);

            var sut = new CircularReferenceBuildStep();

            var actual = sut.IsMatch(parameterInfo, buildConfiguration, buildChain);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsMatchForParameterThrowsExceptionWithNullBuildChain()
        {
            var parameterInfo = typeof(Person).GetConstructors()
                .First(x => x.GetParameters().FirstOrDefault()?.Name == "firstName").GetParameters().First();
            var buildConfiguration = new BuildConfiguration();

            var sut = new CircularReferenceBuildStep();

            Action action = () => sut.IsMatch(parameterInfo, buildConfiguration, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForParameterThrowsExceptionWithNullType()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var sut = new CircularReferenceBuildStep();

            Action action = () => sut.IsMatch((ParameterInfo) null, buildConfiguration, buildChain);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForPropertyReturnsFalseWhenBuildChainDoesNotContainMatchingType()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));

            buildChain.Push(Guid.NewGuid());

            var sut = new CircularReferenceBuildStep();

            var actual = sut.IsMatch(propertyInfo, buildConfiguration, buildChain);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchForPropertyReturnsFalseWhenBuildChainIsEmpty()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));

            var sut = new CircularReferenceBuildStep();

            var actual = sut.IsMatch(propertyInfo, buildConfiguration, buildChain);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchForPropertyReturnsTrueWhenBuildChainContainsMatchingType()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));

            buildChain.Push(Guid.NewGuid());
            buildChain.Push(Guid.NewGuid().ToString());
            buildChain.Push(DateTimeOffset.UtcNow);

            var sut = new CircularReferenceBuildStep();

            var actual = sut.IsMatch(propertyInfo, buildConfiguration, buildChain);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsMatchForPropertyThrowsExceptionWithNullBuildChain()
        {
            var propertyInfo = typeof(Person).GetProperty(nameof(Person.FirstName));
            var buildConfiguration = new BuildConfiguration();

            var sut = new CircularReferenceBuildStep();

            Action action = () => sut.IsMatch(propertyInfo, buildConfiguration, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForPropertyThrowsExceptionWithNullType()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var sut = new CircularReferenceBuildStep();

            Action action = () => sut.IsMatch((PropertyInfo) null, buildConfiguration, buildChain);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForTypeReturnsFalseWhenBuildChainDoesNotContainMatchingType()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var type = typeof(string);

            buildChain.Push(Guid.NewGuid());

            var sut = new CircularReferenceBuildStep();

            var actual = sut.IsMatch(type, buildConfiguration, buildChain);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchForTypeReturnsFalseWhenBuildChainIsEmpty()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var type = typeof(string);

            var sut = new CircularReferenceBuildStep();

            var actual = sut.IsMatch(type, buildConfiguration, buildChain);

            actual.Should().BeFalse();
        }

        [Fact]
        public void IsMatchForTypeReturnsTrueWhenBuildChainContainsMatchingType()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();
            var type = typeof(string);

            buildChain.Push(Guid.NewGuid());
            buildChain.Push(Guid.NewGuid().ToString());
            buildChain.Push(DateTimeOffset.UtcNow);

            var sut = new CircularReferenceBuildStep();

            var actual = sut.IsMatch(type, buildConfiguration, buildChain);

            actual.Should().BeTrue();
        }

        [Fact]
        public void IsMatchForTypeThrowsExceptionWithNullBuildChain()
        {
            var type = typeof(string);
            var buildConfiguration = new BuildConfiguration();

            var sut = new CircularReferenceBuildStep();

            Action action = () => sut.IsMatch(type, buildConfiguration, null);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IsMatchForTypeThrowsExceptionWithNullType()
        {
            var buildConfiguration = new BuildConfiguration();
            var buildChain = new BuildHistory();

            var sut = new CircularReferenceBuildStep();

            Action action = () => sut.IsMatch((Type) null, buildConfiguration, buildChain);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void PriorityReturnsMaximumValue()
        {
            var sut = new CircularReferenceBuildStep();

            sut.Priority.Should().Be(int.MaxValue);
        }
    }
}