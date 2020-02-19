namespace ModelBuilder
{
    using System;

    /// <summary>
    ///     The <see cref="ITypeResolver" />
    ///     interface defines the members for resolving a type to build.
    /// </summary>
    public interface ITypeResolver
    {
        /// <summary>
        ///     Gets the type to build based on the specified type.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="requestedType">The type being requested.</param>
        /// <returns>The type to build.</returns>
        Type GetBuildType(IBuildConfiguration configuration, Type requestedType);
    }
}