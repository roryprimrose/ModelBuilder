namespace ModelBuilder
{
    using System;

    /// <summary>
    ///     The <see cref="Extensions" />
    ///     class provides extension methods for the <see cref="IExecuteStrategy{T}" /> interface.
    /// </summary>
    public static class ExecuteStrategyExtensions
    {
        /// <summary>
        ///     Creates an instance of <typeparamref name="T" /> using the specified execute strategy.
        /// </summary>
        /// <typeparam name="T">The type of instance to create.</typeparam>
        /// <param name="executeStrategy">The execute strategy to create the instance with.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is null.</exception>
        public static T Create<T>(this IExecuteStrategy<T> executeStrategy)
        {
            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            return executeStrategy.Create();
        }

        /// <summary>
        ///     Creates an instance of a type using the specified execute strategy.
        /// </summary>
        /// <param name="executeStrategy">The execute strategy to create the instance with.</param>
        /// <param name="instanceType">The type of instance to create.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="instanceType" /> parameter is null.</exception>
        public static object Create(this IExecuteStrategy executeStrategy, Type instanceType)
        {
            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            if (instanceType == null)
            {
                throw new ArgumentNullException(nameof(instanceType));
            }

            return executeStrategy.Create(instanceType);
        }
    }
}