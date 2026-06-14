namespace ModelBuilder
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     The <see cref="BuildContext" /> class
    ///     carries the per-build state shared by generated builders and value sources: the random
    ///     source, the build log, the build chain used for circular-reference detection, and the build
    ///     path used to report failures.
    /// </summary>
    internal sealed class BuildContext : IBuildContext
    {
        private static readonly IBuildConfiguration _emptyConfiguration = new BuildConfiguration();
        private readonly List<Type> _chain = new List<Type>();
        private readonly BuildConfiguration? _buildConfiguration;
        private readonly List<BuildFrame> _path = new List<BuildFrame>();
        private readonly List<Dictionary<string, object?>> _siblingScopes = new List<Dictionary<string, object?>>();
        private readonly List<Dictionary<string, object?>> _scopedValueScopes = new List<Dictionary<string, object?>>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="BuildContext" /> class.
        /// </summary>
        /// <param name="random">The random source used to generate values.</param>
        /// <param name="log">The optional build log; a no-op log is used when <c>null</c>.</param>
        /// <param name="options">The optional build options; defaults are used when <c>null</c>.</param>
        /// <param name="configuration">The optional build configuration; an empty configuration is used when <c>null</c>.</param>
        /// <param name="valueSources">The optional value source registry; the built-in defaults are used when <c>null</c>.</param>
        /// <param name="namedValueSources">The optional named value source registry; the built-in defaults are used when <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="random" /> parameter is <c>null</c>.</exception>
        internal BuildContext(
            IRandomSource random,
            IBuildLog? log = null,
            BuildContextOptions? options = null,
            IBuildConfiguration? configuration = null,
            ValueSourceRegistry? valueSources = null,
            NamedValueSourceRegistry? namedValueSources = null)
        {
            Random = random ?? throw new ArgumentNullException(nameof(random));
            Log = log ?? NullBuildLog.Instance;

            var resolvedOptions = options ?? new BuildContextOptions();

            MaxDepth = resolvedOptions.MaxDepth;
            NullPercentage = resolvedOptions.NullPercentage;
            MinCount = resolvedOptions.MinCount;
            MaxCount = resolvedOptions.MaxCount;
            Configuration = configuration ?? _emptyConfiguration;
            _buildConfiguration = Configuration as BuildConfiguration;
            ValueSources = valueSources ?? BuiltInValueSources.Default;
            NamedValueSources = namedValueSources ?? BuiltInValueSources.DefaultNamed;
        }

        /// <summary>
        ///     Creates a <see cref="ModelBuildException" /> that captures the current build path,
        ///     target, and log.
        /// </summary>
        /// <param name="message">The message that describes the failure.</param>
        /// <param name="failureKind">The category of the failure.</param>
        /// <param name="innerException">The optional exception that caused the failure.</param>
        /// <returns>The created exception.</returns>
        internal ModelBuildException CreateBuildException(
            string message,
            FailureKind failureKind,
            Exception? innerException = null)
        {
            var target = CurrentTarget;

            return new ModelBuildException(
                message,
                failureKind,
                target?.MemberType,
                target?.MemberName,
                BuildPath,
                Log.Entries,
                innerException);
        }

        /// <summary>
        ///     Enters a build frame, pushing it onto the build path and build chain until the returned
        ///     token is disposed.
        /// </summary>
        /// <param name="frame">The frame to enter.</param>
        /// <returns>A token that exits the frame when disposed.</returns>
        internal IDisposable Enter(BuildFrame frame)
        {
            _path.Add(frame);
            _chain.Add(frame.MemberType);

            return new BuildScope(this);
        }

        /// <summary>
        ///     Enters a member build frame.
        /// </summary>
        /// <param name="declaringType">The type that declares the member.</param>
        /// <param name="memberName">The name of the member.</param>
        /// <param name="memberType">The type of the member.</param>
        /// <returns>A token that exits the frame when disposed.</returns>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="declaringType" />, <paramref name="memberName" />, or
        ///     <paramref name="memberType" /> parameter is <c>null</c>.
        /// </exception>
        internal IDisposable EnterMember(Type declaringType, string memberName, Type memberType)
        {
            declaringType = declaringType ?? throw new ArgumentNullException(nameof(declaringType));
            memberName = memberName ?? throw new ArgumentNullException(nameof(memberName));
            memberType = memberType ?? throw new ArgumentNullException(nameof(memberType));

            return Enter(new BuildFrame(declaringType, memberName, memberType));
        }

        /// <summary>
        ///     Enters the root build frame for a type.
        /// </summary>
        /// <param name="type">The root type.</param>
        /// <returns>A token that exits the frame when disposed.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        internal IDisposable EnterRoot(Type type)
        {
            type = type ?? throw new ArgumentNullException(nameof(type));

            return Enter(new BuildFrame(type, null, type));
        }

        /// <inheritdoc />
        public IDisposable EnterSiblingScope()
        {
            _siblingScopes.Add(new Dictionary<string, object?>(StringComparer.Ordinal));
            _scopedValueScopes.Add(new Dictionary<string, object?>(StringComparer.Ordinal));

            return new SiblingScope(this);
        }

        /// <inheritdoc />
        public T? GetSibling<T>(string memberName)
        {
            memberName = memberName ?? throw new ArgumentNullException(nameof(memberName));

            if (_siblingScopes.Count == 0)
            {
                return default;
            }

            var scope = _siblingScopes[_siblingScopes.Count - 1];

            if (scope.TryGetValue(memberName, out var value) && value is T typed)
            {
                return typed;
            }

            return default;
        }

        /// <inheritdoc />
        public T? GetSibling<T>(params string[] memberNames)
        {
            memberNames = memberNames ?? throw new ArgumentNullException(nameof(memberNames));

            foreach (var memberName in memberNames)
            {
                var value = GetSibling<T>(memberName);

                if (value is not null)
                {
                    return value;
                }
            }

            return default;
        }

        /// <inheritdoc />
        public T GetOrAddScopedValue<T>(string key, Func<IBuildContext, T> factory)
        {
            key = key ?? throw new ArgumentNullException(nameof(key));
            factory = factory ?? throw new ArgumentNullException(nameof(factory));

            if (_scopedValueScopes.Count == 0)
            {
                // No instance is being populated, so there is nothing to share the value with; create a
                // throwaway value without caching it.
                return factory(this);
            }

            var scope = _scopedValueScopes[_scopedValueScopes.Count - 1];

            if (scope.TryGetValue(key, out var existing) && existing is T typed)
            {
                return typed;
            }

            var created = factory(this);

            scope[key] = created;

            return created;
        }

        /// <inheritdoc />
        public void RecordSibling(string memberName, object? value)
        {
            memberName = memberName ?? throw new ArgumentNullException(nameof(memberName));

            if (_siblingScopes.Count == 0)
            {
                return;
            }

            _siblingScopes[_siblingScopes.Count - 1][memberName] = value;
        }

        /// <inheritdoc />
        public T Build<T>(Type declaringType, string memberName)
        {
            declaringType = declaringType ?? throw new ArgumentNullException(nameof(declaringType));
            memberName = memberName ?? throw new ArgumentNullException(nameof(memberName));

            var memberType = typeof(T);

            if (Configuration.TryGetMapping(memberType, out var mappedType))
            {
                if (IsInBuildChain(mappedType))
                {
                    Log.Write(BuildLogEntryKind.SkipMember, mappedType, memberName, "circular-reference guard");

                    return default!;
                }

                if (Model.RegistryInternal.TryGet(mappedType, out var mappedBuilder) && mappedBuilder != null)
                {
                    using (EnterMember(declaringType, memberName, mappedType))
                    {
                        return (T)Invoke(memberName, () => mappedBuilder.Create(this));
                    }
                }
            }

            // Custom value sources registered on the build configuration take precedence over the
            // built-in sources, so a consumer can replace what the library produces for their own types
            // and members. A custom named source is tried before a custom typed source because matching on
            // member name is more specific than matching on type alone.
            if (_buildConfiguration != null
                && _buildConfiguration.HasCustomNamedValueSources
                && _buildConfiguration.CustomNamedValueSources!.TryGet<T>(memberName, out var customNamed)
                && customNamed != null)
            {
                Log.Write(BuildLogEntryKind.CreateValue, memberType, memberName, "custom named value source");

                using (EnterMember(declaringType, memberName, memberType))
                {
                    return Invoke(memberName, () => customNamed.Create(this, new BuildTarget(memberType, memberName)));
                }
            }

            if (_buildConfiguration != null
                && _buildConfiguration.HasCustomValueSources
                && _buildConfiguration.CustomValueSources!.TryGet<T>(out var customTyped)
                && customTyped != null)
            {
                Log.Write(BuildLogEntryKind.CreateValue, memberType, memberName, "custom value source");

                using (EnterMember(declaringType, memberName, memberType))
                {
                    return Invoke(memberName, () => customTyped.Create(this, new BuildTarget(memberType, memberName)));
                }
            }

            var source = ValueSource<T>.Instance;

            if (source != null)
            {
                Log.Write(BuildLogEntryKind.CreateValue, memberType, memberName, "registered value source");

                using (EnterMember(declaringType, memberName, memberType))
                {
                    return Invoke(memberName, () => source.Create(this, new BuildTarget(memberType, memberName)));
                }
            }

            if (NamedValueSources.TryGet<T>(memberName, out var named) && named != null)
            {
                Log.Write(BuildLogEntryKind.CreateValue, memberType, memberName, "named value source");

                using (EnterMember(declaringType, memberName, memberType))
                {
                    return Invoke(memberName, () => named.Create(this, new BuildTarget(memberType, memberName)));
                }
            }

            if (TryResolveValueSource<T>(out var builtIn) && builtIn != null)
            {
                Log.Write(BuildLogEntryKind.CreateValue, memberType, memberName, "built-in value source");

                using (EnterMember(declaringType, memberName, memberType))
                {
                    return Invoke(memberName, () => builtIn.Create(this, new BuildTarget(memberType, memberName)));
                }
            }

            var builder = ModelBuilderSlot<T>.Instance;

            if (builder == null)
            {
                return default!;
            }

            if (IsInBuildChain(memberType))
            {
                Log.Write(BuildLogEntryKind.SkipMember, memberType, memberName, "circular-reference guard");

                return default!;
            }

            if (IsDepthExceeded)
            {
                Log.Write(BuildLogEntryKind.SkipMember, memberType, memberName, "depth guard");

                return default!;
            }

            using (EnterMember(declaringType, memberName, memberType))
            {
                return Invoke(memberName, () => builder.Create(this));
            }
        }

        /// <summary>
        ///     Attempts to build a root value from a registered value source, so that
        ///     <see cref="Model.Create{T}(object[])" /> works for value-source-only types such as enums and
        ///     primitives that have no generated model builder.
        /// </summary>
        /// <typeparam name="T">The root type to build.</typeparam>
        /// <param name="value">The built value when a value source is resolved; otherwise <c>default</c>.</param>
        /// <returns><c>true</c> if a value source produced a value; otherwise, <c>false</c>.</returns>
        internal bool TryBuildRootValue<T>(out T value)
        {
            var targetType = typeof(T);

            if (TryResolveValueSource<T>(out var source) && source != null)
            {
                var target = new BuildTarget(targetType, targetType.Name);

                Log.Write(BuildLogEntryKind.CreateValue, targetType, targetType.Name, "root value source");

                value = Invoke(targetType.Name, () => source.Create(this, target));

                return true;
            }

            value = default!;

            return false;
        }

        private TResult Invoke<TResult>(string memberName, Func<TResult> create)
        {
            try
            {
                return create();
            }
            catch (ModelBuildException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw CreateBuildException(
                    "Failed to create a value for '" + memberName + "'. " + ex.Message,
                    FailureKind.ValueSourceThrew,
                    ex);
            }
        }

        /// <summary>
        ///     Determines whether a type is currently under construction in the build chain, used to
        ///     detect circular references.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns><c>true</c> if the type is in the build chain; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        internal bool IsInBuildChain(Type type)
        {
            type = type ?? throw new ArgumentNullException(nameof(type));

            return _chain.Contains(type);
        }

        /// <inheritdoc />
        public int NextCount()
        {
            var min = MinCount < 0 ? 0 : MinCount;
            var max = MaxCount < min ? min : MaxCount;

            return Random.NextInt32(min, max);
        }

        /// <inheritdoc />
        public bool ShouldPopulate(Type declaringType, string memberName, Type memberType)
        {
            declaringType = declaringType ?? throw new ArgumentNullException(nameof(declaringType));
            memberName = memberName ?? throw new ArgumentNullException(nameof(memberName));
            memberType = memberType ?? throw new ArgumentNullException(nameof(memberType));

            return Configuration.ShouldIgnore(new MemberSignature(declaringType, memberName, memberType)) == false;
        }

        /// <summary>
        ///     Resolves the value source for <typeparamref name="T" />, preferring a registered override
        ///     in the typed-static slot and falling back to the value source registry.
        /// </summary>
        /// <typeparam name="T">The type to resolve a value source for.</typeparam>
        /// <param name="source">The resolved value source, when one exists.</param>
        /// <returns><c>true</c> if a value source was resolved; otherwise, <c>false</c>.</returns>
        internal bool TryResolveValueSource<T>(out IValueSource<T>? source)
        {
            if (_buildConfiguration != null
                && _buildConfiguration.HasCustomValueSources
                && _buildConfiguration.CustomValueSources!.TryGet(out source)
                && source != null)
            {
                return true;
            }

            var slot = ValueSource<T>.Instance;

            if (slot != null)
            {
                source = slot;

                return true;
            }

            return ValueSources.TryGet(out source);
        }

        private void Exit()
        {
            if (_path.Count > 0)
            {
                _path.RemoveAt(_path.Count - 1);
            }

            if (_chain.Count > 0)
            {
                _chain.RemoveAt(_chain.Count - 1);
            }
        }

        private void ExitSiblingScope()
        {
            if (_siblingScopes.Count > 0)
            {
                _siblingScopes.RemoveAt(_siblingScopes.Count - 1);
            }

            if (_scopedValueScopes.Count > 0)
            {
                _scopedValueScopes.RemoveAt(_scopedValueScopes.Count - 1);
            }
        }

        /// <summary>
        ///     Gets the current build path from the root type down to the frame being built.
        /// </summary>
        internal IReadOnlyList<BuildFrame> BuildPath => _path.ToArray();

        /// <inheritdoc />
        public IBuildConfiguration Configuration { get; }

        /// <summary>
        ///     Gets the frame currently being built.
        /// </summary>
        /// <returns>The current frame, or <c>null</c> when no frame has been entered.</returns>
        internal BuildFrame? CurrentTarget => _path.Count > 0 ? _path[_path.Count - 1] : (BuildFrame?)null;

        /// <summary>
        ///     Gets the current build depth.
        /// </summary>
        internal int Depth => _chain.Count;

        /// <summary>
        ///     Gets a value indicating whether the current build depth has reached the configured
        ///     maximum.
        /// </summary>
        /// <returns><c>true</c> if the maximum depth has been reached; otherwise, <c>false</c>.</returns>
        internal bool IsDepthExceeded => _chain.Count >= MaxDepth;

        /// <inheritdoc />
        public IBuildLog Log { get; }

        /// <summary>
        ///     Gets the maximum depth the build may descend before it is considered to have exceeded a
        ///     safe limit.
        /// </summary>
        internal int MaxDepth { get; }

        /// <summary>
        ///     Gets the maximum number of items generated for a collection.
        /// </summary>
        internal int MaxCount { get; }

        /// <summary>
        ///     Gets the registry of entity-style value sources matched by member name.
        /// </summary>
        internal NamedValueSourceRegistry NamedValueSources { get; }

        /// <summary>
        ///     Gets the minimum number of items generated for a collection.
        /// </summary>
        internal int MinCount { get; }

        /// <summary>
        ///     Gets the percentage chance (0 to 100) that a nullable value is produced as <c>null</c>.
        /// </summary>
        internal int NullPercentage { get; }

        /// <inheritdoc />
        public IRandomSource Random { get; }

        /// <summary>
        ///     Gets the registry of value sources consulted when no typed-static override is registered.
        /// </summary>
        internal ValueSourceRegistry ValueSources { get; }

        private sealed class BuildScope : IDisposable
        {
            private BuildContext? _context;

            public BuildScope(BuildContext context)
            {
                _context = context;
            }

            public void Dispose()
            {
                _context?.Exit();
                _context = null;
            }
        }

        private sealed class SiblingScope : IDisposable
        {
            private BuildContext? _context;

            public SiblingScope(BuildContext context)
            {
                _context = context;
            }

            public void Dispose()
            {
                _context?.ExitSiblingScope();
                _context = null;
            }
        }
    }
}
