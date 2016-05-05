using System;
using System.Diagnostics;
using System.Text;

namespace ModelBuilder
{
    public class DefaultBuildLog : IBuildLog
    {
        private readonly StringBuilder _builder = new StringBuilder();
        private int _indent;

        /// <inheritdoc />
        public void Clear()
        {
            _builder.Clear();
        }

        /// <inheritdoc />
        public void CreatedType(Type type, object context)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            _indent--;

            WriteMessage("End creating type {0}", type.FullName);
        }

        /// <inheritdoc />
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

            WriteMessage("Creating parameter {0} ({1}) for type {2}", parameterName, parameterType.FullName,
                instanceType.FullName);
        }

        /// <inheritdoc />
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

            WriteMessage("Creating property {0} ({1}) on type {2}", propertyName, propertyType.FullName,
                context.GetType().FullName);
        }

        /// <inheritdoc />
        public void CreatingType(Type type, object context)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            WriteMessage("Start creating type {0}", type.FullName);

            _indent++;
        }

        public void CreatingValue(Type type, object context)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            WriteMessage("Creating {0} value", type.FullName);
        }

        /// <inheritdoc />
        public void PopulatedInstance(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            _indent--;

            WriteMessage("End populating instance {0}", instance.GetType().FullName);
        }

        /// <inheritdoc />
        public void PopulatingInstance(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            WriteMessage("Start populating instance {0}", instance.GetType().FullName);

            _indent++;
        }

        /// <inheritdoc />
        private void WriteMessage(string message, params object[] args)
        {
            Debug.Assert(message != null, "No message has been provided");
            Debug.Assert(args != null, "No arguments have been provided");

            if (_indent > 0)
            {
                var indent = new string(' ', _indent*4);

                _builder.Append(indent);
            }

            _builder.AppendFormat(message, args);

            _builder.AppendLine();
        }

        public string Output => _builder.ToString();
    }
}