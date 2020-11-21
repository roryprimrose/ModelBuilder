namespace ModelBuilder
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    ///     The <see cref="BuildConfigurationExtensions" />
    ///     class provides extension methods for the <see cref="IBuildConfiguration" /> interface.
    /// </summary>
    public static partial class BuildConfigurationExtensions
    {
        /// <summary>
        ///     Adds a new type mapping rule to the configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="rule">The rule.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="rule" /> parameter is <c>null</c>.</exception>
        public static IBuildConfiguration Add(this IBuildConfiguration configuration, TypeMappingRule rule)
        {
            configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            rule = rule ?? throw new ArgumentNullException(nameof(rule));

            configuration.TypeMappingRules.Add(rule);

            return configuration;
        }

        /// <summary>
        ///     Adds a new type mapping rule to the configuration.
        /// </summary>
        /// <typeparam name="T">The type of rule to add.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildConfiguration AddTypeMappingRule<T>(this IBuildConfiguration configuration)
            where T : TypeMappingRule, new()
        {
            configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            var rule = new T();

            configuration.TypeMappingRules.Add(rule);

            return configuration;
        }

        /// <summary>
        ///     Adds a new type mapping rule to the configuration.
        /// </summary>
        /// <typeparam name="TSource">The source type to match.</typeparam>
        /// <typeparam name="TTarget">The target type to create.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildConfiguration AddTypeMappingRule<TSource, TTarget>(this IBuildConfiguration configuration)
        {
            configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            var rule = new TypeMappingRule(typeof(TSource), typeof(TTarget));

            configuration.TypeMappingRules.Add(rule);

            return configuration;
        }

        /// <summary>
        ///     Appends a new <see cref="TypeMappingRule" /> to the build configuration using the specified types.
        /// </summary>
        /// <typeparam name="TSource">The source type to use for type mapping.</typeparam>
        /// <typeparam name="TTarget">The target type to use for type mapping.</typeparam>
        /// <param name="buildConfiguration">The build configuration to update.</param>
        /// <returns>The build configuration with the new rule.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="buildConfiguration" /> parameter is <c>null</c>.</exception>
        public static IBuildConfiguration Mapping<TSource, TTarget>(this IBuildConfiguration buildConfiguration)
        {
            buildConfiguration = buildConfiguration ?? throw new ArgumentNullException(nameof(buildConfiguration));

            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);

            var rule = new TypeMappingRule(sourceType, targetType);

            return buildConfiguration.Add(rule);
        }

        /// <summary>
        ///     Removes type mapping rules from the configuration that match the specified type.
        /// </summary>
        /// <typeparam name="T">The type of rule to remove.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildConfiguration RemoveTypeMappingRule<T>(this IBuildConfiguration configuration)
            where T : TypeMappingRule
        {
            configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            var itemsToRemove = configuration.TypeMappingRules.Where(x => x.GetType().IsAssignableFrom(typeof(T)))
                .ToList();

            foreach (var rule in itemsToRemove)
            {
                configuration.TypeMappingRules.Remove(rule);
            }

            return configuration;
        }
    }
}