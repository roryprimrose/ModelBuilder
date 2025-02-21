namespace ModelBuilder.Generator;

using System.Diagnostics;

[DebuggerDisplay("{FullTypeName}")]
internal record TargetType(string TypeNamespace, string TypeName)
{
    private string? _fullTypeName = null;
    public string FullTypeName => _fullTypeName ??= string.IsNullOrWhiteSpace(TypeNamespace)
        ? TypeName
        : $"{TypeNamespace}.{TypeName}";
}