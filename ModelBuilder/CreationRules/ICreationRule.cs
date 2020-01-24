namespace ModelBuilder.CreationRules
{
    using System;

    /// <summary>
    ///     The <see cref="ICreationRule" />
    ///     interface defines the members for generating values for simple scenarios.
    /// </summary>
    public interface ICreationRule
    {
        /// <summary>
        ///     Creates a value using the specified type, reference name and execute strategy.
        /// </summary>
        /// <param name="type">The type of value to generate.</param>
        /// <param name="referenceName">Identifies the possible parameter or property name the value is intended for.</param>
        /// <param name="executeStrategy">The execution strategy.</param>
        object Create(Type type, string referenceName, IExecuteStrategy executeStrategy);

        /// <summary>
        ///     Gets whether the specified type and reference name matches this rule.
        /// </summary>
        /// <param name="type">The type to evaluate.</param>
        /// <param name="referenceName">Identifies the possible parameter or property name the value is intended for.</param>
        /// <returns><c>true</c> if the rule matches the specified type and property name; otherwise <c>false</c>.</returns>
        bool IsMatch(Type type, string referenceName);

        /// <summary>
        ///     Gets the priority for this rule.
        /// </summary>
        int Priority { get; }
    }
}