using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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

            if (type.IsInterface)
            {
                var internalType = FindEnumerableTypeArgument(type);
                var listGenericType = typeof(List<string>).GetGenericTypeDefinition();
                var genericType = listGenericType.MakeGenericType(internalType);

                return Activator.CreateInstance(genericType);
            }

            return Activator.CreateInstance(type, args);
        }

        /// <inheritdoc />
        public override bool IsSupported(Type type, string referenceName, object context)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.IsGenericType == false)
            {
                return false;
            }

            // The type may implementation multiple interfaces including IEnumerable<T> where the type generic definition of the type itself is not the same as the IEnumerable<T> definition
            // Dictionary<TKey, TValue> is an example of this where it implements IEnumerable<KeyValuePair<TKey, TValue>>
            var internalType = FindEnumerableTypeArgument(type);

            if (internalType == null)
            {
                // The type does not implement IEnumerable<T>
                return false;
            }

            var readOnlyGenericType = typeof(ReadOnlyCollection<string>).GetGenericTypeDefinition();
            var readOnlyType = readOnlyGenericType.MakeGenericType(internalType);

            if (type == readOnlyType)
            {
                // The type is ReadOnlyCollection<T> which should be supported by DefaultTypeCreator as it will determine the data to build for its constructor
                return false;
            }
            
            if (type.IsInterface == false)
            {
                // Other known collection concrete types are expected to be supported
                return true;
            }

            var listGenericType = typeof(List<string>).GetGenericTypeDefinition();
            var listType = listGenericType.MakeGenericType(internalType);

            if (type.IsAssignableFrom(listType) == false)
            {
                // The instance is not List<T> and therefore cannot be created or populated by this type creator
                return false;
            }

            return true;
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
            var internalType = FindEnumerableTypeArgument(type);

            VerifyCreateRequest(type, null, instance);

            // Get the Add method
            var addMethod = type.GetMethod("Add");

            object previousItem = null;

            for (var index = 0; index < AutoPopulateCount; index++)
            {
                var childInstance = CreateChildItem(internalType, executeStrategy, previousItem);

                addMethod.Invoke(instance, new[] { childInstance });

                previousItem = childInstance;
            }

            return base.Populate(instance, executeStrategy);
        }

        protected virtual object CreateChildItem(Type type, IExecuteStrategy executeStrategy, object previousItem)
        {
            return executeStrategy.CreateWith(type);
        }

        private static Type FindEnumerableTypeArgument(Type type)
        {
            var topLevelTypeArgument = GetEnumerableTypeArgument(type);

            if (topLevelTypeArgument != null)
            {
                // The type itself is IEnumerable<T> so we don't need to search the implemented interfaces
                return topLevelTypeArgument;
            }

            var interfaces = type.GetInterfaces();

            for (var index = 0; index < interfaces.Length; index++)
            {
                var internalType = interfaces[index];

                var genericTypeArgument = GetEnumerableTypeArgument(internalType);

                if (genericTypeArgument != null)
                {
                    return genericTypeArgument;
                }
            }

            return null;
        }

        private static Type GetEnumerableTypeArgument(Type type)
        {
            if (type.IsGenericType == false)
            {
                return null;
            }

            var genericInternalType = type.GetGenericTypeDefinition();

            var enumerableType = typeof(IEnumerable<string>).GetGenericTypeDefinition();

            if (genericInternalType != enumerableType)
            {
                // We don't have a match on IEnumerable
                return null;
            }

            return type.GenericTypeArguments[0];
        }

        /// <summary>
        /// Gets or sets how many instances will be auto-populated into the list by default when the <see cref="EnumerableTypeCreator"/> is created.
        /// </summary>
        public static int DefaultAutoPopulateCount { get; set; } = 10;

        /// <inheritdoc />
        public override bool AutoDetectConstructor => false;

        /// <inheritdoc />
        public override bool AutoPopulate => false;

        /// <summary>
        /// Gets or sets how many instances will be auto-populated into the list.
        /// </summary>
        public int AutoPopulateCount { get; set; } = DefaultAutoPopulateCount;

        /// <inheritdoc />
        public override int Priority => 100;
    }
}