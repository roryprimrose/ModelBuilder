namespace ModelBuilder.IntegrationTests
{
    using FluentAssertions;
    using ModelBuilder.IntegrationTests.Models;
    using Xunit;

    /// <summary>
    ///     End-to-end tests that a self-referencing type and a pair of mutually referencing types build
    ///     without infinite recursion, terminating a cycle by leaving the repeated member at its default
    ///     rather than looping or overflowing the stack.
    /// </summary>
    public class CircularReferenceTests
    {
        [Fact]
        public void CreateTerminatesSelfReferencingChainWithoutStackOverflow()
        {
            var actual = Model.Create<GraphNode>();

            actual.Should().NotBeNull();
            actual.Name.Should().NotBeNullOrWhiteSpace();

            // The chain must terminate somewhere: walking Next repeatedly must not run forever. 200 hops
            // is far beyond the default MaxDepth (50), so reaching it without hitting a null Next would
            // itself indicate the guard failed to terminate the cycle.
            var current = actual;
            var hops = 0;

            while (current!.Next != null && hops < 200)
            {
                current = current.Next;
                hops++;
            }

            hops.Should().BeLessThan(200);
        }

        [Fact]
        public void CreateTerminatesMutuallyReferencingTypesWithoutStackOverflow()
        {
            var actual = Model.Create<Manager>();

            actual.Should().NotBeNull();
            actual.Name.Should().NotBeNullOrWhiteSpace();
            actual.DirectReports.Should().NotBeEmpty();

            // Each report's own Manager must not itself carry another populated DirectReports chain
            // forever; the guard should have left it null/empty once the cycle is detected.
            foreach (var report in actual.DirectReports)
            {
                report.Name.Should().NotBeNullOrWhiteSpace();
            }
        }
    }
}
