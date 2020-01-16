namespace ModelBuilder
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq.Expressions;
    using ModelBuilder.Properties;

    /// <summary>
    ///     The <see cref="Extensions" />
    ///     class provides extension methods for the <see cref="IBuildStrategy" /> interface.
    /// </summary>
    public static class BuildStrategyExtensions
    {
        /// <summary>
        ///     Clones the specified builder strategy and returns a compiler.
        /// </summary>
        /// <param name="configuration">The build configuration to create the instance with.</param>
        /// <returns>The new build strategy compiler.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        public static IBuildStrategyCompiler Clone(this IBuildStrategy configuration)
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
        ///     Creates an instance of <typeparamref name="T" /> using the specified build strategy and any provided constructor
        ///     arguments.
        /// </summary>
        /// <typeparam name="T">The type of instance to create.</typeparam>
        /// <param name="buildStrategy">The build strategy to create the instance with.</param>
        /// <param name="args">The constructor arguments to create the type with.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="buildStrategy" /> parameter is <c>null</c>.</exception>
        public static T Create<T>(this IBuildStrategy buildStrategy, params object[] args)
        {
            if (buildStrategy == null)
            {
                throw new ArgumentNullException(nameof(buildStrategy));
            }

            return buildStrategy.UsingExecuteStrategy<DefaultExecuteStrategy<T>>().Create(args);
        }

        /// <summary>
        ///     Creates an instance of a type using the specified build strategy and any provided constructor arguments.
        /// </summary>
        /// <param name="buildStrategy">The build strategy to create the instance with.</param>
        /// <param name="instanceType">The type of instance to create.</param>
        /// <param name="args">The constructor arguments to create the type with.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="buildStrategy" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="instanceType" /> parameter is <c>null</c>.</exception>
        public static object Create(this IBuildStrategy buildStrategy, Type instanceType, params object[] args)
        {
            if (buildStrategy == null)
            {
                throw new ArgumentNullException(nameof(buildStrategy));
            }

            if (instanceType == null)
            {
                throw new ArgumentNullException(nameof(instanceType));
            }

            return buildStrategy.UsingExecuteStrategy<DefaultExecuteStrategy>().Create(instanceType, args);
        }

        /// <summary>
        ///     Appends a new <see cref="IgnoreRule" /> to the specified <see cref="IExecuteStrategy{T}" /> using the specified
        ///     expression.
        /// </summary>
        /// <typeparam name="T">The type of instance that matches the rule.</typeparam>
        /// <param name="buildStrategy">The build strategy to clone.</param>
        /// <param name="expression">The expression that identifies a property on <typeparamref name="T" /></param>
        /// <returns>A cloned build strategy with the new rule.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="buildStrategy" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="expression" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="expression" /> parameter does not represent a property.</exception>
        /// <exception cref="ArgumentException">
        ///     The <paramref name="expression" /> parameter does not match a property on the type
        ///     to generate.
        /// </exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This type is required in order to support the fluent syntax of call sites.")]
        public static IBuildStrategy Ignoring<T>(
            this IBuildStrategy buildStrategy,
            Expression<Func<T, object>> expression)
        {
            if (buildStrategy == null)
            {
                throw new ArgumentNullException(nameof(buildStrategy));
            }

            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var targetType = typeof(T);
            var property = expression.GetProperty();

            var rule = new IgnoreRule(targetType, property.Name);

            return buildStrategy.Clone().Add(rule).Compile();
        }

        /// <summary>
        ///     Appends a new <see cref="TypeMappingRule" /> to the specified <see cref="IExecuteStrategy{T}" /> using the
        ///     specified types.
        /// </summary>
        /// <typeparam name="TSource">The source type to use for type mapping.</typeparam>
        /// <typeparam name="TTarget">The target type to use for type mapping.</typeparam>
        /// <param name="buildStrategy">The build strategy to clone.</param>
        /// <returns>A cloned build strategy with the new rule.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="buildStrategy" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This type is required in order to support the fluent syntax of call sites.")]
        public static IBuildStrategy Mapping<TSource, TTarget>(this IBuildStrategy buildStrategy)
        {
            if (buildStrategy == null)
            {
                throw new ArgumentNullException(nameof(buildStrategy));
            }

            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);

            var rule = new TypeMappingRule(sourceType, targetType);

            return buildStrategy.Clone().Add(rule).Compile();
        }

        /// <summary>
        ///     Populates the instance using the specified build strategy.
        /// </summary>
        /// <typeparam name="T">The type of instance to populate.</typeparam>
        /// <param name="buildStrategy">The build strategy.</param>
        /// <param name="instance">The instance to populate.</param>
        /// <returns>The updated instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="buildStrategy" /> parameter is <c>null</c>.</exception>
        public static T Populate<T>(this IBuildStrategy buildStrategy, T instance)
        {
            if (buildStrategy == null)
            {
                throw new ArgumentNullException(nameof(buildStrategy));
            }

            return buildStrategy.UsingExecuteStrategy<DefaultExecuteStrategy<T>>().Populate(instance);
        }

        /// <summary>
        ///     Returns a new <see cref="IExecuteStrategy{T}" /> for the specified build strategy.
        /// </summary>
        /// <typeparam name="T">The type of execute strategy to return.</typeparam>
        /// <param name="buildStrategy">The build strategy.</param>
        /// <returns>A new execute strategy.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="buildStrategy" /> parameter is <c>null</c>.</exception>
        public static T UsingExecuteStrategy<T>(this IBuildStrategy buildStrategy) where T : IExecuteStrategy, new()
        {
            if (buildStrategy == null)
            {
                throw new ArgumentNullException(nameof(buildStrategy));
            }

            var buildLog = buildStrategy.GetBuildLog();

            if (buildLog == null)
            {
                var message = string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.BuildStrategy_BuildLogRequired,
                    buildStrategy.GetType().FullName,
                    nameof(IBuildLog),
                    nameof(IExecuteStrategy<T>));

                throw new InvalidOperationException(message);
            }

            var executeStrategy = new T();

            executeStrategy.Initialize(buildStrategy, buildLog);

            return executeStrategy;
        }
    }
}