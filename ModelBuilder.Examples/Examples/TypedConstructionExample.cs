namespace ModelBuilder.Examples.Examples
{
    using System;
    using ModelBuilder;
    using ModelBuilder.Examples.Models;

    /// <summary>
    ///     The <see cref="TypedConstructionExample" /> class
    ///     shows <c>Model.Construct&lt;T&gt;().From(...)</c>, the generated typed <c>From</c> overloads
    ///     that let you pass specific constructor arguments with compile-time checking.
    /// </summary>
    public static class TypedConstructionExample
    {
        /// <summary>
        ///     Runs the typed-construction examples and writes the results to the console.
        /// </summary>
        public static void Run()
        {
            Console.WriteLine("== Typed construction ==");

            // The generator emits a From overload for every public constructor of Person.
            var person = Model.Construct<Person>().From("Fred", "Smith");

            Console.WriteLine($"Constructed person: {person.FirstName} {person.LastName}");

            // From overloads are selected by argument count/type at compile time (no boxing).
            var price = Model.Construct<Money>().From(5);
            var priced = Model.Construct<Money>().From(5, "GBP");

            Console.WriteLine($"Money: {price.Amount} {price.Currency}, {priced.Amount} {priced.Currency}");

            // Constructor parameters that are not exposed as properties still flow into derived
            // members: Contact's Email is built consistently from the firstName/lastName arguments.
            var contact = Model.Construct<Contact>().From("Jane", "Doe");

            Console.WriteLine($"Contact email derived from ctor args: {contact.Email}");

            // Typed construction composes with the rest of the fluent configuration.
            var customerId = Guid.NewGuid();
            var order = Model.Ignoring<Order>(x => x.InternalId)
                .Construct<Order>()
                .From(customerId);

            Console.WriteLine($"Order for customer {order.CustomerId} (InternalId left unset: '{order.InternalId}')");
            Console.WriteLine();
        }
    }
}
