namespace ModelBuilder.IntegrationTests
{
    using FluentAssertions;
    using ModelBuilder.IntegrationTests.Models;
    using Xunit;

    /// <summary>
    ///     End-to-end tests of struct roots and struct-typed members, including nullable structs.
    /// </summary>
    public class StructTests
    {
        [Fact]
        public void CreateBuildsStructRootDirectly()
        {
            var actual = Model.Create<PointStruct>();

            // A struct root is a value-source root like any primitive; either coordinate being
            // non-default is enough to prove real values were generated rather than default(T).
            (actual.X != 0 || actual.Y != 0).Should().BeTrue();
        }

        [Fact]
        public void CreatePopulatesStructMember()
        {
            var actual = Model.Create<ShapeProfile>();

            actual.Label.Should().NotBeNullOrWhiteSpace();
            (actual.Origin.X != 0 || actual.Origin.Y != 0).Should().BeTrue();
        }

        [Fact]
        public void CreatePopulatesNullableStructMemberWhenNullPercentageIsZero()
        {
            var actual = Model.SetOptions(x => x.NullPercentage = 0).Create<ShapeProfile>();

            actual.Bounds.Should().NotBeNull();
        }

        [Fact]
        public void CreateLeavesNullableStructMemberNullWhenNullPercentageIsMaxed()
        {
            var actual = Model.SetOptions(x => x.NullPercentage = 100).Create<ShapeProfile>();

            actual.Bounds.Should().BeNull();
        }
    }
}
