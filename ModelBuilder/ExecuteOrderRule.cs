using System;
using System.Text.RegularExpressions;
using ModelBuilder.Properties;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="ExecuteOrderRule"/>
    /// class is used to define the order of property population.
    /// </summary>
    public class ExecuteOrderRule
    {
        private readonly Func<Type, string, bool> _func;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecuteOrderRule"/> class.
        /// </summary>
        /// <param name="evaluator">The function that determines whether the rule is a match.</param>
        /// <param name="priority">The priority of the rule.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="evaluator"/> parameter is null.</exception>
        public ExecuteOrderRule(Func<Type, string, bool> evaluator, int priority)
        {
            if (evaluator == null)
            {
                throw new ArgumentNullException(nameof(evaluator));
            }

            _func = evaluator;
            Priority = priority;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecuteOrderRule"/> class.
        /// </summary>
        /// <param name="targetType">The target type that matches the rule.</param>
        /// <param name="propertyExpression">The property name regular expression that matches the rule.</param>
        /// <param name="priority">The priority of the rule.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="targetType"/> and <paramref name="propertyExpression"/> parameters are both null.</exception>
        public ExecuteOrderRule(Type targetType, Regex propertyExpression, int priority)
        {
            if (targetType == null &&
                propertyExpression == null)
            {
                throw new ArgumentNullException(Resources.ExecuteOrderRule_NoTargetTypeOrPropetyExpression);
            }

            _func = (type, name) =>
            {
                if (targetType != null &&
                    targetType != type)
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
        /// Initializes a new instance of the <see cref="ExecuteOrderRule"/> class.
        /// </summary>
        /// <param name="targetType">The target type that matches the rule.</param>
        /// <param name="propertyName">The property name that matches the rule.</param>
        /// <param name="priority">The priority of the rule.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="targetType"/> and <paramref name="propertyName"/> parameters are both null.</exception>
        public ExecuteOrderRule(Type targetType, string propertyName, int priority)
        {
            if (targetType == null &&
                propertyName == null)
            {
                throw new ArgumentNullException(Resources.ExecuteOrderRule_NoTargetTypeOrPropetyName);
            }

            _func = (type, name) =>
            {
                if (targetType != null &&
                    targetType != type)
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
        /// Gets whether the specified type and property name match this rule.
        /// </summary>
        /// <param name="type">The type to match.</param>
        /// <param name="propertyName">The property name to match.</param>
        /// <returns><c>true</c> if the rule matches the specified type and property name; otherwise <c>false</c>.</returns>
        public bool IsMatch(Type type, string propertyName)
        {
            return _func(type, propertyName);
        }

        /// <summary>
        /// Gets the priority for this rule.
        /// </summary>
        public int Priority { get; private set; }
    }
}