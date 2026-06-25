namespace ModelBuilder.Examples.Examples
{
    using System;
    using ModelBuilder;
    using ModelBuilder.Examples.Models;

    /// <summary>
    ///     The <see cref="CustomValueSourcesExample" /> class
    ///     shows registering custom value sources — for every value of a type, scoped to specific member
    ///     names, and as a reusable <see cref="IValueSource{T}" /> class.
    /// </summary>
    public static class CustomValueSourcesExample
    {
        /// <summary>
        ///     Runs the custom-value-source examples and writes the results to the console.
        /// </summary>
        public static void Run()
        {
            Console.WriteLine("== Custom value sources ==");

            // For every value of a type: every OrderStatus in the graph becomes Pending.
            var pendingSource = new DelegateValueSource<OrderStatus>(c => OrderStatus.Pending);
            var order = Model.AddValueSource(pendingSource).Create<Order>();

            Console.WriteLine($"Forced status: {order.Status}");

            // Scoped to member names: only members named Reference get this value.
            var referenceSource = new DelegateValueSource<string>(c => "REF-" + c.Random.NextInt32(1000, 9999));
            var scoped = Model.AddValueSource(referenceSource, "Reference").Create<Order>();

            Console.WriteLine($"Scoped Reference: {scoped.Reference}");

            // A reusable IValueSource<T> class, matched to the Sku member.
            var product = Model.AddValueSource(new SkuValueSource(), "Sku").Create<Product>();

            Console.WriteLine($"Product SKU from reusable source: {product.Sku}");

            // The two-argument factory shape receives the BuildTarget so one source can vary output.
            var labelled = new DelegateValueSource<string>((context, target) =>
                target.MemberName is null ? "value" : target.MemberName + "-" + context.Random.NextInt32(1, 999));
            var named = Model.AddValueSource(labelled, "Name").Create<Product>();

            Console.WriteLine($"Target-aware Name: {named.Name}");
            Console.WriteLine();
        }
    }
}
