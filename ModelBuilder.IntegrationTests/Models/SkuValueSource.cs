namespace ModelBuilder.IntegrationTests.Models
{
    using ModelBuilder;

    /// <summary>
    ///     A reusable <see cref="IValueSource{T}" /> class (as opposed to an inline
    ///     <see cref="DelegateValueSource{T}" />) used to verify a promoted, named value source is
    ///     matched and invoked.
    /// </summary>
    public sealed class SkuValueSource : IValueSource<string>
    {
        /// <inheritdoc />
        public string Create(IBuildContext context, in BuildTarget target)
        {
            return "SKU-" + context.Random.NextInt32(10000, 99999);
        }
    }
}
