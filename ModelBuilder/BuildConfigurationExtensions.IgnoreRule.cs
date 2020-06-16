namespace ModelBuilder
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using ModelBuilder.IgnoreRules;

    /// <summary>
    ///     The <see cref="BuildConfigurationExtensions" />
    ///     class provides extension methods for the <see cref="IBuildConfiguration" /> interface.
    /// </summary>
    public static partial class BuildConfigurationExtensions
    {
        /// <summary>
        ///     Adds a new ignore rule to the configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="rule">The rule.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="rule" /> parameter is <c>null</c>.</exception>
        public static IBuildConfiguration Add(this IBuildConfiguration configuration, IIgnoreRule rule)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (rule == null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            configuration.IgnoreRules.Add(rule);

            return configuration;
        }

        /// <summary>
        ///     Adds a new ignore rule to the configuration.
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
        public static IBuildConfiguration AddIgnoreRule<T>(this IBuildConfiguration configuration)
            where T : IIgnoreRule, new()
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var rule = new T();

            configuration.IgnoreRules.Add(rule);

            return configuration;
        }

        /// <summary>
        ///     Adds a new ignore rule to the configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <typeparam name="T">The type that holds the property.</typeparam>
        /// <param name="expression">The expression that identifies a property on <typeparamref name="T" /></param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="expression" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="expression" /> parameter does not represent a property.</exception>
        /// <exception cref="ArgumentException">
        ///     The <paramref name="expression" /> parameter does not match a property on the type
        ///     to generate.
        /// </exception>
        public static IBuildConfiguration AddIgnoreRule<T>(
            this IBuildConfiguration configuration,
            Expression<Func<T, object?>> expression)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var rule = new ExpressionIgnoreRule<T>(expression);

            configuration.IgnoreRules.Add(rule);

            return configuration;
        }

        /// <summary>
        ///     Adds a new <see cref="PredicateIgnoreRule" /> to the configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="predicate">The predicate that matches on a property.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate" /> parameter is <c>null</c>.</exception>
        public static IBuildConfiguration AddIgnoreRule(
            this IBuildConfiguration configuration,
            Predicate<PropertyInfo> predicate)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            var rule = new PredicateIgnoreRule(predicate);

            configuration.IgnoreRules.Add(rule);

            return configuration;
        }

        /// <summary>
        ///     Adds a new <see cref="RegexIgnoreRule" /> to the configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="expression">The expression that matches a property name.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="expression" /> parameter is <c>null</c>.</exception>
        public static IBuildConfiguration AddIgnoreRule(
            this IBuildConfiguration configuration,
            Regex expression)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var rule = new RegexIgnoreRule(expression);

            configuration.IgnoreRules.Add(rule);

            return configuration;
        }

        /// <summary>
        ///     Adds a new <see cref="RegexIgnoreRule" /> to the configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="expression">The expression that matches a property name.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="expression" /> parameter is <c>null</c> or empty.</exception>
        public static IBuildConfiguration AddIgnoreRule(
            this IBuildConfiguration configuration,
            string expression)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (string.IsNullOrEmpty(expression))
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var rule = new RegexIgnoreRule(expression);

            configuration.IgnoreRules.Add(rule);

            return configuration;
        }

        /// <summary>
        ///     Appends a new <see cref="IIgnoreRule" /> to the build configuration using the specified expression.
        /// </summary>
        /// <typeparam name="T">The type of instance that matches the rule.</typeparam>
        /// <param name="buildConfiguration">The build configuration to update.</param>
        /// <param name="expression">The expression that identifies a property on <typeparamref name="T" /></param>
        /// <returns>The build configuration with the new rule.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="buildConfiguration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="expression" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="expression" /> parameter does not represent a property.</exception>
        /// <exception cref="ArgumentException">
        ///     The <paramref name="expression" /> parameter does not match a property on the type
        ///     to generate.
        /// </exception>
        public static IBuildConfiguration Ignoring<T>(
            this IBuildConfiguration buildConfiguration,
            Expression<Func<T, object?>> expression)
        {
            if (buildConfiguration == null)
            {
                throw new ArgumentNullException(nameof(buildConfiguration));
            }

            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var rule = new ExpressionIgnoreRule<T>(expression);

            return Add(buildConfiguration, rule);
        }

        /// <summary>
        ///     Removes ignore rules from the configuration that match the specified type.
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
        public static IBuildConfiguration RemoveIgnoreRule<T>(this IBuildConfiguration configuration)
            where T : IIgnoreRule
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var itemsToRemove = configuration.IgnoreRules.Where(x => x.GetType().IsAssignableFrom(typeof(T))).ToList();

            foreach (var rule in itemsToRemove)
            {
                configuration.IgnoreRules.Remove(rule);
            }

            return configuration;
        }

        /// <summary>
        ///     Updates an ignore rule.
        /// </summary>
        /// <typeparam name="T">The type of ignore rule being updated.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <param name="action">The action to run against the rule.</param>
        /// <returns>The build configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> parameter is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">
        ///     The <typeparamref name="T" /> ignore rule was not found in the build
        ///     configuration.
        /// </exception>
        public static IBuildConfiguration UpdateIgnoreRule<T>(this IBuildConfiguration configuration,
            Action<T> action)
            where T : IIgnoreRule
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
            var rule = configuration.IgnoreRules.OfType<T>().FirstOrDefault(x => x.GetType() == targetType);

            if (rule == null)
            {
                throw new InvalidOperationException(
                    $"IgnoreRule {targetType.FullName} does not exist in the BuildConfiguration");
            }

            action(rule);

            return configuration;
        }
    }
}