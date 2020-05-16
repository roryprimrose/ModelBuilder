namespace ModelBuilder.CreationRules
{
    using System;
    using System.Reflection;

    /// <summary>
    ///     The <see cref="PropertyPredicateCreationRule" />
    ///     class is used to provide a creation rule based on predicate matches on types, parameter or properties.
    /// </summary>
    public class PropertyPredicateCreationRule : ICreationRule
    {
        private readonly Predicate<PropertyInfo> _propertyPredicate;
        private readonly Func<object> _valueGenerator;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PropertyPredicateCreationRule" /> class.
        /// </summary>
        /// <param name="predicate">The predicate to evaluate.</param>
        /// <param name="value">The value that the rule returns.</param>
        /// <param name="priority">The priority to apply to the rule.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate" /> parameter is <c>null</c>.</exception>
        public PropertyPredicateCreationRule(Predicate<PropertyInfo> predicate, object value, int priority) : this(
            predicate,
            () => value, priority)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PropertyPredicateCreationRule" /> class.
        /// </summary>
        /// <param name="predicate">The predicate to evaluate.</param>
        /// <param name="valueGenerator">The value generator used to build the value returned by the rule.</param>
        /// <param name="priority">The priority to apply to the rule.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="valueGenerator" /> parameter is <c>null</c>.</exception>
        public PropertyPredicateCreationRule(Predicate<PropertyInfo> predicate, Func<object> valueGenerator,
            int priority)
        {
            _propertyPredicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            _valueGenerator = valueGenerator ?? throw new ArgumentNullException(nameof(valueGenerator));

            Priority = priority;
        }

        /// <inheritdoc />
        public object? Create(IExecuteStrategy executeStrategy, Type type)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public object? Create(IExecuteStrategy executeStrategy, PropertyInfo propertyInfo)
        {
            return _valueGenerator();
        }

        /// <inheritdoc />
        public object? Create(IExecuteStrategy executeStrategy, ParameterInfo parameterInfo)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public bool IsMatch(Type type)
        {
            return false;
        }

        /// <inheritdoc />
        public bool IsMatch(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            return _propertyPredicate(propertyInfo);
        }

        /// <inheritdoc />
        public bool IsMatch(ParameterInfo parameterInfo)
        {
            return false;
        }

        /// <inheritdoc />
        public int Priority { get; }
    }
}