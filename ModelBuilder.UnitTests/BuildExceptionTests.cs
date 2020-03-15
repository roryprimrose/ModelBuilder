namespace ModelBuilder.UnitTests
{
    using System;
    using FluentAssertions;
    using ModelBuilder.UnitTests.Models;
    using Xunit;

    public class BuildExceptionTests
    {
        [Fact]
        public void CanCreatesWithDefaultValues()
        {
            var target = new BuildException();

            target.BuildLog.Should().BeNull();
            target.Message.Should().NotBeNullOrWhiteSpace();
            target.InnerException.Should().BeNull();
            target.TargetType.Should().BeNull();
            target.Context.Should().BeNull();
        }

        [Fact]
        public void CanCreateWithBuildInformationAndInnerException()
        {
            var message = Guid.NewGuid().ToString();
            var targetType = typeof(Person);
            var referenceName = Guid.NewGuid().ToString();
            var context = new Company();
            var buildLog = Guid.NewGuid().ToString();
            var inner = new TimeoutException();

            var target = new BuildException(message, targetType, referenceName, context, buildLog, inner);

            target.Message.Should().Be(message);
            target.TargetType.Should().Be(targetType);
            target.ReferenceName.Should().Be(referenceName);
            target.Context.Should().Be(context);
            target.BuildLog.Should().Be(buildLog);
            target.InnerException.Should().Be(inner);
        }

        [Fact]
        public void CanCreateWithBuildInformation()
        {
            var message = Guid.NewGuid().ToString();
            var targetType = typeof(Person);
            var referenceName = Guid.NewGuid().ToString();
            var context = new Company();
            var buildLog = Guid.NewGuid().ToString();

            var target = new BuildException(message, targetType, referenceName, context, buildLog);

            target.Message.Should().Be(message);
            target.TargetType.Should().Be(targetType);
            target.ReferenceName.Should().Be(referenceName);
            target.Context.Should().Be(context);
            target.BuildLog.Should().Be(buildLog);
            target.InnerException.Should().BeNull();
        }

        [Fact]
        public void CanCreateWithMessageAndException()
        {
            var message = Guid.NewGuid().ToString();
            var inner = new TimeoutException();

            var target = new BuildException(message, inner);

            target.Message.Should().Be(message);
            target.InnerException.Should().Be(inner);
        }

        [Fact]
        public void CanCreateWithMessage()
        {
            var message = Guid.NewGuid().ToString();

            var target = new BuildException(message);

            target.Message.Should().Be(message);
        }
    }
}