namespace ModelBuilder
{
    using System;
    using System.Collections.Generic;

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
        /// <param name="referenceName">Identifies the possible parameter or property name the instance is intended for.</param>
        /// <param name="buildChain">The chain of instances built up to this point.</param>
        /// <param name="args">The constructor parameters to create the instance with.</param>
        /// <returns>A new instance.</returns>
        object Create(Type type, string referenceName, LinkedList<object> buildChain, params object[] args);

        /// <summary>
        /// Returns whether the specified type is supported by this type creator.
        /// </summary>
        /// <param name="type">The type to evaluate.</param>
        /// <param name="referenceName">Identifies the possible parameter or property name the instance is intended for.</param>
        /// <param name="buildChain">The chain of instances built up to this point.</param>
        /// <returns><c>true</c> if this creator supports the type; otherwise <c>false</c>.</returns>
        bool IsSupported(Type type, string referenceName, LinkedList<object> buildChain);

        /// <summary>
        /// Populates the specified instance using an execution strategy.
        /// </summary>
        /// <param name="instance">The instance to populate.</param>
        /// <param name="executeStrategy">The execution strategy.</param>
        /// <returns>The populated instance.</returns>
        object Populate(object instance, IExecuteStrategy executeStrategy);

        /// <summary>
        /// Gets whether this creator identifies that dynamic constructor resolution and parameter building should occur.
        /// </summary>
        /// <remarks>
        /// Where the value is <c>true</c>, the <see cref="IExecuteStrategy{T}"/> should find the best available constructor and dynamic generate the parameters for it.
        /// Where the value is <c>false</c>, the default constructor on the type should be evaluated.
        /// </remarks>
        bool AutoDetectConstructor
        {
            get;
        }

        /// <summary>
        /// Gets whether this creator identifies that properties on the instance should be automatically populated.
        /// </summary>
        bool AutoPopulate
        {
            get;
        }

        /// <summary>
        /// Gets the priority for this creator.
        /// </summary>
        int Priority
        {
            get;
        }
    }
}