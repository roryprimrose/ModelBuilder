namespace ModelBuilder
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using ModelBuilder.TypeCreators;

    /// <summary>
    ///     The <see cref="BuildConfigurationExtensions" />
    ///     class provides extension methods for the <see cref="IBuildConfiguration" /> interface.
    /// </summary>
    public static partial class BuildConfigurationExtensions
    {
        /// <summary>
        ///     Adds a new type creator to the configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="typeCreator">The type creator.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="typeCreator" /> parameter is <c>null</c>.</exception>
        public static IBuildConfiguration Add(this IBuildConfiguration configuration, ITypeCreator typeCreator)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (typeCreator == null)
            {
                throw new ArgumentNullException(nameof(typeCreator));
            }

            configuration.TypeCreators.Add(typeCreator);

            return configuration;
        }

        /// <summary>
        ///     Adds a new type creator to the configuration.
        /// </summary>
        /// <typeparam name="T">The type of type creator to add.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildConfiguration AddTypeCreator<T>(this IBuildConfiguration configuration)
            where T : ITypeCreator, new()
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var creator = new T();

            configuration.TypeCreators.Add(creator);

            return configuration;
        }

        /// <summary>
        ///     Removes type creators from the configuration that match the specified type.
        /// </summary>
        /// <typeparam name="T">The type of type creator to remove.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildConfiguration RemoveTypeCreator<T>(this IBuildConfiguration configuration)
            where T : ITypeCreator
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var itemsToRemove = configuration.TypeCreators.Where(x => x.GetType().IsAssignableFrom(typeof(T))).ToList();

            foreach (var rule in itemsToRemove)
            {
                configuration.TypeCreators.Remove(rule);
            }

            return configuration;
        }

        /// <summary>
        ///     Updates a type creator.
        /// </summary>
        /// <typeparam name="T">The type of type creator being updated.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <param name="action">The action to run against the type creator.</param>
        /// <returns>The build configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> parameter is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">
        ///     The <typeparamref name="T" /> type creator was not found in the build
        ///     configuration.
        /// </exception>
        public static IBuildConfiguration UpdateTypeCreator<T>(this IBuildConfiguration configuration,
            Action<T> action)
            where T : ITypeCreator
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var targetType = typeof(T);
            var typeCreator = configuration.TypeCreators.OfType<T>().FirstOrDefault(x => x.GetType() == targetType);

            if (typeCreator == null)
            {
                throw new InvalidOperationException(
                    $"TypeCreator {targetType.FullName} does not exist in the BuildConfiguration");
            }

            action(typeCreator);

            return configuration;
        }
    }
}