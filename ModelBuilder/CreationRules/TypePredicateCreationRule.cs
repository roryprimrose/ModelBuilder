namespace ModelBuilder.CreationRules
{
    using System;
    using System.Reflection;

    /// <summary>
    ///     The <see cref="TypePredicateCreationRule" />
    ///     class is used to provide a creation rule based on predicate matches on types, parameter or properties.
    /// </summary>
    public class TypePredicateCreationRule : ICreationRule
    {
        private readonly Predicate<Type> _typePredicate;
        private readonly Func<object> _valueGenerator;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TypePredicateCreationRule" /> class.
        /// </summary>
        /// <param name="predicate">The predicate to evaluate.</param>
        /// <param name="value">The value that the rule returns.</param>
        /// <param name="priority">The priority to apply to the rule.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate" /> parameter is <c>null</c>.</exception>
        public TypePredicateCreationRule(Predicate<Type> predicate, object value, int priority) : this(predicate,
            () => value, priority)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="TypePredicateCreationRule" /> class.
        /// </summary>
        /// <param name="predicate">The predicate to evaluate.</param>
        /// <param name="valueGenerator">The value generator used to build the value returned by the rule.</param>
        /// <param name="priority">The priority to apply to the rule.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="valueGenerator" /> parameter is <c>null</c>.</exception>
        public TypePredicateCreationRule(Predicate<Type> predicate, Func<object> valueGenerator, int priority)
        {
            _typePredicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            _valueGenerator = valueGenerator ?? throw new ArgumentNullException(nameof(valueGenerator));

            Priority = priority;
        }

        /// <inheritdoc />
        public object? Create(IExecuteStrategy executeStrategy, Type type)
        {
            return _valueGenerator();
        }

        /// <inheritdoc />
        public object? Create(IExecuteStrategy executeStrategy, PropertyInfo propertyInfo)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public object? Create(IExecuteStrategy executeStrategy, ParameterInfo parameterInfo)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public bool IsMatch(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return _typePredicate(type);
        }

        /// <inheritdoc />
        public bool IsMatch(PropertyInfo propertyInfo)
        {
            return false;
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