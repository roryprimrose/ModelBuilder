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
        public void CreateBuildsEnumMemberFromGeneratedSource()
        {
            const string source = @"
namespace Sample
{
    public enum Priority { Low, Medium, High }

    public sealed class Job
    {
        public Priority Priority { get; set; }
    }

    public static class Caller
    {
        public static Job Build() => global::ModelBuilder.vNext.Model.Create<Job>();
    }
}";

            var harness = GeneratorTestHarness.Run(source);
            harness.CompilationErrors.Should().BeEmpty();

            var assembly = harness.EmitAndLoad();
            var jobType = assembly.GetType("Sample.Job", throwOnError: true)!;
            var priorityType = assembly.GetType("Sample.Priority", throwOnError: true)!;

            var job = CreateViaModel(jobType);

            var priority = jobType.GetProperty("Priority")!.GetValue(job);

            priority.Should().NotBeNull();
            Enum.IsDefined(priorityType, priority!).Should().BeTrue();
        }

        [Fact]
        public void CreateBuildsNullableEnumWithoutThrowing()
        {
            const string source = @"
namespace Sample
{
    public enum Mood { Calm, Excited, Tense }

    public sealed class Survey
    {
        public Mood? Mood { get; set; }
    }

    public static class Caller
    {
        public static Survey Build() => global::ModelBuilder.vNext.Model.Create<Survey>();
    }
}";

            var harness = GeneratorTestHarness.Run(source);
            harness.CompilationErrors.Should().BeEmpty();

            var assembly = harness.EmitAndLoad();
            var surveyType = assembly.GetType("Sample.Survey", throwOnError: true)!;

            var survey = CreateViaModel(surveyType);

            survey.Should().NotBeNull();
        }

        [Fact]
        public void CreatePopulatesPrimitivesFromBuiltInSources()
        {
            const string source = @"
namespace Sample
{
    public sealed class Customer
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public System.Guid Token { get; set; }
        public System.DateTimeOffset CreatedAt { get; set; }
    }

    public static class Caller
    {
        public static Customer Build() => global::ModelBuilder.vNext.Model.Create<Customer>();
    }
}";

            var harness = GeneratorTestHarness.Run(source);
            harness.CompilationErrors.Should().BeEmpty();

            var assembly = harness.EmitAndLoad();
            var customerType = assembly.GetType("Sample.Customer", throwOnError: true)!;

            var customer = CreateViaModel(customerType);

            customerType.GetProperty("Id")!.GetValue(customer).Should().NotBe(0);
            customerType.GetProperty("Name")!.GetValue(customer).Should().NotBeNull();
            customerType.GetProperty("Token")!.GetValue(customer).Should().NotBe(Guid.Empty);
            customerType.GetProperty("CreatedAt")!.GetValue(customer).Should().NotBe(default(DateTimeOffset));
        }

        [Fact]
        public void CreateBuildsListAndDictionaryWithElements()
        {
            const string source = @"
namespace Sample
{
    public sealed class Bag
    {
        public System.Collections.Generic.List<int> Numbers { get; set; }
        public System.Collections.Generic.Dictionary<int, int> Map { get; set; }
    }

    public static class Caller
    {
        public static Bag Build() => global::ModelBuilder.vNext.Model.Create<Bag>();
    }
}";

            var harness = GeneratorTestHarness.Run(source);
            harness.CompilationErrors.Should().BeEmpty();

            var assembly = harness.EmitAndLoad();
            var bagType = assembly.GetType("Sample.Bag", throwOnError: true)!;

            ValueSource<int>.Instance = new SequentialInt32Source();

            try
            {
                var bag = CreateViaModel(bagType);

                var numbers = (System.Collections.IEnumerable)bagType.GetProperty("Numbers")!.GetValue(bag)!;
                numbers.Cast<int>().Should().NotBeEmpty();

                var map = (System.Collections.IDictionary)bagType.GetProperty("Map")!.GetValue(bag)!;
                map.Count.Should().BeGreaterThan(0);
            }
            finally
            {
                ValueSource<int>.Instance = null;
            }
        }

        [Fact]
        public void CreateBuildsStructWithSettableProperties()
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
            harness.CompilationErrors.Should().BeEmpty();

            var assembly = harness.EmitAndLoad();
            var pointType = assembly.GetType("Sample.Point", throwOnError: true)!;

            ValueSource<int>.Instance = new ConstantInt32Source(11);

            try
            {
                var point = CreateViaModel(pointType);

                pointType.GetProperty("X")!.GetValue(point).Should().Be(11);
                pointType.GetProperty("Y")!.GetValue(point).Should().Be(11);
            }
            finally
            {
                ValueSource<int>.Instance = null;
            }
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

        private sealed class SequentialInt32Source : IValueSource<int>
        {
            private int _next;

            public int Create(BuildContext context, in BuildTarget target)
            {
                return ++_next;
            }
        }
    }

    [CollectionDefinition(Name, DisableParallelization = true)]
    public sealed class GeneratorExecutionCollection
    {
        public const string Name = "GeneratorExecution";
    }
}
