namespace ModelBuilder.Generator
{
    /// <summary>
    ///     The <see cref="CollectionKind" /> enum
    ///     identifies how a discovered collection target is materialized.
    /// </summary>
    internal enum CollectionKind
    {
        Array = 0,
        List,
        Set,
        Dictionary
    }
}
