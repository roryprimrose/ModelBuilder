namespace ModelBuilder.ParityTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Xunit;
    using V8 = ModelBuilder.Model;
    using VNext = ModelBuilder.vNext.Model;

    /// <summary>
    ///     Behavioural parity tests comparing the v8 reflection engine against the vNext source-generated
    ///     engine on the core contract: <c>Create&lt;T&gt;()</c> returns a fully-populated graph. These
    ///     assert structural parity (the same members are populated) rather than identical random values.
    /// </summary>
    public class CreateParityTests
    {
        [Fact]
        public void BothEnginesBuildCollectionMembers()
        {
            var v8 = V8.Create<CollectionPoco>();
            var vnext = VNext.Create<CollectionPoco>();

            v8.Tags.Should().NotBeNullOrEmpty();
            vnext.Tags.Should().NotBeNullOrEmpty();

            v8.Places.Should().NotBeNullOrEmpty();
            vnext.Places.Should().NotBeNullOrEmpty();

            // Nested objects inside the collection are populated by both engines.
            vnext.Places!.Should().OnlyContain(place => place != null);
        }

        [Fact]
        public void BothEnginesBuildNestedObjects()
        {
            var v8 = V8.Create<NestedPoco>();
            var vnext = VNext.Create<NestedPoco>();

            v8.Location.Should().NotBeNull();
            vnext.Location.Should().NotBeNull();

            v8.Location!.City.Should().NotBeNullOrEmpty();
            vnext.Location!.City.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void BothEnginesPopulateEnumToDefinedValue()
        {
            var v8 = V8.Create<EnumPoco>();
            var vnext = VNext.Create<EnumPoco>();

            Enum.IsDefined(typeof(Priority), v8.Priority).Should().BeTrue();
            Enum.IsDefined(typeof(Priority), vnext.Priority).Should().BeTrue();
        }

        [Fact]
        public void BothEnginesPopulateStringMembers()
        {
            var v8 = V8.Create<FlatPoco>();
            var vnext = VNext.Create<FlatPoco>();

            v8.Name.Should().NotBeNullOrEmpty();
            vnext.Name.Should().NotBeNullOrEmpty();

            v8.Description.Should().NotBeNullOrEmpty();
            vnext.Description.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void BothEnginesProduceNonNullInstance()
        {
            V8.Create<FlatPoco>().Should().NotBeNull();
            VNext.Create<FlatPoco>().Should().NotBeNull();
        }

        [Fact]
        public void VNextPopulatesSameReferenceMembersAsV8()
        {
            // The reference-typed members both engines always populate must match, so a consumer
            // migrating from v8 sees the same shape filled in.
            var v8Populated = ReferenceMembersPopulated(V8.Create<NestedPoco>());
            var vnextPopulated = ReferenceMembersPopulated(VNext.Create<NestedPoco>());

            vnextPopulated.Should().BeEquivalentTo(v8Populated);
        }

        private static IReadOnlyCollection<string> ReferenceMembersPopulated(NestedPoco instance)
        {
            var populated = new List<string>();

            if (string.IsNullOrEmpty(instance.Title) == false)
            {
                populated.Add(nameof(NestedPoco.Title));
            }

            if (instance.Location != null)
            {
                populated.Add(nameof(NestedPoco.Location));
            }

            return populated;
        }
    }
}
