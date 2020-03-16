namespace ModelBuilder.ExecuteOrderRules
{
    using System;
    using System.Reflection;
    using System.Text.RegularExpressions;

    /// <summary>
    ///     The <see cref="RegexExecuteOrderRule" />
    ///     class is used to match a property on a type using a <see cref="Regex" />
    ///     on the property name to determine the priority order in populating the property by <see cref="IExecuteStrategy" />.
    /// </summary>
    public class RegexExecuteOrderRule : IExecuteOrderRule
    {
        private readonly Regex _expression;

        /// <summary>
        ///     Creates a new instance of the <see cref="RegexExecuteOrderRule" /> class.
        /// </summary>
        /// <param name="expression">The expression used to match on property name.</param>
        /// <param name="priority">The execution order priority to apply to the property.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="expression" /> parameter is <c>null</c>.</exception>
        public RegexExecuteOrderRule(Regex expression, int priority)
        {
            _expression = expression ?? throw new ArgumentNullException(nameof(expression));
            Priority = priority;
        }

        /// <inheritdoc />
        /// <exception cref="System.ArgumentNullException">The <paramref name="propertyInfo" /> is <c>null</c>.</exception>
        public bool IsMatch(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            return _expression.IsMatch(propertyInfo.Name);
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