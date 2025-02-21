namespace ModelBuilder.Generator;

internal record TargetProperty(string PropertyName, string TypeNamespace, string TypeName)
    : TargetType(TypeNamespace, TypeName);