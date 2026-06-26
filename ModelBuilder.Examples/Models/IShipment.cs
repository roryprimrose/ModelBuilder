namespace ModelBuilder.Examples.Models
{
    /// <summary>
    ///     The <see cref="IShipment" /> interface
    ///     is a sample abstraction that needs a concrete mapping before a graph containing it can be
    ///     built.
    /// </summary>
    public interface IShipment
    {
        /// <summary>
        ///     Gets or sets the carrier handling the shipment.
        /// </summary>
        string Carrier { get; set; }

        /// <summary>
        ///     Gets or sets the tracking number for the shipment.
        /// </summary>
        string TrackingNumber { get; set; }
    }
}
