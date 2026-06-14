namespace ModelBuilder
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///     The <see cref="ModelBuildException" /> class
    ///     is used to report a model build failure with enough context — the failure kind, the build
    ///     path from the root type to the failing member, and the captured build log — to locate and
    ///     fix the problem without a debugger.
    /// </summary>
    public sealed class ModelBuildException : Exception
    {
        private static readonly IReadOnlyList<BuildFrame> _noFrames = Array.Empty<BuildFrame>();
        private static readonly IReadOnlyList<BuildLogEntry> _noLog = Array.Empty<BuildLogEntry>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="ModelBuildException" /> class.
        /// </summary>
        public ModelBuildException() : this("A model build error occurred.")
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ModelBuildException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the failure.</param>
        public ModelBuildException(string message) : base(message)
        {
            FailureKind = FailureKind.Unspecified;
            BuildPath = _noFrames;
            BuildLog = _noLog;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ModelBuildException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the failure.</param>
        /// <param name="innerException">The exception that caused this failure.</param>
        public ModelBuildException(string message, Exception innerException) : base(message, innerException)
        {
            FailureKind = FailureKind.Unspecified;
            BuildPath = _noFrames;
            BuildLog = _noLog;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ModelBuildException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the failure.</param>
        /// <param name="failureKind">The category of the failure.</param>
        public ModelBuildException(string message, FailureKind failureKind) : base(message)
        {
            FailureKind = failureKind;
            BuildPath = _noFrames;
            BuildLog = _noLog;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ModelBuildException" /> class.
        /// </summary>
        /// <param name="message">The message that describes the failure.</param>
        /// <param name="failureKind">The category of the failure.</param>
        /// <param name="targetType">The type that was being built when the failure occurred.</param>
        /// <param name="targetMember">The member that was being built when the failure occurred.</param>
        /// <param name="buildPath">The build path from the root type down to the failure.</param>
        /// <param name="buildLog">The build log captured up to the failure.</param>
        /// <param name="innerException">The exception that caused this failure, if any.</param>
        public ModelBuildException(
            string message,
            FailureKind failureKind,
            Type? targetType,
            string? targetMember,
            IReadOnlyList<BuildFrame> buildPath,
            IReadOnlyList<BuildLogEntry> buildLog,
            Exception? innerException) : base(message, innerException)
        {
            FailureKind = failureKind;
            TargetType = targetType;
            TargetMember = targetMember;
            BuildPath = buildPath ?? _noFrames;
            BuildLog = buildLog ?? _noLog;
        }

        /// <summary>
        ///     Gets the build log captured up to the point of failure.
        /// </summary>
        public IReadOnlyList<BuildLogEntry> BuildLog { get; }

        /// <summary>
        ///     Gets the build path from the root type down to the failing member.
        /// </summary>
        public IReadOnlyList<BuildFrame> BuildPath { get; }

        /// <summary>
        ///     Gets the category of the failure.
        /// </summary>
        public FailureKind FailureKind { get; }

        /// <summary>
        ///     Gets the member that was being built when the failure occurred.
        /// </summary>
        /// <returns>The member name, or <c>null</c> when the failure was not associated with a member.</returns>
        public string? TargetMember { get; }

        /// <summary>
        ///     Gets the type that was being built when the failure occurred.
        /// </summary>
        /// <returns>The target type, or <c>null</c> when the failure was not associated with a type.</returns>
        public Type? TargetType { get; }
    }
}
