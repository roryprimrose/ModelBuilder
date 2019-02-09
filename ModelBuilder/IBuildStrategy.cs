namespace ModelBuilder
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    ///     The <see cref="IBuildStrategy" />
    ///     interface defines the members used to create and populate instances.
    /// </summary>
    public interface IBuildStrategy : IBuildConfiguration
    {
        /// <summary>
        ///     Gets the build log for items created by this strategy.
        /// </summary>
        [SuppressMessage("Microsoft.Design",
            "CA1024:UsePropertiesWhereAppropriate",
            Justification = "A method is used to encourage returning a new instance for each invocation.")]
        IBuildLog GetBuildLog();

        /// <summary>
        ///     Gets an <see cref="IExecuteStrategy{T}" /> for the specified <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The type of instance to generate with the strategy.</typeparam>
        /// <returns>A new execute strategy.</returns>
        IExecuteStrategy<T> GetExecuteStrategy<T>();
    }
}