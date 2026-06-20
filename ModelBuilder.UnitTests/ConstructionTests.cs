namespace ModelBuilder.UnitTests
{
    using System;
    using FluentAssertions;
    using ModelBuilder;
    using Xunit;

    public class ConstructionTests
    {
        [Fact]
        public void ExecuteReturnsValueProducedByBuildCallback()
        {
            var expected = new Sample();

            var actual = Model.Construct<Sample>().Execute(_ => expected);

            actual.Should().BeSameAs(expected);
        }

        [Fact]
        public void ExecutePassesBuildContextToCallback()
        {
            IBuildContext? captured = null;

            Model.Construct<Sample>().Execute(context =>
            {
                captured = context;

                return new Sample();
            });

            captured.Should().NotBeNull();
        }

        [Fact]
        public void ExecuteThrowsWithNullBuild()
        {
            var sut = Model.Construct<Sample>();

            Action action = () => sut.Execute(null!);

            action.Should().Throw<ArgumentNullException>();
        }

        [Fact]
        public void ExecuteRendersLogToSinkWhenConfigured()
        {
            string? rendered = null;

            Model.WriteLog(log => rendered = log)
                .Construct<Sample>()
                .Execute(_ => new Sample());

            rendered.Should().NotBeNull();
        }

        [Fact]
        public void ExecuteUsesConfigurationFromFluentChain()
        {
            // A custom value source registered on the chain must be visible to the build context the
            // construction drives.
            IBuildContext? captured = null;

            Model.AddValueSource(new DelegateValueSource<string>(_ => "configured"))
                .Construct<Sample>()
                .Execute(context =>
                {
                    captured = context;

                    return new Sample();
                });

            captured!.Build<string>(typeof(Sample), "Anything").Should().Be("configured");
        }

        private sealed class Sample
        {
        }
    }
}
