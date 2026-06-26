namespace ModelBuilder.Examples.Models
{
    /// <summary>
    ///     A sample parcel with an abstract <see cref="IShipment" /> member. Building it with no mapping
    ///     throws at runtime; adding a <c>Mapping&lt;IShipment, GroundShipment&gt;()</c> makes it build.
    /// </summary>
    public class Parcel
    {
        public string Label { get; set; } = string.Empty;

        public IShipment? Shipment { get; set; }
    }
}
