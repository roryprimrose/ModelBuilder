namespace ModelBuilder.CreationRules
{
    using System;
    using System.Reflection;
    using System.Text.RegularExpressions;

    /// <summary>
    ///     The <see cref="RegexCreationRule" />
    ///     class is used to provide a creation rule based on regular expression matches on property and parameter names.
    /// </summary>
    public class RegexCreationRule : ICreationRule
    {
        private readonly Regex _expression;
        private readonly Type _targetType;
        private readonly Func<object> _valueGenerator;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RegexCreationRule" /> class.
        /// </summary>
        /// <param name="targetType">The target type that matches the rule.</param>
        /// <param name="expression">The expression used to identify a property on a type.</param>
        /// <param name="value">The value that the rule returns.</param>
        /// <param name="priority">The priority to apply to the rule.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="targetType" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="expression" /> parameter is <c>null</c>.</exception>
        public RegexCreationRule(Type targetType, Regex expression, object value, int priority) : this(targetType,
            expression, () => value, priority)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RegexCreationRule" /> class.
        /// </summary>
        /// <param name="targetType">The target type that matches the rule.</param>
        /// <param name="expression">The expression used to identify a property on a type.</param>
        /// <param name="valueGenerator">The value generator used to build the value returned by the rule.</param>
        /// <param name="priority">The priority to apply to the rule.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="targetType" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="expression" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="valueGenerator" /> parameter is <c>null</c>.</exception>
        public RegexCreationRule(Type targetType, Regex expression, Func<object> valueGenerator, int priority)
        {
            _targetType = targetType ?? throw new ArgumentNullException(nameof(targetType));
            _expression = expression ?? throw new ArgumentNullException(nameof(expression));
            _valueGenerator = valueGenerator ?? throw new ArgumentNullException(nameof(valueGenerator));

            Priority = priority;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RegexCreationRule" /> class.
        /// </summary>
        /// <param name="targetType">The target type that matches the rule.</param>
        /// <param name="expression">The expression used to identify a property on a type.</param>
        /// <param name="value">The value that the rule returns.</param>
        /// <param name="priority">The priority to apply to the rule.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="targetType" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="expression" /> parameter is <c>null</c>.</exception>
        public RegexCreationRule(Type targetType, string expression, object value, int priority) : this(targetType,
            expression, () => value, priority)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RegexCreationRule" /> class.
        /// </summary>
        /// <param name="targetType">The target type that matches the rule.</param>
        /// <param name="expression">The expression used to identify a property on a type.</param>
        /// <param name="valueGenerator">The value generator used to build the value returned by the rule.</param>
        /// <param name="priority">The priority to apply to the rule.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="targetType" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="expression" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="valueGenerator" /> parameter is <c>null</c>.</exception>
        public RegexCreationRule(Type targetType, string expression, Func<object> valueGenerator, int priority)
        {
            _targetType = targetType ?? throw new ArgumentNullException(nameof(targetType));

            if (string.IsNullOrEmpty(expression))
            {
                throw new ArgumentNullException(nameof(expression));
            }

            _expression = new Regex(expression);

            _valueGenerator = valueGenerator ?? throw new ArgumentNullException(nameof(valueGenerator));

            Priority = priority;
        }

        /// <inheritdoc />
        /// <exception cref="NotSupportedException">The class does not support creating values for types.</exception>
        public object? Create(IExecuteStrategy executeStrategy, Type type)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="propertyInfo" /> parameter is <c>null</c>.</exception>
        public object? Create(IExecuteStrategy executeStrategy, PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            return _valueGenerator();
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="parameterInfo" /> parameter is <c>null</c>.</exception>
        public object? Create(IExecuteStrategy executeStrategy, ParameterInfo parameterInfo)
        {
            parameterInfo = parameterInfo ?? throw new ArgumentNullException(nameof(parameterInfo));

            return _valueGenerator();
        }

        /// <inheritdoc />
        public bool IsMatch(Type type)
        {
            return false;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="propertyInfo" /> parameter is <c>null</c>.</exception>
        public bool IsMatch(PropertyInfo propertyInfo)
        {
            propertyInfo = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));

            return IsMatch(propertyInfo.PropertyType, propertyInfo.Name);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="parameterInfo" /> parameter is <c>null</c>.</exception>
        public bool IsMatch(ParameterInfo parameterInfo)
        {
            parameterInfo = parameterInfo ?? throw new ArgumentNullException(nameof(parameterInfo));

            return IsMatch(parameterInfo.ParameterType, parameterInfo.Name!);
        }

        private bool IsMatch(Type targetType, string referenceName)
        {
            if (_targetType != targetType)
            {
                return false;
            }

            if (_expression.IsMatch(referenceName))
            {
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public int Priority { get; }
    }
}