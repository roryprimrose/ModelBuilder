namespace ModelBuilder.Examples.Examples
{
    using System;
    using ModelBuilder;
    using ModelBuilder.Examples.Models;

    /// <summary>
    ///     The <see cref="PopulatingModelsExample" /> class
    ///     shows filling an existing instance in place with <c>Model.Populate</c>, including keeping a
    ///     value already set.
    /// </summary>
    public static class PopulatingModelsExample
    {
        /// <summary>
        ///     Runs the populate examples and writes the results to the console.
        /// </summary>
        public static void Run()
        {
            Console.WriteLine("== Populating a model ==");

            var seeded = new Person
            {
                FirstName = "Jane"
            };

            // Fill every other member but keep the FirstName already set.
            Model.Ignoring<Person>(x => x.FirstName).Populate(seeded);

            Console.WriteLine($"Populated, kept FirstName '{seeded.FirstName}', filled LastName '{seeded.LastName}'");

            var customer = Model.Populate(new Person());

            Console.WriteLine($"Populated fresh instance: {customer.FirstName} {customer.LastName}");
            Console.WriteLine();
        }
    }
}
