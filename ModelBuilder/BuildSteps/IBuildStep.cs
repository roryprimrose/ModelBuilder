namespace ModelBuilder.BuildSteps
{
    using System;
    using System.Reflection;

    /// <summary>
    ///     The <see cref="IBuildStep" />
    ///     interface defines the members for evaluating whether the build step should be used and for building values.
    /// </summary>
    public interface IBuildStep
    {
        /// <summary>
        ///     Builds a value using the specified type and execute strategy.
        /// </summary>
        /// <param name="type">The type of value to generate.</param>
        /// <param name="executeStrategy">The execution strategy.</param>
        /// <returns>The built object which may be <c>null</c>.</returns>
        object Build(Type type, IExecuteStrategy executeStrategy);

        /// <summary>
        ///     Builds a value using the specified parameter and execute strategy.
        /// </summary>
        /// <param name="parameterInfo">The parameter to generate a value for.</param>
        /// <param name="executeStrategy">The execution strategy.</param>
        /// <returns>The built object which may be <c>null</c>.</returns>
        object Build(ParameterInfo parameterInfo, IExecuteStrategy executeStrategy);

        /// <summary>
        ///     Builds a value using the specified property and execute strategy.
        /// </summary>
        /// <param name="propertyInfo">The property to generate a value for.</param>
        /// <param name="executeStrategy">The execution strategy.</param>
        /// <returns>The built object which may be <c>null</c>.</returns>
        object Build(PropertyInfo propertyInfo, IExecuteStrategy executeStrategy);

        /// <summary>
        ///     Gets whether the specified type matches this build step.
        /// </summary>
        /// <param name="type">The type to evaluate.</param>
        /// <param name="buildConfiguration">The build configuration.</param>
        /// <param name="buildChain">The build chain.</param>
        /// <returns><c>true</c> if the build step matches the specified type; otherwise <c>false</c>.</returns>
        bool IsMatch(Type type, IBuildConfiguration buildConfiguration, IBuildChain buildChain);

        /// <summary>
        ///     Gets whether the specified parameter matches this build step.
        /// </summary>
        /// <param name="parameterInfo">The parameter to evaluate.</param>
        /// <param name="buildConfiguration">The build configuration.</param>
        /// <param name="buildChain">The build chain.</param>
        /// <returns><c>true</c> if the build step matches the specified type; otherwise <c>false</c>.</returns>
        bool IsMatch(ParameterInfo parameterInfo, IBuildConfiguration buildConfiguration, IBuildChain buildChain);

        /// <summary>
        ///     Gets whether the specified property matches this build step.
        /// </summary>
        /// <param name="propertyInfo">The property to evaluate.</param>
        /// <param name="buildConfiguration">The build configuration.</param>
        /// <param name="buildChain">The build chain.</param>
        /// <returns><c>true</c> if the build step matches the specified type; otherwise <c>false</c>.</returns>
        bool IsMatch(PropertyInfo propertyInfo, IBuildConfiguration buildConfiguration, IBuildChain buildChain);

        /// <summary>
        ///     Gets the priority for this build step.
        /// </summary>
        int Priority { get; }
    }
}