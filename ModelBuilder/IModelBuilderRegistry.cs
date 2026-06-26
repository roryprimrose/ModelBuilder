namespace ModelBuilder
{
    using System;

    /// <summary>
    ///     The <see cref="IModelBuilderRegistry" /> interface
    ///     maps a runtime <see cref="Type" /> to the builder that creates it, serving the non-generic
    ///     <c>Create(Type)</c> path and polymorphic dispatch without reflection. The generated module
    ///     initializer registers each builder through this interface.
    /// </summary>
    public interface IModelBuilderRegistry
    {
        /// <summary>
        ///     Registers a builder keyed by the type it creates.
        /// </summary>
        /// <param name="builder">The builder to register.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="builder" /> parameter is <c>null</c>.</exception>
        void Register(IModelBuilder builder);
    }
}
