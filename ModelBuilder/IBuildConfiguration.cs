namespace ModelBuilder
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     The <see cref="IBuildConfiguration" /> interface
    ///     defines the read-only view of the slim vNext build configuration: the type mappings and the
    ///     ignore rules consulted while building an object graph.
    /// </summary>
    public interface IBuildConfiguration
    {
        /// <summary>
        ///     Determines whether the specified member should be ignored.
        /// </summary>
        /// <param name="member">The member to evaluate.</param>
        /// <returns><c>true</c> if the member should be ignored; otherwise, <c>false</c>.</returns>
        bool ShouldIgnore(in MemberSignature member);

        /// <summary>
        ///     Attempts to resolve a mapped concrete type for the specified source type.
        /// </summary>
        /// <param name="sourceType">The source type to resolve.</param>
        /// <param name="targetType">The mapped concrete type, when one is registered.</param>
        /// <returns><c>true</c> if a mapping is registered; otherwise, <c>false</c>.</returns>
        bool TryGetMapping(Type sourceType, out Type targetType);

        /// <summary>
        ///     Gets the registered type mappings.
        /// </summary>
        IReadOnlyDictionary<Type, Type> TypeMappings { get; }
    }
}
