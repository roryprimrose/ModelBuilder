namespace ModelBuilder.TypeCreators
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Net.NetworkInformation;
    using System.Reflection;

    /// <summary>
    ///     The <see cref="EnumerableTypeCreator" />
    ///     class is used to create an instance from an <see cref="IEnumerable{T}" /> type.
    /// </summary>
    public class EnumerableTypeCreator : TypeCreatorBase
    {
        private static readonly List<Type> _unsupportedTypes = new List<Type>
        {
            typeof(ArraySegment<>),
            typeof(IPAddressCollection),
            typeof(GatewayIPAddressInformationCollection),
            typeof(IPAddressInformationCollection),
            typeof(MulticastIPAddressInformationCollection),
            typeof(UnicastIPAddressInformationCollection),
            typeof(Dictionary<,>.KeyCollection),
            typeof(Dictionary<,>.ValueCollection),
            typeof(SortedDictionary<,>.KeyCollection),
            typeof(SortedDictionary<,>.ValueCollection)
        };

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        public override bool CanCreate(Type type, string referenceName, IBuildConfiguration configuration,
            IBuildChain buildChain)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var targetType = ResolveBuildType(type, configuration);

            if (targetType.IsClass
                && targetType.IsAbstract)
            {
                // This is an abstract class so we can't create it
                return false;
            }

            if (IsReadOnlyType(targetType) == false)
            {
                return false;
            }

            var internalType = FindEnumerableTypeArgument(targetType);

            if (internalType == null)
            {
                // The type does not implement IEnumerable<T>
                return false;
            }

            if (targetType.IsInterface)
            {
                var listGenericType = typeof(List<string>).GetGenericTypeDefinition();
                var listType = listGenericType.MakeGenericType(internalType);

                if (targetType.IsAssignableFrom(listType))
                {
                    // The instance is assignable from List<T> and therefore can be both created and populated by this type creator
                    return true;
                }

                return false;
            }

            // This is a class that we can presumably create so we need to check if it can be populated
            if (CanPopulate(targetType, referenceName, configuration, buildChain) == false)
            {
                // There is no point trying to create something that we can't populate
                return false;
            }

            return true;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        public override bool CanPopulate(Type type, string referenceName, IBuildConfiguration configuration,
            IBuildChain buildChain)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            var targetType = ResolveBuildType(type, configuration);

            if (IsReadOnlyType(targetType) == false)
            {
                return false;
            }

            var internalType = FindEnumerableTypeArgument(targetType);

            if (internalType == null)
            {
                // The type does not implement IEnumerable<T>
                return false;
            }

            if (IsUnsupportedType(targetType))
            {
                return false;
            }

            var genericTypeDefinition = typeof(ICollection<string>).GetGenericTypeDefinition();
            var genericType = genericTypeDefinition.MakeGenericType(internalType);

            if (genericType.IsAssignableFrom(targetType))
            {
                // The instance is ICollection<T> and therefore can be populated by this type creator
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Creates a child item given the context of a possible previous item being created.
        /// </summary>
        /// <param name="type">The type of value to generate.</param>
        /// <param name="executeStrategy">The execute strategy.</param>
        /// <param name="previousItem">The previous item generated, or <c>null</c>.</param>
        /// <returns>The new item generated.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        protected virtual object CreateChildItem(Type type, IExecuteStrategy executeStrategy, object previousItem)
        {
            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            return executeStrategy.Create(type);
        }

        /// <inheritdoc />
        [SuppressMessage(
            "Microsoft.Design",
            "CA1062:Validate arguments of public methods",
            MessageId = "0",
            Justification = "Type is validated by the base class")]
        protected override object CreateInstance(
            Type type,
            string referenceName,
            IExecuteStrategy executeStrategy,
            params object[] args)
        {
            Debug.Assert(type != null, "type != null");

            if (type.IsInterface)
            {
                var internalType = FindEnumerableTypeArgument(type);
                var genericTypeDefinition = typeof(List<string>).GetGenericTypeDefinition();
                var genericType = genericTypeDefinition.MakeGenericType(internalType);

                return Activator.CreateInstance(genericType);
            }

            return Activator.CreateInstance(type, args);
        }

        /// <inheritdoc />
        [SuppressMessage(
            "Microsoft.Design",
            "CA1062:Validate arguments of public methods",
            MessageId = "0",
            Justification = "Instance is validated by the base class")]
        protected override object PopulateInstance(object instance, IExecuteStrategy executeStrategy)
        {
            Debug.Assert(instance != null, "instance != null");

            var type = instance.GetType();

            var internalType = FindEnumerableTypeArgument(type);
            var collectionGenericTypeDefinition = typeof(ICollection<string>).GetGenericTypeDefinition();
            var collectionType = collectionGenericTypeDefinition.MakeGenericType(internalType);

            // Get the Add method
            var addMethod = collectionType.GetMethod("Add");

            Debug.Assert(addMethod != null, nameof(addMethod) + " != null");

            object previousItem = null;

            for (var index = 0; index < AutoPopulateCount; index++)
            {
                var childInstance = CreateChildItem(internalType, executeStrategy, previousItem);

                addMethod.Invoke(
                    instance,
                    new[]
                    {
                        childInstance
                    });

                previousItem = childInstance;
            }

            return instance;
        }

        private static Type FindEnumerableTypeArgument(Type type)
        {
            // The type may implement multiple interfaces including IEnumerable<T> where the type generic definition of the type itself is not the same as the IEnumerable<T> definition
            // Dictionary<TKey, TValue> is an example of this where it implements IEnumerable<KeyValuePair<TKey, TValue>>
            var topLevelTypeArgument = GetEnumerableTypeArgument(type);

            if (topLevelTypeArgument != null)
            {
                // The type itself is IEnumerable<T> so we don't need to search the implemented interfaces
                return topLevelTypeArgument;
            }

            var interfaces = type.GetInterfaces();

            foreach (var internalType in interfaces)
            {
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

            return type.GetGenericArguments()[0];
        }

        private static bool IsReadOnlyType(MemberInfo type)
        {
            // Check if the type is a ReadOnly type
            // We can't check for the implementation of IReadOnlyCollection<T> because this was introduced in .Net 4.5 
            // however this library targets 4.0
#if NETSTANDARD2_0
            if (type.Name.Contains("ReadOnly"))
#else
            if (type.Name.Contains("ReadOnly", StringComparison.OrdinalIgnoreCase))
#endif
            {
                // Looks like this is read only type
                // This covers ReadOnlyCollection in .Net 4.0 and above
                // and also covers IReadOnlyCollection<T> and IReadOnlyList<T> in .net 4.5 and above
                return false;
            }

            return true;
        }

        private static bool IsUnsupportedType(Type type)
        {
            foreach (var unsupportedType in _unsupportedTypes)
            {
                if (unsupportedType.IsGenericTypeDefinition
                    && type.IsGenericType)
                {
                    var typeDefinition = type.GetGenericTypeDefinition();

                    if (typeDefinition == unsupportedType)
                    {
                        return true;
                    }
                }
                else if (type == unsupportedType)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     Gets or sets how many instances will be auto-populated into the list by default when the
        ///     <see cref="EnumerableTypeCreator" /> is created.
        /// </summary>
        public static int DefaultAutoPopulateCount { get; set; } = 10;

        /// <inheritdoc />
        public override bool AutoDetectConstructor => false;

        /// <inheritdoc />
        public override bool AutoPopulate => false;

        /// <summary>
        ///     Gets or sets how many instances will be auto-populated into the list.
        /// </summary>
        public int AutoPopulateCount { get; set; } = DefaultAutoPopulateCount;

        /// <inheritdoc />
        public override int Priority => 100;
    }
}