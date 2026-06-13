namespace ModelBuilder.Generator.UnitTests
{
    using System.Linq;
    using FluentAssertions;
    using Microsoft.CodeAnalysis;
    using Xunit;

    public class ModelBuilderGeneratorTests
    {
        [Fact]
        public void EmitsBuilderForCreateRoot()
        {
            const string source = @"
namespace Sample
{
    public sealed class Person
    {
        public string? FirstName { get; set; }
        public int Age { get; set; }
    }

    public static class Caller
    {
        public static Person Build() => global::ModelBuilder.vNext.Model.Create<Person>();
    }
}";

            var harness = GeneratorTestHarness.Run(source);

            harness.GeneratorDiagnostics.Should().BeEmpty();
            harness.CompilationErrors.Should().BeEmpty();
            harness.GeneratedSources.Should().ContainSingle();
            harness.GeneratedSources[0].Should().Contain("Sample_PersonBuilder");
            harness.GeneratedSources[0].Should().Contain("ModuleInitializer");
        }

        [Fact]
        public void EmitsBuildersForReachableNestedTypes()
        {
            const string source = @"
namespace Sample
{
    public sealed class Address
    {
        public string? City { get; set; }
    }

    public sealed class Person
    {
        public Address? Home { get; set; }
    }

    public static class Caller
    {
        public static Person Build() => global::ModelBuilder.vNext.Model.Create<Person>();
    }
}";

            var harness = GeneratorTestHarness.Run(source);

            harness.CompilationErrors.Should().BeEmpty();
            harness.GeneratedSources[0].Should().Contain("Sample_PersonBuilder");
            harness.GeneratedSources[0].Should().Contain("Sample_AddressBuilder");
        }

        [Fact]
        public void EmitsBuilderUsingSelectedConstructor()
        {
            const string source = @"
namespace Sample
{
    public sealed class Account
    {
        public Account(string owner, int number)
        {
            Owner = owner;
            Number = number;
        }

        public string Owner { get; }
        public int Number { get; }
    }

    public static class Caller
    {
        public static Account Build() => global::ModelBuilder.vNext.Model.Create<Account>();
    }
}";

            var harness = GeneratorTestHarness.Run(source);

            harness.CompilationErrors.Should().BeEmpty();
            harness.GeneratedSources[0].Should().Contain("new global::Sample.Account(");
            harness.GeneratedSources[0].Should().Contain("Build<string>");
        }

        [Fact]
        public void EmitsNothingWhenNoRootsPresent()
        {
            const string source = @"
namespace Sample
{
    public sealed class Person
    {
        public int Age { get; set; }
    }
}";

            var harness = GeneratorTestHarness.Run(source);

            harness.GeneratorDiagnostics.Should().BeEmpty();
            harness.GeneratedSources.Should().BeEmpty();
        }

        [Fact]
        public void IgnoresUnrelatedGenericCreateCalls()
        {
            const string source = @"
namespace Sample
{
    public static class Other
    {
        public static T Create<T>() where T : new() => new T();
    }

    public sealed class Person
    {
        public int Age { get; set; }
    }

    public static class Caller
    {
        public static Person Build() => Other.Create<Person>();
    }
}";

            var harness = GeneratorTestHarness.Run(source);

            harness.GeneratedSources.Should().BeEmpty();
        }

        [Fact]
        public void TreatsPopulateAsRoot()
        {
            const string source = @"
namespace Sample
{
    public sealed class Person
    {
        public int Age { get; set; }
    }

    public static class Caller
    {
        public static Person Build() => global::ModelBuilder.vNext.Model.Populate(new Person());
    }
}";

            var harness = GeneratorTestHarness.Run(source);

            harness.CompilationErrors.Should().BeEmpty();
            harness.GeneratedSources[0].Should().Contain("Sample_PersonBuilder");
        }
    }
}
