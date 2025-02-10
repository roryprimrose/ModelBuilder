namespace ModelBuilder
{
    public interface IBuildConfiguration
    {
        ///// <summary>
        /////     Gets the creation rules used to quickly generate values without invoking <see cref="ITypeCreator" /> or
        /////     <see cref="IValueGenerator" /> instances.
        ///// </summary>
        //ICollection<ICreationRule> CreationRules { get; }

        ///// <summary>
        /////     Gets the execute order rules used to determine the order that properties are populated.
        ///// </summary>
        //ICollection<IExecuteOrderRule> ExecuteOrderRules { get; }

        ///// <summary>
        /////     Gets the ignore rules used to skip over property population.
        ///// </summary>
        //ICollection<IIgnoreRule> IgnoreRules { get; }

        ///// <summary>
        /////     Gets the post build actions used to modify instances after they have been created or populated.
        ///// </summary>
        //ICollection<IPostBuildAction> PostBuildActions { get; }

        ///// <summary>
        /////     Gets the rules used to map between types before attempting to create a value of the source type.
        ///// </summary>
        //ICollection<TypeMappingRule> TypeMappingRules { get; }

        ///// <summary>
        /////     Gets or sets the type resolver used to determine the build type.
        ///// </summary>
        //ITypeResolver TypeResolver { get; set; }

        /// <summary>
        ///     Gets the generators used to generate values.
        /// </summary>
        ICollection<IGenerator> Generators { get; }
    }


}
