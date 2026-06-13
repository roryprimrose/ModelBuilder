namespace ModelBuilder.vNext
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     The <see cref="NamedValueSourceRegistry" /> class
    ///     holds value sources that apply only to members whose name matches one of a set of names,
    ///     used by the entity-style built-in sources that match on member name rather than type alone.
    /// </summary>
    public sealed class NamedValueSourceRegistry
    {
        private readonly Dictionary<Type, List<Entry>> _byType = new Dictionary<Type, List<Entry>>();

        /// <summary>
        ///     Registers a value source that applies to members of type <typeparamref name="T" /> whose
        ///     name contains one of the supplied names.
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
                    if (memberName!.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        source = (IValueSource<T>)entry.Source;

                        return true;
                    }
                }
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
