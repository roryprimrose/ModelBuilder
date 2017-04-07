namespace ModelBuilder
{
    using System;
    using System.Reflection;

    /// <summary>
    ///     The <see cref="IPropertyResolver" />
    ///     interface defines the members for resolving information about properties.
    /// </summary>
    public interface IPropertyResolver
    {
        /// <summary>
        ///     Determines whether the specified property can be populated.
        /// </summary>
        /// <param name="propertyInfo">The property to evaluate.</param>
        /// <returns><c>true</c> if the property can be populated; otherwise <c>false</c>.</returns>
        bool CanPopulate(PropertyInfo propertyInfo);

        /// <summary>
        ///     Determines whether the property should be populated with a value based on arguments provided.
        /// </summary>
        /// <param name="configuration">The build configuration.</param>
        /// <param name="instance">The instance being populated.</param>
        /// <param name="propertyInfo">The property to evaluate.</param>
        /// <param name="args">The constructor parameters for the instance.</param>
        /// <returns><c>true</c> if the property should be populated; otherwise <c>false</c>.</returns>
        bool ShouldPopulateProperty(
            IBuildConfiguration configuration,
            object instance,
            PropertyInfo propertyInfo,
            object[] args);
    }
}