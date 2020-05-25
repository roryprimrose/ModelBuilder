namespace ModelBuilder
{
    /// <summary>
    ///     The <see cref="IExecuteStrategy{T}" />
    ///     interface defines the members for creating and populating <typeparamref name="T" /> instances.
    /// </summary>
    /// <typeparam name="T">The type to create or populate.</typeparam>
    public interface IExecuteStrategy<T> : IExecuteStrategy
    {
        /// <summary>
        ///     Creates a new instance of <typeparamref name="T" /> using any specified arguments.
        /// </summary>
        /// <param name="args">The constructor arguments of the type.</param>
        /// <returns>A new instance of <typeparamref name="T" /></returns>
#if NETSTANDARD2_1
        [return: System.Diagnostics.CodeAnalysis.MaybeNull]
#endif
        T Create(params object?[]? args);

        /// <summary>
        ///     Populates values onto settable properties of the specified instance.
        /// </summary>
        /// <param name="instance">The instance to populate.</param>
        /// <returns>A new instance of <typeparamref name="T" /></returns>
        T Populate(T instance);
    }
}