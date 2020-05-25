namespace ModelBuilder.ValueGenerators
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;

    /// <summary>
    ///     The <see cref="RelativeValueGenerator" />
    ///     class is used to assist in generating a value that is related to another value for a given context.
    /// </summary>
    public abstract class RelativeValueGenerator : ValueGeneratorMatcher
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RelativeValueGenerator" /> class.
        /// </summary>
        /// <param name="targetNameExpression">The expression to match the target property or parameter.</param>
        /// <param name="types">The types the generator can match.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="targetNameExpression" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="types" /> parameter is <c>null</c>.</exception>
        protected RelativeValueGenerator(Regex targetNameExpression, params Type[] types)
            : base(targetNameExpression, types)
        {
        }

        /// <summary>
        ///     Gets the property value using the specified expression and context.
        /// </summary>
        /// <typeparam name="T">The type of value to return.</typeparam>
        /// <param name="expression">The expression used to identify the property.</param>
        /// <param name="context">The context to use for reference information.</param>
        /// <returns>The string value of the source property.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="expression" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="context" /> parameter is <c>null</c>.</exception>
        protected virtual T GetValue<T>(Regex expression, object context)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var value = GetPropertyValue(expression, context);

            if (value == null)
            {
                return default!;
            }

            var expectedType = typeof(T);

            if (expectedType.IsNullable())
            {
                // Hijack the type to generator so we can continue with the normal code pointed at the correct type to generate
                expectedType = expectedType.GetGenericArguments()[0];
            }

            return (T) Convert.ChangeType(value, expectedType, CultureInfo.CurrentCulture);
        }

        /// <summary>
        ///     Gets whether the build context being created is/should represents a male gender.
        /// </summary>
        /// <param name="executeStrategy">The execute strategy.</param>
        /// <returns><c>true</c> if the object is/should represent a male, otherwise <c>false</c>.</returns>
        /// <remarks>The value returned will be random if there is no supported gender identifier found.</remarks>
        protected bool IsMale(IExecuteStrategy executeStrategy)
        {
            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            string? gender = null;
            var context = executeStrategy.BuildChain?.Last;

            if (context != null)
            {
                gender = GetValue<string>(NameExpression.Gender, context);
            }

            if (gender == null)
            {
                // Randomly assign a gender so that we can pick from a gender data set rather than limiting to a specific one
                var nextValue = Generator.NextValue(0, 1);

                if (nextValue == 0)
                {
                    return false;
                }

                return true;
            }

            if (string.Equals(gender, "male", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        protected override bool IsMatch(IBuildChain buildChain, Type type, string? referenceName)
        {
            var baseSupported = base.IsMatch(buildChain, type, referenceName);

            if (baseSupported == false)
            {
                // The type and name do not match what we are looking for   
                return false;
            }

            var context = buildChain?.Last;

            if (context == null)
            {
                // This is a top level item being generated and probably a primitive type
                // There will not be other relative values to read
                return false;
            }

            if (context is string)
            {
                // We can't look at properties on a string
                return false;
            }

            if (context.GetType().IsPrimitive)
            {
                // We can't look at properties on a primitive type
                return false;
            }

            var propertyNames = GetPropertyNames(context);

            if (propertyNames.Any())
            {
                return true;
            }

            // There are no properties on this type
            return false;
        }

        private static IEnumerable<PropertyInfo> GetDeclaredProperties(object context)
        {
            var contextType = context.GetType();

            return contextType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        }

        private static IEnumerable<string> GetDynamicProperties(ExpandoObject context)
        {
            return context.Select(x => x.Key);
        }

        private static IEnumerable<string> GetPropertyNames(object context)
        {
            if (context is ExpandoObject dynamicObject)
            {
                return GetDynamicProperties(dynamicObject);
            }

            var declaredProperties = GetDeclaredProperties(context);

            return from x in declaredProperties
                select x.Name;
        }

        private static object? GetPropertyValue(Regex expression, object context)
        {
            if (context is ExpandoObject dynamicObject)
            {
                var properties = GetDynamicProperties(dynamicObject);

                var matchingName = properties.FirstOrDefault(expression.IsMatch);

                if (matchingName == null)
                {
                    // There is no property matching the expression
                    return null;
                }

                var keyedProperties = (IDictionary<string, object>) dynamicObject;

                return keyedProperties[matchingName];
            }

            // This is not an ExpandoObject so we are expecting declared properties
            var declaredProperties = GetDeclaredProperties(context);
            var property = declaredProperties.FirstOrDefault(x => expression.IsMatch(x.Name));

            if (property == null)
            {
                return null;
            }

            return property.GetValue(context);
        }
    }
}