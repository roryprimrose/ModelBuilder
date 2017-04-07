namespace ModelBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using ModelBuilder.Properties;

    /// <summary>
    ///     The <see cref="RelativeValueGenerator" />
    ///     class is used to assist in generating a value that is related to another value for a given context.
    /// </summary>
    public abstract class RelativeValueGenerator : ValueGeneratorMatcher
    {
        private readonly Regex _sourceExpression;
        private readonly Regex _targetExpression;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RelativeValueGenerator" /> class.
        /// </summary>
        /// <param name="targetNameExpression">The expression to match the target property or parameter.</param>
        /// <param name="types">The types the generator can match.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="targetNameExpression" /> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="types" /> parameter is null.</exception>
        protected RelativeValueGenerator(Regex targetNameExpression, params Type[] types)
            : this(targetNameExpression, null, types)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="RelativeValueGenerator" /> class.
        /// </summary>
        /// <param name="targetNameExpression">The expression to match the target property or parameter.</param>
        /// <param name="sourceNameExpression">The expression to match the source property.</param>
        /// <param name="types">The types the generator can match.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="targetNameExpression" /> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="sourceNameExpression" /> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="types" /> parameter is null.</exception>
        protected RelativeValueGenerator(Regex targetNameExpression, Regex sourceNameExpression, params Type[] types)
            : base(types)
        {
            _targetExpression = targetNameExpression ?? throw new ArgumentNullException(nameof(targetNameExpression));
            _sourceExpression = sourceNameExpression;
        }

        /// <inheritdoc />
        public override bool IsSupported(Type type, string referenceName, LinkedList<object> buildChain)
        {
            var baseSupported = base.IsSupported(type, referenceName, buildChain);

            if (baseSupported == false)
            {
                return false;
            }

            if (referenceName == null)
            {
                // This is probably a constructor parameter
                return false;
            }

            if (buildChain == null)
            {
                // This is either a top level item being generated or a constructor parameter
                return false;
            }

            if (buildChain.Count == 0)
            {
                // This is either a top level item being generated or a constructor parameter
                return false;
            }

            if (_targetExpression.IsMatch(referenceName) == false)
            {
                return false;
            }

            if (_sourceExpression == null)
            {
                // There is no source expression to validate against the model
                return true;
            }

            var context = buildChain.Last.Value;

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
        ///     Gets the source property value for the specified context.
        /// </summary>
        /// <typeparam name="T">The type of value to return.</typeparam>
        /// <param name="context">The context to use for reference informamtion.</param>
        /// <returns>The string value of the source property.</returns>
        /// <exception cref="InvalidOperationException">The generator was not created with a source expression.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="context" /> parameter is null.</exception>
        protected virtual T GetSourceValue<T>(object context)
        {
            if (_sourceExpression == null)
            {
                throw new InvalidOperationException(Resources.RelativeValueGenerator_NoSourceExpression);
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return GetValue<T>(_sourceExpression, context);
        }

        /// <summary>
        ///     Gets the property value using the specified expression and context.
        /// </summary>
        /// <typeparam name="T">The type of value to return.</typeparam>
        /// <param name="expression">The expression used to identify the property.</param>
        /// <param name="context">The context to use for reference informamtion.</param>
        /// <returns>The string value of the source property.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="expression" /> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="context" /> parameter is null.</exception>
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

            var property = GetMatchingProperty(expression, context);

            if (property == null)
            {
                return default(T);
            }

            var value = property.GetValue(context, null);

            if (value == null)
            {
                return default(T);
            }

            return (T)Convert.ChangeType(value, typeof(T), CultureInfo.CurrentCulture);
        }

        private static PropertyInfo GetMatchingProperty(Regex expression, object context)
        {
            var contextType = context.GetType();
            var properties = contextType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var matchingProperty = properties.FirstOrDefault(x => expression.IsMatch(x.Name));

            return matchingProperty;
        }
    }
}