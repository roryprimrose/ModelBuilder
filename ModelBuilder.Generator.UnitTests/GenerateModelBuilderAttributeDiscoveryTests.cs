namespace ModelBuilder.Generator.UnitTests
{
    using FluentAssertions;
    using Xunit;

    public class GenerateModelBuilderAttributeDiscoveryTests
    {
        [Fact]
        public void EmitsBuilderForTypeLevelAttributeRoot()
        {
            const string source = @"
namespace Sample
{
    public interface IManifest
    {
    }

    [ModelBuilder.GenerateModelBuilder]
    public sealed class Manifest : IManifest
    {
        public string? Name { get; set; }
    }
}";

            var harness = GeneratorTestHarness.Run(source);

            harness.GeneratorDiagnostics.Should().BeEmpty();
            harness.CompilationErrors.Should().BeEmpty();
            harness.GeneratedSources.Should().ContainSingle();
            harness.GeneratedSources[0].Should().Contain("Sample_ManifestBuilder");
        }

        [Fact]
        public void EmitsBuilderForAssemblyLevelAttributeRoot()
        {
            const string source = @"
[assembly: ModelBuilder.GenerateModelBuilder(typeof(Sample.Manifest))]

namespace Sample
{
    public interface IManifest
    {
    }

    public sealed class Manifest : IManifest
    {
        public string? Name { get; set; }
    }
}";

            var harness = GeneratorTestHarness.Run(source);

            harness.GeneratorDiagnostics.Should().BeEmpty();
            harness.CompilationErrors.Should().BeEmpty();
            harness.GeneratedSources.Should().ContainSingle();
            harness.GeneratedSources[0].Should().Contain("Sample_ManifestBuilder");
        }

        [Fact]
        public void EmitsBuildersForEveryTargetOfMultipleAssemblyLevelAttributes()
        {
            const string source = @"
[assembly: ModelBuilder.GenerateModelBuilder(typeof(Sample.First))]
[assembly: ModelBuilder.GenerateModelBuilder(typeof(Sample.Second))]

namespace Sample
{
    public sealed class First
    {
        public int Value { get; set; }
    }

    public sealed class Second
    {
        public int Value { get; set; }
    }
}";

            var harness = GeneratorTestHarness.Run(source);

            harness.GeneratorDiagnostics.Should().BeEmpty();
            harness.CompilationErrors.Should().BeEmpty();
            harness.GeneratedSources[0].Should().Contain("Sample_FirstBuilder");
            harness.GeneratedSources[0].Should().Contain("Sample_SecondBuilder");
        }

        [Fact]
        public void ReachableTypesFromAttributeRootAlsoGetBuilders()
        {
            const string source = @"
namespace Sample
{
    public sealed class Address
    {
        public string? City { get; set; }
    }

    [ModelBuilder.GenerateModelBuilder]
    public sealed class Manifest
    {
        public Address? Location { get; set; }
    }
}";

            var harness = GeneratorTestHarness.Run(source);

            harness.CompilationErrors.Should().BeEmpty();
            harness.GeneratedSources[0].Should().Contain("Sample_ManifestBuilder");
            harness.GeneratedSources[0].Should().Contain("Sample_AddressBuilder");
        }

        [Fact]
        public void ReportsMB1001ForAbstractTypeLevelAttributeRoot()
        {
            const string source = @"
namespace Sample
{
    [ModelBuilder.GenerateModelBuilder]
    public abstract class Shape
    {
        public int Sides { get; set; }
    }
}";

            var harness = GeneratorTestHarness.Run(source);

            harness.GeneratorDiagnostics.Should().Contain(d => d.Id == "MB1001");
        }

        [Fact]
        public void ReportsMB1002ForAssemblyLevelAttributeRootWithoutAccessibleConstructor()
        {
            const string source = @"
[assembly: ModelBuilder.GenerateModelBuilder(typeof(Sample.Locked))]

namespace Sample
{
    public sealed class Locked
    {
        private Locked() { }

        public int Value { get; set; }
    }
}";

            var harness = GeneratorTestHarness.Run(source);

            harness.GeneratorDiagnostics.Should().Contain(d => d.Id == "MB1002");
        }

        [Fact]
        public void DoesNotDuplicateBuilderWhenAttributeRootIsAlsoUsedInCreate()
        {
            const string source = @"
namespace Sample
{
    [ModelBuilder.GenerateModelBuilder]
    public sealed class Manifest
    {
        public string? Name { get; set; }
    }

    public static class Caller
    {
        public static Manifest Build() => global::ModelBuilder.Model.Create<Manifest>();
    }
}";

            var harness = GeneratorTestHarness.Run(source);

            harness.GeneratorDiagnostics.Should().BeEmpty();
            harness.CompilationErrors.Should().BeEmpty();
            harness.GeneratedSources.Should().ContainSingle();

            var occurrences = harness.GeneratedSources[0].Split("Sample_ManifestBuilder").Length - 1;

            occurrences.Should().BeGreaterThan(0);
        }
    }
}
