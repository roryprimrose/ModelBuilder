namespace ModelBuilder.Generator.UnitTests
{
    using System;
    using System.Reflection;
    using FluentAssertions;
    using Xunit;

    [Collection(GeneratorExecutionCollection.Name)]
    public class GeneratedConstructionExecutionTests
    {
        [Fact]
        public void ConstructFromUsesSuppliedConstructorArgumentsAndPopulatesOtherMembers()
        {
            const string source = @"
using ModelBuilder;

namespace Sample
{
    public sealed class Person
    {
        public Person(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string? Title { get; set; }
    }

    public static class Caller
    {
        public static Person Build() => global::ModelBuilder.Model.Construct<Person>().From(""Fred"", ""Smith"");
    }
}";

            var harness = GeneratorTestHarness.Run(source);
            harness.CompilationErrors.Should().BeEmpty();

            var assembly = harness.EmitAndLoad();
            var personType = assembly.GetType("Sample.Person", throwOnError: true)!;

            var person = InvokeCaller(assembly);

            // The constructor-supplied values are preserved (not re-randomised).
            personType.GetProperty("FirstName")!.GetValue(person).Should().Be("Fred");
            personType.GetProperty("LastName")!.GetValue(person).Should().Be("Smith");

            // A member not assigned by the constructor is still populated.
            personType.GetProperty("Title")!.GetValue(person).Should().NotBeNull();
        }

        [Fact]
        public void ConstructFromEmitsAnOverloadPerPublicConstructor()
        {
            const string source = @"
using ModelBuilder;

namespace Sample
{
    public sealed class Money
    {
        public Money(int amount) { Amount = amount; }

        public Money(int amount, string currency) { Amount = amount; Currency = currency; }

        public int Amount { get; set; }

        public string? Currency { get; set; }
    }

    public static class Caller
    {
        public static Money Build() => global::ModelBuilder.Model.Construct<Money>().From(5);

        public static Money BuildWithCurrency() => global::ModelBuilder.Model.Construct<Money>().From(5, ""USD"");
    }
}";

            var harness = GeneratorTestHarness.Run(source);
            harness.CompilationErrors.Should().BeEmpty();

            // Both constructors produce a From overload.
            CountOccurrences(harness.GeneratedSources[0], "From(this global::ModelBuilder.Construction")
                .Should().Be(2);

            var assembly = harness.EmitAndLoad();
            var moneyType = assembly.GetType("Sample.Money", throwOnError: true)!;

            var withCurrency = assembly.GetType("Sample.Caller", throwOnError: true)!
                .GetMethod("BuildWithCurrency")!
                .Invoke(null, null);

            moneyType.GetProperty("Amount")!.GetValue(withCurrency).Should().Be(5);
            moneyType.GetProperty("Currency")!.GetValue(withCurrency).Should().Be("USD");
        }

        [Fact]
        public void ConstructFromCopiesConstructorXmlDocumentation()
        {
            const string source = @"
using ModelBuilder;

namespace Sample
{
    public sealed class Account
    {
        /// <summary>
        /// Creates an account for a customer.
        /// </summary>
        /// <param name=""owner"">The account owner.</param>
        public Account(string owner) { Owner = owner; }

        public string Owner { get; set; }
    }

    public static class Caller
    {
        public static Account Build() => global::ModelBuilder.Model.Construct<Account>().From(""Jane"");
    }
}";

            var harness = GeneratorTestHarness.Run(source);
            harness.CompilationErrors.Should().BeEmpty();

            harness.GeneratedSources[0].Should().Contain("Creates an account for a customer.");
            harness.GeneratedSources[0].Should().Contain("<param name=\"owner\">");
        }

        [Fact]
        public void CreateEmitsConstructorSuppressionForParameterisedType()
        {
            const string source = @"
using ModelBuilder;

namespace Sample
{
    public sealed class Person
    {
        public Person(string firstName) { FirstName = firstName; }

        public string FirstName { get; set; }

        public string? Title { get; set; }
    }

    public static class Caller
    {
        public static Person Build() => global::ModelBuilder.Model.Create<Person>();
    }
}";

            var harness = GeneratorTestHarness.Run(source);
            harness.CompilationErrors.Should().BeEmpty();

            // FirstName matches the constructor parameter, so the Create path suppresses it: it is only
            // assigned in the standalone Populate method (which populates every member), not in Create.
            // Title has no matching parameter, so it is assigned in both Create's complement and Populate.
            CountOccurrences(harness.GeneratedSources[0], "instance.FirstName = context.Build").Should().Be(1);
            CountOccurrences(harness.GeneratedSources[0], "instance.Title = context.Build").Should().Be(2);
        }

        [Fact]
        public void ConstructFromDerivesMemberFromConstructorOnlyParameters()
        {
            const string source = @"
using ModelBuilder;

namespace Sample
{
    public sealed class Contact
    {
        public Contact(string firstName, string lastName)
        {
            // firstName and lastName are constructor parameters only - never exposed as properties.
        }

        public string? Email { get; set; }
    }

    public static class Caller
    {
        public static Contact Build() => global::ModelBuilder.Model.Construct<Contact>().From(""Fred"", ""Smith"");
    }
}";

            var harness = GeneratorTestHarness.Run(source);
            harness.CompilationErrors.Should().BeEmpty();

            var assembly = harness.EmitAndLoad();
            var contactType = assembly.GetType("Sample.Contact", throwOnError: true)!;

            var contact = InvokeCaller(assembly);
            var email = (string)contactType.GetProperty("Email")!.GetValue(contact)!;

            // Email derives from the constructor-only firstName/lastName arguments via siblings.
            email.Should().StartWith("fred.smith@");
        }

        [Fact]
        public void CreateRecordsConstructorParametersAsSiblings()
        {
            const string source = @"
using ModelBuilder;

namespace Sample
{
    public sealed class Contact
    {
        public Contact(string firstName, string lastName)
        {
        }

        public string? Email { get; set; }
    }

    public static class Caller
    {
        public static Contact Build() => global::ModelBuilder.Model.Create<Contact>();
    }
}";

            var harness = GeneratorTestHarness.Run(source);
            harness.CompilationErrors.Should().BeEmpty();

            // The generated Create records each constructor argument as a sibling under the parameter
            // name, so members like Email can derive from them.
            harness.GeneratedSources[0].Should().Contain("context.RecordSibling(\"firstName\"");
            harness.GeneratedSources[0].Should().Contain("context.RecordSibling(\"lastName\"");

            // The email still derives from those siblings at runtime.
            var assembly = harness.EmitAndLoad();
            var contactType = assembly.GetType("Sample.Contact", throwOnError: true)!;
            var contact = InvokeCaller(assembly);
            var email = (string)contactType.GetProperty("Email")!.GetValue(contact)!;

            email.Should().Contain("@");
            email.Should().Contain(".");
        }

        [Fact]
        public void ConstructFromRejectsWrongArgumentTypeAtCompileTime()
        {
            const string source = @"
using ModelBuilder;

namespace Sample
{
    public sealed class Person
    {
        public Person(string firstName) { FirstName = firstName; }

        public string FirstName { get; set; }
    }

    public static class Caller
    {
        public static Person Build() => global::ModelBuilder.Model.Construct<Person>().From(123);
    }
}";

            var harness = GeneratorTestHarness.Run(source);

            // The typed From overload gives compile-time validation: an int does not satisfy string.
            harness.CompilationErrors.Should().NotBeEmpty();
        }

        [Fact]
        public void ConstructFromResolvesViaUnqualifiedModelAndModelBuilderNamespace()
        {
            // The realistic call shape: Model used unqualified under `using ModelBuilder;`. The generated
            // From extension lives in the ModelBuilder namespace, so the same using brings it into scope
            // with no per-namespace emission.
            const string source = @"
using ModelBuilder;

namespace Sample
{
    public sealed class Person
    {
        public Person(string firstName, string lastName) { FirstName = firstName; LastName = lastName; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }

    public static class Caller
    {
        public static Person Build() => Model.Construct<Person>().From(""Fred"", ""Smith"");
    }
}";

            var harness = GeneratorTestHarness.Run(source);
            harness.CompilationErrors.Should().BeEmpty();

            // A single extension class is emitted into the ModelBuilder namespace (not per calling namespace).
            harness.GeneratedSources[0].Should().Contain("namespace ModelBuilder");
            harness.GeneratedSources[0].Should().NotContain("namespace Sample");

            var assembly = harness.EmitAndLoad();
            var personType = assembly.GetType("Sample.Person", throwOnError: true)!;
            var person = InvokeCaller(assembly);

            personType.GetProperty("FirstName")!.GetValue(person).Should().Be("Fred");
            personType.GetProperty("LastName")!.GetValue(person).Should().Be("Smith");
        }

        [Fact]
        public void ConstructFromUsesDefaultsWhenAllParametersAreOptional()
        {
            const string source = @"
using ModelBuilder;

namespace Sample
{
    public sealed class Settings
    {
        public Settings(int retries = 3, string mode = ""fast"")
        {
            Retries = retries;
            Mode = mode;
        }

        public int Retries { get; set; }

        public string Mode { get; set; }
    }

    public static class Caller
    {
        // Every parameter is optional, so From can be called with no arguments.
        public static Settings Build() => global::ModelBuilder.Model.Construct<Settings>().From();
    }
}";

            var harness = GeneratorTestHarness.Run(source);
            harness.CompilationErrors.Should().BeEmpty();

            // The generated From overload carries the declared defaults so the call can omit them.
            harness.GeneratedSources[0].Should().Contain("retries = 3");
            harness.GeneratedSources[0].Should().Contain("mode = \"fast\"");

            var assembly = harness.EmitAndLoad();
            var settingsType = assembly.GetType("Sample.Settings", throwOnError: true)!;
            var settings = InvokeCaller(assembly);

            settingsType.GetProperty("Retries")!.GetValue(settings).Should().Be(3);
            settingsType.GetProperty("Mode")!.GetValue(settings).Should().Be("fast");
        }

        [Fact]
        public void ConstructFromUsesDefaultsForOmittedOptionalParameters()
        {
            const string source = @"
using ModelBuilder;

namespace Sample
{
    public sealed class Connection
    {
        public Connection(string host, int port = 8080, bool secure = true)
        {
            Host = host;
            Port = port;
            Secure = secure;
        }

        public string Host { get; set; }

        public int Port { get; set; }

        public bool Secure { get; set; }
    }

    public static class Caller
    {
        // Pass only the required host; port and secure fall back to their defaults.
        public static Connection Build() => global::ModelBuilder.Model.Construct<Connection>().From(""db1"");
    }
}";

            var harness = GeneratorTestHarness.Run(source);
            harness.CompilationErrors.Should().BeEmpty();

            var assembly = harness.EmitAndLoad();
            var connectionType = assembly.GetType("Sample.Connection", throwOnError: true)!;
            var connection = InvokeCaller(assembly);

            connectionType.GetProperty("Host")!.GetValue(connection).Should().Be("db1");
            connectionType.GetProperty("Port")!.GetValue(connection).Should().Be(8080);
            connectionType.GetProperty("Secure")!.GetValue(connection).Should().Be(true);
        }

        [Fact]
        public void ConstructFromAllowsOverridingAnOptionalParameter()
        {
            const string source = @"
using ModelBuilder;

namespace Sample
{
    public sealed class Connection
    {
        public Connection(string host, int port = 8080, bool secure = true)
        {
            Host = host;
            Port = port;
            Secure = secure;
        }

        public string Host { get; set; }

        public int Port { get; set; }

        public bool Secure { get; set; }
    }

    public static class Caller
    {
        // Override port, leave secure at its default.
        public static Connection Build() => global::ModelBuilder.Model.Construct<Connection>().From(""db1"", 9090);
    }
}";

            var harness = GeneratorTestHarness.Run(source);
            harness.CompilationErrors.Should().BeEmpty();

            var assembly = harness.EmitAndLoad();
            var connectionType = assembly.GetType("Sample.Connection", throwOnError: true)!;
            var connection = InvokeCaller(assembly);

            connectionType.GetProperty("Port")!.GetValue(connection).Should().Be(9090);
            connectionType.GetProperty("Secure")!.GetValue(connection).Should().Be(true);
        }

        [Fact]
        public void ConstructFromEmitsTypedDefaultLiteralsForEachParameterKind()
        {
            const string source = @"
using ModelBuilder;

namespace Sample
{
    public enum Speed { Slow, Fast }

    public sealed class Options
    {
        public Options(
            int count = 5,
            long size = 100,
            double ratio = 1.5,
            bool flag = true,
            string name = ""abc"",
            Speed speed = Speed.Fast,
            string? note = null)
        {
            Count = count;
            Size = size;
            Ratio = ratio;
            Flag = flag;
            Name = name;
            Speed = speed;
            Note = note;
        }

        public int Count { get; set; }

        public long Size { get; set; }

        public double Ratio { get; set; }

        public bool Flag { get; set; }

        public string Name { get; set; }

        public Speed Speed { get; set; }

        public string? Note { get; set; }
    }

    public static class Caller
    {
        public static Options Build() => global::ModelBuilder.Model.Construct<Options>().From();
    }
}";

            var harness = GeneratorTestHarness.Run(source);
            harness.CompilationErrors.Should().BeEmpty();

            var generated = harness.GeneratedSources[0];

            // Each default is rendered as a valid C# literal for its parameter type.
            generated.Should().Contain("count = 5");
            generated.Should().Contain("size = 100L");
            generated.Should().Contain("ratio = 1.5D");
            generated.Should().Contain("flag = true");
            generated.Should().Contain("name = \"abc\"");
            generated.Should().Contain("speed = (global::Sample.Speed)(1)");
            generated.Should().Contain("note = null");

            var assembly = harness.EmitAndLoad();
            var optionsType = assembly.GetType("Sample.Options", throwOnError: true)!;
            var options = InvokeCaller(assembly);

            optionsType.GetProperty("Count")!.GetValue(options).Should().Be(5);
            optionsType.GetProperty("Size")!.GetValue(options).Should().Be(100L);
            optionsType.GetProperty("Ratio")!.GetValue(options).Should().Be(1.5d);
            optionsType.GetProperty("Flag")!.GetValue(options).Should().Be(true);
            optionsType.GetProperty("Name")!.GetValue(options).Should().Be("abc");
            optionsType.GetProperty("Speed")!.GetValue(options)!.ToString().Should().Be("Fast");
            optionsType.GetProperty("Note")!.GetValue(options).Should().BeNull();
        }

        [Fact]
        public void CreateUsesConstructorDefaultsForAllOptionalParametersWhenEnabled()
        {
            const string source = @"
using ModelBuilder;

namespace Sample
{
    public sealed class Defaults
    {
        public Defaults(int a = 1, string b = ""x"", bool c = true)
        {
            A = a;
            B = b;
            C = c;
        }

        public int A { get; set; }

        public string B { get; set; }

        public bool C { get; set; }
    }

    public static class Caller
    {
        public static Defaults Build() =>
            global::ModelBuilder.Model.SetOptions(o => o.UseConstructorDefaults = true).Create<Defaults>();
    }
}";

            var harness = GeneratorTestHarness.Run(source);
            harness.CompilationErrors.Should().BeEmpty();

            var assembly = harness.EmitAndLoad();
            var defaultsType = assembly.GetType("Sample.Defaults", throwOnError: true)!;
            var defaults = InvokeCaller(assembly);

            // With the option on, every optional parameter uses its declared default.
            defaultsType.GetProperty("A")!.GetValue(defaults).Should().Be(1);
            defaultsType.GetProperty("B")!.GetValue(defaults).Should().Be("x");
            defaultsType.GetProperty("C")!.GetValue(defaults).Should().Be(true);
        }

        [Fact]
        public void CreateUsesConstructorDefaultsForSomeOptionalParametersWhenEnabled()
        {
            const string source = @"
using ModelBuilder;

namespace Sample
{
    public sealed class Widget
    {
        public Widget(string label, int size = 7)
        {
            Label = label;
            Size = size;
        }

        public string Label { get; set; }

        public int Size { get; set; }
    }

    public static class Caller
    {
        public static Widget Build() =>
            global::ModelBuilder.Model.SetOptions(o => o.UseConstructorDefaults = true).Create<Widget>();
    }
}";

            var harness = GeneratorTestHarness.Run(source);
            harness.CompilationErrors.Should().BeEmpty();

            var assembly = harness.EmitAndLoad();
            var widgetType = assembly.GetType("Sample.Widget", throwOnError: true)!;
            var widget = InvokeCaller(assembly);

            // The optional size uses its default; the required label is still generated.
            widgetType.GetProperty("Size")!.GetValue(widget).Should().Be(7);
            widgetType.GetProperty("Label")!.GetValue(widget).Should().NotBeNull();
        }

        [Fact]
        public void CreateGeneratesOptionalParameterValuesWhenOptionDisabled()
        {
            const string source = @"
using ModelBuilder;

namespace Sample
{
    public sealed class Widget
    {
        public Widget(int size = 7)
        {
            Size = size;
        }

        public int Size { get; set; }
    }

    public static class Caller
    {
        public static Widget Build() => global::ModelBuilder.Model.Create<Widget>();
    }
}";

            var harness = GeneratorTestHarness.Run(source);
            harness.CompilationErrors.Should().BeEmpty();

            // The Create path is guarded by the runtime option: it uses the default only when the option
            // is set, and otherwise builds a value.
            harness.GeneratedSources[0].Should().Contain("context.UseConstructorDefaults ? 7 : context.Build");

            var assembly = harness.EmitAndLoad();
            InvokeCaller(assembly).Should().NotBeNull();
        }

        [Fact]
        public void PopulateRetainsAssignedMemberValueWhenEnabled()
        {
            const string source = @"
using ModelBuilder;

namespace Sample
{
    public sealed class Holder
    {
        public string Preset { get; set; } = ""kept"";

        public string? Generated { get; set; }
    }

    public static class Caller
    {
        public static Holder Build()
        {
            var instance = new Holder();

            return global::ModelBuilder.Model.SetOptions(o => o.RetainAssignedValues = true).Populate(instance);
        }
    }
}";

            var harness = GeneratorTestHarness.Run(source);
            harness.CompilationErrors.Should().BeEmpty();

            var assembly = harness.EmitAndLoad();
            var holderType = assembly.GetType("Sample.Holder", throwOnError: true)!;
            var holder = InvokeCaller(assembly);

            // With the option on, the pre-assigned member is retained while the default member is still generated.
            holderType.GetProperty("Preset")!.GetValue(holder).Should().Be("kept");
            holderType.GetProperty("Generated")!.GetValue(holder).Should().NotBeNull();
        }

        [Fact]
        public void PopulateOverwritesAssignedMemberByDefault()
        {
            const string source = @"
using ModelBuilder;

namespace Sample
{
    public sealed class Holder
    {
        public string Preset { get; set; } = ""kept"";
    }

    public static class Caller
    {
        public static Holder Build()
        {
            var instance = new Holder();

            return global::ModelBuilder.Model.Populate(instance);
        }
    }
}";

            var harness = GeneratorTestHarness.Run(source);
            harness.CompilationErrors.Should().BeEmpty();

            var assembly = harness.EmitAndLoad();
            var holderType = assembly.GetType("Sample.Holder", throwOnError: true)!;
            var holder = InvokeCaller(assembly);

            // By default the pre-assigned value is overwritten with a generated value.
            holderType.GetProperty("Preset")!.GetValue(holder).Should().NotBe("kept");
        }

        [Fact]
        public void CreatePopulatesAssignedMemberByDefault()
        {
            const string source = @"
using ModelBuilder;

namespace Sample
{
    public sealed class Detail
    {
        public string? Name { get; set; }
    }

    public sealed class Holder
    {
        public Holder()
        {
            Detail = new Detail();
        }

        public Detail Detail { get; set; }
    }

    public static class Caller
    {
        public static Holder Build() => global::ModelBuilder.Model.Create<Holder>();
    }
}";

            var harness = GeneratorTestHarness.Run(source);
            harness.CompilationErrors.Should().BeEmpty();

            var assembly = harness.EmitAndLoad();
            var holderType = assembly.GetType("Sample.Holder", throwOnError: true)!;
            var detailType = assembly.GetType("Sample.Detail", throwOnError: true)!;
            var holder = InvokeCaller(assembly);

            // By default a member assigned a newly constructed instance is still populated, so its own
            // members are not left unset.
            var detail = holderType.GetProperty("Detail")!.GetValue(holder);
            detail.Should().NotBeNull();
            detailType.GetProperty("Name")!.GetValue(detail).Should().NotBeNull();
        }

        [Fact]
        public void CreateRetainsConstructorAssignedMemberWhenEnabled()
        {
            const string source = @"
using ModelBuilder;

namespace Sample
{
    public class Animal
    {
    }

    public sealed class Dog : Animal
    {
    }

    public sealed class Owner
    {
        public Owner()
        {
            Pet = new Dog();
        }

        public Animal Pet { get; set; }
    }

    public static class Caller
    {
        public static Owner Build() => global::ModelBuilder.Model.SetOptions(o => o.RetainAssignedValues = true).Create<Owner>();
    }
}";

            var harness = GeneratorTestHarness.Run(source);
            harness.CompilationErrors.Should().BeEmpty();

            var assembly = harness.EmitAndLoad();
            var ownerType = assembly.GetType("Sample.Owner", throwOnError: true)!;
            var dogType = assembly.GetType("Sample.Dog", throwOnError: true)!;
            var owner = InvokeCaller(assembly);

            // With the option on, the more derived instance assigned by the constructor is retained
            // rather than replaced with a generated value of the less derived member type.
            ownerType.GetProperty("Pet")!.GetValue(owner).Should().BeOfType(dogType);
        }

        [Fact]
        public void PopulateGuardsMemberAssignmentWithRetainOption()
        {
            const string source = @"
using ModelBuilder;

namespace Sample
{
    public sealed class Item
    {
        public string? Name { get; set; }
    }

    public static class Caller
    {
        public static Item Build() => global::ModelBuilder.Model.Create<Item>();
    }
}";

            var harness = GeneratorTestHarness.Run(source);
            harness.CompilationErrors.Should().BeEmpty();

            // The generated assignment is guarded by the runtime retain option.
            harness.GeneratedSources[0].Should().Contain("context.RetainAssignedValues == false");
        }

        private static object InvokeCaller(Assembly assembly)
        {
            return assembly.GetType("Sample.Caller", throwOnError: true)!
                .GetMethod("Build")!
                .Invoke(null, null)!;
        }

        private static int CountOccurrences(string value, string token)
        {
            var count = 0;
            var index = 0;

            while ((index = value.IndexOf(token, index, StringComparison.Ordinal)) >= 0)
            {
                count++;
                index += token.Length;
            }

            return count;
        }
    }
}
