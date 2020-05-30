namespace ModelBuilder
{
    using System;
    using System.Linq.Expressions;
    using ModelBuilder.IgnoreRules;

    /// <summary>
    ///     The <see cref="Model" />
    ///     class provides the main entry point into generating model instances.
    /// </summary>
    public static class Model
    {
        /// <summary>
        ///     Creates an instance of a type using the default build and execute strategies and constructor any provided
        ///     arguments.
        /// </summary>
        /// <param name="instanceType">The type of instance to create.</param>
        /// <param name="args">The constructor arguments to create the type with.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="instanceType" /> parameter is <c>null</c>.</exception>
        public static object Create(Type instanceType, params object?[]? args)
        {
            if (instanceType == null)
            {
                throw new ArgumentNullException(nameof(instanceType));
            }

            return ResolveDefault().Create(instanceType, args);
        }

        /// <summary>
        ///     Creates an instance of <typeparamref name="T" /> using the default build and execute strategies and any provided
        ///     constructor
        ///     arguments.
        /// </summary>
        /// <typeparam name="T">The type of instance to create.</typeparam>
        /// <param name="args">The constructor arguments to create the type with.</param>
        /// <returns>The new instance.</returns>
        public static T Create<T>(params object?[]? args) where T : notnull
        {
            return ResolveDefault<T>().Create(args);
        }

        /// <summary>
        ///     Returns a <see cref="IBuildConfiguration" /> with a new <see cref="IIgnoreRule" /> that matches the specified
        ///     expression.
        /// </summary>
        /// <typeparam name="T">The type of instance that matches the rule.</typeparam>
        /// <param name="expression">The expression that identifies a property on <typeparamref name="T" /></param>
        /// <returns>A new build configuration.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="expression" /> parameter is <c>null</c>.</exception>
        public static IBuildConfiguration Ignoring<T>(Expression<Func<T, object>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            return UsingDefaultConfiguration().Ignoring(expression);
        }

        /// <summary>
        ///     Returns a <see cref="IBuildConfiguration" /> with a new <see cref="TypeMappingRule" /> that matches the specified
        ///     expression.
        /// </summary>
        /// <typeparam name="TSource">The source type to use for type mapping.</typeparam>
        /// <typeparam name="TTarget">The target type to use for type mapping.</typeparam>
        /// <returns>A new build configuration.</returns>
        public static IBuildConfiguration Mapping<TSource, TTarget>()
        {
            return UsingDefaultConfiguration().Mapping<TSource, TTarget>();
        }

        /// <summary>
        ///     Populates the properties of the specified instance using the default build and execute strategies.
        /// </summary>
        /// <typeparam name="T">The type of instance to create.</typeparam>
        /// <returns>The new instance.</returns>
        public static T Populate<T>(T instance) where T : notnull
        {
            return ResolveDefault<T>().Populate(instance);
        }

        /// <summary>
        ///     Returns a new <see cref="IBuildConfiguration" /> that is configured using <see cref="DefaultConfigurationModule" />
        ///     .
        /// </summary>
        /// <returns>The new build configuration.</returns>
        public static IBuildConfiguration UsingDefaultConfiguration()
        {
			var configuration = new BuildConfiguration();
			
            return configuration.UsingModule<DefaultConfigurationModule>();
        }

        /// <summary>
        ///     Returns a new execute strategy configured with <see cref="DefaultConfigurationModule" />.
        /// </summary>
        /// <typeparam name="T">The type of execute strategy to create.</typeparam>
        /// <returns>A new execute strategy.</returns>
        public static T UsingExecuteStrategy<T>() where T : IExecuteStrategy, new()
        {
            return UsingDefaultConfiguration().UsingExecuteStrategy<T>();
        }

        /// <summary>
        ///     Returns a configuration using the specified <see cref="IConfigurationModule" />.
        /// </summary>
        /// <typeparam name="T">The type of configuration module to use.</typeparam>
        /// <returns>The build configuration.</returns>
        public static IBuildConfiguration UsingModule<T>() where T : IConfigurationModule, new()
        {
            return UsingDefaultConfiguration().UsingModule<T>();
        }

        private static IExecuteStrategy ResolveDefault()
        {
            return UsingExecuteStrategy<DefaultExecuteStrategy>();
        }

        private static IExecuteStrategy<T> ResolveDefault<T>() where T : notnull
        {
            return UsingExecuteStrategy<DefaultExecuteStrategy<T>>();
        }
    }
}