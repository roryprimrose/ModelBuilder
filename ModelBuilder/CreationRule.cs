namespace ModelBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using ModelBuilder.Properties;

    /// <summary>
    /// The <see cref="CreationRule"/>
    /// class is used to define simple value creation rules that bypass <see cref="ITypeCreator"/> and <see cref="IValueGenerator"/> usages.
    /// </summary>
    public class CreationRule
    {
        private readonly Func<Type, string, LinkedList<object>, object> _creator;
        private readonly Func<Type, string, bool> _evaluator;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreationRule"/> class.
        /// </summary>
        /// <param name="evaluator">The function that determines whether the rule is a match.</param>
        /// <param name="priority">The priority of the rule.</param>
        /// <param name="creator">The function that creates the value for the rule.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="evaluator"/> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="creator"/> parameter is null.</exception>
        public CreationRule(
            Func<Type, string, bool> evaluator,
            int priority,
            Func<Type, string, LinkedList<object>, object> creator)
        {
            if (evaluator == null)
            {
                throw new ArgumentNullException(nameof(evaluator));
            }

            if (creator == null)
            {
                throw new ArgumentNullException(nameof(creator));
            }

            _evaluator = evaluator;
            _creator = creator;
            Priority = priority;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreationRule"/> class.
        /// </summary>
        /// <param name="evaluator">The function that determines whether the rule is a match.</param>
        /// <param name="priority">The priority of the rule.</param>
        /// <param name="value">The static value returned by the rule.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="evaluator"/> parameter is null.</exception>
        public CreationRule(Func<Type, string, bool> evaluator, int priority, object value)
            : this(evaluator, priority, (type, name, context) => value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreationRule"/> class.
        /// </summary>
        /// <param name="targetType">The target type that matches the rule.</param>
        /// <param name="propertyExpression">The property name regular expression that matches the rule.</param>
        /// <param name="priority">The priority of the rule.</param>
        /// <param name="creator">The function that creates the value for the rule.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="targetType"/> and <paramref name="propertyExpression"/> parameters are both null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="creator"/> parameter is null.</exception>
        public CreationRule(
            Type targetType,
            Regex propertyExpression,
            int priority,
            Func<Type, string, LinkedList<object>, object> creator)
        {
            if (targetType == null &&
                propertyExpression == null)
            {
                throw new ArgumentNullException(Resources.NoTargetTypeOrPropertyExpression);
            }

            if (creator == null)
            {
                throw new ArgumentNullException(nameof(creator));
            }

            _evaluator = (type, name) =>
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
            _creator = creator;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreationRule"/> class.
        /// </summary>
        /// <param name="targetType">The target type that matches the rule.</param>
        /// <param name="propertyExpression">The property name regular expression that matches the rule.</param>
        /// <param name="priority">The priority of the rule.</param>
        /// <param name="value">The static value returned by the rule.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="targetType"/> and <paramref name="propertyExpression"/> parameters are both null.</exception>
        public CreationRule(Type targetType, Regex propertyExpression, int priority, object value)
            : this(targetType, propertyExpression, priority, (type, name, buildChain) => value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreationRule"/> class.
        /// </summary>
        /// <param name="targetType">The target type that matches the rule.</param>
        /// <param name="propertyName">The property name that matches the rule.</param>
        /// <param name="priority">The priority of the rule.</param>
        /// <param name="creator">The function that creates the value for the rule.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="targetType"/> and <paramref name="propertyName"/> parameters are both null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="creator"/> parameter is null.</exception>
        public CreationRule(
            Type targetType,
            string propertyName,
            int priority,
            Func<Type, string, LinkedList<object>, object> creator)
        {
            if (targetType == null &&
                propertyName == null)
            {
                throw new ArgumentNullException(Resources.NoTargetTypeOrPropertyName);
            }

            if (creator == null)
            {
                throw new ArgumentNullException(nameof(creator));
            }

            _evaluator = (type, name) =>
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
            _creator = creator;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CreationRule"/> class.
        /// </summary>
        /// <param name="targetType">The target type that matches the rule.</param>
        /// <param name="propertyName">The property name that matches the rule.</param>
        /// <param name="priority">The priority of the rule.</param>
        /// <param name="value">The static value returned by the rule.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="targetType"/> and <paramref name="propertyName"/> parameters are both null.</exception>
        public CreationRule(Type targetType, string propertyName, int priority, object value)
            : this(targetType, propertyName, priority, (type, name, buildChain) => value)
        {
        }

        /// <summary>
        /// Creates a value using the specified type, property name and context.
        /// </summary>
        /// <param name="type">The type to match.</param>
        /// <param name="propertyName">The property name to match.</param>
        /// <param name="buildChain">The chain of instances built up to this point.</param>
        public object Create(Type type, string propertyName, LinkedList<object> buildChain)
        {
            if (IsMatch(type, propertyName) == false)
            {
                var typeName = "<null>";

                if (type != null)
                {
                    typeName = type.FullName;
                }

                var message = string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.Rule_InvalidMatch,
                    GetType().Name,
                    typeName,
                    propertyName);

                throw new NotSupportedException(message);
            }
            
            return _creator(type, propertyName, buildChain);
        }

        /// <summary>
        /// Gets whether the specified type and property name match this rule.
        /// </summary>
        /// <param name="type">The type to match.</param>
        /// <param name="propertyName">The property name to match.</param>
        /// <returns><c>true</c> if the rule matches the specified type and property name; otherwise <c>false</c>.</returns>
        public bool IsMatch(Type type, string propertyName)
        {
            return _evaluator(type, propertyName);
        }

        /// <summary>
        /// Gets the priority for this rule.
        /// </summary>
        public int Priority
        {
            get;
            private set;
        }
    }
}