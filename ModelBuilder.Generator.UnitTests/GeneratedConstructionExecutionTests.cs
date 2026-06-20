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

        public string Email { get; set; } = string.Empty;
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

        public string Email { get; set; } = string.Empty;
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
