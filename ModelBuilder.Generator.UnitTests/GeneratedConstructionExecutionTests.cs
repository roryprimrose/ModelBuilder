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

            // The Create path routes through the suppression helper instead of the all-members Populate.
            harness.GeneratedSources[0].Should().Contain("PopulateAfterConstruction");
        }

        [Fact]
        public void ConstructFromRejectsWrongArgumentTypeAtCompileTime()
        {
            const string source = @"
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
