namespace ModelBuilder
{
    /// <summary>
    ///     The <see cref="FailureKind" /> enum
    ///     is used to categorize why a model build failed so that callers and tests can branch on the
    ///     cause rather than parsing an error message.
    /// </summary>
    public enum FailureKind
    {
        /// <summary>
        ///     The failure reason was not specified.
        /// </summary>
        Unspecified = 0,

        /// <summary>
        ///     A required constructor was not accessible to the generated code.
        /// </summary>
        InaccessibleConstructor,

        /// <summary>
        ///     An abstract or interface type was reached with no mapping to a concrete type.
        /// </summary>
        UnmappedAbstractType,

        /// <summary>
        ///     No value source matched the requested target.
        /// </summary>
        NoValueSource,

        /// <summary>
        ///     No builder was generated for the requested type.
        /// </summary>
        NoBuilderForType,

        /// <summary>
        ///     A circular reference was detected in the object graph.
        /// </summary>
        CircularReference,

        /// <summary>
        ///     The maximum build depth was exceeded.
        /// </summary>
        DepthExceeded,

        /// <summary>
        ///     A value source threw an exception while creating a value.
        /// </summary>
        ValueSourceThrew
    }
}
