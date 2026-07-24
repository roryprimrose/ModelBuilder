namespace ModelBuilder.UnitTests
{
    using System;
    using System.Collections.Generic;
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
        public void TryGetMappingConstructsClosedTargetFromOpenGenericMapping()
        {
            var sut = new BuildConfiguration();

            sut.AddMapping(typeof(IList<>), typeof(List<>));

            sut.TryGetMapping(typeof(IList<int>), out var target).Should().BeTrue();
            target.Should().Be(typeof(List<int>));
        }

        [Fact]
        public void TryGetMappingPrefersExactClosedMappingOverOpenGenericMapping()
        {
            var sut = new BuildConfiguration();

            sut.AddMapping(typeof(IList<>), typeof(List<>));
            sut.AddMapping(typeof(IList<int>), typeof(CustomIntList));

            sut.TryGetMapping(typeof(IList<int>), out var target).Should().BeTrue();
            target.Should().Be(typeof(CustomIntList));
        }

        [Fact]
        public void TryGetMappingReturnsRegisteredClosedTargetForOpenGenericSource()
        {
            var sut = new BuildConfiguration();

            sut.AddMapping(typeof(IList<>), typeof(CustomIntList));

            sut.TryGetMapping(typeof(IList<int>), out var target).Should().BeTrue();
            target.Should().Be(typeof(CustomIntList));
        }

        [Fact]
        public void TryGetMappingReturnsFalseWhenOpenGenericSourceHasNoMapping()
        {
            var sut = new BuildConfiguration();

            sut.TryGetMapping(typeof(IComparable<int>), out _).Should().BeFalse();
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
        public void ShouldIgnoreAppliesRuleFromBaseTypeToDerivedType()
        {
            var sut = new BuildConfiguration();

            sut.Ignore(typeof(Exception), "Message");

            sut.ShouldIgnore(new MemberSignature(typeof(InvalidOperationException), "Message", typeof(string))).Should().BeTrue();
        }

        [Fact]
        public void ShouldIgnoreAppliesRuleFromInterfaceToImplementingType()
        {
            var sut = new BuildConfiguration();

            sut.Ignore(typeof(IDisposable), "Tag");

            sut.ShouldIgnore(new MemberSignature(typeof(MemoryStream), "Tag", typeof(string))).Should().BeTrue();
        }

        [Fact]
        public void ShouldIgnoreDoesNotApplyRuleToUnrelatedType()
        {
            var sut = new BuildConfiguration();

            sut.Ignore(typeof(MemoryStream), "Length");

            sut.ShouldIgnore(new MemberSignature(typeof(Uri), "Length", typeof(int))).Should().BeFalse();
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

        [Theory]
        [InlineData(typeof(IReadOnlyDictionary<string, int>), typeof(Dictionary<string, int>))]
        [InlineData(typeof(IDictionary<string, int>), typeof(Dictionary<string, int>))]
        [InlineData(typeof(IReadOnlyList<int>), typeof(List<int>))]
        [InlineData(typeof(IReadOnlyCollection<int>), typeof(List<int>))]
        [InlineData(typeof(IList<int>), typeof(List<int>))]
        [InlineData(typeof(ICollection<int>), typeof(List<int>))]
        [InlineData(typeof(IEnumerable<int>), typeof(List<int>))]
        [InlineData(typeof(ISet<int>), typeof(HashSet<int>))]
        public void TryGetMappingResolvesBuiltInCollectionInterfaceMapping(Type source, Type expectedTarget)
        {
            var sut = new BuildConfiguration();

            sut.TryGetMapping(source, out var target).Should().BeTrue();
            target.Should().Be(expectedTarget);
        }

        [Fact]
        public void TryGetMappingPrefersUserMappingOverBuiltIn()
        {
            var sut = new BuildConfiguration();

            sut.AddMapping(typeof(IList<>), typeof(CustomIntList));

            sut.TryGetMapping(typeof(IList<int>), out var target).Should().BeTrue();
            target.Should().Be(typeof(CustomIntList));
        }

        private class CustomIntList : List<int>
        {
        }
    }
}
