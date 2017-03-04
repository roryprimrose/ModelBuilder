namespace ModelBuilder
{
    using System.Collections.ObjectModel;

    /// <summary>
    ///     The <see cref="IBuildStrategy" />
    ///     interface defines the configuration used to create and populate instances.
    /// </summary>
    public interface IBuildConfiguration
    {
        /// <summary>
        ///     Gets the constructor resolver used to create an instance of a type.
        /// </summary>
        IConstructorResolver ConstructorResolver { get; }

        /// <summary>
        ///     Gets the creation rules used to quickly generate values without invoking <see cref="ITypeCreator" /> or
        ///     <see cref="IValueGenerator" /> instances.
        /// </summary>
        ReadOnlyCollection<CreationRule> CreationRules { get; }

        /// <summary>
        ///     Gets the execute order rules used to determine the order that properties are populated.
        /// </summary>
        ReadOnlyCollection<ExecuteOrderRule> ExecuteOrderRules { get; }

        /// <summary>
        ///     Gets the ignore rules used to skip over property population.
        /// </summary>
        ReadOnlyCollection<IgnoreRule> IgnoreRules { get; }

        /// <summary>
        ///     Gets the post build actions used to modify instances after they have been created or populated.
        /// </summary>
        ReadOnlyCollection<IPostBuildAction> PostBuildActions { get; }

        /// <summary>
        ///     Gets the property resolver used to populate an instance of a type.
        /// </summary>
        IPropertyResolver PropertyResolver { get; }

        /// <summary>
        ///     Gets the type creators used to create instances.
        /// </summary>
        ReadOnlyCollection<ITypeCreator> TypeCreators { get; }

        /// <summary>
        ///     Gets the value generators used to generate flat values.
        /// </summary>
        ReadOnlyCollection<IValueGenerator> ValueGenerators { get; }
    }
}