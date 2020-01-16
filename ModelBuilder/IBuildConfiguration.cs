namespace ModelBuilder
{
    using System.Collections.ObjectModel;
    using ModelBuilder.TypeCreators;
    using ModelBuilder.ValueGenerators;

    /// <summary>
    ///     The <see cref="IBuildConfiguration" />
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
        Collection<CreationRule> CreationRules { get; }

        /// <summary>
        ///     Gets the execute order rules used to determine the order that properties are populated.
        /// </summary>
        Collection<ExecuteOrderRule> ExecuteOrderRules { get; }

        /// <summary>
        ///     Gets the ignore rules used to skip over property population.
        /// </summary>
        Collection<IgnoreRule> IgnoreRules { get; }

        /// <summary>
        ///     Gets the post build actions used to modify instances after they have been created or populated.
        /// </summary>
        Collection<IPostBuildAction> PostBuildActions { get; }

        /// <summary>
        ///     Gets the property resolver used to populate an instance of a type.
        /// </summary>
        IPropertyResolver PropertyResolver { get; }

        /// <summary>
        ///     Gets the type creators used to create instances.
        /// </summary>
        Collection<ITypeCreator> TypeCreators { get; }

        /// <summary>
        ///     Gets the rules used to map between types before attempting to create a value of the source type.
        /// </summary>
        Collection<TypeMappingRule> TypeMappingRules { get; }

        /// <summary>
        ///     Gets the value generators used to generate flat values.
        /// </summary>
        Collection<IValueGenerator> ValueGenerators { get; }
    }
}