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
        /// Gets the constructor resolver used to create an instance of a type.
        /// </summary>
        IConstructorResolver ConstructorResolver { get; set; }

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