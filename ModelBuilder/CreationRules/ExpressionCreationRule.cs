namespace ModelBuilder.CreationRules
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;
    using ModelBuilder.Properties;

    /// <summary>
    ///     The <see cref="ExpressionCreationRule{T}" />
    ///     class is used to match a property using an expression in order to return a value.
    /// </summary>
    /// <typeparam name="T">The type of value to return.</typeparam>
    public class ExpressionCreationRule<T> : ICreationRule
    {
        private readonly Expression<Func<T, object>> _expression;
        private readonly PropertyInfo _propertyInfo;
        private readonly Func<object> _valueGenerator;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExpressionCreationRule{T}" /> class.
        /// </summary>
        /// <param name="expression">The expression used to identify a property on a type.</param>
        /// <param name="value">The value that the rule returns.</param>
        /// <param name="priority">The priority to apply to the rule.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="expression" /> parameter is <c>null</c>.</exception>
        public ExpressionCreationRule(Expression<Func<T, object>> expression, object value, int priority) : this(
            expression,
            () => value, priority)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExpressionCreationRule{T}" /> class.
        /// </summary>
        /// <param name="expression">The expression used to identify a property on a type.</param>
        /// <param name="valueGenerator">The value generator used to build the value returned by the rule.</param>
        /// <param name="priority">The priority to apply to the rule.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="expression" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="valueGenerator" /> parameter is <c>null</c>.</exception>
        public ExpressionCreationRule(Expression<Func<T, object>> expression, Func<object> valueGenerator, int priority)
        {
            _expression = expression ?? throw new ArgumentNullException(nameof(expression));
            _valueGenerator = valueGenerator ?? throw new ArgumentNullException(nameof(valueGenerator));
            _propertyInfo = expression.GetProperty();

            Priority = priority;
        }

        /// <inheritdoc />
        /// <exception cref="NotSupportedException">The class does not support creating values for types.</exception>
        public object Create(Type type, IExecuteStrategy executeStrategy)
        {
            throw new NotSupportedException(Resources.ExpressionCreationRule_CreateWithTypeNotSupported);
        }

        /// <inheritdoc />
        public object Create(PropertyInfo propertyInfo, IExecuteStrategy executeStrategy)
        {
            return _valueGenerator();
        }

        /// <inheritdoc />
        /// <exception cref="NotSupportedException">The class does not support creating values for parameters.</exception>
        public object Create(ParameterInfo parameterInfo, IExecuteStrategy executeStrategy)
        {
            throw new NotSupportedException(Resources.ExpressionCreationRule_CreateWithParameterNotSupported);
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
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            if (propertyInfo.Name != _propertyInfo.Name)
            {
                return false;
            }

            if (propertyInfo.DeclaringType != _propertyInfo.DeclaringType)
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc />
        public bool IsMatch(ParameterInfo parameterInfo)
        {
            return false;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return _expression.ToString();
        }

        /// <inheritdoc />
        public int Priority { get; }
    }
}