namespace ModelBuilder.IntegrationTests.Models
{
    /// <summary>
    ///     The concrete, generic <see cref="IRepository{T}" /> implementation mapped in the open generic
    ///     mapping scenario tests.
    /// </summary>
    public class Repository<T> : IRepository<T>
    {
        public T? Item { get; set; }
    }
}
