using System;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="IgnoreRule"/>
    /// class describes a property on a type that should not be set when populating an instance.
    /// </summary>
    public class IgnoreRule
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IgnoreRule"/> class.
        /// </summary>
        /// <param name="targetType">The target type that matches the rule.</param>
        /// <param name="propertyName">The property name that matches the rule.</param>
        public IgnoreRule(Type targetType, string propertyName)
        {
            if (targetType == null)
            {
                throw new ArgumentNullException(nameof(targetType));
            }

            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            TargetType = targetType;
            PropertyName = propertyName;
        }

        /// <summary>
        /// Gets the property name to ignore.
        /// </summary>
        public string PropertyName { get; private set; }

        /// <summary>
        /// Gets the target type to evaluate.
        /// </summary>
        public Type TargetType { get; private set; }
    }
}