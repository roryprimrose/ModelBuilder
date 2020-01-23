namespace ModelBuilder
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using ModelBuilder.ExecuteOrderRules;
    using ModelBuilder.IgnoreRules;
    using ModelBuilder.TypeCreators;
    using ModelBuilder.ValueGenerators;

    /// <summary>
    ///     The <see cref="BuildConfigurationExtensions" />
    ///     class provides extension methods for the <see cref="IBuildConfiguration" /> interface.
    /// </summary>
    public static class BuildConfigurationExtensions
    {
        /// <summary>
        ///     Adds a new post-build action to the configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="postBuildAction">The post-build action.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="postBuildAction" /> parameter is <c>null</c>.</exception>
        public static IBuildConfiguration Add(this IBuildConfiguration configuration, IPostBuildAction postBuildAction)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (postBuildAction == null)
            {
                throw new ArgumentNullException(nameof(postBuildAction));
            }

            configuration.PostBuildActions.Add(postBuildAction);

            return configuration;
        }

        /// <summary>
        ///     Adds a new execute order rule to the configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="rule">The rule.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="rule" /> parameter is <c>null</c>.</exception>
        public static IBuildConfiguration Add(this IBuildConfiguration configuration, IExecuteOrderRule rule)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (rule == null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            configuration.ExecuteOrderRules.Add(rule);

            return configuration;
        }

        /// <summary>
        ///     Adds a new ignore rule to the configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="rule">The rule.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="rule" /> parameter is <c>null</c>.</exception>
        public static IBuildConfiguration Add(this IBuildConfiguration configuration, IIgnoreRule rule)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (rule == null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            configuration.IgnoreRules.Add(rule);

            return configuration;
        }

        /// <summary>
        ///     Adds a new type mapping rule to the configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="rule">The rule.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="rule" /> parameter is <c>null</c>.</exception>
        public static IBuildConfiguration Add(this IBuildConfiguration configuration, TypeMappingRule rule)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (rule == null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            configuration.TypeMappingRules.Add(rule);

            return configuration;
        }

        /// <summary>
        ///     Adds a new creation rule to the configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="rule">The rule.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="rule" /> parameter is <c>null</c>.</exception>
        public static IBuildConfiguration Add(this IBuildConfiguration configuration, CreationRule rule)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (rule == null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            configuration.CreationRules.Add(rule);

            return configuration;
        }

        /// <summary>
        ///     Adds a new type creator to the configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="typeCreator">The type creator.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="typeCreator" /> parameter is <c>null</c>.</exception>
        public static IBuildConfiguration Add(this IBuildConfiguration configuration, ITypeCreator typeCreator)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (typeCreator == null)
            {
                throw new ArgumentNullException(nameof(typeCreator));
            }

            configuration.TypeCreators.Add(typeCreator);

            return configuration;
        }

        /// <summary>
        ///     Adds a new value generator to the configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="valueGenerator">The value generator.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="valueGenerator" /> parameter is <c>null</c>.</exception>
        public static IBuildConfiguration Add(this IBuildConfiguration configuration, IValueGenerator valueGenerator)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (valueGenerator == null)
            {
                throw new ArgumentNullException(nameof(valueGenerator));
            }

            configuration.ValueGenerators.Add(valueGenerator);

            return configuration;
        }

        /// <summary>
        ///     Adds configuration provided by the specified configuration module.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="module">The module.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="module" /> parameter is <c>null</c>.</exception>
        public static IBuildConfiguration Add(this IBuildConfiguration configuration, IConfigurationModule module)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (module == null)
            {
                throw new ArgumentNullException(nameof(module));
            }

            module.Configure(configuration);

            return configuration;
        }

        /// <summary>
        ///     Adds a new creation rule to the configuration.
        /// </summary>
        /// <typeparam name="T">The type of rule to add.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildConfiguration AddCreationRule<T>(this IBuildConfiguration configuration)
            where T : CreationRule, new()
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var rule = new T();

            configuration.CreationRules.Add(rule);

            return configuration;
        }

        /// <summary>
        ///     Adds a new creation rule to the configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <typeparam name="T">The type that holds the property.</typeparam>
        /// <param name="expression">The expression that identifies a property on <typeparamref name="T" /></param>
        /// <param name="priority">The priority of the rule.</param>
        /// <param name="value">The static value returned by the rule.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="expression" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="expression" /> parameter does not represent a property.</exception>
        /// <exception cref="ArgumentException">
        ///     The <paramref name="expression" /> parameter does not match a property on the type
        ///     to generate.
        /// </exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildConfiguration AddCreationRule<T>(
            this IBuildConfiguration configuration,
            Expression<Func<T, object>> expression,
            int priority,
            object value)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var targetType = typeof(T);
            var property = expression.GetProperty();

            var rule = new CreationRule(targetType, property.Name, priority, value);

            configuration.CreationRules.Add(rule);

            return configuration;
        }

        /// <summary>
        ///     Adds a new execute order rule to the configuration.
        /// </summary>
        /// <typeparam name="T">The type of rule to add.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildConfiguration AddExecuteOrderRule<T>(this IBuildConfiguration configuration)
            where T : IExecuteOrderRule, new()
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var rule = new T();

            configuration.ExecuteOrderRules.Add(rule);

            return configuration;
        }

        /// <summary>
        ///     Adds a new execute order rule to the configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <typeparam name="T">The type that holds the property.</typeparam>
        /// <param name="expression">The expression that identifies a property on <typeparamref name="T" /></param>
        /// <param name="priority">The priority of the rule.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="expression" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="expression" /> parameter does not represent a property.</exception>
        /// <exception cref="ArgumentException">
        ///     The <paramref name="expression" /> parameter does not match a property on the type
        ///     to generate.
        /// </exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildConfiguration AddExecuteOrderRule<T>(
            this IBuildConfiguration configuration,
            Expression<Func<T, object>> expression,
            int priority)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var targetType = typeof(T);
            var property = expression.GetProperty();

            var rule = new ExecuteOrderRule(targetType, property.PropertyType, property.Name, priority);

            configuration.ExecuteOrderRules.Add(rule);

            return configuration;
        }

        /// <summary>
        ///     Adds a new ignore rule to the configuration.
        /// </summary>
        /// <typeparam name="T">The type of rule to add.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildConfiguration AddIgnoreRule<T>(this IBuildConfiguration configuration)
            where T : IIgnoreRule, new()
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var rule = new T();

            configuration.IgnoreRules.Add(rule);

            return configuration;
        }

        /// <summary>
        ///     Adds a new ignore rule to the configuration.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <typeparam name="T">The type that holds the property.</typeparam>
        /// <param name="expression">The expression that identifies a property on <typeparamref name="T" /></param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="expression" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">The <paramref name="expression" /> parameter does not represent a property.</exception>
        /// <exception cref="ArgumentException">
        ///     The <paramref name="expression" /> parameter does not match a property on the type
        ///     to generate.
        /// </exception>
        public static IBuildConfiguration AddIgnoreRule<T>(
            this IBuildConfiguration configuration,
            Expression<Func<T, object>> expression)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var rule = new ExpressionIgnoreRule<T>(expression);

            configuration.IgnoreRules.Add(rule);

            return configuration;
        }

        /// <summary>
        ///     Adds a new post-build action to the configuration.
        /// </summary>
        /// <typeparam name="T">The type of post-build action to add.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildConfiguration AddPostBuildAction<T>(this IBuildConfiguration configuration)
            where T : IPostBuildAction, new()
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var postBuildAction = new T();

            configuration.PostBuildActions.Add(postBuildAction);

            return configuration;
        }

        /// <summary>
        ///     Adds a new type creator to the configuration.
        /// </summary>
        /// <typeparam name="T">The type of type creator to add.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildConfiguration AddTypeCreator<T>(this IBuildConfiguration configuration)
            where T : ITypeCreator, new()
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var creator = new T();

            configuration.TypeCreators.Add(creator);

            return configuration;
        }

        /// <summary>
        ///     Adds a new type mapping rule to the configuration.
        /// </summary>
        /// <typeparam name="T">The type of rule to add.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildConfiguration AddTypeMappingRule<T>(this IBuildConfiguration configuration)
            where T : TypeMappingRule, new()
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var rule = new T();

            configuration.TypeMappingRules.Add(rule);

            return configuration;
        }

        /// <summary>
        ///     Adds a new value generator to the configuration.
        /// </summary>
        /// <typeparam name="T">The type of value generator to add.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildConfiguration AddValueGenerator<T>(this IBuildConfiguration configuration)
            where T : IValueGenerator, new()
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var generator = new T();

            configuration.ValueGenerators.Add(generator);

            return configuration;
        }


        /// <summary>
        ///     Creates an instance of <typeparamref name="T" /> using the specified build configuration and any provided
        ///     constructor arguments.
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
        ///     Appends a new <see cref="IIgnoreRule" /> to the build configuration using the specified expression.
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

            var rule = new ExpressionIgnoreRule<T>(expression);

            return Add(buildConfiguration, rule);
        }

        /// <summary>
        ///     Appends a new <see cref="TypeMappingRule" /> to the build configuration using the specified types.
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

            return buildConfiguration.Add(rule);
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
        ///     Removes creation rules from the configuration that match the specified type.
        /// </summary>
        /// <typeparam name="T">The type of rule to remove.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildConfiguration RemoveCreationRule<T>(this IBuildConfiguration configuration)
            where T : CreationRule
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var itemsToRemove = configuration.CreationRules.Where(x => x.GetType().IsAssignableFrom(typeof(T)))
                .ToList();

            foreach (var rule in itemsToRemove)
            {
                configuration.CreationRules.Remove(rule);
            }

            return configuration;
        }

        /// <summary>
        ///     Removes execute order rules from the configuration that match the specified type.
        /// </summary>
        /// <typeparam name="T">The type of rule to remove.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildConfiguration RemoveExecuteOrderRule<T>(this IBuildConfiguration configuration)
            where T : IExecuteOrderRule
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var itemsToRemove = configuration.ExecuteOrderRules.Where(x => x.GetType().IsAssignableFrom(typeof(T)))
                .ToList();

            foreach (var rule in itemsToRemove)
            {
                configuration.ExecuteOrderRules.Remove(rule);
            }

            return configuration;
        }

        /// <summary>
        ///     Removes ignore rules from the configuration that match the specified type.
        /// </summary>
        /// <typeparam name="T">The type of rule to remove.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildConfiguration RemoveIgnoreRule<T>(this IBuildConfiguration configuration)
            where T : IIgnoreRule
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var itemsToRemove = configuration.IgnoreRules.Where(x => x.GetType().IsAssignableFrom(typeof(T))).ToList();

            foreach (var rule in itemsToRemove)
            {
                configuration.IgnoreRules.Remove(rule);
            }

            return configuration;
        }

        /// <summary>
        ///     Removes post-build actions from the configuration that match the specified type.
        /// </summary>
        /// <typeparam name="T">The type of post-build action to remove.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildConfiguration RemovePostBuildAction<T>(this IBuildConfiguration configuration)
            where T : IPostBuildAction
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var itemsToRemove = configuration.PostBuildActions.Where(x => x.GetType().IsAssignableFrom(typeof(T)))
                .ToList();

            foreach (var rule in itemsToRemove)
            {
                configuration.PostBuildActions.Remove(rule);
            }

            return configuration;
        }

        /// <summary>
        ///     Removes type creators from the configuration that match the specified type.
        /// </summary>
        /// <typeparam name="T">The type of type creator to remove.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildConfiguration RemoveTypeCreator<T>(this IBuildConfiguration configuration)
            where T : ITypeCreator
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var itemsToRemove = configuration.TypeCreators.Where(x => x.GetType().IsAssignableFrom(typeof(T))).ToList();

            foreach (var rule in itemsToRemove)
            {
                configuration.TypeCreators.Remove(rule);
            }

            return configuration;
        }

        /// <summary>
        ///     Removes type mapping rules from the configuration that match the specified type.
        /// </summary>
        /// <typeparam name="T">The type of rule to remove.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildConfiguration RemoveTypeMappingRule<T>(this IBuildConfiguration configuration)
            where T : TypeMappingRule
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var itemsToRemove = configuration.TypeMappingRules.Where(x => x.GetType().IsAssignableFrom(typeof(T)))
                .ToList();

            foreach (var rule in itemsToRemove)
            {
                configuration.TypeMappingRules.Remove(rule);
            }

            return configuration;
        }

        /// <summary>
        ///     Removes value generators from the configuration that match the specified type.
        /// </summary>
        /// <typeparam name="T">The type of value generator to remove.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildConfiguration RemoveValueGenerator<T>(this IBuildConfiguration configuration)
            where T : IValueGenerator
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var itemsToRemove = configuration.ValueGenerators.Where(x => x.GetType().IsAssignableFrom(typeof(T)))
                .ToList();

            foreach (var rule in itemsToRemove)
            {
                configuration.ValueGenerators.Remove(rule);
            }

            return configuration;
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

            var executeStrategy = new T();

            executeStrategy.Initialize(buildConfiguration);

            return executeStrategy;
        }

        /// <summary>
        ///     Adds configuration using the specified module type.
        /// </summary>
        /// <typeparam name="T">The type of configuration module to add.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <returns>The configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification =
                "This signature is designed for ease of use rather than requiring that T is either a parameter or return type.")]
        public static IBuildConfiguration UsingModule<T>(this IBuildConfiguration configuration)
            where T : IConfigurationModule, new()
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var module = new T();

            module.Configure(configuration);

            return configuration;
        }
    }
}