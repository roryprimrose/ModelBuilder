using System;
using ModelBuilder.Properties;

namespace ModelBuilder
{
    public class ExecuteOrderRule
    {
        private readonly Func<Type, string, bool> _func;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExecuteOrderRule"/> class.
        /// </summary>
        /// <param name="evaluator">The function that determines whether the rule is a match.</param>
        /// <param name="priority">The priority of the rule.</param>
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
        /// <param name="propertyName">The property name that matches the rule.</param>
        /// <param name="priority">The priority of the rule.</param>
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