namespace ModelBuilder
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
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

            if (IsEnabled == false)
            {
                return;
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

            if (IsEnabled == false)
            {
                return;
            }

            FormatAndWriteMessage(Resources.DefaultBuildLog_CircularReferenceDetected, type.FullName);
        }

        /// <inheritdoc />
        public void Clear()
        {
            _builder.Clear();
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="parameterInfo" /> parameter is <c>null</c>.</exception>
        public void CreatedParameter(ParameterInfo parameterInfo, object? context)
        {
            if (parameterInfo == null)
            {
                throw new ArgumentNullException(nameof(parameterInfo));
            }

            if (IsEnabled == false)
            {
                return;
            }

            _indent--;

            FormatAndWriteMessage(
                Resources.DefaultBuildLog_CreatedParameter,
                parameterInfo.Name,
                parameterInfo.ParameterType.FullName,
                parameterInfo.Member.DeclaringType?.FullName ?? "<unknown>");
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="propertyInfo" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="context" /> parameter is <c>null</c>.</exception>
        public void CreatedProperty(PropertyInfo propertyInfo, object context)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (IsEnabled == false)
            {
                return;
            }

            _indent--;

            FormatAndWriteMessage(
                Resources.DefaultBuildLog_CreatedProperty,
                propertyInfo.Name,
                propertyInfo.PropertyType.FullName,
                context.GetType().FullName);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        public void CreatedType(Type type, object? context)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (IsEnabled == false)
            {
                return;
            }

            _indent--;

            FormatAndWriteMessage(Resources.DefaultBuildLog_CreatedType, type.FullName);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="parameterInfo" /> parameter is <c>null</c>.</exception>
        public void CreatingParameter(ParameterInfo parameterInfo, object? context)
        {
            if (parameterInfo == null)
            {
                throw new ArgumentNullException(nameof(parameterInfo));
            }

            if (IsEnabled == false)
            {
                return;
            }

            FormatAndWriteMessage(
                Resources.DefaultBuildLog_CreatingParameter,
                parameterInfo.Name,
                parameterInfo.ParameterType.FullName,
                parameterInfo.Member.DeclaringType?.FullName ?? "<unknown>");

            _indent++;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="propertyInfo" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="context" /> parameter is <c>null</c>.</exception>
        public void CreatingProperty(PropertyInfo propertyInfo, object context)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (IsEnabled == false)
            {
                return;
            }

            FormatAndWriteMessage(
                Resources.DefaultBuildLog_CreatingProperty,
                propertyInfo.Name,
                propertyInfo.PropertyType.FullName,
                context.GetType().FullName);

            _indent++;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="creatorType" /> parameter is <c>null</c>.</exception>
        public void CreatingType(Type type, Type creatorType, object? context)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (creatorType == null)
            {
                throw new ArgumentNullException(nameof(creatorType));
            }

            if (IsEnabled == false)
            {
                return;
            }

            FormatAndWriteMessage(Resources.DefaultBuildLog_CreatingType, type.FullName, creatorType.FullName);

            _indent++;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="generatorType" /> parameter is <c>null</c>.</exception>
        public void CreatingValue(Type type, Type generatorType, object? context)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (generatorType == null)
            {
                throw new ArgumentNullException(nameof(generatorType));
            }

            if (IsEnabled == false)
            {
                return;
            }

            FormatAndWriteMessage(Resources.DefaultBuildLog_CreatingValue, type.FullName, generatorType.FullName);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="propertyInfo" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="context" /> parameter is <c>null</c>.</exception>
        public void IgnoringProperty(PropertyInfo propertyInfo, object context)
        {
            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (IsEnabled == false)
            {
                return;
            }

            FormatAndWriteMessage(
                Resources.DefaultBuildLog_IgnoringProperty,
                propertyInfo.Name,
                propertyInfo.PropertyType.FullName,
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

            if (IsEnabled == false)
            {
                return;
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

            if (IsEnabled == false)
            {
                return;
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

            if (IsEnabled == false)
            {
                return;
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

            if (IsEnabled == false)
            {
                return;
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
        public bool IsEnabled { get; set; } = false;

        /// <inheritdoc />
        public string Output => _builder.ToString();
    }
}