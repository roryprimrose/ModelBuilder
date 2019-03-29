#if NET452
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
#endif

namespace ModelBuilder.UnitTests
{
    using System;
    using FluentAssertions;
    using ModelBuilder.UnitTests.Models;
    using Xunit;

    public class BuildExceptionTests
    {
        [Fact]
        public void CanCreatesWithDefaultValuesTest()
        {
            var target = new BuildException();

            target.BuildLog.Should().BeNull();
            target.Message.Should().NotBeNullOrWhiteSpace();
            target.InnerException.Should().BeNull();
            target.TargetType.Should().BeNull();
            target.Context.Should().BeNull();
        }

        [Fact]
        public void CanCreateWithBuildInformationAndInnerExceptionTest()
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
        public void CanCreateWithBuildInformationTest()
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
        public void CanCreateWithMessageAndExceptionTest()
        {
            var message = Guid.NewGuid().ToString();
            var inner = new TimeoutException();

            var target = new BuildException(message, inner);

            target.Message.Should().Be(message);
            target.InnerException.Should().Be(inner);
        }

        [Fact]
        public void CanCreateWithMessageTest()
        {
            var message = Guid.NewGuid().ToString();

            var target = new BuildException(message);

            target.Message.Should().Be(message);
        }

#if NET452
        [Fact]
        public void CanBeSerializedAndDeserializedTest()
        {
            var message = Guid.NewGuid().ToString();
            var targetType = typeof(Person);
            var referenceName = Guid.NewGuid().ToString();
            var context = new Company();
            var buildLog = Guid.NewGuid().ToString();
            var inner = new TimeoutException();
            var formatter = new BinaryFormatter();

            var target = new BuildException(message, targetType, referenceName, context, buildLog, inner);

            using (var ms = new MemoryStream())
            {
                formatter.Serialize(ms, target);
                ms.Seek(0, SeekOrigin.Begin);

                var outputException = formatter.Deserialize(ms) as BuildException;

                outputException.Message.Should().Be(message);
                outputException.TargetType.Should().Be(targetType);
                outputException.ReferenceName.Should().Be(referenceName);
                outputException.Context.Should().BeNull();
                outputException.BuildLog.Should().Be(buildLog);
                outputException.InnerException.Message.Should().BeEquivalentTo(inner.Message);
            }
        }

        [Fact]
        public void GetObjectDataThrowsExceptionWithNullInfoTest()
        {
            var streamingContext = new StreamingContext();

            var target = new BuildException();

            Action action = () => { target.GetObjectData(null, streamingContext); };

            action.Should().Throw<ArgumentNullException>();
        }
#endif
    }
}