namespace ModelBuilder.UnitTests
{
    using System;
    using FluentAssertions;
    using ModelBuilder;
    using Xunit;

    public class NullBuildLogTests
    {
        [Fact]
        public void BeginScopeReturnsReusableNoopToken()
        {
            var sut = NullBuildLog.Instance;

            using (sut.BeginScope(BuildLogEntryKind.CreateInstance, typeof(int)))
            {
                sut.Write(BuildLogEntryKind.CreateValue, typeof(int), "Value");
            }

            sut.Entries.Should().BeEmpty();
        }

        [Fact]
        public void InstanceReturnsSharedSingleton()
        {
            NullBuildLog.Instance.Should().BeSameAs(NullBuildLog.Instance);
        }

        [Fact]
        public void IsEnabledReturnsFalse()
        {
            NullBuildLog.Instance.IsEnabled.Should().BeFalse();
        }
    }
}
