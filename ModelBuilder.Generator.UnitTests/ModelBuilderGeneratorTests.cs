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
        public void EmitsBuilderForStructRoot()
        {
            const string source = @"
namespace Sample
{
    public struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public static class Caller
    {
        public static Point Build() => global::ModelBuilder.vNext.Model.Create<Point>();
    }
}";

            var harness = GeneratorTestHarness.Run(source);

            harness.GeneratorDiagnostics.Should().BeEmpty();
            harness.CompilationErrors.Should().BeEmpty();
            harness.GeneratedSources[0].Should().Contain("Sample_PointBuilder");
            harness.GeneratedSources[0].Should().Contain("new global::Sample.Point(");
        }

        [Fact]
        public void EmitsEnumValueSourceForReachableEnum()
        {
            const string source = @"
namespace Sample
{
    public enum Gender { Female, Male, NonBinary }

    public sealed class Person
    {
        public Gender Gender { get; set; }
    }

    public static class Caller
    {
        public static Person Build() => global::ModelBuilder.vNext.Model.Create<Person>();
    }
}";

            var harness = GeneratorTestHarness.Run(source);

            harness.GeneratorDiagnostics.Should().BeEmpty();
            harness.CompilationErrors.Should().BeEmpty();
            harness.GeneratedSources[0].Should().Contain("Sample_GenderValueSource");
            harness.GeneratedSources[0].Should().Contain("global::ModelBuilder.vNext.ValueSource<global::Sample.Gender>.Instance");
        }

        [Fact]
        public void EmitsFlagsEnumValueSourceWithBitwiseCombine()
        {
            const string source = @"
namespace Sample
{
    [System.Flags]
    public enum Access { None = 0, Read = 1, Write = 2, Delete = 4 }

    public sealed class Resource
    {
        public Access Access { get; set; }
    }

    public static class Caller
    {
        public static Resource Build() => global::ModelBuilder.vNext.Model.Create<Resource>();
    }
}";

            var harness = GeneratorTestHarness.Run(source);

            harness.GeneratorDiagnostics.Should().BeEmpty();
            harness.CompilationErrors.Should().BeEmpty();
            harness.GeneratedSources[0].Should().Contain("result |= global::Sample.Access.Read");
            harness.GeneratedSources[0].Should().NotContain("Access.None");
        }

        [Fact]
        public void EmitsNullableValueSourceForNullableEnum()
        {
            const string source = @"
namespace Sample
{
    public enum Gender { Female, Male }

    public sealed class Person
    {
        public Gender? Gender { get; set; }
    }

    public static class Caller
    {
        public static Person Build() => global::ModelBuilder.vNext.Model.Create<Person>();
    }
}";

            var harness = GeneratorTestHarness.Run(source);

            harness.GeneratorDiagnostics.Should().BeEmpty();
            harness.CompilationErrors.Should().BeEmpty();
            harness.GeneratedSources[0].Should().Contain("Sample_GenderValueSource");
            harness.GeneratedSources[0].Should().Contain("NullableValueSource<global::Sample.Gender>");
        }

        [Fact]
        public void ReportsMB1001ForAbstractRoot()
        {
            const string source = @"
namespace Sample
{
    public abstract class Shape
    {
        public int Sides { get; set; }
    }

    public static class Caller
    {
        public static Shape Build() => global::ModelBuilder.vNext.Model.Create<Shape>();
    }
}";

            var harness = GeneratorTestHarness.Run(source);

            harness.GeneratorDiagnostics.Should().Contain(d => d.Id == "MB1001");
        }

        [Fact]
        public void ReportsMB1001ForInterfaceRoot()
        {
            const string source = @"
namespace Sample
{
    public interface IShape
    {
        int Sides { get; set; }
    }

    public static class Caller
    {
        public static IShape Build() => global::ModelBuilder.vNext.Model.Create<IShape>();
    }
}";

            var harness = GeneratorTestHarness.Run(source);

            harness.GeneratorDiagnostics.Should().Contain(d => d.Id == "MB1001");
        }

        [Fact]
        public void ReportsMB1002ForRootWithoutAccessibleConstructor()
        {
            const string source = @"
namespace Sample
{
    public sealed class Locked
    {
        private Locked() { }

        public int Value { get; set; }
    }

    public static class Caller
    {
        public static Locked Build() => global::ModelBuilder.vNext.Model.Create<Locked>();
    }
}";

            var harness = GeneratorTestHarness.Run(source);

            harness.GeneratorDiagnostics.Should().Contain(d => d.Id == "MB1002");
        }

        [Fact]
        public void DoesNotReportDiagnosticsForBuildableRoot()
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
        public static Person Build() => global::ModelBuilder.vNext.Model.Create<Person>();
    }
}";

            var harness = GeneratorTestHarness.Run(source);

            harness.GeneratorDiagnostics.Should().BeEmpty();
        }

        [Fact]
        public void EmitsListAndArrayValueSources()
        {
            const string source = @"
namespace Sample
{
    public sealed class Bag
    {
        public System.Collections.Generic.List<int> Numbers { get; set; }
        public string[] Tags { get; set; }
    }

    public static class Caller
    {
        public static Bag Build() => global::ModelBuilder.vNext.Model.Create<Bag>();
    }
}";

            var harness = GeneratorTestHarness.Run(source);

            harness.GeneratorDiagnostics.Should().BeEmpty();
            harness.CompilationErrors.Should().BeEmpty();
            harness.GeneratedSources[0].Should().Contain("new global::System.Collections.Generic.List<int>(count)");
            harness.GeneratedSources[0].Should().Contain("new string[count]");
        }

        [Fact]
        public void EmitsDictionaryValueSource()
        {
            const string source = @"
namespace Sample
{
    public sealed class Lookup
    {
        public System.Collections.Generic.Dictionary<int, string> Map { get; set; }
    }

    public static class Caller
    {
        public static Lookup Build() => global::ModelBuilder.vNext.Model.Create<Lookup>();
    }
}";

            var harness = GeneratorTestHarness.Run(source);

            harness.GeneratorDiagnostics.Should().BeEmpty();
            harness.CompilationErrors.Should().BeEmpty();
            harness.GeneratedSources[0].Should().Contain("new global::System.Collections.Generic.Dictionary<int, string>()");
        }

        [Fact]
        public void EmitsListForCollectionInterface()
        {
            const string source = @"
namespace Sample
{
    public sealed class Bag
    {
        public System.Collections.Generic.IReadOnlyList<int> Numbers { get; set; }
    }

    public static class Caller
    {
        public static Bag Build() => global::ModelBuilder.vNext.Model.Create<Bag>();
    }
}";

            var harness = GeneratorTestHarness.Run(source);

            harness.CompilationErrors.Should().BeEmpty();
            harness.GeneratedSources[0].Should().Contain("global::System.Collections.Generic.IReadOnlyList<int>");
            harness.GeneratedSources[0].Should().Contain("new global::System.Collections.Generic.List<int>(count)");
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
