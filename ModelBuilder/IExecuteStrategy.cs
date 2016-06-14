using System;

namespace ModelBuilder
{
    using System.Collections.Generic;

    /// <summary>
    /// The <see cref="IExecuteStrategy"/>
    /// interface defines the dependencies used to create and populate instances.
    /// </summary>
    public interface IExecuteStrategy
    {
        /// <summary>
        /// Creates a new instance of the specified type with optional constructor arguments.
        /// </summary>
        /// <param name="type">The type of object to create.</param>
        /// <param name="args">The constructor arguments of the type.</param>
        /// <returns>A new instance of the specified type.</returns>
        object CreateWith(Type type, params object[] args);

        /// <summary>
        /// Populates values onto settable properties of the specified instance.
        /// </summary>
        /// <param name="instance">The instance to populate.</param>
        /// <returns>The populated object.</returns>
        object Populate(object instance);

        /// <summary>
        /// Gets or sets the build strategy.
        /// </summary>
        IBuildStrategy BuildStrategy { get; set; }

        /// <summary>
        /// Gets the build chain for objects up to the current build execution.
        /// </summary>
        LinkedList<object> BuildChain
        {
            get;
        }
    }
}