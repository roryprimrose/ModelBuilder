namespace ModelBuilder.UnitTests
{
    using System;
    using FluentAssertions;
    using ModelBuilder;
    using Xunit;

    public class BuildContextConfigurationTests
    {
        [Fact]
        public void ConfigurationDefaultsToEmptyWhenNotSupplied()
        {
            var sut = new BuildContext(new ModelBuilder.RandomSource(1));

            sut.Configuration.Should().NotBeNull();
            sut.Configuration.TypeMappings.Should().BeEmpty();
        }

        [Fact]
        public void ShouldPopulateReturnsFalseForIgnoredMember()
        {
            var configuration = new BuildConfiguration();

            configuration.Ignore(typeof(Sample), "Secret");

            var sut = new BuildContext(new ModelBuilder.RandomSource(1), null, null, configuration);

            sut.ShouldPopulate(typeof(Sample), "Secret", typeof(string)).Should().BeFalse();
            sut.ShouldPopulate(typeof(Sample), "Name", typeof(string)).Should().BeTrue();
        }

        [Fact]
        public void ShouldPopulateReturnsTrueWhenNoConfiguration()
        {
            var sut = new BuildContext(new ModelBuilder.RandomSource(1));

            sut.ShouldPopulate(typeof(Sample), "Name", typeof(string)).Should().BeTrue();
        }

        [Fact]
        public void ShouldPopulateThrowsWithNullMemberName()
        {
            var sut = new BuildContext(new ModelBuilder.RandomSource(1));

            Action action = () => sut.ShouldPopulate(typeof(Sample), null!, typeof(string));

            action.Should().Throw<ArgumentNullException>();
        }

        private sealed class Sample
        {
        }
    }
}
