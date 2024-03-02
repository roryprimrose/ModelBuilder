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
            var sut = new BuildException();

            sut.BuildLog.Should().BeNull();
            sut.Message.Should().NotBeNullOrWhiteSpace();
            sut.InnerException.Should().BeNull();
            sut.TargetType.Should().BeNull();
            sut.Context.Should().BeNull();
        }

        [Fact]
        public void CanCreateWithBuildInformation()
        {
            var message = Guid.NewGuid().ToString();
            var targetType = typeof(Person);
            var referenceName = Guid.NewGuid().ToString();
            var context = new Company();
            var buildLog = Guid.NewGuid().ToString();

            var sut = new BuildException(message, targetType, referenceName, context, buildLog);

            sut.Message.Should().Be(message);
            sut.TargetType.Should().Be(targetType);
            sut.ReferenceName.Should().Be(referenceName);
            sut.Context.Should().Be(context);
            sut.BuildLog.Should().Be(buildLog);
            sut.InnerException.Should().BeNull();
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

            var sut = new BuildException(message, targetType, referenceName, context, buildLog, inner);

            sut.Message.Should().Be(message);
            sut.TargetType.Should().Be(targetType);
            sut.ReferenceName.Should().Be(referenceName);
            sut.Context.Should().Be(context);
            sut.BuildLog.Should().Be(buildLog);
            sut.InnerException.Should().Be(inner);
        }

        [Fact]
        public void CanCreateWithMessage()
        {
            var message = Guid.NewGuid().ToString();

            var sut = new BuildException(message);

            sut.Message.Should().Be(message);
        }

        [Fact]
        public void CanCreateWithMessageAndException()
        {
            var message = Guid.NewGuid().ToString();
            var inner = new TimeoutException();

            var sut = new BuildException(message, inner);

            sut.Message.Should().Be(message);
            sut.InnerException.Should().Be(inner);
        }
    }
}