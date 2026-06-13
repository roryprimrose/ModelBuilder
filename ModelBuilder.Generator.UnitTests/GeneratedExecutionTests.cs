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
        public void WriteLogCapturesNestedBuildTrace()
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
        public string? FirstName { get; set; }
        public Address? Home { get; set; }
    }

    public static class Caller
    {
        public static (Person, string) Build()
        {
            string captured = string.Empty;
            var person = global::ModelBuilder.vNext.Model.WriteLog(log => captured = log).Create<Person>();
            return (person, captured);
        }
    }
}";

            var harness = GeneratorTestHarness.Run(source);
            harness.CompilationErrors.Should().BeEmpty();

            var assembly = harness.EmitAndLoad();
            var callerType = assembly.GetType("Sample.Caller", throwOnError: true)!;

            var result = callerType.GetMethod("Build")!.Invoke(null, null)!;
            var captured = (string)result.GetType().GetField("Item2")!.GetValue(result)!;

            captured.Should().Contain("Create Person");
            captured.Should().Contain("Populate Person");
            captured.Should().Contain("FirstName");
            captured.Should().Contain("Create Address");
        }

        [Fact]
        public void IgnoringLeavesMemberAtDefault()
        {
            const string source = @"
namespace Sample
{
    public sealed class Person
    {
        public int Age { get; set; }
        public string? Notes { get; set; }
    }

    public static class Caller
    {
        public static Person Build() => global::ModelBuilder.vNext.Model.Ignoring<Person>(p => p.Notes).Create<Person>();
    }
}";

            var harness = GeneratorTestHarness.Run(source);
            harness.CompilationErrors.Should().BeEmpty();

            var assembly = harness.EmitAndLoad();
            var personType = assembly.GetType("Sample.Person", throwOnError: true)!;

            // Build through the generated ignoring call by invoking the static caller.
            var callerType = assembly.GetType("Sample.Caller", throwOnError: true)!;
            var person = callerType.GetMethod("Build")!.Invoke(null, null)!;

            personType.GetProperty("Notes")!.GetValue(person).Should().BeNull();
            personType.GetProperty("Age")!.GetValue(person).Should().NotBe(0);
        }

        [Fact]
        public void MappingBuildsConcreteTypeForAbstractMember()
        {
            const string source = @"
namespace Sample
{
    public abstract class Animal
    {
        public string? Name { get; set; }
    }

    public sealed class Dog : Animal
    {
        public int Legs { get; set; }
    }

    public sealed class Owner
    {
        public Animal? Pet { get; set; }
    }

    public static class Caller
    {
        public static Owner Build() =>
            global::ModelBuilder.vNext.Model.Mapping<Animal, Dog>().Create<Owner>();
    }
}";

            var harness = GeneratorTestHarness.Run(source);
            harness.CompilationErrors.Should().BeEmpty();

            var assembly = harness.EmitAndLoad();
            var ownerType = assembly.GetType("Sample.Owner", throwOnError: true)!;
            var dogType = assembly.GetType("Sample.Dog", throwOnError: true)!;
            var callerType = assembly.GetType("Sample.Caller", throwOnError: true)!;

            var owner = callerType.GetMethod("Build")!.Invoke(null, null)!;

            var pet = ownerType.GetProperty("Pet")!.GetValue(owner);

            pet.Should().NotBeNull();
            pet!.GetType().Should().Be(dogType);
        }

        [Fact]
        public void CreateTerminatesOnSelfReferencingType()
        {
            const string source = @"
namespace Sample
{
    public sealed class Node
    {
        public int Value { get; set; }
        public Node? Next { get; set; }
    }

    public static class Caller
    {
        public static Node Build() => global::ModelBuilder.vNext.Model.Create<Node>();
    }
}";

            var harness = GeneratorTestHarness.Run(source);
            harness.CompilationErrors.Should().BeEmpty();

            var assembly = harness.EmitAndLoad();
            var nodeType = assembly.GetType("Sample.Node", throwOnError: true)!;

            // Must not StackOverflow; the circular-reference guard returns null past the first re-entry.
            var node = CreateViaModel(nodeType);

            node.Should().NotBeNull();
            nodeType.GetProperty("Next")!.GetValue(node).Should().BeNull();
        }

        [Fact]
        public void CreateDerivesEmailFromSiblingNameMembers()
        {
            const string source = @"
namespace Sample
{
    public sealed class Person
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
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

            var firstName = (string)personType.GetProperty("FirstName")!.GetValue(person)!;
            var lastName = (string)personType.GetProperty("LastName")!.GetValue(person)!;
            var email = (string)personType.GetProperty("Email")!.GetValue(person)!;

            email.Should().Contain(firstName.ToLowerInvariant());
            email.Should().Contain(lastName.ToLowerInvariant());
        }

        [Fact]
        public void CreatePopulatesEntityMembersFromNamedSources()
        {
            const string source = @"
namespace Sample
{
    public sealed class Contact
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Company { get; set; }
    }

    public static class Caller
    {
        public static Contact Build() => global::ModelBuilder.vNext.Model.Create<Contact>();
    }
}";

            var harness = GeneratorTestHarness.Run(source);
            harness.CompilationErrors.Should().BeEmpty();

            var assembly = harness.EmitAndLoad();
            var contactType = assembly.GetType("Sample.Contact", throwOnError: true)!;

            var contact = CreateViaModel(contactType);

            var firstName = (string)contactType.GetProperty("FirstName")!.GetValue(contact)!;
            var email = (string)contactType.GetProperty("Email")!.GetValue(contact)!;

            firstName.Should().NotBeNullOrEmpty();
            email.Should().Contain("@");
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
