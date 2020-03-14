﻿namespace ModelBuilder.CreationRules
{
    using System;
    using System.Reflection;

    /// <summary>
    ///     The <see cref="ICreationRule" />
    ///     interface defines the members for generating values for simple scenarios.
    /// </summary>
    public interface ICreationRule
    {
        /// <summary>
        ///     Creates a new value of the specified type.
        /// </summary>
        /// <param name="type">The type of value to create.</param>
        /// <param name="executeStrategy">The execution strategy.</param>
        /// <returns>A new value of the type.</returns>
        object Create(Type type, IExecuteStrategy executeStrategy);

        /// <summary>
        ///     Creates a new value of the specified type.
        /// </summary>
        /// <param name="propertyInfo">The property to create the value for.</param>
        /// <param name="executeStrategy">The execution strategy.</param>
        /// <returns>A new value of the type.</returns>
        object Create(PropertyInfo propertyInfo, IExecuteStrategy executeStrategy);

        /// <summary>
        ///     Creates a new value of the specified type.
        /// </summary>
        /// <param name="parameterInfo">The parameter to create the value for.</param>
        /// <param name="executeStrategy">The execution strategy.</param>
        /// <returns>A new value of the type.</returns>
        object Create(ParameterInfo parameterInfo, IExecuteStrategy executeStrategy);

        /// <summary>
        ///     Returns whether the specified type matches this rule.
        /// </summary>
        /// <param name="type">The type to evaluate.</param>
        /// <returns><c>true</c> if the type matches this rule; otherwise <c>false</c>.</returns>
        bool IsMatch(Type type);

        /// <summary>
        ///     Returns whether the specified property matches this rule.
        /// </summary>
        /// <param name="propertyInfo">The property to generate the value for.</param>
        /// <returns><c>true</c> if the property matches this rule; otherwise <c>false</c>.</returns>
        bool IsMatch(PropertyInfo propertyInfo);

        /// <summary>
        ///     Returns whether the specified parameter matches this rule.
        /// </summary>
        /// <param name="parameterInfo">The parameter to generate the value for.</param>
        /// <returns><c>true</c> if the parameter matches this rule; otherwise <c>false</c>.</returns>
        bool IsMatch(ParameterInfo parameterInfo);

        /// <summary>
        ///     Gets the priority for this rule.
        /// </summary>
        int Priority { get; }
    }
}