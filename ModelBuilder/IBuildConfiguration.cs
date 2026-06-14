namespace ModelBuilder
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     The <see cref="IBuildConfiguration" /> interface
    ///     defines the slim vNext build configuration: the type mappings and ignore rules that
    ///     configuration modules populate and that the engine consults while building an object graph.
    /// </summary>
    public interface IBuildConfiguration
    {
        /// <summary>
        ///     Registers a mapping from a source type to a concrete target type.
        /// </summary>
        /// <param name="sourceType">The source type, typically an interface or abstract type.</param>
        /// <param name="targetType">The concrete type to build in its place.</param>
        /// <returns>The same configuration instance for chaining.</returns>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="sourceType" /> or <paramref name="targetType" /> parameter is <c>null</c>.
        /// </exception>
        IBuildConfiguration AddMapping(Type sourceType, Type targetType);

        /// <summary>
        ///     Registers a mapping from a source type to a concrete target type.
        /// </summary>
        /// <typeparam name="TSource">The source type, typically an interface or abstract type.</typeparam>
        /// <typeparam name="TTarget">The concrete type to build in its place.</typeparam>
        /// <returns>The same configuration instance for chaining.</returns>
        IBuildConfiguration AddMapping<TSource, TTarget>()
            where TTarget : TSource;

        /// <summary>
        ///     Registers a targeted ignore rule for a specific member on a specific type.
        /// </summary>
        /// <param name="declaringType">The type that declares the member.</param>
        /// <param name="memberName">The name of the member to ignore.</param>
        /// <returns>The same configuration instance for chaining.</returns>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="declaringType" /> or <paramref name="memberName" /> parameter is <c>null</c>.
        /// </exception>
        IBuildConfiguration Ignore(Type declaringType, string memberName);

        /// <summary>
        ///     Registers a type-agnostic ignore rule that applies to any member matching the predicate.
        /// </summary>
        /// <param name="predicate">The predicate evaluated against each member.</param>
        /// <returns>The same configuration instance for chaining.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate" /> parameter is <c>null</c>.</exception>
        IBuildConfiguration IgnoreAny(Func<MemberSignature, bool> predicate);

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
