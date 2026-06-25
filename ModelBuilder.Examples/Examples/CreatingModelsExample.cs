namespace ModelBuilder.Examples.Examples
{
    using System;
    using ModelBuilder;
    using ModelBuilder.Examples.Models;

    /// <summary>
    ///     The <see cref="CreatingModelsExample" /> class
    ///     shows the basic build entry points: a typed root, value-source roots, and building by a
    ///     runtime <see cref="Type" />.
    /// </summary>
    public static class CreatingModelsExample
    {
        /// <summary>
        ///     Runs the creating-a-model examples and writes the results to the console.
        /// </summary>
        public static void Run()
        {
            Console.WriteLine("== Creating a model ==");

            // Build a class and the whole graph reachable from it.
            var person = Model.Create<Person>();

            Console.WriteLine($"Person: {person.FirstName} {person.LastName} <{person.Email}>, lives in {person.Address.City}");

            // Value-source types such as enums and primitives build as roots directly.
            var status = Model.Create<OrderStatus>();
            var id = Model.Create<Guid>();

            Console.WriteLine($"Status root: {status}, Guid root: {id}");

            // Build by runtime Type when the type is only known at runtime. The type must still be
            // discoverable at compile time — Person is, because Model.Create<Person>() appears above.
            var runtimeType = typeof(Person);
            var boxed = Model.Create(runtimeType);

            Console.WriteLine($"Runtime-typed build produced a {boxed.GetType().Name}");
            Console.WriteLine();
        }
    }
}
