namespace ModelBuilder.Examples.Examples
{
    using System;
    using ModelBuilder;
    using ModelBuilder.Examples.Models;

    /// <summary>
    ///     The <see cref="MappingTypesExample" /> class
    ///     shows mapping an abstract or interface member to a concrete type so a graph containing it can
    ///     be built.
    /// </summary>
    public static class MappingTypesExample
    {
        /// <summary>
        ///     Runs the mapping examples and writes the results to the console.
        /// </summary>
        public static void Run()
        {
            Console.WriteLine("== Mapping abstract and interface types ==");

            // Parcel.Shipment is an IShipment; map it to a concrete type so the build can complete.
            var parcel = Model.Mapping<IShipment, GroundShipment>().Create<Parcel>();
            var shipment = parcel.Shipment!;

            Console.WriteLine($"Parcel '{parcel.Label}' shipped via {shipment.Carrier} ({shipment.GetType().Name})");
            Console.WriteLine();
        }
    }
}
