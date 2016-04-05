using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ModelBuilder.Properties;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="EnumerableTypeCreator"/>
    /// class is used to create an instance from an <see cref="IEnumerable{T}"/> type.
    /// </summary>
    public class EnumerableTypeCreator : TypeCreatorBase
    {
        /// <inheritdoc />
        public override object Create(Type type, string referenceName, object context, params object[] args)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            VerifyCreateRequest(type, referenceName, context);

            var listGenericType = typeof (List<string>).GetGenericTypeDefinition();
            var typeArgument = type.GenericTypeArguments.Single();
            var genericType = listGenericType.MakeGenericType(typeArgument);

            return Activator.CreateInstance(genericType);
        }

        /// <inheritdoc />
        public override bool IsSupported(Type type, string referenceName, object context)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.IsValueType)
            {
                return false;
            }

            if (type.IsGenericType == false)
            {
                return false;
            }

            var expectedGenericType = typeof (IEnumerable<string>).GetGenericTypeDefinition();

            if (type.GetGenericTypeDefinition() == expectedGenericType)
            {
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        public override object Populate(object instance, IExecuteStrategy executeStrategy)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            var type = instance.GetType();
            var listGenericType = typeof (List<string>).GetGenericTypeDefinition();
            var typeArgument = type.GenericTypeArguments.Single();
            var expectedGenericType = listGenericType.MakeGenericType(typeArgument);

            if (expectedGenericType != type)
            {
                // The instance is not List<T> and therefore cannot be populated by this type creator
                var message = string.Format(CultureInfo.CurrentCulture, Resources.Error_TypeNotSupportedFormat,
                    GetType().FullName, type.FullName);

                throw new NotSupportedException(message);
            }

            // Get the Add method
            var addMethod = type.GetMethod("Add");

            for (var index = 0; index < AutoPopulateCount; index++)
            {
                var childInstance = executeStrategy.CreateWith(typeArgument);

                addMethod.Invoke(instance, new[] {childInstance});
            }

            return base.Populate(instance, executeStrategy);
        }

        /// <summary>
        /// Gets or sets how many instances will be auto-populated into the list.
        /// </summary>
        public static int AutoPopulateCount { get; set; } = 10;

        /// <inheritdoc />
        public override bool AutoDetectConstructor => false;

        /// <inheritdoc />
        public override bool AutoPopulate => false;
    }
}