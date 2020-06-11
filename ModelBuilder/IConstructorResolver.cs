namespace ModelBuilder
{
    using System;
    using System.Reflection;

    /// <summary>
    ///     The <see cref="IConstructorResolver" />
    ///     interface defines the members for resolving constructors.
    /// </summary>
    public interface IConstructorResolver
    {
        /// <summary>
        ///     Returns the <see cref="ConstructorInfo" /> for the type, matching on the specified arguments.
        /// </summary>
        /// <param name="type">The type to evaluate.</param>
        /// <param name="args">The optional argument list for the constructor.</param>
        /// <returns>The constructor matching the type and arguments; or <c>null</c> if no constructor is found.</returns>
        ConstructorInfo? Resolve(Type type, params object?[]? args);
    }
}