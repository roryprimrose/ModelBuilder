namespace ModelBuilder
{
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    ///     The <see cref="IParameterResolver" />
    ///     interface defines the members for getting parameters that need to be created.
    /// </summary>
    public interface IParameterResolver
    {
        /// <summary>
        ///     Gets the parameters on <paramref name="method" /> that are to be populated in the order identified by
        ///     <see cref="IBuildConfiguration.ExecuteOrderRules" />.
        /// </summary>
        /// <param name="configuration">The build configuration.</param>
        /// <param name="method">The method used to create an object.</param>
        /// <returns>The set of parameters to populate in the order they are to be populated.</returns>
        IEnumerable<ParameterInfo> GetOrderedParameters(IBuildConfiguration configuration, MethodBase method);
    }
}