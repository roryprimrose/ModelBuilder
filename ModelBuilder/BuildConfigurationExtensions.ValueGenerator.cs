namespace ModelBuilder
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using ModelBuilder.ValueGenerators;

    /// <summary>
    ///     The <see cref="BuildConfigurationExtensions" />
    ///     class provides extension methods for the <see cref="IBuildConfiguration" /> interface.
    /// </summary>
    public static partial class BuildConfigurationExtensions
    {
        /// <summary>
        ///     Adds a new value generator to the configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="valueGenerator">The value generator.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="valueGenerator" /> parameter is <c>null</c>.</exception>
        public static IBuildConfiguration Add(this IBuildConfiguration configuration, IValueGenerator valueGenerator)
        {
            configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            valueGenerator = valueGenerator ?? throw new ArgumentNullException(nameof(valueGenerator));

            configuration.ValueGenerators.Add(valueGenerator);

            return configuration;
        }

        /// <summary>
        ///     Adds a new value generator to the configuration.
        /// </summary>
        /// <typeparam name="T">The type of value generator to add.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildConfiguration AddValueGenerator<T>(this IBuildConfiguration configuration)
            where T : IValueGenerator, new()
        {
            configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            var generator = new T();

            configuration.ValueGenerators.Add(generator);

            return configuration;
        }

        /// <summary>
        ///     Removes value generators from the configuration that match the specified type.
        /// </summary>
        /// <typeparam name="T">The type of value generator to remove.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildConfiguration RemoveValueGenerator<T>(this IBuildConfiguration configuration)
            where T : IValueGenerator
        {
            configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            var itemsToRemove = configuration.ValueGenerators.Where(x => x.GetType().IsAssignableFrom(typeof(T)))
                .ToList();

            foreach (var rule in itemsToRemove)
            {
                configuration.ValueGenerators.Remove(rule);
            }

            return configuration;
        }

        /// <summary>
        ///     Updates a value generator.
        /// </summary>
        /// <typeparam name="T">The type of value generator being updated.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <param name="action">The action to run against the value generator.</param>
        /// <returns>The build configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> parameter is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">
        ///     The <typeparamref name="T" /> value generator was not found in the build
        ///     configuration.
        /// </exception>
        public static IBuildConfiguration UpdateValueGenerator<T>(this IBuildConfiguration configuration,
            Action<T> action)
            where T : IValueGenerator
        {
            configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            action = action ?? throw new ArgumentNullException(nameof(action));

            var targetType = typeof(T);
            var valueGenerator =
                configuration.ValueGenerators.OfType<T>().FirstOrDefault(x => x.GetType() == targetType);

            if (valueGenerator == null)
            {
                throw new InvalidOperationException(
                    $"ValueGenerator {targetType.FullName} does not exist in the BuildConfiguration");
            }

            action(valueGenerator);

            return configuration;
        }
    }
}