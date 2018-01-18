﻿namespace ModelBuilder
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
        ///     Creates an instance of a type using the default build and execute strategies.
        /// </summary>
        /// <param name="instanceType">The type of instance to create.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="instanceType" /> parameter is null.</exception>
        public static object Create(Type instanceType)
        {
            if (instanceType == null)
            {
                throw new ArgumentNullException(nameof(instanceType));
            }

            return With<DefaultExecuteStrategy>().Create(instanceType);
        }

        /// <summary>
        ///     Creates an instance of <typeparamref name="T" /> using the default build and execute strategies.
        /// </summary>
        /// <typeparam name="T">The type of instance to create.</typeparam>
        /// <returns>The new instance.</returns>
        public static T Create<T>()
        {
            return For<T>().Create();
        }

        /// <summary>
        ///     Creates an instance of a type using the default build and execute strategies and constructor
        ///     arguments.
        /// </summary>
        /// <param name="instanceType">The type of instance to create.</param>
        /// <param name="args">The constructor arguments to create the type with.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="instanceType" /> parameter is null.</exception>
        public static object CreateWith(Type instanceType, params object[] args)
        {
            if (instanceType == null)
            {
                throw new ArgumentNullException(nameof(instanceType));
            }

            return With<DefaultExecuteStrategy>().CreateWith(instanceType, args);
        }

        /// <summary>
        ///     Creates an instance of <typeparamref name="T" /> using the default build and execute strategies and constructor
        ///     arguments.
        /// </summary>
        /// <typeparam name="T">The type of instance to create.</typeparam>
        /// <param name="args">The constructor arguments to create the type with.</param>
        /// <returns>The new instance.</returns>
        public static T CreateWith<T>(params object[] args)
        {
            return For<T>().CreateWith(args);
        }

        /// <summary>
        ///     Returns a default execute strategy with a default build strategy.
        /// </summary>
        /// <typeparam name="T">The type of instance to create using the execute strategy.</typeparam>
        /// <returns>A new execute strategy.</returns>
        public static IExecuteStrategy<T> For<T>()
        {
            return With<DefaultExecuteStrategy<T>>();
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
        ///     Populates the properties of the specified instance using the default build and execute strategies.
        /// </summary>
        /// <typeparam name="T">The type of instance to create.</typeparam>
        /// <returns>The new instance.</returns>
        public static T Populate<T>(T instance)
        {
            return For<T>().Populate(instance);
        }

        /// <summary>
        ///     Returns a new <see cref="IBuildStrategy" /> of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of build strategy to create.</typeparam>
        /// <returns>A new build strategy.</returns>
        public static T Using<T>() where T : IBuildStrategy, new()
        {
            var strategy = new T();

            return strategy;
        }

        /// <summary>
        ///     Returns a new execute strategy using <see cref="ModelBuilder.BuildStrategy" />.
        /// </summary>
        /// <typeparam name="T">The type of execute strategy to create.</typeparam>
        /// <returns>A new execute strategy.</returns>
        public static T With<T>() where T : IExecuteStrategy, new()
        {
            return BuildStrategy.With<T>();
        }

        private static IBuildStrategy CreateDefaultBuildStrategy()
        {
            var compiler = new DefaultBuildStrategyCompiler();

#if NET45
            compiler.ScanModules();
#endif

            return compiler.Compile();
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