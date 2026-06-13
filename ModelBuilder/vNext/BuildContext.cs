namespace ModelBuilder.vNext
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     The <see cref="BuildContext" /> class
    ///     carries the per-build state shared by generated builders and value sources: the random
    ///     source, the build log, the build chain used for circular-reference detection, and the build
    ///     path used to report failures.
    /// </summary>
    public sealed class BuildContext
    {
        private static readonly IBuildConfiguration _emptyConfiguration = new BuildConfiguration();
        private readonly List<Type> _chain = new List<Type>();
        private readonly List<BuildFrame> _path = new List<BuildFrame>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="BuildContext" /> class.
        /// </summary>
        /// <param name="random">The random source used to generate values.</param>
        /// <param name="log">The optional build log; a no-op log is used when <c>null</c>.</param>
        /// <param name="options">The optional build options; defaults are used when <c>null</c>.</param>
        /// <param name="configuration">The optional build configuration; an empty configuration is used when <c>null</c>.</param>
        /// <exception cref="ArgumentNullException">The <paramref name="random" /> parameter is <c>null</c>.</exception>
        public BuildContext(
            IRandomSource random,
            IBuildLog? log = null,
            BuildContextOptions? options = null,
            IBuildConfiguration? configuration = null)
        {
            Random = random ?? throw new ArgumentNullException(nameof(random));
            Log = log ?? NullBuildLog.Instance;
            MaxDepth = (options ?? new BuildContextOptions()).MaxDepth;
            Configuration = configuration ?? _emptyConfiguration;
        }

        /// <summary>
        ///     Creates a <see cref="ModelBuildException" /> that captures the current build path,
        ///     target, and log.
        /// </summary>
        /// <param name="message">The message that describes the failure.</param>
        /// <param name="failureKind">The category of the failure.</param>
        /// <param name="innerException">The optional exception that caused the failure.</param>
        /// <returns>The created exception.</returns>
        public ModelBuildException CreateBuildException(
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
        public IDisposable Enter(BuildFrame frame)
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
        public IDisposable EnterMember(Type declaringType, string memberName, Type memberType)
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
        public IDisposable EnterRoot(Type type)
        {
            type = type ?? throw new ArgumentNullException(nameof(type));

            return Enter(new BuildFrame(type, null, type));
        }

        /// <summary>
        ///     Determines whether a type is currently under construction in the build chain, used to
        ///     detect circular references.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns><c>true</c> if the type is in the build chain; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        public bool IsInBuildChain(Type type)
        {
            type = type ?? throw new ArgumentNullException(nameof(type));

            return _chain.Contains(type);
        }

        /// <summary>
        ///     Determines whether a member should be populated according to the configured ignore rules.
        /// </summary>
        /// <param name="declaringType">The type that declares the member.</param>
        /// <param name="memberName">The name of the member.</param>
        /// <param name="memberType">The type of the member.</param>
        /// <returns><c>true</c> if the member should be populated; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">
        ///     The <paramref name="declaringType" />, <paramref name="memberName" />, or
        ///     <paramref name="memberType" /> parameter is <c>null</c>.
        /// </exception>
        public bool ShouldPopulate(Type declaringType, string memberName, Type memberType)
        {
            declaringType = declaringType ?? throw new ArgumentNullException(nameof(declaringType));
            memberName = memberName ?? throw new ArgumentNullException(nameof(memberName));
            memberType = memberType ?? throw new ArgumentNullException(nameof(memberType));

            return Configuration.ShouldIgnore(new MemberSignature(declaringType, memberName, memberType)) == false;
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

        /// <summary>
        ///     Gets the current build path from the root type down to the frame being built.
        /// </summary>
        public IReadOnlyList<BuildFrame> BuildPath => _path.ToArray();

        /// <summary>
        ///     Gets the build configuration for the current build.
        /// </summary>
        public IBuildConfiguration Configuration { get; }

        /// <summary>
        ///     Gets the frame currently being built.
        /// </summary>
        /// <returns>The current frame, or <c>null</c> when no frame has been entered.</returns>
        public BuildFrame? CurrentTarget => _path.Count > 0 ? _path[_path.Count - 1] : (BuildFrame?)null;

        /// <summary>
        ///     Gets the current build depth.
        /// </summary>
        public int Depth => _chain.Count;

        /// <summary>
        ///     Gets a value indicating whether the current build depth has reached the configured
        ///     maximum.
        /// </summary>
        /// <returns><c>true</c> if the maximum depth has been reached; otherwise, <c>false</c>.</returns>
        public bool IsDepthExceeded => _chain.Count >= MaxDepth;

        /// <summary>
        ///     Gets the build log for the current build.
        /// </summary>
        public IBuildLog Log { get; }

        /// <summary>
        ///     Gets the maximum depth the build may descend before it is considered to have exceeded a
        ///     safe limit.
        /// </summary>
        public int MaxDepth { get; }

        /// <summary>
        ///     Gets the random source for the current build.
        /// </summary>
        public IRandomSource Random { get; }

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
    }
}
