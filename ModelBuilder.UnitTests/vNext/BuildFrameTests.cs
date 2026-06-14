namespace ModelBuilder.UnitTests.vNext
{
    using System;
    using FluentAssertions;
    using ModelBuilder;
    using Xunit;

    public class BuildFrameTests
    {
        [Fact]
        public void ConstructorStoresValues()
        {
            var sut = new BuildFrame(typeof(Uri), "Endpoint", typeof(string));

            sut.DeclaringType.Should().Be(typeof(Uri));
            sut.MemberName.Should().Be("Endpoint");
            sut.MemberType.Should().Be(typeof(string));
        }

        [Fact]
        public void ConstructorThrowsWithNullDeclaringType()
        {
            Action action = () => _ = new BuildFrame(null!, "Member", typeof(int));

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ConstructorThrowsWithNullMemberType()
        {
            Action action = () => _ = new BuildFrame(typeof(int), "Member", null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void MemberNameIsNullForRootFrame()
        {
            var sut = new BuildFrame(typeof(int), null, typeof(int));

            sut.MemberName.Should().BeNull();
        }
    }
}
