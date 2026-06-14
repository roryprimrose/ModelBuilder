namespace ModelBuilder
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     The <see cref="NamedValueSourceRegistry" /> class
    ///     holds value sources that apply only to members whose name contains one of a set of names as a
    ///     whole PascalCase or camelCase word, used by the entity-style built-in sources that match on
    ///     member name rather than type alone.
    /// </summary>
    internal sealed class NamedValueSourceRegistry
    {
        private readonly Dictionary<Type, List<Entry>> _byType = new Dictionary<Type, List<Entry>>();

        /// <summary>
        ///     Registers a value source that applies to members of type <typeparamref name="T" /> whose
        ///     name contains one of the supplied names as a whole PascalCase or camelCase word.
        /// </summary>
        /// <typeparam name="T">The member type the source produces.</typeparam>
        /// <param name="source">The value source.</param>
        /// <param name="names">The member names the source matches.</param>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="source" /> or <paramref name="names" /> parameter is <c>null</c>.
        /// </exception>
        public void Register<T>(IValueSource<T> source, params string[] names)
        {
            source = source ?? throw new ArgumentNullException(nameof(source));
            names = names ?? throw new ArgumentNullException(nameof(names));

            if (_byType.TryGetValue(typeof(T), out var entries) == false)
            {
                entries = new List<Entry>();
                _byType.Add(typeof(T), entries);
            }

            entries.Add(new Entry(names, source));
        }

        /// <summary>
        ///     Attempts to resolve a value source matching the member name for type <typeparamref name="T" />.
        /// </summary>
        /// <typeparam name="T">The member type to resolve a source for.</typeparam>
        /// <param name="memberName">The member name to match.</param>
        /// <param name="source">The matched source, when one exists.</param>
        /// <returns><c>true</c> if a source matched the member name; otherwise, <c>false</c>.</returns>
        public bool TryGet<T>(string? memberName, out IValueSource<T>? source)
        {
            source = null;

            if (string.IsNullOrEmpty(memberName))
            {
                return false;
            }

            if (_byType.TryGetValue(typeof(T), out var entries) == false)
            {
                return false;
            }

            foreach (var entry in entries)
            {
                foreach (var name in entry.Names)
                {
                    if (MatchesName(memberName!, name))
                    {
                        source = (IValueSource<T>)entry.Source;

                        return true;
                    }
                }
            }

            return false;
        }

        private static bool MatchesName(string memberName, string name)
        {
            // Match the alias as a whole PascalCase/camelCase word (or run of words) within the member
            // name rather than a raw substring. This keeps descriptive matches such as
            // "ContactEmailAddress" -> "Email" working while preventing short aliases like "Age" from
            // matching mid-word, for example the "age" inside "PageCount" or "StorageSize".
            var index = memberName.IndexOf(name, StringComparison.OrdinalIgnoreCase);

            while (index >= 0)
            {
                var startIsBoundary = index == 0 || char.IsUpper(memberName[index]);
                var endIndex = index + name.Length;
                var endIsBoundary = endIndex == memberName.Length || char.IsUpper(memberName[endIndex]);

                if (startIsBoundary && endIsBoundary)
                {
                    return true;
                }

                index = memberName.IndexOf(name, index + 1, StringComparison.OrdinalIgnoreCase);
            }

            return false;
        }

        private readonly struct Entry
        {
            public Entry(string[] names, object source)
            {
                Names = names;
                Source = source;
            }

            public string[] Names { get; }

            public object Source { get; }
        }
    }
}
