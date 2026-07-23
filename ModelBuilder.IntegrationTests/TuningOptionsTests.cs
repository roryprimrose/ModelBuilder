namespace ModelBuilder.IntegrationTests
{
    using System.Collections.Generic;
    using FluentAssertions;
    using ModelBuilder.IntegrationTests.Models;
    using Xunit;

    /// <summary>
    ///     End-to-end tests of the tuning options exposed on <see cref="BuildOptions" />: collection size
    ///     bounds, maximum graph depth, and applying a module that sets options centrally.
    /// </summary>
    public class TuningOptionsTests
    {
        [Fact]
        public void SetOptionsBoundsCollectionSizeBetweenMinAndMax()
        {
            var actual = Model.SetOptions(x =>
                {
                    x.MinCount = 2;
                    x.MaxCount = 4;
                })
                .Create<List<Widget>>();

            actual.Count.Should().BeGreaterThanOrEqualTo(2);
            actual.Count.Should().BeLessThanOrEqualTo(4);
        }

        [Fact]
        public void SetOptionsMaxDepthTerminatesSelfReferencingChainAtConfiguredDepth()
        {
            var actual = Model.SetOptions(x => x.MaxDepth = 2).Create<GraphNode>();

            var current = actual;
            var hops = 0;

            while (current!.Next != null && hops < 10)
            {
                current = current.Next;
                hops++;
            }

            hops.Should().BeLessThan(10);
        }

        [Fact]
        public void ConfigurationModuleAppliesMaxCountAcrossBuild()
        {
            var actual = Model.UsingModule<IntegrationTestModule>().Create<CollectionsProfile>();

            actual.ListItems.Count.Should().BeLessThanOrEqualTo(3);
        }
    }
}
