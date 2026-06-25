namespace ModelBuilder.Examples.Models
{
    /// <summary>
    ///     A concrete <see cref="IShipment" /> used as the mapping target in the examples.
    /// </summary>
    public class GroundShipment : IShipment
    {
        public string Carrier { get; set; }

        public string TrackingNumber { get; set; }
    }
}
