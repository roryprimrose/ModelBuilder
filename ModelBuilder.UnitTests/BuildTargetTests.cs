namespace ModelBuilder.UnitTests
{
    using System;
    using FluentAssertions;
    using ModelBuilder;
    using Xunit;

    public class BuildTargetTests
    {
        [Fact]
        public void ConstructorStoresTypeAndMemberName()
        {
            var sut = new BuildTarget(typeof(string), "FirstName");

            sut.Type.Should().Be(typeof(string));
            sut.MemberName.Should().Be("FirstName");
        }

        [Fact]
        public void ConstructorThrowsWithNullType()
        {
            Action action = () => _ = new BuildTarget(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void MemberNameIsNullWhenNotProvided()
        {
            var sut = new BuildTarget(typeof(Guid));

            sut.MemberName.Should().BeNull();
        }
    }
}
