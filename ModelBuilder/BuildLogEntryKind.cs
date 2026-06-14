namespace ModelBuilder
{
    /// <summary>
    ///     The <see cref="BuildLogEntryKind" /> enum
    ///     is used to categorize the kind of action a <see cref="BuildLogEntry" /> records.
    /// </summary>
    public enum BuildLogEntryKind
    {
        /// <summary>
        ///     An instance of a type was created.
        /// </summary>
        CreateInstance = 0,

        /// <summary>
        ///     An instance of a type was populated.
        /// </summary>
        PopulateInstance,

        /// <summary>
        ///     A value was created for a member.
        /// </summary>
        CreateValue,

        /// <summary>
        ///     A member was skipped and left at its default value.
        /// </summary>
        SkipMember
    }
}
