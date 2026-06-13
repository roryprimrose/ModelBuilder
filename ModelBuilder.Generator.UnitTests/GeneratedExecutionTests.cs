namespace ModelBuilder.Generator.UnitTests
{
    using System;
    using System.Linq;
    using System.Reflection;
    using FluentAssertions;
    using ModelBuilder.vNext;
    using Xunit;

    [Collection(GeneratorExecutionCollection.Name)]
    public class GeneratedExecutionTests
    {
        [Fact]
        public void CreateRecursivelyBuildsNestedTypes()
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

            var assembly = harness.EmitAndLoad();
            var personType = assembly.GetType("Sample.Person", throwOnError: true)!;

            var person = CreateViaModel(personType);

            person.Should().NotBeNull();

            var home = personType.GetProperty("Home")!.GetValue(person);

            home.Should().NotBeNull();
        }

        [Fact]
        public void CreatePopulatesLeafFromRegisteredValueSource()
        {
            const string source = @"
namespace Sample
{
    public sealed class Widget
    {
        public int Serial { get; set; }
    }

    public static class Caller
    {
        public static Widget Build() => global::ModelBuilder.vNext.Model.Create<Widget>();
    }
}";

            var harness = GeneratorTestHarness.Run(source);
            harness.CompilationErrors.Should().BeEmpty();

            var assembly = harness.EmitAndLoad();
            var widgetType = assembly.GetType("Sample.Widget", throwOnError: true)!;

            ValueSource<int>.Instance = new ConstantInt32Source(4242);

            try
            {
                var widget = CreateViaModel(widgetType);

                var serial = widgetType.GetProperty("Serial")!.GetValue(widget);

                serial.Should().Be(4242);
            }
            finally
            {
                ValueSource<int>.Instance = null;
            }
        }

        private static object CreateViaModel(Type instanceType)
        {
            var createGeneric = typeof(Model)
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .First(method => method.Name == "Create" && method.IsGenericMethodDefinition && method.GetParameters().Length == 1);

            var closed = createGeneric.MakeGenericMethod(instanceType);

            return closed.Invoke(null, new object?[] { null })!;
        }

        private sealed class ConstantInt32Source : IValueSource<int>
        {
            private readonly int _value;

            public ConstantInt32Source(int value)
            {
                _value = value;
            }

            public int Create(BuildContext context, in BuildTarget target)
            {
                return _value;
            }
        }
    }

    [CollectionDefinition(Name, DisableParallelization = true)]
    public sealed class GeneratorExecutionCollection
    {
        public const string Name = "GeneratorExecution";
    }
}
