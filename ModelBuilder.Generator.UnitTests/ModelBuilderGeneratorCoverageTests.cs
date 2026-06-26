namespace ModelBuilder.Generator.UnitTests
{
    using FluentAssertions;
    using Xunit;

    public class ModelBuilderGeneratorCoverageTests
    {
        [Fact]
        public void DisambiguatesBuilderNamesThatSanitizeToTheSameName()
        {
            // Foo.Bar.Baz and Foo.Bar_Baz both sanitise to the candidate name Foo_Bar_Baz, forcing the
            // name allocator to append a numeric suffix to the second builder.
            const string source = @"
namespace Foo.Bar
{
    public sealed class Baz
    {
        public string? Label { get; set; }
    }
}

namespace Foo
{
    public sealed class Bar_Baz
    {
        public int Value { get; set; }
    }
}

namespace Sample
{
    public sealed class Holder
    {
        public global::Foo.Bar.Baz? First { get; set; }
        public global::Foo.Bar_Baz? Second { get; set; }
    }

    public static class Caller
    {
        public static Holder Build() => global::ModelBuilder.Model.Create<Holder>();
    }
}";

            var harness = GeneratorTestHarness.Run(source);

            harness.CompilationErrors.Should().BeEmpty();
            harness.GeneratedSources[0].Should().Contain("Foo_Bar_BazBuilder");
            harness.GeneratedSources[0].Should().Contain("Foo_Bar_BazBuilder1");
        }

        [Fact]
        public void EmitsDefaultLiteralsForConstructorParameters()
        {
            // Every optional parameter exercises a different default-literal formatting branch: the
            // numeric type suffixes (F/M/U/UL), a char literal, a string with characters that need
            // escaping, a nullable value type, and a struct default rendered as default(T). If any
            // literal is rendered incorrectly the generated builder will not compile, so an empty
            // compilation-error set proves they are all valid.
            const string source = @"
namespace Sample
{
    public struct Tag
    {
        public int Value { get; set; }
    }

    public sealed class Thing
    {
        public Thing(
            float single = 1.5f,
            decimal money = 2.5m,
            ulong big = 7,
            uint small = 4,
            char letter = 'x',
            char apostrophe = '\'',
            string text = ""a\\b\0c\nd\re\tf\""g\u0001h"",
            int? maybe = 5,
            Tag tag = default)
        {
        }

        public string? Name { get; set; }
    }

    public static class Caller
    {
        public static Thing Build() => global::ModelBuilder.Model.Create<Thing>();
    }
}";

            var harness = GeneratorTestHarness.Run(source);

            harness.CompilationErrors.Should().BeEmpty();
            harness.GeneratedSources[0].Should().Contain("1.5F");
            harness.GeneratedSources[0].Should().Contain("2.5M");
            harness.GeneratedSources[0].Should().Contain("7UL");
            harness.GeneratedSources[0].Should().Contain("4U");
            harness.GeneratedSources[0].Should().Contain("default(global::Sample.Tag)");
        }

        [Fact]
        public void EmitsFlagsEnumValueSourcesForEveryIntegralUnderlyingType()
        {
            // A flags enum with a zero member is detected per underlying integral type, exercising each
            // arm of the zero-member check.
            const string source = @"
namespace Sample
{
    [System.Flags] public enum ByteFlags : byte { None = 0, A = 1 }
    [System.Flags] public enum SByteFlags : sbyte { None = 0, A = 1 }
    [System.Flags] public enum ShortFlags : short { None = 0, A = 1 }
    [System.Flags] public enum UShortFlags : ushort { None = 0, A = 1 }
    [System.Flags] public enum UIntFlags : uint { None = 0, A = 1 }
    [System.Flags] public enum LongFlags : long { None = 0, A = 1 }
    [System.Flags] public enum ULongFlags : ulong { None = 0, A = 1 }

    public sealed class Holder
    {
        public ByteFlags ByteValue { get; set; }
        public SByteFlags SByteValue { get; set; }
        public ShortFlags ShortValue { get; set; }
        public UShortFlags UShortValue { get; set; }
        public UIntFlags UIntValue { get; set; }
        public LongFlags LongValue { get; set; }
        public ULongFlags ULongValue { get; set; }
    }

    public static class Caller
    {
        public static Holder Build() => global::ModelBuilder.Model.Create<Holder>();
    }
}";

            var harness = GeneratorTestHarness.Run(source);

            harness.CompilationErrors.Should().BeEmpty();
            harness.GeneratedSources[0].Should().Contain("Sample_ByteFlagsValueSource");
            harness.GeneratedSources[0].Should().Contain("Sample_ULongFlagsValueSource");
        }

        [Fact]
        public void EmitsSourcesForSetAndDictionaryMembers()
        {
            // A HashSet member is classified as a set collection, and a dictionary with a reference-type
            // key emits a null-key guard during element population.
            const string source = @"
namespace Sample
{
    public sealed class Holder
    {
        public System.Collections.Generic.HashSet<int> Tags { get; set; }
        public System.Collections.Generic.Dictionary<string, int> Map { get; set; }
    }

    public static class Caller
    {
        public static Holder Build() => global::ModelBuilder.Model.Create<Holder>();
    }
}";

            var harness = GeneratorTestHarness.Run(source);

            harness.CompilationErrors.Should().BeEmpty();
            harness.GeneratedSources[0].Should().Contain("System.Collections.Generic.HashSet<");
            harness.GeneratedSources[0].Should().Contain("if (key is null)");
        }
    }
}
