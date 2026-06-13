namespace ModelBuilder.vNext
{
    using System;

    /// <summary>
    ///     The <see cref="Model" /> class
    ///     is the static entry point for the vNext source-generated engine. It dispatches to the
    ///     generated builders through the typed-static slots on the generic path and through the
    ///     registry on the runtime-<see cref="Type" /> path.
    /// </summary>
    public static class Model
    {
        private static readonly ModelBuilderRegistry _registry = new ModelBuilderRegistry();

        /// <summary>
        ///     Creates a populated instance of the specified type.
        /// </summary>
        /// <param name="instanceType">The type to create.</param>
        /// <param name="args">The optional constructor arguments.</param>
        /// <returns>The created instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="instanceType" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ModelBuildException">No builder has been generated for the type.</exception>
        public static object Create(Type instanceType, params object?[]? args)
        {
            instanceType = instanceType ?? throw new ArgumentNullException(nameof(instanceType));

            if (_registry.TryGet(instanceType, out var builder) == false
                || builder is null)
            {
                throw NoBuilder(instanceType);
            }

            var context = NewContext();

            using (context.EnterRoot(instanceType))
            {
                return builder.Create(context, args);
            }
        }

        /// <summary>
        ///     Creates a populated instance of <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The type to create.</typeparam>
        /// <param name="args">The optional constructor arguments.</param>
        /// <returns>The created instance.</returns>
        /// <exception cref="ModelBuildException">No builder has been generated for the type.</exception>
        public static T Create<T>(params object?[]? args)
        {
            var builder = ModelBuilderSlot<T>.Instance ?? throw NoBuilder(typeof(T));

            var context = NewContext();

            using (context.EnterRoot(typeof(T)))
            {
                return builder.Create(context, args);
            }
        }

        /// <summary>
        ///     Populates an existing instance of <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The type to populate.</typeparam>
        /// <param name="instance">The instance to populate.</param>
        /// <returns>The populated instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="instance" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ModelBuildException">No builder has been generated for the type.</exception>
        public static T Populate<T>(T instance)
        {
            if (instance is null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            var builder = ModelBuilderSlot<T>.Instance ?? throw NoBuilder(typeof(T));

            var context = NewContext();

            using (context.EnterRoot(typeof(T)))
            {
                return builder.Populate(context, instance);
            }
        }

        private static BuildContext NewContext()
        {
            return new BuildContext(new RandomSource());
        }

        private static ModelBuildException NoBuilder(Type type)
        {
            var message = "No builder was generated for '"
                          + type.FullName
                          + "'. Make it discoverable: call Model.Create<"
                          + type.Name
                          + ">() somewhere, add a Mapping<,> to it, or annotate it with [GenerateModelBuilder].";

            return new ModelBuildException(message, FailureKind.NoBuilderForType);
        }

        /// <summary>
        ///     Gets the registry that maps a runtime <see cref="Type" /> to its generated builder for the
        ///     non-generic <see cref="Create(Type, object[])" /> path.
        /// </summary>
        public static ModelBuilderRegistry Registry => _registry;
    }
}
