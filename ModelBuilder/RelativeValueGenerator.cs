using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="RelativeValueGenerator"/>
    /// class is used to assist in generating a value that is related to another value for a given context.
    /// </summary>
    public abstract class RelativeValueGenerator : StringValueGenerator
    {
        private readonly Regex _sourceExpression;
        private readonly Regex _targetExpression;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelativeValueGenerator"/> class.
        /// </summary>
        /// <param name="targetNameExpression">The expression to match the target property or parameter.</param>
        /// <param name="sourceNameExpression">The expression to match the source property.</param>
        protected RelativeValueGenerator(Regex targetNameExpression, Regex sourceNameExpression)
        {
            if (targetNameExpression == null)
            {
                throw new ArgumentNullException(nameof(targetNameExpression));
            }

            if (sourceNameExpression == null)
            {
                throw new ArgumentNullException(nameof(sourceNameExpression));
            }

            _targetExpression = targetNameExpression;
            _sourceExpression = sourceNameExpression;
        }

        /// <inheritdoc />
        public override bool IsSupported(Type type, string referenceName, object context)
        {
            var baseSupported = base.IsSupported(type, referenceName, context);

            if (baseSupported == false)
            {
                return false;
            }

            if (referenceName == null)
            {
                // This is probably a constructor parameter
                return false;
            }

            if (context == null)
            {
                // This is either a top level item being generated or a constructor parameter
                return false;
            }

            if (_targetExpression.IsMatch(referenceName) == false)
            {
                return false;
            }

            // Check if the context has a property matching the source expression
            var matchingProperty = GetMatchingProperty(_sourceExpression, context);

            if (matchingProperty == null)
            {
                return false;
            }

            // The context object has properties that match the source and target expressions for the relative generator
            return true;
        }

        /// <summary>
        /// Gets the source property value for the specified context.
        /// </summary>
        /// <param name="context">The context to use for reference informamtion.</param>
        /// <returns>The string value of the source property.</returns>
        protected virtual string GetSourceValue(object context)
        {
            return GetValue(_sourceExpression, context);
        }

        /// <summary>
        /// Gets the property value using the specified expression and context.
        /// </summary>
        /// <param name="expression">The expression used to identify the property.</param>
        /// <param name="context">The context to use for reference informamtion.</param>
        /// <returns>The string value of the source property.</returns>
        protected virtual string GetValue(Regex expression, object context)
        {
            var property = GetMatchingProperty(expression, context);

            Debug.Assert(property != null, "The property was not found");

            var value = property.GetValue(context);

            if (value == null)
            {
                return null;
            }

            if (property.PropertyType == typeof(string))
            {
                return (string) value;
            }

            return value.ToString();
        }

        private static PropertyInfo GetMatchingProperty(Regex expression, object context)
        {
            var contextType = context.GetType();
            var properties = contextType.GetProperties();
            var matchingProperty = properties.FirstOrDefault(x => expression.IsMatch(x.Name));

            return matchingProperty;
        }
    }
}