namespace ModelBuilder.Examples.Examples
{
    using System;
    using ModelBuilder;
    using ModelBuilder.Examples.Models;

    /// <summary>
    ///     The <see cref="ExampleModule" /> class
    ///     packages reusable build configuration — a mapping, an ignore rule, a predicate ignore and an
    ///     option override — so several builds can share the exact same setup.
    /// </summary>
    public sealed class ExampleModule : IConfigurationModule
    {
        /// <inheritdoc />
        public void Configure(IBuildConfiguration configuration)
        {
            // Use a concrete type wherever IShipment is needed.
            configuration.AddMapping<IShipment, GroundShipment>();

            // Never populate Request.Data.
            configuration.Ignore(typeof(Request), nameof(Request.Data));

            // Ignore any member whose name ends in "Internal", across all types.
            configuration.IgnoreAny(member => member.Name.EndsWith("Internal", StringComparison.Ordinal));

            // Keep collections small for every build that uses this module.
            configuration.SetOptions(x => x.MaxCount = 3);
        }
    }
}
