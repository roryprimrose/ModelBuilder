namespace ModelBuilder.IntegrationTests
{
    using System.Collections.Generic;
    using FluentAssertions;
    using ModelBuilder.IntegrationTests.Models;
    using Xunit;

    /// <summary>
    ///     End-to-end tests of the post-build helpers: <c>Set</c> for a single instance and
    ///     <c>SetEach</c> (plain and indexed) for a collection of built instances.
    /// </summary>
    public class PostBuildTests
    {
        [Fact]
        public void SetAppliesChangeAfterCreate()
        {
            var actual = Model.Create<Widget>().Set(x => x.Name = "Overridden");

            actual.Name.Should().Be("Overridden");
        }

        [Fact]
        public void SetEachAppliesChangeToEveryItemInList()
        {
            var actual = Model.SetOptions(x =>
                {
                    x.MinCount = 3;
                    x.MaxCount = 3;
                })
                .Create<List<Widget>>()
                .SetEach(x => x.Price = 1.5m);

            actual.Should().HaveCount(3);
            actual.Should().OnlyContain(w => w.Price == 1.5m);
        }

        [Fact]
        public void SetEachByIndexAppliesIndexDependentChangeToEveryItemInList()
        {
            var actual = Model.SetOptions(x =>
                {
                    x.MinCount = 3;
                    x.MaxCount = 3;
                })
                .Create<List<Widget>>()
                .SetEach((index, x) => x.Name = $"Widget-{index}");

            actual.Should().HaveCount(3);
            actual[0].Name.Should().Be("Widget-0");
            actual[1].Name.Should().Be("Widget-1");
            actual[2].Name.Should().Be("Widget-2");
        }
    }
}
