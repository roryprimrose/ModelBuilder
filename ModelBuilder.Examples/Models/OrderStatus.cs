namespace ModelBuilder.Examples.Models
{
    /// <summary>
    ///     The <see cref="OrderStatus" /> enum
    ///     is a sample enum used to show that enums build as roots and through custom value sources.
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        ///     The order has been created but not yet processed.
        /// </summary>
        Pending,

        /// <summary>
        ///     The order has been shipped.
        /// </summary>
        Shipped,

        /// <summary>
        ///     The order has been delivered.
        /// </summary>
        Delivered,

        /// <summary>
        ///     The order has been cancelled.
        /// </summary>
        Cancelled
    }
}
