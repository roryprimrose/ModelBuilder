namespace ModelBuilder.Examples.Examples
{
    using System;
    using ModelBuilder;
    using ModelBuilder.Data;
    using ModelBuilder.Examples.Models;

    /// <summary>
    ///     The <see cref="BuiltInDataExample" /> class
    ///     shows the entity-style data matched by member name, cross-field consistency, and direct use
    ///     of the embedded <see cref="TestData" /> reference data sets.
    /// </summary>
    public static class BuiltInDataExample
    {
        /// <summary>
        ///     Runs the built-in data examples and writes the results to the console.
        /// </summary>
        public static void Run()
        {
            Console.WriteLine("== Built-in data ==");

            // Name fields, email and location all read naturally and agree with each other.
            var person = Model.Create<Person>();

            Console.WriteLine($"Entity data: {person.FirstName} {person.LastName}, {person.Email}");
            Console.WriteLine($"Consistent location: {person.Address.City}, {person.Address.State}, {person.Address.Country}");

            // The same embedded data is exposed directly through TestData.
            Console.WriteLine($"TestData has {TestData.Companies.Count} companies and {TestData.Locations.Count} locations");

            var index = new Random().Next(0, TestData.Locations.Count);
            var location = TestData.Locations[index];

            Console.WriteLine($"A consistent location row: {location.City}, {location.State}, {location.Country}");
            Console.WriteLine();
        }
    }
}
