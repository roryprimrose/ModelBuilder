namespace ModelBuilder.IntegrationTests.Models
{
    /// <summary>
    ///     Exposes several public constructors of increasing arity so tests can confirm the generator
    ///     selects the greediest constructor for <c>Model.Create&lt;T&gt;()</c> and that
    ///     <c>Model.Construct&lt;T&gt;().From(...)</c> exposes one typed overload per constructor.
    /// </summary>
    public class MultiConstructorWidget
    {
        public MultiConstructorWidget()
        {
            Name = "Default";
        }

        public MultiConstructorWidget(string name)
        {
            Name = name;
        }

        public MultiConstructorWidget(string name, int quantity)
        {
            Name = name;
            Quantity = quantity;
        }

        public MultiConstructorWidget(string name, int quantity, decimal price)
        {
            Name = name;
            Quantity = quantity;
            Price = price;
        }

        public string Name { get; }

        public int Quantity { get; }

        public decimal Price { get; }
    }
}
