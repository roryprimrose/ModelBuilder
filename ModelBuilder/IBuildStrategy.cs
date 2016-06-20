namespace ModelBuilder
{
    using System.Collections.ObjectModel;

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
        /// Gets the build log for items created by this strategy.
        /// </summary>
        IBuildLog BuildLog { get; }

        /// <summary>
        /// Gets the constructor resolver used to create an instance of a type.
        /// </summary>
        IConstructorResolver ConstructorResolver { get; }

        /// <summary>
        /// Gets the creation rules used to quickly generate values without invoking <see cref="ITypeCreator"/> or <see cref="IValueGenerator"/> instances.
        /// </summary>
        ReadOnlyCollection<CreationRule> CreationRules { get; }

        /// <summary>
        /// Gets the execute order rules used to determine the order that properties are populated.
        /// </summary>
        ReadOnlyCollection<ExecuteOrderRule> ExecuteOrderRules { get; }

        /// <summary>
        /// Gets the ignore rules used to skip over property population.
        /// </summary>
        ReadOnlyCollection<IgnoreRule> IgnoreRules { get; }

        /// <summary>
        /// Gets the type creators used to create instances.
        /// </summary>
        ReadOnlyCollection<ITypeCreator> TypeCreators { get; }

        /// <summary>
        /// Gets the value generators used to generate flat values.
        /// </summary>
        ReadOnlyCollection<IValueGenerator> ValueGenerators { get; }
    }
}