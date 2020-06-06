namespace ModelBuilder
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
        ///     Gets the build capability for the specified type.
        /// </summary>
        /// <param name="executeStrategy">The execute strategy.</param>
        /// <param name="buildRequirement">The build capability.</param>
        /// <param name="type">The type to evaluate.</param>
        /// <returns>A <see cref="BuildCapability" /> indicating instance creation support via a <see cref="IBuildAction" />.</returns>
        IBuildCapability GetBuildCapability(IExecuteStrategy executeStrategy,
            BuildRequirement buildRequirement, Type type);

        /// <summary>
        ///     Gets the build capability for the specified parameter.
        /// </summary>
        /// <param name="executeStrategy">The execute strategy.</param>
        /// <param name="buildRequirement">The build capability.</param>
        /// <param name="parameterInfo">The parameter to evaluate.</param>
        /// <returns>A <see cref="BuildCapability" /> indicating instance creation support via a <see cref="IBuildAction" />.</returns>
        IBuildCapability GetBuildCapability(IExecuteStrategy executeStrategy,
            BuildRequirement buildRequirement,
            ParameterInfo parameterInfo);

        /// <summary>
        ///     Gets the build capability for the specified property.
        /// </summary>
        /// <param name="executeStrategy">The execute strategy.</param>
        /// <param name="buildRequirement">The build capability.</param>
        /// <param name="propertyInfo">The property to evaluate.</param>
        /// <returns>A <see cref="BuildCapability" /> indicating instance creation support via a <see cref="IBuildAction" />.</returns>
        IBuildCapability GetBuildCapability(IExecuteStrategy executeStrategy,
            BuildRequirement buildRequirement,
            PropertyInfo propertyInfo);
    }
}