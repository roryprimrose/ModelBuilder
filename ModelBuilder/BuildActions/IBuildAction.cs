﻿namespace ModelBuilder.BuildActions
{
    using System;
    using System.Reflection;

    /// <summary>
    ///     The <see cref="IBuildAction" />
    ///     interface defines the members for evaluating whether the build action should be used and for building values.
    /// </summary>
    public interface IBuildAction
    {
        /// <summary>
        ///     Builds a value using the specified type and execute strategy.
        /// </summary>
        /// <param name="executeStrategy">The execution strategy.</param>
        /// <param name="type">The type of value to generate.</param>
        /// <param name="arguments">The constructor parameters to create the instance with.</param>
        /// <returns>The built object which may be <c>null</c>.</returns>
        object Build(IExecuteStrategy executeStrategy, Type type, params object[] arguments);

        /// <summary>
        ///     Builds a value using the specified parameter and execute strategy.
        /// </summary>
        /// <param name="executeStrategy">The execution strategy.</param>
        /// <param name="parameterInfo">The parameter to generate a value for.</param>
        /// <param name="arguments">The constructor parameters to create the instance with.</param>
        /// <returns>The built object which may be <c>null</c>.</returns>
        object Build(IExecuteStrategy executeStrategy, ParameterInfo parameterInfo, params object[] arguments);

        /// <summary>
        ///     Builds a value using the specified property and execute strategy.
        /// </summary>
        /// <param name="executeStrategy">The execution strategy.</param>
        /// <param name="propertyInfo">The property to generate a value for.</param>
        /// <param name="arguments">The constructor parameters to create the instance with.</param>
        /// <returns>The built object which may be <c>null</c>.</returns>
        object Build(IExecuteStrategy executeStrategy, PropertyInfo propertyInfo, params object[] arguments);

        /// <summary>
        ///     Gets the build capabilities of the build step for the specified type.
        /// </summary>
        /// <param name="buildConfiguration">The build configuration.</param>
        /// <param name="buildChain">The build chain.</param>
        /// <param name="type">The type to evaluate.</param>
        /// <returns>A <see cref="BuildCapability" /> indicating instance creation support via a <see cref="IBuildAction" />.</returns>
        BuildCapability GetBuildCapability(IBuildConfiguration buildConfiguration, IBuildChain buildChain, Type type);

        /// <summary>
        ///     Gets the build capabilities of the build step for the specified parameter.
        /// </summary>
        /// <param name="buildConfiguration">The build configuration.</param>
        /// <param name="buildChain">The build chain.</param>
        /// <param name="parameterInfo">The parameter to evaluate.</param>
        /// <returns>A <see cref="BuildCapability" /> indicating instance creation support via a <see cref="IBuildAction" />.</returns>
        BuildCapability GetBuildCapability(IBuildConfiguration buildConfiguration, IBuildChain buildChain,
            ParameterInfo parameterInfo);

        /// <summary>
        ///     Gets the build capabilities of the build step for the specified property.
        /// </summary>
        /// <param name="buildConfiguration">The build configuration.</param>
        /// <param name="buildChain">The build chain.</param>
        /// <param name="propertyInfo">The property to evaluate.</param>
        /// <returns>A <see cref="BuildCapability" /> indicating instance creation support via a <see cref="IBuildAction" />.</returns>
        BuildCapability GetBuildCapability(IBuildConfiguration buildConfiguration, IBuildChain buildChain,
            PropertyInfo propertyInfo);

        /// <summary>
        ///     Populates the specified instance using an execution strategy.
        /// </summary>
        /// <param name="executeStrategy">The execution strategy.</param>
        /// <param name="instance">The instance to populate.</param>
        /// <returns>The populated instance.</returns>
        object Populate(IExecuteStrategy executeStrategy, object instance);

        /// <summary>
        ///     Gets the priority for this build step.
        /// </summary>
        int Priority { get; }
    }
}