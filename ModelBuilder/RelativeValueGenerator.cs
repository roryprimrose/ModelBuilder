using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace ModelBuilder
{
    public abstract class RelativeValueGenerator : StringValueGenerator
    {
        private readonly string _sourceExpression;
        private readonly string _targetExpression;

        protected RelativeValueGenerator(string targetNameExpression, string sourceNameExpression)
        {
            if (string.IsNullOrEmpty(targetNameExpression))
            {
                throw new ArgumentException(nameof(targetNameExpression));
            }

            if (string.IsNullOrEmpty(sourceNameExpression))
            {
                throw new ArgumentException(nameof(sourceNameExpression));
            }

            _targetExpression = targetNameExpression;
            _sourceExpression = sourceNameExpression;
        }

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

            var targetExpression = new Regex(_targetExpression);

            if (targetExpression.IsMatch(referenceName) == false)
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

        protected virtual string GetSourceValue(object context)
        {
            return GetValue(_sourceExpression, context);
        }

        protected virtual string GetValue(string expression, object context)
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

        private PropertyInfo GetMatchingProperty(string expression, object context)
        {
            var regex = new Regex(expression);
            var contextType = context.GetType();
            var properties = contextType.GetProperties();
            var matchingProperty = properties.FirstOrDefault(x => regex.IsMatch(x.Name));

            return matchingProperty;
        }
    }
}