namespace ModelBuilder.IntegrationTests
{
    using FluentAssertions;
    using ModelBuilder.IntegrationTests.Models;
    using Xunit;

    /// <summary>
    ///     End-to-end test that <c>Model.WriteLog</c> captures a non-empty, structured build log
    ///     describing a nested build.
    /// </summary>
    public class LoggingTests
    {
        [Fact]
        public void WriteLogCapturesRenderedBuildLogForNestedGraph()
        {
            string? capturedLog = null;

            var actual = Model.WriteLog(log => capturedLog = log).Create<Invoice>();

            actual.Should().NotBeNull();
            capturedLog.Should().NotBeNullOrWhiteSpace();

            // The rendered log should mention the root type and at least one nested member name.
            capturedLog.Should().Contain(nameof(Invoice));
            capturedLog.Should().Contain(nameof(Invoice.Reference));
        }
    }
}
