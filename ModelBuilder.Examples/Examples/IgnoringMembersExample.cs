namespace ModelBuilder.Examples.Examples
{
    using System;
    using ModelBuilder;
    using ModelBuilder.Examples.Models;

    /// <summary>
    ///     The <see cref="IgnoringMembersExample" /> class
    ///     shows the three ignore forms: a strongly typed member expression, a type-and-name pair, and a
    ///     predicate applied to any member across the graph.
    /// </summary>
    public static class IgnoringMembersExample
    {
        /// <summary>
        ///     Runs the ignoring-members examples and writes the results to the console.
        /// </summary>
        public static void Run()
        {
            Console.WriteLine("== Ignoring members ==");

            // Per-build, strongly typed: leave FirstName at its default.
            var person = Model.Ignoring<Person>(x => x.FirstName).Create<Person>();

            Console.WriteLine($"FirstName ignored (left '{person.FirstName}'), LastName built '{person.LastName}'");

            // The rule applies wherever the member appears, including nested types.
            var withAddress = Model.Ignoring<Address>(x => x.AddressLine1).Create<Person>();

            Console.WriteLine($"Nested Address.AddressLine1 ignored (left '{withAddress.Address.AddressLine1}')");

            // Non-generic by type and member name.
            var request = Model.Ignoring(typeof(Request), nameof(Request.Data)).Create<Request>();

            Console.WriteLine($"Request.Data ignored (left '{request.Data}'), Reference built '{request.Reference}'");

            // Predicate across all types: ignore every member whose name ends in "Internal".
            var ignoredInternal = Model.IgnoringAny(member => member.Name.EndsWith("Internal", StringComparison.Ordinal))
                .Create<Request>();

            Console.WriteLine($"SecretInternal ignored (left '{ignoredInternal.SecretInternal}')");
            Console.WriteLine();
        }
    }
}
