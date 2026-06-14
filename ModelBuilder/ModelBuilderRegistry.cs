namespace ModelBuilder
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     The <see cref="ModelBuilderRegistry" /> class
    ///     maps a runtime <see cref="Type" /> to the builder that creates it, serving the non-generic
    ///     <c>Create(Type)</c> path and polymorphic dispatch without reflection.
    /// </summary>
    internal sealed class ModelBuilderRegistry : IModelBuilderRegistry
    {
        private readonly Dictionary<Type, IModelBuilder> _builders = new Dictionary<Type, IModelBuilder>();

        /// <inheritdoc />
        public void Register(IModelBuilder builder)
        {
            builder = builder ?? throw new ArgumentNullException(nameof(builder));

            _builders[builder.BuildType] = builder;
        }

        /// <summary>
        ///     Attempts to resolve the builder registered for the specified type.
        /// </summary>
        /// <param name="type">The type to resolve a builder for.</param>
        /// <param name="builder">The registered builder, when one exists.</param>
        /// <returns><c>true</c> if a builder is registered; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        internal bool TryGet(Type type, out IModelBuilder? builder)
        {
            type = type ?? throw new ArgumentNullException(nameof(type));

            return _builders.TryGetValue(type, out builder);
        }

        /// <summary>
        ///     Gets the types that have a registered builder.
        /// </summary>
        internal IReadOnlyCollection<Type> RegisteredTypes => _builders.Keys;
    }
}
