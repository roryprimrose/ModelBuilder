namespace ModelBuilder
{
    using System;

    /// <summary>
    ///     The <see cref="IModelBuilder" /> interface
    ///     defines the non-generic builder surface used for runtime-<see cref="Type" /> dispatch through
    ///     the registry, where an <see cref="object" />-returning entry point is inherent.
    /// </summary>
    public interface IModelBuilder
    {
        /// <summary>
        ///     Creates and populates a new instance of the built type.
        /// </summary>
        /// <param name="context">The build context for the current build.</param>
        /// <param name="args">The optional constructor arguments.</param>
        /// <returns>The created instance.</returns>
        object Create(IBuildContext context, params object?[]? args);

        /// <summary>
        ///     Populates an existing instance of the built type.
        /// </summary>
        /// <param name="context">The build context for the current build.</param>
        /// <param name="instance">The instance to populate.</param>
        /// <param name="args">The optional constructor arguments.</param>
        /// <returns>The populated instance.</returns>
        object Populate(IBuildContext context, object instance, object?[]? args = null);

        /// <summary>
        ///     Gets the type this builder creates.
        /// </summary>
        Type BuildType { get; }
    }
}

