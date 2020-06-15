namespace ModelBuilder.BuildActions
{
    using System;
    using System.Reflection;

    /// <summary>
    ///     The <see cref="IBuildCapability" />
    ///     interface defines the members for describing value creation support and creating instances.
    /// </summary>
    public interface IBuildCapability
    {
        /// <summary>
        ///     Creates an instance of the type with the specified arguments.
        /// </summary>
        /// <param name="executeStrategy">The execution strategy.</param>
        /// <param name="parameterInfo">The parameter to evaluate.</param>
        /// <param name="args">The constructor parameters to create the instance with.</param>
        /// <returns>A new instance.</returns>
        object? CreateParameter(IExecuteStrategy executeStrategy, ParameterInfo parameterInfo, object?[]? args);

        /// <summary>
        ///     Creates an instance of the type with the specified arguments.
        /// </summary>
        /// <param name="executeStrategy">The execution strategy.</param>
        /// <param name="propertyInfo">The property to evaluate.</param>
        /// <param name="args">The constructor parameters to create the instance with.</param>
        /// <returns>A new instance.</returns>
        object? CreateProperty(IExecuteStrategy executeStrategy, PropertyInfo propertyInfo, object?[]? args);

        /// <summary>
        ///     Creates an instance of the type with the specified arguments.
        /// </summary>
        /// <param name="executeStrategy">The execution strategy.</param>
        /// <param name="targetType">The type of instance to create.</param>
        /// <param name="args">The constructor parameters to create the instance with.</param>
        /// <returns>A new instance.</returns>
        object? CreateType(IExecuteStrategy executeStrategy, Type targetType, object?[]? args);

        /// <summary>
        ///     Populates the specified instance using an execution strategy.
        /// </summary>
        /// <param name="executeStrategy">The execution strategy.</param>
        /// <param name="instance">The instance to populate.</param>
        /// <returns>The populated instance.</returns>
        object Populate(IExecuteStrategy executeStrategy, object instance);

        /// <summary>
        ///     Gets whether properties on the created value should be automatically populated.
        /// </summary>
        bool AutoPopulate { get; }

        /// <summary>
        ///     Gets the type that will be used to run a create or populate process.
        /// </summary>
        Type ImplementedByType { get; }

        /// <summary>
        ///     Gets whether there the <see cref="IBuildAction" /> supports the requested scenario.
        /// </summary>
        bool SupportsCreate { get; }

        /// <summary>
        ///     Gets whether a build action supports populating the created value with its own logic.
        /// </summary>
        bool SupportsPopulate { get; }
    }
}