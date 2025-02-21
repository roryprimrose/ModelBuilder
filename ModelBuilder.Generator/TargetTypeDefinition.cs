namespace ModelBuilder.Generator;

internal record TargetTypeDefinition(
    string FullTypeName,
    string SafeTypeName,
    IReadOnlyCollection<TargetProperty> Properties,
    IReadOnlyCollection<TargetParameter> Parameters);