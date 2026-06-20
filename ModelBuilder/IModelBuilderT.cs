namespace ModelBuilder
{
    /// <summary>
    ///     The <see cref="IModelBuilder{T}" /> interface
    ///     defines the single abstraction a generated builder implements to create and populate
    ///     instances of a type.
    /// </summary>
    /// <typeparam name="T">The type the builder creates and populates.</typeparam>
    public interface IModelBuilder<T>
    {
        /// <summary>
        ///     Creates and populates a new instance of <typeparamref name="T" />.
        /// </summary>
        /// <param name="context">The build context for the current build.</param>
        /// <returns>The created instance.</returns>
        T Create(IBuildContext context);

        /// <summary>
        ///     Populates an existing instance of <typeparamref name="T" />.
        /// </summary>
        /// <param name="context">The build context for the current build.</param>
        /// <param name="instance">The instance to populate.</param>
        /// <returns>The populated instance.</returns>
        T Populate(IBuildContext context, T instance);
    }
}
