namespace ModelBuilder.UnitTests.vNext
{
    using System;
    using System.IO;
    using FluentAssertions;
    using ModelBuilder;
    using Xunit;

    public class ConfigurationModuleTests
    {
        [Fact]
        public void ModuleAppliesMappingsAndIgnoreRules()
        {
            var configuration = new BuildConfiguration();
            IConfigurationModule sut = new SampleModule();

            sut.Configure(configuration);

            configuration.TryGetMapping(typeof(Stream), out var target).Should().BeTrue();
            target.Should().Be(typeof(MemoryStream));
            configuration.ShouldIgnore(new MemberSignature(typeof(Uri), "Fragment", typeof(string))).Should().BeTrue();
        }

        private sealed class SampleModule : IConfigurationModule
        {
            public void Configure(IBuildConfiguration configuration)
            {
                configuration.AddMapping<Stream, MemoryStream>()
                    .IgnoreAny(member => member.Name == "Fragment");
            }
        }
    }
}
