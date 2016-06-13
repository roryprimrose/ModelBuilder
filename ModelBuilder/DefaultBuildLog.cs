namespace ModelBuilder
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using ModelBuilder.Properties;

    /// <summary>
    /// The <see cref="DefaultBuildLog"/>
    /// class provides default implementation for creating a build log when creating types and values.
    /// </summary>
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

            WriteMessage(ex.ToString());
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> parameter is null.</exception>
        public void CircularReferenceDetected(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            WriteMessage(Resources.DefaultBuildLog_CircularReferenceDetected, type.FullName);
        }

        /// <inheritdoc />
        public void Clear()
        {
            _builder.Clear();
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> parameter is null.</exception>
        public void CreatedType(Type type, object context)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            _indent--;

            WriteMessage(Resources.DefaultBuildLog_CreatedType, type.FullName);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="instanceType"/> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterType"/> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="parameterName"/> parameter is null.</exception>
        public void CreateParameter(Type instanceType, Type parameterType, string parameterName, object context)
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

            WriteMessage(
                Resources.DefaultBuildLog_CreateParameter,
                parameterName,
                parameterType.FullName,
                instanceType.FullName);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="propertyType"/> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyName"/> parameter is null.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="context"/> parameter is null.</exception>
        public void CreateProperty(Type propertyType, string propertyName, object context)
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

            WriteMessage(
                Resources.DefaultBuildLog_CreateProperty,
                propertyName,
                propertyType.FullName,
                context.GetType().FullName);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> parameter is null.</exception>
        public void CreatingType(Type type, object context)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            WriteMessage(Resources.DefaultBuildLog_CreatingType, type.FullName);

            _indent++;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> parameter is null.</exception>
        public void CreatingValue(Type type, object context)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            WriteMessage(Resources.DefaultBuildLog_CreatingValue, type.FullName);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> parameter is null.</exception>
        public void PopulatedInstance(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            _indent--;

            WriteMessage(Resources.DefaultBuildLog_PopulatedInstance, instance.GetType().FullName);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="instance"/> parameter is null.</exception>
        public void PopulatingInstance(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            WriteMessage(Resources.DefaultBuildLog_PopulatingInstance, instance.GetType().FullName);

            _indent++;
        }

        /// <inheritdoc />
        private void WriteMessage(string message, params object[] args)
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

            _builder.Append(messageToWrite);
            _builder.AppendLine();
        }

        /// <inheritdoc />
        public string Output => _builder.ToString();
    }
}