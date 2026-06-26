namespace ModelBuilder
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     The <see cref="IBuildLog" /> interface
    ///     defines a structured, opt-in trace of what the builder did while creating an object graph.
    /// </summary>
    public interface IBuildLog
    {
        /// <summary>
        ///     Begins a nested scope that subsequent entries are recorded beneath until the returned
        ///     token is disposed.
        /// </summary>
        /// <param name="kind">The kind of action the scope records.</param>
        /// <param name="targetType">The type the scope relates to.</param>
        /// <param name="memberName">The optional name of the member the scope relates to.</param>
        /// <param name="reason">The optional reason recorded for the scope.</param>
        /// <returns>A token that ends the scope when disposed.</returns>
        IDisposable BeginScope(
            BuildLogEntryKind kind,
            Type targetType,
            string? memberName = null,
            string? reason = null);

        /// <summary>
        ///     Records a leaf entry beneath the current scope.
        /// </summary>
        /// <param name="kind">The kind of action the entry records.</param>
        /// <param name="targetType">The type the entry relates to.</param>
        /// <param name="memberName">The optional name of the member the entry relates to.</param>
        /// <param name="reason">The optional reason recorded for the entry.</param>
        void Write(
            BuildLogEntryKind kind,
            Type targetType,
            string? memberName = null,
            string? reason = null);

        /// <summary>
        ///     Gets the root entries recorded in the log.
        /// </summary>
        IReadOnlyList<BuildLogEntry> Entries { get; }

        /// <summary>
        ///     Gets a value indicating whether the log captures entries.
        /// </summary>
        /// <returns><c>true</c> if the log captures entries; otherwise, <c>false</c>.</returns>
        bool IsEnabled { get; }
    }
}
