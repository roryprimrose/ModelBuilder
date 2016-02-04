using System;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="ITypeCreator"/>
    /// interface defines the members for creating an instance of a type.
    /// </summary>
    public interface ITypeCreator
    {
        /// <summary>
        /// Creates an instance of the type with the specified arguments.
        /// </summary>
        /// <param name="type">The type of instance to create.</param>
        /// <param name="args">The constructor parameters to create the instance with.</param>
        /// <returns>A new instance.</returns>
        object Create(Type type, params object[] args);

        /// <summary>
        /// Returns whether the specified type is supported by this type creator.
        /// </summary>
        /// <param name="type">The type to evaluate.</param>
        /// <returns><c>true</c> if this creator supports the type; otherwise <c>false</c>.</returns>
        bool IsSupported(Type type);

        /// <summary>
        /// Gets whether this creator identifies that dynamic constructor resolution and parameter building should occur.
        /// </summary>
        /// <remarks>
        /// Where the value is <c>true</c>, the <see cref="IExecuteStrategy{T}"/> should find the best available constructor and dynamic generate the parameters for it.
        /// Where the value is <c>false</c>, the default constructor on the type should be evaluated.
        /// </remarks>
        bool AutoDetectConstructor { get; }
    }
}