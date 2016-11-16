namespace ModelBuilder
{
    /// <summary>
    ///     The <see cref="IBuildStrategy" />
    ///     interface defines the members used to create and populate instances.
    /// </summary>
    public interface IBuildStrategy : IBuildConfiguration
    {
        /// <summary>
        ///     Gets the build log for items created by this strategy.
        /// </summary>
        IBuildLog GetBuildLog();

        /// <summary>
        ///     Gets an <see cref="IExecuteStrategy{T}" /> for the specified <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The type of instance to generate with the strategy.</typeparam>
        /// <returns>A new execute strategy.</returns>
        IExecuteStrategy<T> GetExecuteStrategy<T>();
    }
}