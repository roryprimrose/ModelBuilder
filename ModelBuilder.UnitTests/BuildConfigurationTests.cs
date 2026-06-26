namespace ModelBuilder.UnitTests
{
    using System;
    using System.IO;
    using FluentAssertions;
    using ModelBuilder;
    using Xunit;

    public class BuildConfigurationTests
    {
        [Fact]
        public void AddMappingGenericRegistersMapping()
        {
            var sut = new BuildConfiguration();

            sut.AddMapping<Stream, MemoryStream>();

            sut.TryGetMapping(typeof(Stream), out var target).Should().BeTrue();
            target.Should().Be(typeof(MemoryStream));
        }

        [Fact]
        public void AddMappingReturnsSameInstanceForChaining()
        {
            var sut = new BuildConfiguration();

            var actual = sut.AddMapping(typeof(Stream), typeof(MemoryStream));

            actual.Should().BeSameAs(sut);
        }

        [Fact]
        public void AddMappingThrowsWithNullSourceType()
        {
            var sut = new BuildConfiguration();

            Action action = () => sut.AddMapping(null!, typeof(MemoryStream));

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void IgnoreAnyAppliesPredicateAcrossTypes()
        {
            var sut = new BuildConfiguration();

            sut.IgnoreAny(member => member.Name == "Description");

            sut.ShouldIgnore(new MemberSignature(typeof(Uri), "Description", typeof(string))).Should().BeTrue();
            sut.ShouldIgnore(new MemberSignature(typeof(Version), "Description", typeof(string))).Should().BeTrue();
            sut.ShouldIgnore(new MemberSignature(typeof(Uri), "Host", typeof(string))).Should().BeFalse();
        }

        [Fact]
        public void IgnoreRegistersTargetedRule()
        {
            var sut = new BuildConfiguration();

            sut.Ignore(typeof(Uri), "Host");

            sut.ShouldIgnore(new MemberSignature(typeof(Uri), "Host", typeof(string))).Should().BeTrue();
            sut.ShouldIgnore(new MemberSignature(typeof(Uri), "Port", typeof(int))).Should().BeFalse();
            sut.ShouldIgnore(new MemberSignature(typeof(Version), "Host", typeof(string))).Should().BeFalse();
        }

        [Fact]
        public void ShouldIgnoreReturnsFalseWhenNoRulesRegistered()
        {
            var sut = new BuildConfiguration();

            sut.ShouldIgnore(new MemberSignature(typeof(Uri), "Host", typeof(string))).Should().BeFalse();
        }

        [Fact]
        public void SetOptionsMutatesOptions()
        {
            var sut = new BuildConfiguration();

            sut.SetOptions(x =>
            {
                x.MinCount = 2;
                x.MaxCount = 5;
                x.NullPercentage = 0;
                x.MaxDepth = 12;
            });

            sut.Options.MinCount.Should().Be(2);
            sut.Options.MaxCount.Should().Be(5);
            sut.Options.NullPercentage.Should().Be(0);
            sut.Options.MaxDepth.Should().Be(12);
        }

        [Fact]
        public void SetOptionsReturnsSameInstanceForChaining()
        {
            var sut = new BuildConfiguration();

            var actual = sut.SetOptions(_ => { });

            actual.Should().BeSameAs(sut);
        }

        [Fact]
        public void SetOptionsThrowsWithNullConfigure()
        {
            var sut = new BuildConfiguration();

            Action action = () => sut.SetOptions(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void TryGetMappingReturnsFalseWhenNoMappingRegistered()
        {
            var sut = new BuildConfiguration();

            sut.TryGetMapping(typeof(Stream), out _).Should().BeFalse();
        }
    }
}
