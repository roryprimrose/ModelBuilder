namespace ModelBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    /// <summary>
    ///     The <see cref="IConstructorResolver" />
    ///     interface defines the members for resolving constructors.
    /// </summary>
    public interface IConstructorResolver
    {
        /// <summary>
        ///     Gets the parameters on <paramref name="constructor" /> that are to be populated in the order identified by
        ///     <see cref="IBuildConfiguration.ExecuteOrderRules" />.
        /// </summary>
        /// <param name="configuration">The build configuration.</param>
        /// <param name="constructor">The constructor used to create an object.</param>
        /// <returns>The set of parameters to populate in the order they are to be populated.</returns>
        IEnumerable<ParameterInfo> GetOrderedParameters(IBuildConfiguration configuration, ConstructorInfo constructor);

        /// <summary>
        ///     Returns the <see cref="ConstructorInfo" /> for the type, matching on the specified arguments.
        /// </summary>
        /// <param name="type">The type to evaluate.</param>
        /// <param name="args">The optional argument list for the constructor.</param>
        /// <returns>The constructor matching the type and arguments; or <c>null</c> if no constructor is found.</returns>
        ConstructorInfo Resolve(Type type, params object[] args);
    }
}