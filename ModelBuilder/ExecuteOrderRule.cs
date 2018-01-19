namespace ModelBuilder
{
    using System;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using ModelBuilder.Properties;

    /// <summary>
    ///     The <see cref="ExecuteOrderRule" />
    ///     class is used to define the order of property population.
    /// </summary>
    public class ExecuteOrderRule
    {
        private readonly Func<Type, Type, string, bool> _func;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExecuteOrderRule" /> class.
        /// </summary>
        /// <param name="evaluator">The function that determines whether the rule is a match.</param>
        /// <param name="priority">The priority of the rule.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="evaluator" /> parameter is null.</exception>
        public ExecuteOrderRule(Func<Type, Type, string, bool> evaluator, int priority)
        {
            if (evaluator == null)
            {
                throw new ArgumentNullException(nameof(evaluator));
            }

            _func = evaluator;
            Priority = priority;
        }
        
        /// <summary>
        ///     Initializes a new instance of the <see cref="ExecuteOrderRule" /> class.
        /// </summary>
        /// <param name="declaringType">The declaring type with the property that matches the rule.</param>
        /// <param name="propertyType">The property type that matches the rule.</param>
        /// <param name="propertyExpression">The property name regular expression that matches the rule.</param>
        /// <param name="priority">The priority of the rule.</param>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="propertyType" /> and <paramref name="propertyExpression" />
        ///     parameters are both null.
        /// </exception>
        public ExecuteOrderRule(Type declaringType, Type propertyType, Regex propertyExpression, int priority)
        {
            if (declaringType == null &&
                propertyType == null &&
                propertyExpression == null)
            {
                throw new ArgumentNullException(Resources.NoOwnerTypePropertyTypeOrPropertyExpression);
            }

            _func = (parentType, propType, name) =>
            {
                if (declaringType != null &&
                    declaringType != parentType)
                {
                    return false;
                }

                if (propertyType != null &&
                    propertyType != propType)
                {
                    return false;
                }

                if (propertyExpression != null &&
                    propertyExpression.IsMatch(name) == false)
                {
                    return false;
                }

                return true;
            };

            Priority = priority;
        }
        
        /// <summary>
        ///     Initializes a new instance of the <see cref="ExecuteOrderRule" /> class.
        /// </summary>
        /// <param name="declaringType">The declaring type with the property that matches the rule.</param>
        /// <param name="propertyType">The property type that matches the rule.</param>
        /// <param name="propertyName">The property name that matches the rule.</param>
        /// <param name="priority">The priority of the rule.</param>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="propertyType" /> and <paramref name="propertyName" />
        ///     parameters are both null.
        /// </exception>
        public ExecuteOrderRule(Type declaringType, Type propertyType, string propertyName, int priority)
        {
            if (declaringType == null &&
                propertyType == null &&
                propertyName == null)
            {
                throw new ArgumentNullException(Resources.NoOwnerTypePropertyTypeOrPropertyName);
            }

            _func = (parentType, propType, name) =>
            {
                if (declaringType != null &&
                    declaringType != parentType)
                {
                    return false;
                }

                if (propertyType != null &&
                    propertyType != propType)
                {
                    return false;
                }

                if (propertyName != null &&
                    propertyName != name)
                {
                    return false;
                }

                return true;
            };

            Priority = priority;
        }
        
        /// <summary>
        ///     Gets whether the specified type and property name match this rule.
        /// </summary>
        /// <param name="property">The property to evaluate against the rule.</param>
        /// <returns><c>true</c> if the rule matches the specified type and property name; otherwise <c>false</c>.</returns>
        public bool IsMatch(PropertyInfo property)
        {
            return _func(property.DeclaringType, property.PropertyType, property.Name);
        }

        /// <summary>
        ///     Gets the priority for this rule.
        /// </summary>
        public int Priority { get; }
    }
}