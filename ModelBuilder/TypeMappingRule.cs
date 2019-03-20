namespace ModelBuilder
{
    using System;
    using System.Globalization;
    using ModelBuilder.Properties;

    /// <summary>
    ///     The <see cref="TypeMappingRule" />
    ///     class defines a mapping between two types.
    /// </summary>
    public class TypeMappingRule
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="TypeMappingRule" /> class.
        /// </summary>
        /// <param name="sourceType">The source type.</param>
        /// <param name="targetType">The target type</param>
        /// <exception cref="ArgumentNullException">The <paramref name="sourceType" /> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="targetType" /> parameter is null.</exception>
        public TypeMappingRule(Type sourceType, Type targetType)
        {
            SourceType = sourceType ?? throw new ArgumentNullException(nameof(sourceType));
            TargetType = targetType ?? throw new ArgumentNullException(nameof(targetType));

            if (SourceType.IsAssignableFrom(TargetType) == false)
            {
                var message = string.Format(CultureInfo.CurrentCulture,
                    Resources.TypeMappingRule_TypeNotAssignable,
                    sourceType.FullName,
                    targetType.FullName);

                throw new ArgumentException(message, nameof(targetType));
            }
        }

        /// <summary>
        ///     Gets the source type.
        /// </summary>
        public Type SourceType { get; }

        /// <summary>
        ///     Gets the target type.
        /// </summary>
        public Type TargetType { get; }
    }
}