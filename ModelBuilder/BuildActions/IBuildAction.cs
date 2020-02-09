namespace ModelBuilder.BuildActions
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
        /// <returns>The built object which may be <c>null</c>.</returns>
        object Build(IExecuteStrategy executeStrategy, ParameterInfo parameterInfo);

        /// <summary>
        ///     Builds a value using the specified property and execute strategy.
        /// </summary>
        /// <param name="executeStrategy">The execution strategy.</param>
        /// <param name="propertyInfo">The property to generate a value for.</param>
        /// <returns>The built object which may be <c>null</c>.</returns>
        object Build(IExecuteStrategy executeStrategy, PropertyInfo propertyInfo);

        /// <summary>
        ///     Gets whether the specified type matches this build step.
        /// </summary>
        /// <param name="buildConfiguration">The build configuration.</param>
        /// <param name="buildChain">The build chain.</param>
        /// <param name="type">The type to evaluate.</param>
        /// <returns><c>true</c> if the build step matches the specified type; otherwise <c>false</c>.</returns>
        bool IsMatch(IBuildConfiguration buildConfiguration, IBuildChain buildChain, Type type);

        /// <summary>
        ///     Gets whether the specified parameter matches this build step.
        /// </summary>
        /// <param name="buildConfiguration">The build configuration.</param>
        /// <param name="buildChain">The build chain.</param>
        /// <param name="parameterInfo">The parameter to evaluate.</param>
        /// <returns><c>true</c> if the build step matches the specified type; otherwise <c>false</c>.</returns>
        bool IsMatch(IBuildConfiguration buildConfiguration, IBuildChain buildChain, ParameterInfo parameterInfo);

        /// <summary>
        ///     Gets whether the specified property matches this build step.
        /// </summary>
        /// <param name="buildConfiguration">The build configuration.</param>
        /// <param name="buildChain">The build chain.</param>
        /// <param name="propertyInfo">The property to evaluate.</param>
        /// <returns><c>true</c> if the build step matches the specified type; otherwise <c>false</c>.</returns>
        bool IsMatch(IBuildConfiguration buildConfiguration, IBuildChain buildChain, PropertyInfo propertyInfo);

        /// <summary>
        ///     Gets the priority for this build step.
        /// </summary>
        int Priority { get; }
    }
}