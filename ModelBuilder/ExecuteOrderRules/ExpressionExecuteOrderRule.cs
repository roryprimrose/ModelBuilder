namespace ModelBuilder.ExecuteOrderRules
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    ///     The <see cref="ExpressionExecuteOrderRule{T}" />
    ///     class is used to identify a property to determine the priority order in populating the property by
    ///     <see cref="IExecuteStrategy" />.
    /// </summary>
    /// <typeparam name="T">The type being evaluated.</typeparam>
    public class ExpressionExecuteOrderRule<T> : IExecuteOrderRule
    {
        private readonly Expression<Func<T, object?>> _expression;

        /// <summary>
        ///     Creates a new instance of the <see cref="ExpressionExecuteOrderRule{T}" /> class.
        /// </summary>
        /// <param name="expression">The expression used to identify a property on a type.</param>
        /// <param name="priority">The execution order priority to apply to the property.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="expression" /> parameter is <c>null</c>.</exception>
        public ExpressionExecuteOrderRule(Expression<Func<T, object?>> expression, int priority)
        {
            _expression = expression ?? throw new ArgumentNullException(nameof(expression));
            Priority = priority;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="parameterInfo" /> parameter is <c>null</c>.</exception>
        public bool IsMatch(ParameterInfo parameterInfo)
        {
            return false;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="propertyInfo" /> parameter is <c>null</c>.</exception>
        public bool IsMatch(PropertyInfo propertyInfo)
        {
            propertyInfo = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));

            var expressionProperty = _expression.GetProperty();

            if (propertyInfo.Name != expressionProperty.Name)
            {
                return false;
            }

            if (propertyInfo.DeclaringType != expressionProperty.DeclaringType)
            {
                return false;
            }

            return true;
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