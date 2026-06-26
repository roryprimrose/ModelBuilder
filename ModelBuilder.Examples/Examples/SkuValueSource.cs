namespace ModelBuilder.Examples.Examples
{
    using ModelBuilder;

    /// <summary>
    ///     The <see cref="SkuValueSource" /> class
    ///     is a reusable <see cref="IValueSource{T}" /> that produces a SKU-formatted string. Promoting
    ///     a value source to a class (rather than an inline <see cref="DelegateValueSource{T}" />) gives
    ///     it a descriptive name and a single place to unit-test.
    /// </summary>
    public sealed class SkuValueSource : IValueSource<string>
    {
        /// <inheritdoc />
        public string Create(IBuildContext context, in BuildTarget target)
        {
            var suffix = context.Random.NextInt32(10000, 99999);

            return "SKU-" + suffix;
        }
    }
}
