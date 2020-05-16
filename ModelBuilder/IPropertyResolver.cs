namespace ModelBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    ///     The <see cref="IPropertyResolver" />
    ///     interface defines the members for resolving information about properties.
    /// </summary>
    public interface IPropertyResolver
    {
        /// <summary>
        ///     Gets the properties on <paramref name="targetType" /> that are to be populated in the order identified by
        ///     <see cref="IBuildConfiguration.ExecuteOrderRules" />.
        /// </summary>
        /// <param name="configuration">The build configuration.</param>
        /// <param name="targetType">The target type to populate.</param>
        /// <returns>The set of properties to populate in the order they are to be populated.</returns>
        IEnumerable<PropertyInfo> GetOrderedProperties(IBuildConfiguration configuration, Type targetType);

        /// <summary>
        ///     Determines whether the property should be populated with a value based on arguments provided.
        /// </summary>
        /// <param name="configuration">The build configuration.</param>
        /// <param name="instance">The instance being populated.</param>
        /// <param name="propertyInfo">The property to evaluate.</param>
        /// <param name="args">The constructor parameters for the instance.</param>
        /// <returns><c>true</c> if the property should be populated; otherwise <c>false</c>.</returns>
        bool IsIgnored(
            IBuildConfiguration configuration,
            object instance,
            PropertyInfo propertyInfo,
            object?[]? args);
    }
}