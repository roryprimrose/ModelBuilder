namespace ModelBuilder.Generator;

internal record BindTarget(string CallingNamespace, string TypeNamespace, string TypeName)
    : TargetType(TypeNamespace, TypeName)
{
    public BindTarget(string CallingNamespace, TargetType targetType) : this(CallingNamespace, targetType.TypeNamespace,
        targetType.TypeName)
    {
    }
}