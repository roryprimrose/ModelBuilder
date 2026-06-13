namespace ModelBuilder.vNext
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     The <see cref="NullBuildLog" /> class
    ///     provides an <see cref="IBuildLog" /> that records nothing, so build logging carries no cost
    ///     when it is not enabled.
    /// </summary>
    public sealed class NullBuildLog : IBuildLog
    {
        private static readonly IDisposable _scope = new NoopScope();

        private NullBuildLog()
        {
        }

        /// <inheritdoc />
        public IDisposable BeginScope(
            BuildLogEntryKind kind,
            Type targetType,
            string? memberName = null,
            string? reason = null)
        {
            return _scope;
        }

        /// <inheritdoc />
        public void Write(
            BuildLogEntryKind kind,
            Type targetType,
            string? memberName = null,
            string? reason = null)
        {
        }

        /// <inheritdoc />
        public IReadOnlyList<BuildLogEntry> Entries { get; } = Array.Empty<BuildLogEntry>();

        /// <summary>
        ///     Gets the shared <see cref="NullBuildLog" /> instance.
        /// </summary>
        public static NullBuildLog Instance { get; } = new NullBuildLog();

        /// <inheritdoc />
        public bool IsEnabled => false;

        private sealed class NoopScope : IDisposable
        {
            public void Dispose()
            {
            }
        }
    }
}
