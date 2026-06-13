namespace ModelBuilder.vNext
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     The <see cref="BuildLogEntry" /> class
    ///     is used to represent a single node in the structured build log tree, recording what the
    ///     builder did and why.
    /// </summary>
    public sealed class BuildLogEntry
    {
        private readonly List<BuildLogEntry> _children = new List<BuildLogEntry>();

        internal BuildLogEntry(BuildLogEntryKind kind, Type targetType, string? memberName, string? reason)
        {
            Kind = kind;
            TargetType = targetType;
            MemberName = memberName;
            Reason = reason;
        }

        internal void Add(BuildLogEntry child)
        {
            _children.Add(child);
        }

        /// <summary>
        ///     Gets the child entries nested beneath this entry.
        /// </summary>
        public IReadOnlyList<BuildLogEntry> Children => _children;

        /// <summary>
        ///     Gets the kind of action this entry records.
        /// </summary>
        public BuildLogEntryKind Kind { get; }

        /// <summary>
        ///     Gets the name of the member this entry relates to.
        /// </summary>
        /// <returns>The member name, or <c>null</c> when the entry does not relate to a named member.</returns>
        public string? MemberName { get; }

        /// <summary>
        ///     Gets the reason recorded for this entry.
        /// </summary>
        /// <returns>The reason text, or <c>null</c> when no reason was recorded.</returns>
        public string? Reason { get; }

        /// <summary>
        ///     Gets the type this entry relates to.
        /// </summary>
        public Type TargetType { get; }
    }
}
