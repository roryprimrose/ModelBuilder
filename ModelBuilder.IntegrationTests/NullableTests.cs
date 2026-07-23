namespace ModelBuilder.IntegrationTests
{
    using FluentAssertions;
    using ModelBuilder.IntegrationTests.Models;
    using Xunit;

    /// <summary>
    ///     End-to-end tests of nullable value/reference type members under both extremes of
    ///     <see cref="ModelBuilder.BuildOptions.NullPercentage" />.
    /// </summary>
    public class NullableTests
    {
        [Fact]
        public void CreatePopulatesAllNullableMembersWhenNullPercentageIsZero()
        {
            var actual = Model.SetOptions(x => x.NullPercentage = 0).Create<NullableProfile>();

            actual.NullableInt.Should().NotBeNull();
            actual.NullableDate.Should().NotBeNull();
            actual.NullableGuid.Should().NotBeNull();
            actual.NullableBool.Should().NotBeNull();
            actual.NullableColour.Should().NotBeNull();
            actual.NullableText.Should().NotBeNull();
            actual.NonNullableInt.Should().NotBe(0);
            actual.NonNullableText.Should().NotBeNullOrWhiteSpace();
        }

        [Fact]
        public void CreateLeavesAllNullableMembersNullWhenNullPercentageIsMaxed()
        {
            var actual = Model.SetOptions(x => x.NullPercentage = 100).Create<NullableProfile>();

            actual.NullableInt.Should().BeNull();
            actual.NullableDate.Should().BeNull();
            actual.NullableGuid.Should().BeNull();
            actual.NullableBool.Should().BeNull();
            actual.NullableColour.Should().BeNull();

            // NullPercentage only governs Nullable<T> value types. A nullable-annotated reference type
            // (string?) has no distinct runtime representation from a non-nullable string, so it
            // continues to build a real value regardless of NullPercentage, exactly like NonNullableText.
            actual.NullableText.Should().NotBeNull();
            actual.NonNullableText.Should().NotBeNullOrWhiteSpace();
        }
    }
}
