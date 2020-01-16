namespace ModelBuilder
{
    using System.Collections.Generic;
    using ModelBuilder.TypeCreators;
    using ModelBuilder.ValueGenerators;

    /// <summary>
    ///     The <see cref="IBuildConfiguration" />
    ///     interface defines the configuration used to create and populate instances.
    /// </summary>
    public interface IBuildConfiguration
    {
        /// <summary>
        ///     Gets or sets the constructor resolver used to create an instance of a type.
        /// </summary>
        IConstructorResolver ConstructorResolver { get; set; }

        /// <summary>
        ///     Gets the creation rules used to quickly generate values without invoking <see cref="ITypeCreator" /> or
        ///     <see cref="IValueGenerator" /> instances.
        /// </summary>
        ICollection<CreationRule> CreationRules { get; }

        /// <summary>
        ///     Gets the execute order rules used to determine the order that properties are populated.
        /// </summary>
        ICollection<ExecuteOrderRule> ExecuteOrderRules { get; }

        /// <summary>
        ///     Gets the ignore rules used to skip over property population.
        /// </summary>
        ICollection<IgnoreRule> IgnoreRules { get; }

        /// <summary>
        ///     Gets the post build actions used to modify instances after they have been created or populated.
        /// </summary>
        ICollection<IPostBuildAction> PostBuildActions { get; }

        /// <summary>
        ///     Gets or sets the property resolver used to populate an instance of a type.
        /// </summary>
        IPropertyResolver PropertyResolver { get; set;  }

        /// <summary>
        ///     Gets the type creators used to create instances.
        /// </summary>
        ICollection<ITypeCreator> TypeCreators { get; }

        /// <summary>
        ///     Gets the rules used to map between types before attempting to create a value of the source type.
        /// </summary>
        ICollection<TypeMappingRule> TypeMappingRules { get; }

        /// <summary>
        ///     Gets the value generators used to generate flat values.
        /// </summary>
        ICollection<IValueGenerator> ValueGenerators { get; }
    }
}