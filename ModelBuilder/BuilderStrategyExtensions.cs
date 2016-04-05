using System;
using System.Linq;
using System.Linq.Expressions;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="Extensions"/>
    /// class provides extension methods for the <see cref="IBuildStrategy"/> interface.
    /// </summary>
    public static class BuilderStrategyExtensions
    {
        /// <summary>
        /// Creates an instance of <typeparamref name="T"/> using the specified build strategy.
        /// </summary>
        /// <typeparam name="T">The type of instance to create.</typeparam>
        /// <param name="buildStrategy">The build strategy to create the instance with.</param>
        /// <returns>The new instance.</returns>
        public static T Create<T>(this IBuildStrategy buildStrategy)
        {
            if (buildStrategy == null)
            {
                throw new ArgumentNullException(nameof(buildStrategy));
            }

            return buildStrategy.CreateWith<T>();
        }

        /// <summary>
        /// Creates an instance of <typeparamref name="T"/> using the specified build strategy and constructor arguments.
        /// </summary>
        /// <typeparam name="T">The type of instance to create.</typeparam>
        /// <param name="buildStrategy">The build strategy to create the instance with.</param>
        /// <param name="args">The constructor arguments to create the type with.</param>
        /// <returns>The new instance.</returns>
        public static T CreateWith<T>(this IBuildStrategy buildStrategy, params object[] args)
        {
            if (buildStrategy == null)
            {
                throw new ArgumentNullException(nameof(buildStrategy));
            }

            return buildStrategy.With<DefaultExecuteStrategy<T>>().CreateWith(args);
        }

        /// <summary>
        /// Returns an <see cref="IExecuteStrategy{T}"/> for the specified build strategy with a new <see cref="IgnoreRule"/> that matches the specified expression.
        /// </summary>
        /// <typeparam name="T">The type of instance that matches the rule.</typeparam>
        /// <typeparam name="TProperty">The type of property to ignore.</typeparam>
        /// <param name="buildStrategy">The build strategy.</param>
        /// <param name="expression">The expression that identifies a property on <typeparamref name="T"/></param>
        /// <returns>A new execute strategy.</returns>
        public static IExecuteStrategy<T> Ignoring<T, TProperty>(this IBuildStrategy buildStrategy,
            Expression<Func<T, TProperty>> expression)
        {
            if (buildStrategy == null)
            {
                throw new ArgumentNullException(nameof(buildStrategy));
            }

            return buildStrategy.GetExecuteStrategy<T>().Ignoring(expression);
        }

        /// <summary>
        /// Populates the instance using the specified build strategy.
        /// </summary>
        /// <typeparam name="T">The type of instance to populate.</typeparam>
        /// <param name="buildStrategy">The build strategy.</param>
        /// <param name="instance">The instance to populate.</param>
        /// <returns>The updated instance.</returns>
        public static T Populate<T>(this IBuildStrategy buildStrategy, T instance)
        {
            if (buildStrategy == null)
            {
                throw new ArgumentNullException(nameof(buildStrategy));
            }

            return buildStrategy.With<DefaultExecuteStrategy<T>>().Populate(instance);
        }

        /// <summary>
        /// Returns a new <see cref="IExecuteStrategy{T}"/> for the specified build strategy.
        /// </summary>
        /// <typeparam name="T">The type of execute strategy to return.</typeparam>
        /// <param name="buildStrategy">The build strategy.</param>
        /// <returns>A new execute strategy.</returns>
        public static T With<T>(this IBuildStrategy buildStrategy) where T : IExecuteStrategy, new()
        {
            if (buildStrategy == null)
            {
                throw new ArgumentNullException(nameof(buildStrategy));
            }

            var executeStrategy = new T {ConstructorResolver = buildStrategy.ConstructorResolver};

            buildStrategy.IgnoreRules.ToList().ForEach(x => executeStrategy.IgnoreRules.Add(x));
            buildStrategy.TypeCreators.ToList().ForEach(x => executeStrategy.TypeCreators.Add(x));
            buildStrategy.ValueGenerators.ToList().ForEach(x => executeStrategy.ValueGenerators.Add(x));

            return executeStrategy;
        }
    }
}