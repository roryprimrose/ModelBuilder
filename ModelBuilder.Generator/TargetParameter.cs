namespace ModelBuilder.Generator;

internal record TargetParameter(string ParameterName, string TypeNamespace, string TypeName)
    : TargetType(TypeNamespace, TypeName);