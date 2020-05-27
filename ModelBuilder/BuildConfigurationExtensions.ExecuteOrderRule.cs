namespace ModelBuilder
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using ModelBuilder.ExecuteOrderRules;

    /// <summary>
    ///     The <see cref="BuildConfigurationExtensions" />
    ///     class provides extension methods for the <see cref="IBuildConfiguration" /> interface.
    /// </summary>
    public static partial class BuildConfigurationExtensions
    {
        /// <summary>
        ///     Adds a new execute order rule to the configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="rule">The rule.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="rule" /> parameter is <c>null</c>.</exception>
        public static IBuildConfiguration Add(this IBuildConfiguration configuration, IExecuteOrderRule rule)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (rule == null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            configuration.ExecuteOrderRules.Add(rule);

            return configuration;
        }

        /// <summary>
        ///     Adds a new execute order rule to the configuration.
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
        public static IBuildConfiguration AddExecuteOrderRule<T>(this IBuildConfiguration configuration)
            where T : IExecuteOrderRule, new()
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var rule = new T();

            configuration.ExecuteOrderRules.Add(rule);

            return configuration;
        }

        /// <summary>
        ///     Adds a new execute order rule to the configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <typeparam name="T">The type that holds the property.</typeparam>
        /// <param name="expression">The expression that identifies a property on <typeparamref name="T" /></param>
        /// <param name="priority">The priority of the rule.</param>
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
        public static IBuildConfiguration AddExecuteOrderRule<T>(
            this IBuildConfiguration configuration,
            Expression<Func<T, object>> expression,
            int priority)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var rule = new ExpressionExecuteOrderRule<T>(expression, priority);

            configuration.ExecuteOrderRules.Add(rule);

            return configuration;
        }

        /// <summary>
        ///     Adds a new <see cref="RegexExecuteOrderRule" /> to the configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="expression">The expression that matches a property name.</param>
        /// <param name="priority">The priority of the rule.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="expression" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildConfiguration AddExecuteOrderRule(
            this IBuildConfiguration configuration,
            Regex expression,
            int priority)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var rule = new RegexExecuteOrderRule(expression, priority);

            configuration.ExecuteOrderRules.Add(rule);

            return configuration;
        }

        /// <summary>
        ///     Adds a new <see cref="PropertyPredicateExecuteOrderRule" /> to the configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="predicate">The predicate that matches on a property.</param>
        /// <param name="priority">The priority of the rule.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate" /> parameter is <c>null</c>.</exception>
        public static IBuildConfiguration AddExecuteOrderRule(
            this IBuildConfiguration configuration,
            Predicate<PropertyInfo> predicate,
            int priority)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            var rule = new PropertyPredicateExecuteOrderRule(predicate, priority);

            configuration.ExecuteOrderRules.Add(rule);

            return configuration;
        }

        /// <summary>
        ///     Removes execute order rules from the configuration that match the specified type.
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
        public static IBuildConfiguration RemoveExecuteOrderRule<T>(this IBuildConfiguration configuration)
            where T : IExecuteOrderRule
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var itemsToRemove = configuration.ExecuteOrderRules.Where(x => x.GetType().IsAssignableFrom(typeof(T)))
                .ToList();

            foreach (var rule in itemsToRemove)
            {
                configuration.ExecuteOrderRules.Remove(rule);
            }

            return configuration;
        }

        /// <summary>
        ///     Updates an execute order rule.
        /// </summary>
        /// <typeparam name="T">The type of execute order rule being updated.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <param name="action">The action to run against the rule.</param>
        /// <returns>The build configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="action" /> parameter is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">
        ///     The <typeparamref name="T" /> execute order rule was not found in the build
        ///     configuration.
        /// </exception>
        public static IBuildConfiguration UpdateExecuteOrderRule<T>(this IBuildConfiguration configuration,
            Action<T> action)
            where T : IExecuteOrderRule
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
            var rule = configuration.ExecuteOrderRules.OfType<T>().FirstOrDefault(x => x.GetType() == targetType);

            if (rule == null)
            {
                throw new InvalidOperationException(
                    $"ExecuteOrderRule {targetType.FullName} does not exist in the BuildConfiguration");
            }

            action(rule);

            return configuration;
        }
    }
}