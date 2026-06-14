namespace ModelBuilder.UnitTests.vNext
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using ModelBuilder;
    using Xunit;

    public class ModelBuildExceptionTests
    {
        [Fact]
        public void DefaultConstructorSetsUnspecifiedFailureKind()
        {
            var sut = new ModelBuildException();

            sut.FailureKind.Should().Be(FailureKind.Unspecified);
            sut.BuildPath.Should().BeEmpty();
            sut.BuildLog.Should().BeEmpty();
        }

        [Fact]
        public void FullConstructorExposesStructuredPayload()
        {
            var path = new List<BuildFrame>
            {
                new BuildFrame(typeof(Uri), null, typeof(Uri)),
                new BuildFrame(typeof(Uri), "Host", typeof(string))
            };
            var inner = new InvalidOperationException("boom");

            var sut = new ModelBuildException(
                "Failed",
                FailureKind.NoValueSource,
                typeof(string),
                "Host",
                path,
                Array.Empty<BuildLogEntry>(),
                inner);

            sut.Message.Should().Be("Failed");
            sut.FailureKind.Should().Be(FailureKind.NoValueSource);
            sut.TargetType.Should().Be(typeof(string));
            sut.TargetMember.Should().Be("Host");
            sut.BuildPath.Should().HaveCount(2);
            sut.InnerException.Should().BeSameAs(inner);
        }

        [Fact]
        public void MessageConstructorSetsMessage()
        {
            var sut = new ModelBuildException("Something failed");

            sut.Message.Should().Be("Something failed");
        }

        [Fact]
        public void MessageWithFailureKindConstructorSetsBoth()
        {
            var sut = new ModelBuildException("No builder", FailureKind.NoBuilderForType);

            sut.Message.Should().Be("No builder");
            sut.FailureKind.Should().Be(FailureKind.NoBuilderForType);
        }
    }
}
