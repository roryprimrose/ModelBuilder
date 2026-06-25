namespace ModelBuilder.Examples.Models
{
    /// <summary>
    ///     A sample value type built only through its constructors, to show typed construction with
    ///     <c>Model.Construct&lt;Money&gt;().From(...)</c> picking an overload by argument count.
    /// </summary>
    public class Money
    {
        public Money(decimal amount)
        {
            Amount = amount;
            Currency = "USD";
        }

        public Money(decimal amount, string currency)
        {
            Amount = amount;
            Currency = currency;
        }

        public decimal Amount { get; }

        public string Currency { get; }
    }
}
