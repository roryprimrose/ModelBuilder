using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ModelBuilder.Properties;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="Extensions"/>
    /// class provides common extension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Creates an instance of <typeparamref name="T"/> using the specified build strategy.
        /// </summary>
        /// <typeparam name="T">The type of instance to create.</typeparam>
        /// <param name="buildStrategy">The build strategy to create the instance with.</param>
        /// <returns>The new instance.</returns>
        public static T Create<T>(this IBuildStrategy buildStrategy)
        {
            return CreateWith<T>(buildStrategy);
        }

        /// <summary>
        /// Creates an instance of <typeparamref name="T"/> using the specified execute strategy.
        /// </summary>
        /// <typeparam name="T">The type of instance to create.</typeparam>
        /// <param name="executeStrategy">The execute strategy to create the instance with.</param>
        /// <returns>The new instance.</returns>
        public static T Create<T>(this IExecuteStrategy<T> executeStrategy)
        {
            return executeStrategy.CreateWith();
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
            return buildStrategy.With<DefaultExecuteStrategy<T>>().CreateWith(args);
        }

        /// <summary>
        /// Returns an <see cref="IExecuteStrategy{T}"/> for the specified build strategy with a new <see cref="IgnoreRule"/> that matches the specified expression.
        /// </summary>
        /// <typeparam name="T">The type of instance that matches the rule.</typeparam>
        /// <typeparam name="TP">The type of property to ignore.</typeparam>
        /// <param name="buildStrategy">The build strategy.</param>
        /// <param name="expression">The expression that identifies a property on <typeparamref name="T"/></param>
        /// <returns>A new execute strategy.</returns>
        public static IExecuteStrategy<T> Ignoring<T, TP>(this IBuildStrategy buildStrategy,
            Expression<Func<T, TP>> expression)
        {
            return buildStrategy.GetExecuteStrategy<T>().Ignoring(expression);
        }

        /// <summary>
        /// Appends a new <see cref="IgnoreRule"/> to the specified <see cref="IExecuteStrategy{T}"/> using the specified expression.
        /// </summary>
        /// <typeparam name="T">The type of instance that matches the rule.</typeparam>
        /// <typeparam name="TP">The type of property to ignore.</typeparam>
        /// <param name="executeStrategy">The execute strategy to update.</param>
        /// <param name="expression">The expression that identifies a property on <typeparamref name="T"/></param>
        /// <returns>An execute strategy with the new rule.</returns>
        public static IExecuteStrategy<T> Ignoring<T, TP>(this IExecuteStrategy<T> executeStrategy,
            Expression<Func<T, TP>> expression)
        {
            var type = typeof (T);

            var member = expression.Body as MemberExpression;

            if (member == null)
            {
                var message = string.Format(CultureInfo.CurrentCulture,
                    Resources.Error_ExpressionRefersToMethodFormat,
                    expression);

                throw new ArgumentException(message);
            }

            var propInfo = member.Member as PropertyInfo;

            if (propInfo == null)
            {
                var message = string.Format(CultureInfo.CurrentCulture,
                    Resources.Error_ExpressionRefersToFieldFormat,
                    expression);

                throw new ArgumentException(message);
            }

            if (propInfo.ReflectedType.IsAssignableFrom(type) == false)
            {
                var message = string.Format(CultureInfo.CurrentCulture,
                    Resources.Error_ExpressionRefersToPropertyOnWrongType,
                    expression,
                    type);

                throw new ArgumentException(message);
            }

            var rule = new IgnoreRule(type, propInfo.Name);

            executeStrategy.IgnoreRules.Add(rule);

            return executeStrategy;
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
            return buildStrategy.With<DefaultExecuteStrategy<T>>().Populate(instance);
        }

        /// <summary>
        /// Makes a change to the instance using the specified action.
        /// </summary>
        /// <typeparam name="T">The type of instance being changed.</typeparam>
        /// <param name="instance">The instance to update.</param>
        /// <param name="action">The action to run against the instance.</param>
        /// <returns>The updated instance.</returns>
        public static T Set<T>(this T instance, Action<T> action)
        {
            action(instance);

            return instance;
        }

        /// <summary>
        /// Makes a change to each instance using the specified action.
        /// </summary>
        /// <typeparam name="T">The type of instance being changed.</typeparam>
        /// <param name="instances">The instances to update.</param>
        /// <param name="action">The action to run against the instance.</param>
        /// <returns>The updated instances.</returns>
        public static T SetEach<T>(this T instances, Action<T> action) where T : List<T>
        {
            instances.ForEach(action);

            return instances;
        }

        /// <summary>
        /// Returns a new <see cref="IExecuteStrategy{T}"/> for the specified build strategy.
        /// </summary>
        /// <typeparam name="T">The type of execute strategy to return.</typeparam>
        /// <param name="buildStrategy">The build strategy.</param>
        /// <returns>A new execute strategy.</returns>
        public static T With<T>(this IBuildStrategy buildStrategy) where T : IExecuteStrategy, new()
        {
            var executeStrategy = new T {ConstructorResolver = buildStrategy.ConstructorResolver};

            buildStrategy.IgnoreRules.ToList().ForEach(x => executeStrategy.IgnoreRules.Add(x));
            buildStrategy.TypeCreators.ToList().ForEach(x => executeStrategy.TypeCreators.Add(x));
            buildStrategy.ValueGenerators.ToList().ForEach(x => executeStrategy.ValueGenerators.Add(x));

            return executeStrategy;
        }
    }
}