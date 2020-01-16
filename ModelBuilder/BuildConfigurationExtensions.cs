namespace ModelBuilder
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    ///     The <see cref="BuildConfigurationExtensions" />
    ///     class provides extension methods for the <see cref="IBuildConfiguration" /> interface.
    /// </summary>
    public static class BuildConfigurationExtensions
    {
        /// <summary>
        ///     Clones the specified builder strategy and returns a compiler.
        /// </summary>
        /// <param name="configuration">The build configuration to create the instance with.</param>
        /// <returns>The new build strategy compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
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

        /// <summary>
        ///     Creates an instance of <typeparamref name="T" /> using the specified build configuration and any provided
        ///     constructor
        ///     arguments.
        /// </summary>
        /// <typeparam name="T">The type of instance to create.</typeparam>
        /// <param name="buildConfiguration">The build configuration to create the instance with.</param>
        /// <param name="args">The constructor arguments to create the type with.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="buildConfiguration" /> parameter is <c>null</c>.</exception>
        /// <remarks>This method uses <see cref="DefaultExecuteStrategy{T}" /> to create the instance.</remarks>
        public static T Create<T>(this IBuildConfiguration buildConfiguration, params object[] args)
        {
            if (buildConfiguration == null)
            {
                throw new ArgumentNullException(nameof(buildConfiguration));
            }

            return buildConfiguration.UsingExecuteStrategy<DefaultExecuteStrategy<T>>().Create(args);
        }

        /// <summary>
        ///     Creates an instance of a type using the specified build configuration and any provided constructor arguments.
        /// </summary>
        /// <param name="buildConfiguration">The build configuration to create the instance with.</param>
        /// <param name="instanceType">The type of instance to create.</param>
        /// <param name="args">The constructor arguments to create the type with.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="buildConfiguration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="instanceType" /> parameter is <c>null</c>.</exception>
        /// <remarks>This method uses <see cref="DefaultExecuteStrategy" /> to create the instance.</remarks>
        public static object Create(this IBuildConfiguration buildConfiguration, Type instanceType,
            params object[] args)
        {
            if (buildConfiguration == null)
            {
                throw new ArgumentNullException(nameof(buildConfiguration));
            }

            if (instanceType == null)
            {
                throw new ArgumentNullException(nameof(instanceType));
            }

            return buildConfiguration.UsingExecuteStrategy<DefaultExecuteStrategy>().Create(instanceType, args);
        }

        /// <summary>
        ///     Appends a new <see cref="IgnoreRule" /> to the build configuration using the specified
        ///     expression.
        /// </summary>
        /// <typeparam name="T">The type of instance that matches the rule.</typeparam>
        /// <param name="buildConfiguration">The build configuration to update.</param>
        /// <param name="expression">The expression that identifies a property on <typeparamref name="T" /></param>
        /// <returns>The build configuration with the new rule.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="buildConfiguration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="expression" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="expression" /> parameter does not represent a property.</exception>
        /// <exception cref="ArgumentException">
        ///     The <paramref name="expression" /> parameter does not match a property on the type
        ///     to generate.
        /// </exception>
        public static IBuildConfiguration Ignoring<T>(
            this IBuildConfiguration buildConfiguration,
            Expression<Func<T, object>> expression)
        {
            if (buildConfiguration == null)
            {
                throw new ArgumentNullException(nameof(buildConfiguration));
            }

            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var targetType = typeof(T);
            var property = expression.GetProperty();

            var rule = new IgnoreRule(targetType, property.Name);

            return buildConfiguration.Clone().Add(rule).Compile();
        }

        /// <summary>
        ///     Appends a new <see cref="TypeMappingRule" /> to the build configuration using the
        ///     specified types.
        /// </summary>
        /// <typeparam name="TSource">The source type to use for type mapping.</typeparam>
        /// <typeparam name="TTarget">The target type to use for type mapping.</typeparam>
        /// <param name="buildConfiguration">The build configuration to update.</param>
        /// <returns>The build configuration with the new rule.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="buildConfiguration" /> parameter is <c>null</c>.</exception>
        public static IBuildConfiguration Mapping<TSource, TTarget>(this IBuildConfiguration buildConfiguration)
        {
            if (buildConfiguration == null)
            {
                throw new ArgumentNullException(nameof(buildConfiguration));
            }

            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);

            var rule = new TypeMappingRule(sourceType, targetType);

            return buildConfiguration.Clone().Add(rule).Compile();
        }

        /// <summary>
        ///     Populates the specified instance using the provided build configuration.
        /// </summary>
        /// <typeparam name="T">The type of instance to populate.</typeparam>
        /// <param name="buildConfiguration">The build configuration to use.</param>
        /// <param name="instance">The instance to populate.</param>
        /// <returns>The updated instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="buildConfiguration" /> parameter is <c>null</c>.</exception>
        public static T Populate<T>(this IBuildConfiguration buildConfiguration, T instance)
        {
            if (buildConfiguration == null)
            {
                throw new ArgumentNullException(nameof(buildConfiguration));
            }

            return buildConfiguration.UsingExecuteStrategy<DefaultExecuteStrategy<T>>().Populate(instance);
        }

        /// <summary>
        ///     Returns a new <see cref="IExecuteStrategy{T}" /> for the specified build configuration.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IExecuteStrategy{T}" /> to return.</typeparam>
        /// <param name="buildConfiguration">The build configuration.</param>
        /// <returns>A new execute strategy.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="buildConfiguration" /> parameter is <c>null</c>.</exception>
        public static T UsingExecuteStrategy<T>(this IBuildConfiguration buildConfiguration)
            where T : IExecuteStrategy, new()
        {
            if (buildConfiguration == null)
            {
                throw new ArgumentNullException(nameof(buildConfiguration));
            }

            var buildLog = new DefaultBuildLog();

            var executeStrategy = new T();

            executeStrategy.Initialize(buildConfiguration, buildLog);

            return executeStrategy;
        }
    }
}