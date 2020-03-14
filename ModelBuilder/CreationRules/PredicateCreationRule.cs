namespace ModelBuilder.CreationRules
{
    using System;
    using System.Reflection;

    /// <summary>
    ///     The <see cref="PredicateCreationRule" />
    ///     class is used to provide a creation rule based on predicate matches on types, parameter or properties.
    /// </summary>
    public class PredicateCreationRule : ICreationRule
    {
        private readonly Predicate<ParameterInfo> _parameterPredicate;
        private readonly Predicate<PropertyInfo> _propertyPredicate;
        private readonly Predicate<Type> _typePredicate;
        private readonly Func<object> _valueGenerator;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PredicateCreationRule" /> class.
        /// </summary>
        /// <param name="predicate">The predicate to evaluate.</param>
        /// <param name="value">The value that the rule returns.</param>
        /// <param name="priority">The priority to apply to the rule.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate" /> parameter is <c>null</c>.</exception>
        public PredicateCreationRule(Predicate<Type> predicate, object value, int priority) : this(predicate,
            () => value, priority)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PredicateCreationRule" /> class.
        /// </summary>
        /// <param name="predicate">The predicate to evaluate.</param>
        /// <param name="valueGenerator">The value generator used to build the value returned by the rule.</param>
        /// <param name="priority">The priority to apply to the rule.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="valueGenerator" /> parameter is <c>null</c>.</exception>
        public PredicateCreationRule(Predicate<Type> predicate, Func<object> valueGenerator, int priority)
        {
            _typePredicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            _valueGenerator = valueGenerator ?? throw new ArgumentNullException(nameof(valueGenerator));

            Priority = priority;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PredicateCreationRule" /> class.
        /// </summary>
        /// <param name="predicate">The predicate to evaluate.</param>
        /// <param name="value">The value that the rule returns.</param>
        /// <param name="priority">The priority to apply to the rule.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate" /> parameter is <c>null</c>.</exception>
        public PredicateCreationRule(Predicate<ParameterInfo> predicate, object value, int priority) : this(predicate,
            () => value, priority)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PredicateCreationRule" /> class.
        /// </summary>
        /// <param name="predicate">The predicate to evaluate.</param>
        /// <param name="valueGenerator">The value generator used to build the value returned by the rule.</param>
        /// <param name="priority">The priority to apply to the rule.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="valueGenerator" /> parameter is <c>null</c>.</exception>
        public PredicateCreationRule(Predicate<ParameterInfo> predicate, Func<object> valueGenerator, int priority)
        {
            _parameterPredicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            _valueGenerator = valueGenerator ?? throw new ArgumentNullException(nameof(valueGenerator));

            Priority = priority;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PredicateCreationRule" /> class.
        /// </summary>
        /// <param name="predicate">The predicate to evaluate.</param>
        /// <param name="value">The value that the rule returns.</param>
        /// <param name="priority">The priority to apply to the rule.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate" /> parameter is <c>null</c>.</exception>
        public PredicateCreationRule(Predicate<PropertyInfo> predicate, object value, int priority) : this(predicate,
            () => value, priority)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PredicateCreationRule" /> class.
        /// </summary>
        /// <param name="predicate">The predicate to evaluate.</param>
        /// <param name="valueGenerator">The value generator used to build the value returned by the rule.</param>
        /// <param name="priority">The priority to apply to the rule.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="valueGenerator" /> parameter is <c>null</c>.</exception>
        public PredicateCreationRule(Predicate<PropertyInfo> predicate, Func<object> valueGenerator, int priority)
        {
            _propertyPredicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            _valueGenerator = valueGenerator ?? throw new ArgumentNullException(nameof(valueGenerator));

            Priority = priority;
        }

        /// <inheritdoc />
        public object Create(IExecuteStrategy executeStrategy, Type type)
        {
            return _valueGenerator();
        }

        /// <inheritdoc />
        public object Create(IExecuteStrategy executeStrategy, PropertyInfo propertyInfo)
        {
            return _valueGenerator();
        }

        /// <inheritdoc />
        public object Create(IExecuteStrategy executeStrategy, ParameterInfo parameterInfo)
        {
            return _valueGenerator();
        }

        /// <inheritdoc />
        public bool IsMatch(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (_typePredicate == null)
            {
                return false;
            }

            return _typePredicate(type);
        }

        /// <inheritdoc />
        public bool IsMatch(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            if (_propertyPredicate == null)
            {
                return false;
            }

            return _propertyPredicate(propertyInfo);
        }

        /// <inheritdoc />
        public bool IsMatch(ParameterInfo parameterInfo)
        {
            if (parameterInfo == null)
            {
                throw new ArgumentNullException(nameof(parameterInfo));
            }

            if (_parameterPredicate == null)
            {
                return false;
            }

            return _parameterPredicate(parameterInfo);
        }

        /// <inheritdoc />
        public int Priority { get; }
    }
}