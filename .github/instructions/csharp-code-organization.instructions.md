---
description: 'C# code organization conventions for the ModelBuilder library.'
applyTo: '**/*.cs'
---

# C# Code Organization

## One type per file

Each `.cs` file must contain exactly **one** top-level type declaration (class, interface,
struct, enum, or record). Nested types that are private implementation details of their
containing type are permitted; any type that is `internal` or wider must live in its own file.

The file name must match the type name.

### Generic and non-generic counterparts

When a generic type shares its base name with a non-generic counterpart (a common pattern for
the typed and non-generic forms of an abstraction), the two types still go in separate files. To
disambiguate the file names, suffix the generic type's file with its type parameter name(s):

| Type | File |
| --- | --- |
| `IModelBuilder` | `IModelBuilder.cs` |
| `IModelBuilder<T>` | `IModelBuilderT.cs` |
| `IValueSource` | `IValueSource.cs` |
| `IValueSource<T>` | `IValueSourceT.cs` |

For a type with multiple parameters, concatenate the parameter names (for example
`IConverter<TIn, TOut>` → `IConverterTInTOut.cs`).
