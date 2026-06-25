namespace ModelBuilder.Examples.Models
{
    using System;

    /// <summary>
    ///     A sample order model. It has a parameterless constructor (used by <c>Model.Create</c>) and a
    ///     constructor that takes the customer id (used by <c>Model.Construct&lt;Order&gt;().From(...)</c>).
    /// </summary>
    public class Order
    {
        public Order()
        {
        }

        public Order(Guid customerId)
        {
            CustomerId = customerId;
        }

        public Guid CustomerId { get; set; }

        public OrderStatus Status { get; set; }

        public string Reference { get; set; } = string.Empty;

        public string InternalId { get; set; } = string.Empty;
    }
}
