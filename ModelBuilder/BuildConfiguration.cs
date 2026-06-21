namespace ModelBuilder
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     The <see cref="BuildConfiguration" /> class
    ///     is the mutable, fluent implementation of <see cref="IBuildConfiguration" /> that
    ///     configuration modules populate with type mappings and ignore rules.
    /// </summary>
    internal sealed class BuildConfiguration : IBuildConfiguration
    {
        private readonly HashSet<MemberKey> _ignoredMembers = new HashSet<MemberKey>();
        private readonly List<Func<MemberSignature, bool>> _ignorePredicates = new List<Func<MemberSignature, bool>>();
        private readonly Dictionary<Type, Type> _typeMappings = new Dictionary<Type, Type>();
        private ValueSourceRegistry? _valueSources;
        private NamedValueSourceRegistry? _namedValueSources;

        /// <inheritdoc />
        public IBuildConfiguration AddMapping(Type sourceType, Type targetType)
        {
            sourceType = sourceType ?? throw new ArgumentNullException(nameof(sourceType));
            targetType = targetType ?? throw new ArgumentNullException(nameof(targetType));

            _typeMappings[sourceType] = targetType;

            return this;
        }

        /// <inheritdoc />
        public IBuildConfiguration AddMapping<TSource, TTarget>()
            where TTarget : TSource
        {
            return AddMapping(typeof(TSource), typeof(TTarget));
        }

        /// <inheritdoc />
        public IBuildConfiguration AddValueSource<T>(IValueSource<T> source)
        {
            source = source ?? throw new ArgumentNullException(nameof(source));

            _valueSources ??= new ValueSourceRegistry();
            _valueSources.Register(source);

            return this;
        }

        /// <inheritdoc />
        public IBuildConfiguration AddValueSource<T>(IValueSource<T> source, params string[] names)
        {
            source = source ?? throw new ArgumentNullException(nameof(source));
            names = names ?? throw new ArgumentNullException(nameof(names));

            if (names.Length == 0)
            {
                throw new ArgumentException("At least one name must be supplied.", nameof(names));
            }

            foreach (var name in names)
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new ArgumentException("Names must not be null, empty or whitespace.", nameof(names));
                }
            }

            _namedValueSources ??= new NamedValueSourceRegistry();
            _namedValueSources.Register(source, names);

            return this;
        }

        /// <inheritdoc />
        public IBuildConfiguration Ignore(Type declaringType, string memberName)
        {
            declaringType = declaringType ?? throw new ArgumentNullException(nameof(declaringType));
            memberName = memberName ?? throw new ArgumentNullException(nameof(memberName));

            _ignoredMembers.Add(new MemberKey(declaringType, memberName));

            return this;
        }

        /// <inheritdoc />
        public IBuildConfiguration IgnoreAny(Func<MemberSignature, bool> predicate)
        {
            predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));

            _ignorePredicates.Add(predicate);

            return this;
        }

        /// <inheritdoc />
        public bool ShouldIgnore(in MemberSignature member)
        {
            if (_ignoredMembers.Contains(new MemberKey(member.DeclaringType, member.Name)))
            {
                return true;
            }

            foreach (var predicate in _ignorePredicates)
            {
                if (predicate(member))
                {
                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc />
        public bool TryGetMapping(Type sourceType, out Type targetType)
        {
            sourceType = sourceType ?? throw new ArgumentNullException(nameof(sourceType));

            return _typeMappings.TryGetValue(sourceType, out targetType!);
        }

        /// <inheritdoc />
        public IReadOnlyDictionary<Type, Type> TypeMappings => _typeMappings;

        /// <summary>
        ///     Gets a value indicating whether any custom typed value sources have been registered.
        /// </summary>
        /// <returns><c>true</c> if a custom typed value source has been registered; otherwise, <c>false</c>.</returns>
        internal bool HasCustomValueSources => _valueSources != null;

        /// <summary>
        ///     Gets the registry of custom typed value sources, or <c>null</c> when none have been registered.
        /// </summary>
        internal ValueSourceRegistry? CustomValueSources => _valueSources;

        /// <summary>
        ///     Gets a value indicating whether any custom named value sources have been registered.
        /// </summary>
        /// <returns><c>true</c> if a custom named value source has been registered; otherwise, <c>false</c>.</returns>
        internal bool HasCustomNamedValueSources => _namedValueSources != null;

        /// <summary>
        ///     Gets the registry of custom named value sources, or <c>null</c> when none have been registered.
        /// </summary>
        internal NamedValueSourceRegistry? CustomNamedValueSources => _namedValueSources;

        private readonly struct MemberKey : IEquatable<MemberKey>
        {
            private readonly Type _declaringType;
            private readonly string _memberName;

            public MemberKey(Type declaringType, string memberName)
            {
                _declaringType = declaringType;
                _memberName = memberName;
            }

            public bool Equals(MemberKey other)
            {
                return _declaringType == other._declaringType
                       && string.Equals(_memberName, other._memberName, StringComparison.Ordinal);
            }

            public override bool Equals(object? obj)
            {
                return obj is MemberKey other && Equals(other);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return _declaringType.GetHashCode() * 397 ^ StringComparer.Ordinal.GetHashCode(_memberName);
                }
            }
        }
    }
}
