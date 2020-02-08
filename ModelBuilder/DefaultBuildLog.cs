﻿namespace ModelBuilder
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using ModelBuilder.Properties;

    /// <summary>
    ///     The <see cref="DefaultBuildLog" />
    ///     class provides default implementation for creating a build log when creating types and values.
    /// </summary>
    /// <threadsafety instance="false" />
    public class DefaultBuildLog : IBuildLog
    {
        private readonly StringBuilder _builder = new StringBuilder();
        private int _indent;

        /// <inheritdoc />
        public void BuildFailure(Exception ex)
        {
            if (ex == null)
            {
                throw new ArgumentNullException(nameof(ex));
            }

            FormatAndWriteMessage(ex.ToString());
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        public void CircularReferenceDetected(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            FormatAndWriteMessage(Resources.DefaultBuildLog_CircularReferenceDetected, type.FullName);
        }

        /// <inheritdoc />
        public void Clear()
        {
            _builder.Clear();
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="instanceType" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterType" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterName" /> parameter is <c>null</c>.</exception>
        public void CreatedParameter(Type instanceType, Type parameterType, string parameterName, object context)
        {
            if (instanceType == null)
            {
                throw new ArgumentNullException(nameof(instanceType));
            }

            if (parameterType == null)
            {
                throw new ArgumentNullException(nameof(parameterType));
            }

            if (parameterName == null)
            {
                throw new ArgumentNullException(nameof(parameterName));
            }

            _indent--;

            FormatAndWriteMessage(
                Resources.DefaultBuildLog_CreatedParameter,
                parameterName,
                parameterType.FullName,
                instanceType.FullName);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="propertyType" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyName" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="context" /> parameter is <c>null</c>.</exception>
        public void CreatedProperty(Type propertyType, string propertyName, object context)
        {
            if (propertyType == null)
            {
                throw new ArgumentNullException(nameof(propertyType));
            }

            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            _indent--;

            FormatAndWriteMessage(
                Resources.DefaultBuildLog_CreatedProperty,
                propertyName,
                propertyType.FullName,
                context.GetType().FullName);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        public void CreatedType(Type type, object context)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            _indent--;

            FormatAndWriteMessage(Resources.DefaultBuildLog_CreatedType, type.FullName);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="instanceType" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterType" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterName" /> parameter is <c>null</c>.</exception>
        public void CreatingParameter(Type instanceType, Type parameterType, string parameterName, object context)
        {
            if (instanceType == null)
            {
                throw new ArgumentNullException(nameof(instanceType));
            }

            if (parameterType == null)
            {
                throw new ArgumentNullException(nameof(parameterType));
            }

            if (parameterName == null)
            {
                throw new ArgumentNullException(nameof(parameterName));
            }

            FormatAndWriteMessage(
                Resources.DefaultBuildLog_CreatingParameter,
                parameterName,
                parameterType.FullName,
                instanceType.FullName);

            _indent++;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="propertyType" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyName" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="context" /> parameter is <c>null</c>.</exception>
        public void CreatingProperty(Type propertyType, string propertyName, object context)
        {
            if (propertyType == null)
            {
                throw new ArgumentNullException(nameof(propertyType));
            }

            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            FormatAndWriteMessage(
                Resources.DefaultBuildLog_CreatingProperty,
                propertyName,
                propertyType.FullName,
                context.GetType().FullName);

            _indent++;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="creatorType" /> parameter is <c>null</c>.</exception>
        public void CreatingType(Type type, Type creatorType, object context)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (creatorType == null)
            {
                throw new ArgumentNullException(nameof(creatorType));
            }

            FormatAndWriteMessage(Resources.DefaultBuildLog_CreatingType, type.FullName, creatorType.FullName);

            _indent++;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="generatorType" /> parameter is <c>null</c>.</exception>
        public void CreatingValue(Type type, Type generatorType, object context)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (generatorType == null)
            {
                throw new ArgumentNullException(nameof(generatorType));
            }

            FormatAndWriteMessage(Resources.DefaultBuildLog_CreatingValue, type.FullName, generatorType.FullName);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="propertyType" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyName" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="context" /> parameter is <c>null</c>.</exception>
        public void IgnoringProperty(Type propertyType, string propertyName, object context)
        {
            if (propertyType == null)
            {
                throw new ArgumentNullException(nameof(propertyType));
            }

            if (propertyName == null)
            {
                throw new ArgumentNullException(nameof(propertyName));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            FormatAndWriteMessage(
                Resources.DefaultBuildLog_IgnoringProperty,
                propertyName,
                propertyType.FullName,
                context.GetType().FullName);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="source" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="target" /> parameter is <c>null</c>.</exception>
        public void MappedType(Type source, Type target)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            FormatAndWriteMessage(Resources.DefaultBuildLog_MappingType, source.FullName, target.FullName);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="instance" /> parameter is <c>null</c>.</exception>
        public void PopulatedInstance(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            _indent--;

            FormatAndWriteMessage(Resources.DefaultBuildLog_PopulatedInstance, instance.GetType().FullName);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="instance" /> parameter is <c>null</c>.</exception>
        public void PopulatingInstance(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            FormatAndWriteMessage(Resources.DefaultBuildLog_PopulatingInstance, instance.GetType().FullName);

            _indent++;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="postBuildType" /> parameter is <c>null</c>.</exception>
        public void PostBuildAction(Type type, Type postBuildType, object context)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (postBuildType == null)
            {
                throw new ArgumentNullException(nameof(postBuildType));
            }

            FormatAndWriteMessage(Resources.DefaultBuildLog_PostBuild, type.FullName, postBuildType.FullName);
        }

        /// <summary>
        ///     Writes the specified message to the log.
        /// </summary>
        /// <param name="message">The message to write.</param>
        protected virtual void WriteMessage(string message)
        {
            _builder.Append(message);
            _builder.AppendLine();
        }

        private void FormatAndWriteMessage(string message, params object[] args)
        {
            Debug.Assert(message != null, "No message has been provided");

            var messageToWrite = message;

            if (args.Length > 0)
            {
                messageToWrite = string.Format(CultureInfo.CurrentCulture, message, args);
            }

            if (_indent > 0)
            {
                var lines = messageToWrite.Split(
                    new[]
                    {
                        Environment.NewLine
                    },
                    StringSplitOptions.RemoveEmptyEntries);
                var indent = new string(' ', _indent * 4);

                // Add the indent to each line and rebuild the message
                var indentedLines = lines.Select(x => indent + x);

                messageToWrite = string.Join(Environment.NewLine, indentedLines);
            }

            WriteMessage(messageToWrite);
        }

        /// <inheritdoc />
        public string Output => _builder.ToString();
    }
}