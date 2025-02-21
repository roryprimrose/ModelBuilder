namespace ModelBuilder.Generator;

using Microsoft.CodeAnalysis;

internal interface ITypeStore
{
    TargetType AddType(ITypeSymbol typeSymbol, bool includeConstructors = false);
    string CreateSafeName(string name, string defaultName = "");
    TargetTypeDefinition GetDefinition(TargetType targetType);
}