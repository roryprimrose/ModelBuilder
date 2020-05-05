namespace ModelBuilder
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using ModelBuilder.CreationRules;

    /// <summary>
    ///     The <see cref="BuildConfigurationExtensions" />
    ///     class provides extension methods for the <see cref="IBuildConfiguration" /> interface.
    /// </summary>
    public static partial class BuildConfigurationExtensions
    {
        /// <summary>
        ///     Adds a new creation rule to the configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="rule">The rule.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="rule" /> parameter is <c>null</c>.</exception>
        public static IBuildConfiguration Add(this IBuildConfiguration configuration, ICreationRule rule)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (rule == null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            configuration.CreationRules.Add(rule);

            return configuration;
        }

        /// <summary>
        ///     Adds a new creation rule to the configuration.
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
        public static IBuildConfiguration AddCreationRule<T>(this IBuildConfiguration configuration)
            where T : ICreationRule, new()
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var rule = new T();

            configuration.CreationRules.Add(rule);

            return configuration;
        }

        /// <summary>
        ///     Adds a new creation rule to the configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <typeparam name="T">The type that holds the property.</typeparam>
        /// <param name="expression">The expression that identifies a property on <typeparamref name="T" /></param>
        /// <param name="priority">The priority of the rule.</param>
        /// <param name="value">The static value returned by the rule.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="expression" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="expression" /> parameter does not represent a property.</exception>
        /// <exception cref="ArgumentException">
        ///     The <paramref name="expression" /> parameter does not match a property on the type
        ///     to generate.
        /// </exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildConfiguration AddCreationRule<T>(
            this IBuildConfiguration configuration,
            Expression<Func<T, object>> expression,
            int priority,
            object value)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var rule = new ExpressionCreationRule<T>(expression, value, priority);

            configuration.CreationRules.Add(rule);

            return configuration;
        }

        /// <summary>
        ///     Removes creation rules from the configuration that match the specified type.
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
        public static IBuildConfiguration RemoveCreationRule<T>(this IBuildConfiguration configuration)
            where T : ICreationRule
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var itemsToRemove = configuration.CreationRules.Where(x => x.GetType().IsAssignableFrom(typeof(T)))
                .ToList();

            foreach (var rule in itemsToRemove)
            {
                configuration.CreationRules.Remove(rule);
            }

            return configuration;
        }

        /// <summary>
        ///     Updates a creation rule.
        /// </summary>
        /// <typeparam name="T">The type of creation rule being updated.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <param name="action">The action to run against the rule.</param>
        /// <returns>The build configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> parameter is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">
        ///     The <typeparamref name="T" /> creation rule was not found in the build
        ///     configuration.
        /// </exception>
        public static IBuildConfiguration UpdateCreationRule<T>(this IBuildConfiguration configuration,
            Action<T> action)
            where T : ICreationRule
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var rule = configuration.CreationRules.OfType<T>().FirstOrDefault();

            if (rule == null)
            {
                throw new InvalidOperationException(
                    $"CreationRule {typeof(T).FullName} does not exist in the BuildConfiguration");
            }

            action(rule);

            return configuration;
        }
    }
}