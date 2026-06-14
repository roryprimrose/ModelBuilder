namespace ModelBuilder.UnitTests.vNext
{
    using System;
    using FluentAssertions;
    using ModelBuilder;
    using Xunit;

    public class BuildContextSiblingTests
    {
        [Fact]
        public void GetSiblingReturnsDefaultWhenNoScopeOpen()
        {
            var sut = new BuildContext(new RandomSource(1));

            sut.GetSibling<string>("FirstName").Should().BeNull();
        }

        [Fact]
        public void GetSiblingReturnsDefaultWhenMemberNotRecorded()
        {
            var sut = new BuildContext(new RandomSource(1));

            using (sut.EnterSiblingScope())
            {
                sut.GetSibling<string>("Missing").Should().BeNull();
            }
        }

        [Fact]
        public void GetSiblingReturnsRecordedValueWithinScope()
        {
            var sut = new BuildContext(new RandomSource(1));

            using (sut.EnterSiblingScope())
            {
                sut.RecordSibling("FirstName", "Janet");

                sut.GetSibling<string>("FirstName").Should().Be("Janet");
            }
        }

        [Fact]
        public void GetSiblingReturnsDefaultForWrongType()
        {
            var sut = new BuildContext(new RandomSource(1));

            using (sut.EnterSiblingScope())
            {
                sut.RecordSibling("Age", 42);

                sut.GetSibling<string>("Age").Should().BeNull();
            }
        }

        [Fact]
        public void GetSiblingWithAliasesReturnsFirstRecordedMatch()
        {
            var sut = new BuildContext(new RandomSource(1));

            using (sut.EnterSiblingScope())
            {
                sut.RecordSibling("GivenName", "Janet");

                // FirstName is not recorded, so the alias lookup falls through to GivenName.
                sut.GetSibling<string>("FirstName", "GivenName").Should().Be("Janet");
            }
        }

        [Fact]
        public void GetSiblingWithAliasesPrefersEarlierNameWhenBothRecorded()
        {
            var sut = new BuildContext(new RandomSource(1));

            using (sut.EnterSiblingScope())
            {
                sut.RecordSibling("FirstName", "Janet");
                sut.RecordSibling("GivenName", "Other");

                sut.GetSibling<string>("FirstName", "GivenName").Should().Be("Janet");
            }
        }

        [Fact]
        public void GetSiblingWithAliasesReturnsDefaultWhenNoneRecorded()
        {
            var sut = new BuildContext(new RandomSource(1));

            using (sut.EnterSiblingScope())
            {
                sut.RecordSibling("Age", 42);

                sut.GetSibling<string>("FirstName", "GivenName").Should().BeNull();
            }
        }

        [Fact]
        public void NestedScopeDoesNotLeakToOuterScope()
        {
            var sut = new BuildContext(new RandomSource(1));

            using (sut.EnterSiblingScope())
            {
                sut.RecordSibling("Outer", "outer");

                using (sut.EnterSiblingScope())
                {
                    sut.RecordSibling("Inner", "inner");

                    sut.GetSibling<string>("Inner").Should().Be("inner");
                    sut.GetSibling<string>("Outer").Should().BeNull();
                }

                sut.GetSibling<string>("Outer").Should().Be("outer");
                sut.GetSibling<string>("Inner").Should().BeNull();
            }
        }

        [Fact]
        public void RecordSiblingIsNoOpWhenNoScopeOpen()
        {
            var sut = new BuildContext(new RandomSource(1));

            Action action = () => sut.RecordSibling("FirstName", "Janet");

            action.Should().NotThrow();
        }

        [Fact]
        public void GetSiblingThrowsWithNullMemberName()
        {
            var sut = new BuildContext(new RandomSource(1));

            Action action = () => sut.GetSibling<string>((string)null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void GetSiblingThrowsWithNullMemberNames()
        {
            var sut = new BuildContext(new RandomSource(1));

            Action action = () => sut.GetSibling<string>((string[])null!);

            action.Should().Throw<ArgumentNullException>();
        }
    }
}
