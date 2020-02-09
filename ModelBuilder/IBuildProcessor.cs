﻿namespace ModelBuilder
{
    using System;
    using System.Reflection;
    using ModelBuilder.BuildActions;

    /// <summary>
    ///     The <see cref="IBuildProcessor" />
    ///     interface defines the members for building a new value.
    /// </summary>
    public interface IBuildProcessor
    {
        /// <summary>
        ///     Builds a value of the specified type.
        /// </summary>
        /// <param name="executeStrategy">The execute strategy.</param>
        /// <param name="type">The type of value to build.</param>
        /// <param name="arguments">The constructor parameters to create the instance with.</param>
        /// <returns>The new value.</returns>
        object Build(IExecuteStrategy executeStrategy, Type type, params object[] arguments);

        /// <summary>
        ///     Builds a value of the specified type.
        /// </summary>
        /// <param name="executeStrategy">The execute strategy.</param>
        /// <param name="parameterInfo">The parameter of value to build.</param>
        /// <returns>The new value.</returns>
        object Build(IExecuteStrategy executeStrategy, ParameterInfo parameterInfo);

        /// <summary>
        ///     Builds a value of the specified type.
        /// </summary>
        /// <param name="executeStrategy">The execute strategy.</param>
        /// <param name="propertyInfo">The property of value to build.</param>
        /// <returns>The new value.</returns>
        object Build(IExecuteStrategy executeStrategy, PropertyInfo propertyInfo);

        /// <summary>
        ///     Gets the build plan for the specified type.
        /// </summary>
        /// <param name="buildConfiguration">The build configuration.</param>
        /// <param name="buildChain">The build chain.</param>
        /// <param name="type">The type to evaluate.</param>
        /// <returns>A <see cref="MatchResult" /> indicating instance creation support via a <see cref="IBuildAction" />.</returns>
        BuildPlan GetBuildPlan(IBuildConfiguration buildConfiguration, IBuildChain buildChain, Type type);

        /// <summary>
        ///     Gets the build plan for the specified parameter.
        /// </summary>
        /// <param name="buildConfiguration">The build configuration.</param>
        /// <param name="buildChain">The build chain.</param>
        /// <param name="parameterInfo">The parameter to evaluate.</param>
        /// <returns>A <see cref="MatchResult" /> indicating instance creation support via a <see cref="IBuildAction" />.</returns>
        BuildPlan GetBuildPlan(IBuildConfiguration buildConfiguration, IBuildChain buildChain,
            ParameterInfo parameterInfo);

        /// <summary>
        ///     Gets the build plan for the specified property.
        /// </summary>
        /// <param name="buildConfiguration">The build configuration.</param>
        /// <param name="buildChain">The build chain.</param>
        /// <param name="propertyInfo">The property to evaluate.</param>
        /// <returns>A <see cref="MatchResult" /> indicating instance creation support via a <see cref="IBuildAction" />.</returns>
        BuildPlan GetBuildPlan(IBuildConfiguration buildConfiguration, IBuildChain buildChain,
            PropertyInfo propertyInfo);
    }
}