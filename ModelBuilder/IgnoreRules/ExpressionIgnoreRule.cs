namespace ModelBuilder.IgnoreRules
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    ///     The <see cref="ExpressionIgnoreRule{T}" />
    ///     class is used to identify a property on a type that should be ignored by <see cref="IExecuteStrategy" /> and not be
    ///     populated.
    /// </summary>
    /// <typeparam name="T">The type being evaluated.</typeparam>
    public class ExpressionIgnoreRule<T> : IIgnoreRule
    {
        private readonly Expression<Func<T, object>> _expression;

        /// <summary>
        ///     Creates a new instance of the <see cref="ExpressionIgnoreRule{T}" /> class.
        /// </summary>
        /// <param name="expression">The expression used to identify a property on a type.</param>
        public ExpressionIgnoreRule(Expression<Func<T, object>> expression)
        {
            _expression = expression ?? throw new ArgumentNullException(nameof(expression));
        }

        /// <inheritdoc />
        public bool IsMatch(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

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
    }
}