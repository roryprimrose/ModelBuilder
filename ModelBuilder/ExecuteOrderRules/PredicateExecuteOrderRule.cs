namespace ModelBuilder.ExecuteOrderRules
{
    using System;
    using System.Reflection;

    /// <summary>
    ///     The <see cref="PredicateExecuteOrderRule" />
    ///     class is used to match the rule to a <see cref="PropertyInfo" /> using a predicate.
    /// </summary>
    public class PredicateExecuteOrderRule : IExecuteOrderRule
    {
        private readonly Predicate<PropertyInfo> _predicate;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PredicateExecuteOrderRule" /> class.
        /// </summary>
        /// <param name="predicate">The predicate to evaluate.</param>
        /// <param name="priority">The execution order priority to apply to the property.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate" /> parameter is <c>null</c>.</exception>
        public PredicateExecuteOrderRule(Predicate<PropertyInfo> predicate, int priority)
        {
            _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            Priority = priority;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="propertyInfo" /> parameter is <c>null</c>.</exception>
        public bool IsMatch(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            return _predicate(propertyInfo);
        }

        /// <inheritdoc />
        public int Priority { get; }
    }
}