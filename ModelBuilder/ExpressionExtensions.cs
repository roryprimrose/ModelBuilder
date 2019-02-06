namespace ModelBuilder
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using ModelBuilder.Properties;

    /// <summary>
    ///     The <see cref="Extensions" />
    ///     class provides extension methods for the <see cref="Expression{TDelegate}" /> class.
    /// </summary>
    public static class ExpressionExtensions
    {
        /// <summary>
        ///     Gets the property from the specified expression.
        /// </summary>
        /// <typeparam name="T">The type that holds the property.</typeparam>
        /// <param name="expression">The expression that identifies a property on <typeparamref name="T" /></param>
        /// <returns>The property in the expression.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="expression" /> parameter is null.</exception>
        /// <exception cref="ArgumentException">The <paramref name="expression" /> parameter does not represent a property.</exception>
        /// <exception cref="ArgumentException">
        ///     The <paramref name="expression" /> parameter does not match a property on the type
        ///     to generate.
        /// </exception>
        public static PropertyInfo GetProperty<T>(this Expression<Func<T, object>> expression)
        {
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

            var type = typeof(T);
            var typeProperties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            if (typeProperties.Any(x =>
                    x.DeclaringType == propInfo.DeclaringType && x.PropertyType == propInfo.PropertyType
                                                              && x.Name == propInfo.Name) == false)
            {
                var message = string.Format(CultureInfo.CurrentCulture,
                    Resources.Error_ExpressionTargetsWrongType,
                    propInfo.Name,
                    type.FullName);

                throw new ArgumentException(message);
            }

            return propInfo;
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