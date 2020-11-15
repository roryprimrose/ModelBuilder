namespace ModelBuilder.ExecuteOrderRules
{
    using System;
    using System.Reflection;

    /// <summary>
    ///     The <see cref="ParameterPredicateExecuteOrderRule" />
    ///     class is used to match the rule to a <see cref="PropertyInfo" /> using a predicate.
    /// </summary>
    public class ParameterPredicateExecuteOrderRule : IExecuteOrderRule
    {
        private readonly Predicate<ParameterInfo> _predicate;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ParameterPredicateExecuteOrderRule" /> class.
        /// </summary>
        /// <param name="predicate">The predicate to evaluate.</param>
        /// <param name="priority">The execution order priority to apply to the parameter.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate" /> parameter is <c>null</c>.</exception>
        public ParameterPredicateExecuteOrderRule(Predicate<ParameterInfo> predicate, int priority)
        {
            _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));

            Priority = priority;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="parameterInfo" /> parameter is <c>null</c>.</exception>
        public bool IsMatch(ParameterInfo parameterInfo)
        {
            parameterInfo = parameterInfo ?? throw new ArgumentNullException(nameof(parameterInfo));

            return _predicate(parameterInfo);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="propertyInfo" /> parameter is <c>null</c>.</exception>
        public bool IsMatch(PropertyInfo propertyInfo)
        {
            return false;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return _predicate.ToString() ?? "<predicate>";
        }

        /// <inheritdoc />
        public int Priority { get; }
    }
}