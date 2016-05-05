using System;
using System.Collections.Generic;

namespace ModelBuilder
{
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
        /// Gets or sets the build log for items created by this strategy.
        /// </summary>
        IBuildLog BuildLog { get; set; }

        /// <summary>
        /// Gets or sets the constructor resolver used to create an instance of a type.
        /// </summary>
        IConstructorResolver ConstructorResolver { get; set; }

        /// <summary>
        /// Gets the execute order rules used to determine the order that properties are populated.
        /// </summary>
        ICollection<ExecuteOrderRule> ExecuteOrderRules { get; }

        /// <summary>
        /// Gets the ignore rules used to skip over property population.
        /// </summary>
        ICollection<IgnoreRule> IgnoreRules { get; }

        /// <summary>
        /// Gets the type creators used to create instances.
        /// </summary>
        ICollection<ITypeCreator> TypeCreators { get; }

        /// <summary>
        /// Gets the value generators used to generate flat values.
        /// </summary>
        ICollection<IValueGenerator> ValueGenerators { get; }
    }
}