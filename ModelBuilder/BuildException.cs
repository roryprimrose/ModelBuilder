#if NET45
using System.Runtime.Serialization;
#endif

namespace ModelBuilder
{
    using System;

    /// <summary>
    ///     The <see cref="BuildException" />
    ///     class is used to describe a failure to build an instance of a type.
    /// </summary>
#if NET45
    [Serializable]
#endif
    public class BuildException : Exception
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BuildException" /> class.
        /// </summary>
        public BuildException()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BuildException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public BuildException(string message)
            : base(message)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BuildException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="inner">The inner exception.</param>
        public BuildException(string message, Exception inner)
            : base(message, inner)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BuildException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="targetType">The type of instance to create.</param>
        /// <param name="referenceName">Identifies the possible parameter or property name the instance is intended for.</param>
        /// <param name="context">The possible context object the instance is being created for.</param>
        /// <param name="buildLog">The build log.</param>
        public BuildException(string message, Type targetType, string referenceName, object context, string buildLog)
            : this(message, targetType, referenceName, context, buildLog, null)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BuildException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="targetType">The type of instance to create.</param>
        /// <param name="referenceName">Identifies the possible parameter or property name the instance is intended for.</param>
        /// <param name="context">The possible context object the instance is being created for.</param>
        /// <param name="buildLog">The build log.</param>
        /// <param name="inner">The inner exception.</param>
        public BuildException(
            string message,
            Type targetType,
            string referenceName,
            object context,
            string buildLog,
            Exception inner)
            : base(message, inner)
        {
            TargetType = targetType;
            Context = context;
            ReferenceName = referenceName;
            BuildLog = buildLog;
        }

#if NET45
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildException"/> class.
        /// </summary>
        /// <param name="info">
        /// The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.
        /// </param>
        /// <param name="context">
        /// The <see cref="StreamingContext"/> that contains contextual information about the source or destination.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// The <paramref name="info"/> parameter is null. 
        /// </exception>
        /// <exception cref="SerializationException">
        /// The class name is null or <see cref="System.Exception.HResult"/> is zero (0). 
        /// </exception>
        protected BuildException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
            BuildLog = info.GetString(nameof(BuildLog));
            ReferenceName = info.GetString(nameof(ReferenceName));
            TargetType = (Type) info.GetValue(nameof(TargetType), typeof(Type));
        }

        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue(nameof(BuildLog), BuildLog);
            info.AddValue(nameof(ReferenceName), ReferenceName);
            info.AddValue(nameof(TargetType), TargetType);

            base.GetObjectData(info, context);
        }
#endif

        /// <summary>
        ///     Gets or sets the build log.
        /// </summary>
        public string BuildLog { get; set; }

        /// <summary>
        ///     Gets or sets the context of the build action.
        /// </summary>
        public object Context { get; set; }

        /// <summary>
        ///     Gets or sets the reference name of the build action.
        /// </summary>
        public string ReferenceName { get; set; }

        /// <summary>
        ///     Gets or sets the target type of the build action.
        /// </summary>
        public Type TargetType { get; set; }
    }
}