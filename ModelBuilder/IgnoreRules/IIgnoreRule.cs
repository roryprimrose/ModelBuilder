namespace ModelBuilder.IgnoreRules
{
    using System.Reflection;

    /// <summary>
    ///     The <see cref="IIgnoreRule" />
    ///     interface defines how a rule can identify a property that should be ignored by <see cref="IExecuteStrategy" /> and
    ///     not populated with a value.
    /// </summary>
    public interface IIgnoreRule
    {
        /// <summary>
        ///     Returns whether the rule matches the specified property.
        /// </summary>
        /// <param name="propertyInfo">The property to evaluate.</param>
        /// <returns><c>true</c> if the property matches and the rule; otherwise <c>false</c>.</returns>
        /// <remarks>If the property matches the rule, the <see cref="IExecuteStrategy" /> should not populate the property.</remarks>
        bool IsMatch(PropertyInfo propertyInfo);
    }
}