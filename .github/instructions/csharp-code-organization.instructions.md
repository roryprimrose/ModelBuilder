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

## No nested calls in arguments or indexers

Do not use a method or function call directly as an argument to another method or constructor, or
as the expression inside an indexer (`[...]`). Assign the call's result to a well-named local first,
then pass or index with that local. This keeps each call site readable and makes every intermediate
value inspectable in the debugger.

This rule covers call results used as a method/constructor argument (including array-size and
collection-index expressions). It does **not** force locals for a call that is the whole expression
(such as a `return` value or a single-expression lambda body), nor for member-access or operator
chains like `value.ToString()` or `a + b`.

```csharp
// Avoid: a call nested inside an indexer or as an argument to another call.
return values[context.Random.NextInt32(0, values.Count - 1)];
var buffer = new byte[context.Random.NextInt32(1, 16)];
return new DateTimeOffset(NextDateTime(context));

// Prefer: name the intermediate value, then index or pass it.
var index = context.Random.NextInt32(0, values.Count - 1);
return values[index];

var length = context.Random.NextInt32(1, 16);
var buffer = new byte[length];

var value = NextDateTime(context);
return new DateTimeOffset(value);
```

When a single-expression lambda would otherwise nest a call inside an argument, promote it to a
named method so the body can introduce the local:

```csharp
// Avoid:
registry.Register<TimeSpan>(new DelegateValueSource<TimeSpan>(c => TimeSpan.FromMinutes(c.Random.NextInt32(0, 1440))));

// Prefer:
registry.Register<TimeSpan>(new DelegateValueSource<TimeSpan>(NextTimeSpan));

private static TimeSpan NextTimeSpan(IBuildContext context)
{
    var minutes = context.Random.NextInt32(0, 1440);

    return TimeSpan.FromMinutes(minutes);
}
```
