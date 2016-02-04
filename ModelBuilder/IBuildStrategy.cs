using System.Collections.Generic;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="IBuildStrategy"/>
    /// interface defines the members used to create and populate instances.
    /// </summary>
    public interface IBuildStrategy
    {
        /// <summary>
        /// Gets an <see cref="IExecuteStrategy{T}"/> for the specified <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of instance to generate with the strategy.</typeparam>
        /// <returns>A new execute strategy.</returns>
        IExecuteStrategy<T> GetExecuteStrategy<T>();

        /// <summary>
        /// Gets the constructor resolver used to create an instance of a type.
        /// </summary>
        IConstructorResolver ConstructorResolver { get; }

        /// <summary>
        /// Gets the ignore rules used to skip over property population.
        /// </summary>
        IReadOnlyCollection<IgnoreRule> IgnoreRules { get; }

        /// <summary>
        /// Gets the type creators used to create instances.
        /// </summary>
        IReadOnlyCollection<ITypeCreator> TypeCreators { get; }

        /// <summary>
        /// Gets the value generators used to generate flat values.
        /// </summary>
        IReadOnlyCollection<IValueGenerator> ValueGenerators { get; }
    }
}