using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using ModelBuilder.Properties;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="Extensions"/>
    /// class provides extension methods for the <see cref="IExecuteStrategy{T}"/> interface.
    /// </summary>
    public static class ExecuteStrategyExtensions
    {
        /// <summary>
        /// Creates an instance of <typeparamref name="T"/> using the specified execute strategy.
        /// </summary>
        /// <typeparam name="T">The type of instance to create.</typeparam>
        /// <param name="executeStrategy">The execute strategy to create the instance with.</param>
        /// <returns>The new instance.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy"/> parameter is null.</exception>
        public static T Create<T>(this IExecuteStrategy<T> executeStrategy)
        {
            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            return executeStrategy.CreateWith();
        }

        /// <summary>
        /// Appends a new <see cref="IgnoreRule"/> to the specified <see cref="IExecuteStrategy{T}"/> using the specified expression.
        /// </summary>
        /// <typeparam name="T">The type of instance that matches the rule.</typeparam>
        /// <param name="executeStrategy">The execute strategy to update.</param>
        /// <param name="expression">The expression that identifies a property on <typeparamref name="T"/></param>
        /// <returns>An execute strategy with the new rule.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy"/> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="expression"/> parameter is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="expression"/> parameter does not represent a property.</exception>
        /// <exception cref="ArgumentException">The <paramref name="expression"/> parameter does not match a property on the type to generate.</exception>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters",
            Justification = "This type is required in order to support the fluent syntax of call sites.")]
        public static IExecuteStrategy<T> Ignoring<T>(this IExecuteStrategy<T> executeStrategy,
            Expression<Func<T, object>> expression)
        {
            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var propInfo = GetPropertyInfo(expression);

            if (propInfo == null)
            {
                var message = string.Format(CultureInfo.CurrentCulture,
                    Resources.Error_ExpressionNotPropertyFormat,
                    expression);

                throw new ArgumentException(message);
            }
            
            var type = typeof (T);
            var typeProperties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy);
            
            if (typeProperties.Any(x => x.DeclaringType == propInfo.DeclaringType && x.PropertyType == propInfo.PropertyType && x.Name == propInfo.Name) == false)
            {
                var message = string.Format(CultureInfo.CurrentCulture,
                    Resources.ExecuteStrategy_ExpressionTargetsWrongType, propInfo.Name, type.FullName);

                throw new ArgumentException(message);
            }

            var rule = new IgnoreRule(type, propInfo.Name);

            executeStrategy.IgnoreRules.Add(rule);

            return executeStrategy;
        }

        private static PropertyInfo GetPropertyInfo<T>(Expression<Func<T, object>> expression)
        {
            PropertyInfo property = null;

            var unaryExpression = expression.Body as UnaryExpression;

            if (unaryExpression != null)
            {
                property = ((MemberExpression) unaryExpression.Operand).Member as PropertyInfo;
            }

            if (property != null)
            {
                return property;
            }

            var memberExpression = expression.Body as MemberExpression;

            if (memberExpression != null)
            {
                return memberExpression.Member as PropertyInfo;
            }

            return null;
        }
    }
}