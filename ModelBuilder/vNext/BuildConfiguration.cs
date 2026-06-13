namespace ModelBuilder.vNext
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     The <see cref="BuildConfiguration" /> class
    ///     is the mutable, fluent implementation of <see cref="IBuildConfiguration" /> that
    ///     configuration modules populate with type mappings and ignore rules.
    /// </summary>
    public sealed class BuildConfiguration : IBuildConfiguration
    {
        private readonly HashSet<MemberKey> _ignoredMembers = new HashSet<MemberKey>();
        private readonly List<Func<MemberSignature, bool>> _ignorePredicates = new List<Func<MemberSignature, bool>>();
        private readonly Dictionary<Type, Type> _typeMappings = new Dictionary<Type, Type>();

        /// <summary>
        ///     Registers a mapping from a source type to a concrete target type.
        /// </summary>
        /// <param name="sourceType">The source type, typically an interface or abstract type.</param>
        /// <param name="targetType">The concrete type to build in its place.</param>
        /// <returns>The same configuration instance for chaining.</returns>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="sourceType" /> or <paramref name="targetType" /> parameter is <c>null</c>.
        /// </exception>
        public BuildConfiguration AddMapping(Type sourceType, Type targetType)
        {
            sourceType = sourceType ?? throw new ArgumentNullException(nameof(sourceType));
            targetType = targetType ?? throw new ArgumentNullException(nameof(targetType));

            _typeMappings[sourceType] = targetType;

            return this;
        }

        /// <summary>
        ///     Registers a mapping from a source type to a concrete target type.
        /// </summary>
        /// <typeparam name="TSource">The source type, typically an interface or abstract type.</typeparam>
        /// <typeparam name="TTarget">The concrete type to build in its place.</typeparam>
        /// <returns>The same configuration instance for chaining.</returns>
        public BuildConfiguration AddMapping<TSource, TTarget>()
            where TTarget : TSource
        {
            return AddMapping(typeof(TSource), typeof(TTarget));
        }

        /// <summary>
        ///     Registers a targeted ignore rule for a specific member on a specific type.
        /// </summary>
        /// <param name="declaringType">The type that declares the member.</param>
        /// <param name="memberName">The name of the member to ignore.</param>
        /// <returns>The same configuration instance for chaining.</returns>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="declaringType" /> or <paramref name="memberName" /> parameter is <c>null</c>.
        /// </exception>
        public BuildConfiguration Ignore(Type declaringType, string memberName)
        {
            declaringType = declaringType ?? throw new ArgumentNullException(nameof(declaringType));
            memberName = memberName ?? throw new ArgumentNullException(nameof(memberName));

            _ignoredMembers.Add(new MemberKey(declaringType, memberName));

            return this;
        }

        /// <summary>
        ///     Registers a type-agnostic ignore rule that applies to any member matching the predicate.
        /// </summary>
        /// <param name="predicate">The predicate evaluated against each member.</param>
        /// <returns>The same configuration instance for chaining.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="predicate" /> parameter is <c>null</c>.</exception>
        public BuildConfiguration IgnoreAny(Func<MemberSignature, bool> predicate)
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
