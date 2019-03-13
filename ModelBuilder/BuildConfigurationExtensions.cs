namespace ModelBuilder
{
    using System;

    /// <summary>
    ///     The <see cref="Extensions" />
    ///     class provides extension methods for the <see cref="IBuildConfiguration" /> interface.
    /// </summary>
    public static class BuildConfigurationExtensions
    {
        /// <summary>
        ///     Clones the specified builder strategy and returns a compiler.
        /// </summary>
        /// <param name="configuration">The build configuration to create the instance with.</param>
        /// <returns>The new build strategy compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is null.</exception>
        public static IBuildStrategyCompiler Clone(this IBuildConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var compiler = new BuildStrategyCompiler
            {
                ConstructorResolver = configuration.ConstructorResolver,
                PropertyResolver = configuration.PropertyResolver
            };

            foreach (var executeOrderRule in configuration.ExecuteOrderRules)
            {
                compiler.ExecuteOrderRules.Add(executeOrderRule);
            }

            foreach (var ignoreRule in configuration.IgnoreRules)
            {
                compiler.IgnoreRules.Add(ignoreRule);
            }
            
            foreach (var typeMappingRule in configuration.TypeMappingRules)
            {
                compiler.TypeMappingRules.Add(typeMappingRule);
            }

            foreach (var creationRule in configuration.CreationRules)
            {
                compiler.CreationRules.Add(creationRule);
            }

            foreach (var typeCreator in configuration.TypeCreators)
            {
                compiler.TypeCreators.Add(typeCreator);
            }

            foreach (var valueGenerator in configuration.ValueGenerators)
            {
                compiler.ValueGenerators.Add(valueGenerator);
            }

            return compiler;
        }
    }
}