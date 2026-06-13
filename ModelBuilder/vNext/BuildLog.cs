namespace ModelBuilder.vNext
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    ///     The <see cref="BuildLog" /> class
    ///     provides a capturing <see cref="IBuildLog" /> that records a nested tree of build entries and
    ///     can render them as indented text.
    /// </summary>
    public sealed class BuildLog : IBuildLog
    {
        private const int IndentWidth = 2;
        private readonly List<BuildLogEntry> _root = new List<BuildLogEntry>();
        private readonly Stack<BuildLogEntry> _scopes = new Stack<BuildLogEntry>();

        /// <inheritdoc />
        public IDisposable BeginScope(
            BuildLogEntryKind kind,
            Type targetType,
            string? memberName = null,
            string? reason = null)
        {
            targetType = targetType ?? throw new ArgumentNullException(nameof(targetType));

            var entry = new BuildLogEntry(kind, targetType, memberName, reason);

            AddEntry(entry);
            _scopes.Push(entry);

            return new ScopeToken(this, entry);
        }

        /// <summary>
        ///     Renders the captured entries as an indented, human-readable trace.
        /// </summary>
        /// <returns>The rendered build log.</returns>
        public string Render()
        {
            var builder = new StringBuilder();

            foreach (var entry in _root)
            {
                RenderEntry(builder, entry, 0);
            }

            return builder.ToString();
        }

        /// <inheritdoc />
        public void Write(
            BuildLogEntryKind kind,
            Type targetType,
            string? memberName = null,
            string? reason = null)
        {
            targetType = targetType ?? throw new ArgumentNullException(nameof(targetType));

            AddEntry(new BuildLogEntry(kind, targetType, memberName, reason));
        }

        private static void RenderEntry(StringBuilder builder, BuildLogEntry entry, int depth)
        {
            builder.Append(' ', depth * IndentWidth);

            switch (entry.Kind)
            {
                case BuildLogEntryKind.CreateInstance:
                    builder.Append("Create ").Append(entry.TargetType.Name);

                    break;
                case BuildLogEntryKind.PopulateInstance:
                    builder.Append("Populate ").Append(entry.TargetType.Name);

                    break;
                case BuildLogEntryKind.SkipMember:
                    builder.Append(entry.MemberName).Append(" -> SKIPPED");

                    break;
                default:
                    builder.Append(entry.MemberName ?? entry.TargetType.Name)
                        .Append(" : ")
                        .Append(entry.TargetType.Name);

                    break;
            }

            if (entry.Reason is not null)
            {
                builder.Append("  (").Append(entry.Reason).Append(')');
            }

            builder.Append('\n');

            foreach (var child in entry.Children)
            {
                RenderEntry(builder, child, depth + 1);
            }
        }

        private void AddEntry(BuildLogEntry entry)
        {
            if (_scopes.Count > 0)
            {
                _scopes.Peek().Add(entry);
            }
            else
            {
                _root.Add(entry);
            }
        }

        private void EndScope(BuildLogEntry entry)
        {
            if (_scopes.Count > 0
                && ReferenceEquals(_scopes.Peek(), entry))
            {
                _scopes.Pop();
            }
        }

        /// <inheritdoc />
        public IReadOnlyList<BuildLogEntry> Entries => _root;

        /// <inheritdoc />
        public bool IsEnabled => true;

        private sealed class ScopeToken : IDisposable
        {
            private readonly BuildLogEntry _entry;
            private BuildLog? _log;

            public ScopeToken(BuildLog log, BuildLogEntry entry)
            {
                _log = log;
                _entry = entry;
            }

            public void Dispose()
            {
                _log?.EndScope(_entry);
                _log = null;
            }
        }
    }
}
