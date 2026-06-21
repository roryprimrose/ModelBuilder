namespace ModelBuilder.AotValidation
{
    using System;
    using System.Collections.Generic;
    using ModelBuilder;

    /// <summary>
    ///     The <see cref="Program" /> class
    ///     exercises a representative slice of the v9 engine so the trim and AOT analyzers can prove
    ///     the build path is reflection-free. It is not a functional test; it exists to fail the build
    ///     if any AOT/trim warning is introduced.
    /// </summary>
    internal static class Program
    {
        private static int Main()
        {
            var order = Model.Create<Order>();
            var status = Model.Create<Status>();
            var nullable = Model.Create<NullableHolder>();
            var configured = Model.Ignoring<Order>(o => o.Reference).Create<Order>();
            var mapped = Model.Mapping<IShipment, GroundShipment>().Create<Parcel>();

            Console.WriteLine(order.Customer?.FirstName);
            Console.WriteLine(status);
            Console.WriteLine(nullable.OptionalCount);
            Console.WriteLine(configured.Reference);
            Console.WriteLine(mapped.Shipment?.GetType().Name);

            return 0;
        }
    }

    internal enum Status
    {
        Pending,
        Active,
        Closed
    }

    internal sealed class Customer
    {
        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }
    }

    internal sealed class Order
    {
        public Guid Id { get; set; }

        public int Reference { get; set; }

        public Status Status { get; set; }

        public Customer? Customer { get; set; }

        public List<string>? Tags { get; set; }
    }

    internal sealed class NullableHolder
    {
        public int? OptionalCount { get; set; }

        public Status? OptionalStatus { get; set; }
    }

    internal interface IShipment
    {
        string? Carrier { get; set; }
    }

    internal sealed class GroundShipment : IShipment
    {
        public string? Carrier { get; set; }

        public int Days { get; set; }
    }

    internal sealed class Parcel
    {
        public IShipment? Shipment { get; set; }
    }
}
