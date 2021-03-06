﻿namespace ModelBuilder.ExecuteOrderRules
{
    using System.Reflection;

    /// <summary>
    ///     The <see cref="IExecuteOrderRule" />
    ///     interface defines how a rule can identify the order in which properties should be populated by
    ///     <see cref="IExecuteStrategy" />.
    /// </summary>
    public interface IExecuteOrderRule
    {
        /// <summary>
        ///     Returns whether the rule matches the specified parameter.
        /// </summary>
        /// <param name="parameterInfo">The parameter to evaluate.</param>
        /// <returns><c>true</c> if the parameter matches and the rule; otherwise <c>false</c>.</returns>
        /// <remarks>
        ///     If the parameter matches the rule, the <see cref="IExecuteStrategy" /> should use the priority to evaluate
        ///     ordering.
        /// </remarks>
        bool IsMatch(ParameterInfo parameterInfo);

        /// <summary>
        ///     Returns whether the rule matches the specified property.
        /// </summary>
        /// <param name="propertyInfo">The property to evaluate.</param>
        /// <returns><c>true</c> if the property matches and the rule; otherwise <c>false</c>.</returns>
        /// <remarks>
        ///     If the property matches the rule, the <see cref="IExecuteStrategy" /> should use the priority to evaluate
        ///     ordering.
        /// </remarks>
        bool IsMatch(PropertyInfo propertyInfo);

        /// <summary>
        ///     Gets the priority for this rule.
        /// </summary>
        int Priority { get; }
    }
}