namespace ModelBuilder
{
    using System;

    /// <summary>
    /// The <see cref="Extensions"/>
    /// class provides extension methods for the <see cref="IExecuteStrategy{T}"/> interface.
    /// </summary>
    public static class ExecuteStrategyExtensions
    {
        /// <summary>
        /// Creates an instance of <typeparamref name="T"/> using the specified execute strategy.
        /// </summary>
        /// <typeparam name="T">The type of instance to create.</typeparam>
        /// <param name="executeStrategy">The execute strategy to create the instance with.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy"/> parameter is null.</exception>
        public static T Create<T>(this IExecuteStrategy<T> executeStrategy)
        {
            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            return executeStrategy.CreateWith();
        }
    }
}