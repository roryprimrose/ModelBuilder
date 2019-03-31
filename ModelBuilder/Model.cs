namespace ModelBuilder
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    ///     The <see cref="Model" />
    ///     class provides the main entry point into generating model instances.
    /// </summary>
    public static class Model
    {
        private static IBuildStrategy _buildStrategy;

        /// <summary>
        ///     Creates an instance of a type using the default build and execute strategies and constructor any provided
        ///     arguments.
        /// </summary>
        /// <param name="instanceType">The type of instance to create.</param>
        /// <param name="args">The constructor arguments to create the type with.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="instanceType" /> parameter is null.</exception>
        public static object Create(Type instanceType, params object[] args)
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
        public static T Create<T>(params object[] args)
        {
            return ResolveDefault<T>().Create(args);
        }

        /// <summary>
        ///     Returns a <see cref="IBuildStrategy" /> with a new <see cref="IgnoreRule" /> that matches the specified expression.
        /// </summary>
        /// <typeparam name="T">The type of instance that matches the rule.</typeparam>
        /// <param name="expression">The expression that identifies a property on <typeparamref name="T" /></param>
        /// <returns>A new build strategy.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="expression" /> parameter is null.</exception>
        public static IBuildStrategy Ignoring<T>(Expression<Func<T, object>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            return BuildStrategy.Ignoring(expression);
        }

        /// <summary>
        ///     Returns a <see cref="IBuildStrategy" /> with a new <see cref="TypeMappingRule" /> that matches the specified
        ///     expression.
        /// </summary>
        /// <typeparam name="TSource">The source type to use for type mapping.</typeparam>
        /// <typeparam name="TTarget">The target type to use for type mapping.</typeparam>
        /// <returns>A new build strategy.</returns>
        public static IBuildStrategy Mapping<TSource, TTarget>()
        {
            return BuildStrategy.Mapping<TSource, TTarget>();
        }

        /// <summary>
        ///     Populates the properties of the specified instance using the default build and execute strategies.
        /// </summary>
        /// <typeparam name="T">The type of instance to create.</typeparam>
        /// <returns>The new instance.</returns>
        public static T Populate<T>(T instance)
        {
            return ResolveDefault<T>().Populate(instance);
        }

        /// <summary>
        ///     Returns a new <see cref="IBuildStrategy" /> of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of build strategy to create.</typeparam>
        /// <returns>A new build strategy.</returns>
        public static T UsingBuildStrategy<T>() where T : IBuildStrategy, new()
        {
            return new T();
        }

        /// <summary>
        ///     Returns a new execute strategy using <see cref="ModelBuilder.BuildStrategy" />.
        /// </summary>
        /// <typeparam name="T">The type of execute strategy to create.</typeparam>
        /// <returns>A new execute strategy.</returns>
        public static T UsingExecuteStrategy<T>() where T : IExecuteStrategy, new()
        {
            return BuildStrategy.UsingExecuteStrategy<T>();
        }

        /// <summary>
        ///     Returns a new build strategy using the specified <see cref="ICompilerModule" /> with
        ///     <see cref="ModelBuilder.BuildStrategy" />.
        /// </summary>
        /// <typeparam name="T">The type of compiler module to create the build strategy with.</typeparam>
        /// <returns>A new execute strategy.</returns>
        public static IBuildStrategy UsingModule<T>() where T : ICompilerModule, new()
        {
            var compiler = BuildStrategy.Clone();

            compiler.AddCompilerModule<T>();

            return compiler.Compile();
        }

        private static IBuildStrategy CreateDefaultBuildStrategy()
        {
            var compiler = new DefaultBuildStrategyCompiler();

            return compiler.Compile();
        }

        private static IExecuteStrategy ResolveDefault()
        {
            return UsingExecuteStrategy<DefaultExecuteStrategy>();
        }

        private static IExecuteStrategy<T> ResolveDefault<T>()
        {
            return UsingExecuteStrategy<DefaultExecuteStrategy<T>>();
        }

        /// <summary>
        ///     Gets or sets the current build strategy to use in this application domain.
        /// </summary>
        /// <exception cref="ArgumentNullException">The <paramref name="value" /> parameter is null.</exception>
        public static IBuildStrategy BuildStrategy
        {
            get
            {
                // Handle the edge case where the _buildStrategy may have been assigned in the static before _defaultBuildStrategy
                if (_buildStrategy == null)
                {
                    _buildStrategy = DefaultBuildStrategy;
                }

                return _buildStrategy;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _buildStrategy = value;
            }
        }

        /// <summary>
        ///     Gets or sets the default build strategy.
        /// </summary>
        public static IBuildStrategy DefaultBuildStrategy { get; } = CreateDefaultBuildStrategy();
    }
}