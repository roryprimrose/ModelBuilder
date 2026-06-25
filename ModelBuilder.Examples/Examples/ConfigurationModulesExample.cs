namespace ModelBuilder.Examples.Examples
{
    using System;
    using ModelBuilder;
    using ModelBuilder.Examples.Models;

    /// <summary>
    ///     The <see cref="ConfigurationModulesExample" /> class
    ///     shows applying a reusable <see cref="IConfigurationModule" /> per build, and layering
    ///     modules.
    /// </summary>
    public static class ConfigurationModulesExample
    {
        /// <summary>
        ///     Runs the configuration-module example and writes the results to the console.
        /// </summary>
        public static void Run()
        {
            Console.WriteLine("== Configuration modules ==");

            // The module maps IShipment, ignores Request.Data and *Internal members, and caps counts.
            var parcel = Model.UsingModule<ExampleModule>().Create<Parcel>();

            Console.WriteLine($"Module-built parcel shipped via {parcel.Shipment.Carrier}");

            var request = Model.UsingModule<ExampleModule>().Create<Request>();

            Console.WriteLine($"Module ignored Data ('{request.Data}') and SecretInternal ('{request.SecretInternal}')");
            Console.WriteLine();
        }
    }
}
