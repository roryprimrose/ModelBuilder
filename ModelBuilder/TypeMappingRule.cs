namespace ModelBuilder
{
    using System;

    public class TypeMappingRule
    {
        public TypeMappingRule(Type sourceType, Type targetType)
        {
            SourceType = sourceType ?? throw new ArgumentNullException(nameof(sourceType));
            TargetType = targetType ?? throw new ArgumentNullException(nameof(targetType));
        }

        public Type SourceType { get; }

        public Type TargetType { get; }
    }
}