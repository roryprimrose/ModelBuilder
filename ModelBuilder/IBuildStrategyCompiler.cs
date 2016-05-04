using System.Collections.Generic;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="IBuildStrategyCompiler"/>
    /// interface defines the members to assist in creating new <see cref="IBuildStrategy"/> instances.
    /// </summary>
    public interface IBuildStrategyCompiler
    {
        /// <summary>
        /// Compiles the configuration of this compiler into a new <see cref="IBuildStrategy"/> instance.
        /// </summary>
        /// <returns>A new builder strategy.</returns>
        IBuildStrategy Compile();

        /// <summary>
        /// Gets the constructor resolver used to create an instance of a type.
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