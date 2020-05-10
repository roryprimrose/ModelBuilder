namespace ModelBuilder.IgnoreRules
{
    using System;
    using System.Reflection;
    using System.Text.RegularExpressions;

    /// <summary>
    ///     The <see cref="RegexIgnoreRule" />
    ///     class is used to match a property on a type using a <see cref="Regex" />
    ///     on the property name for whether the property should be ignored by <see cref="IExecuteStrategy" />
    ///     and not be populated.
    /// </summary>
    public class RegexIgnoreRule : IIgnoreRule
    {
        private readonly Regex _expression;

        /// <summary>
        ///     Creates a new instance of the <see cref="RegexIgnoreRule" /> class.
        /// </summary>
        /// <param name="expression">The expression used to match on property name.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="expression" /> parameter is <c>null</c>.</exception>
        public RegexIgnoreRule(string expression)
        {
            if (string.IsNullOrEmpty(expression))
            {
                throw new ArgumentNullException(nameof(expression));
            }

            _expression = new Regex(expression);
        }

        /// <summary>
        ///     Creates a new instance of the <see cref="RegexIgnoreRule" /> class.
        /// </summary>
        /// <param name="expression">The expression used to match on property name.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="expression" /> parameter is <c>null</c>.</exception>
        public RegexIgnoreRule(Regex expression)
        {
            _expression = expression ?? throw new ArgumentNullException(nameof(expression));
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="propertyInfo" /> parameter is <c>null</c>.</exception>
        public bool IsMatch(PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            return _expression.IsMatch(propertyInfo.Name);
        }
    }
}