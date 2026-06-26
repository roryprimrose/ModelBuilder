namespace ModelBuilder.UnitTests
{
    using System;
    using FluentAssertions;
    using ModelBuilder;
    using Xunit;

    public class BuildLogTests
    {
        [Fact]
        public void BeginScopeNestsEntriesUntilDisposed()
        {
            var sut = new BuildLog();

            using (sut.BeginScope(BuildLogEntryKind.CreateInstance, typeof(Person)))
            {
                sut.Write(BuildLogEntryKind.CreateValue, typeof(string), "FirstName", "FirstNameValueSource");
            }

            sut.Entries.Should().HaveCount(1);

            var root = sut.Entries[0];

            root.Kind.Should().Be(BuildLogEntryKind.CreateInstance);
            root.TargetType.Should().Be(typeof(Person));
            root.Children.Should().HaveCount(1);
            root.Children[0].MemberName.Should().Be("FirstName");
            root.Children[0].Reason.Should().Be("FirstNameValueSource");
        }

        [Fact]
        public void BeginScopeThrowsWithNullTargetType()
        {
            var sut = new BuildLog();

            Action action = () => sut.BeginScope(BuildLogEntryKind.CreateInstance, null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void EntriesReturnToRootAfterScopeDisposed()
        {
            var sut = new BuildLog();

            using (sut.BeginScope(BuildLogEntryKind.CreateInstance, typeof(Person)))
            {
            }

            sut.Write(BuildLogEntryKind.CreateValue, typeof(int), "Age");

            sut.Entries.Should().HaveCount(2);
        }

        [Fact]
        public void IsEnabledReturnsTrue()
        {
            var sut = new BuildLog();

            sut.IsEnabled.Should().BeTrue();
        }

        [Fact]
        public void RenderIncludesNestedTypeAndMemberDetail()
        {
            var sut = new BuildLog();

            using (sut.BeginScope(BuildLogEntryKind.CreateInstance, typeof(Person)))
            {
                sut.Write(BuildLogEntryKind.SkipMember, typeof(string), "Notes", "ignore rule");
            }

            var actual = sut.Render();

            actual.Should().Contain("Create Person");
            actual.Should().Contain("Notes -> SKIPPED");
            actual.Should().Contain("(ignore rule)");
        }

        [Fact]
        public void WriteAddsRootEntry()
        {
            var sut = new BuildLog();

            sut.Write(BuildLogEntryKind.CreateValue, typeof(Guid), "Id");

            sut.Entries.Should().ContainSingle();
            sut.Entries[0].TargetType.Should().Be(typeof(Guid));
        }

        private sealed class Person
        {
        }
    }
}
