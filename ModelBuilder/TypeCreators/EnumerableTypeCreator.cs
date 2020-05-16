namespace ModelBuilder.TypeCreators
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Net.NetworkInformation;

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
        protected override bool CanCreate(IBuildConfiguration configuration,
            IBuildChain buildChain, Type type, string? referenceName)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.IsClass
                && type.IsAbstract)
            {
                // This is an abstract class so we can't create it
                return false;
            }

            var typeToCreate = DetermineTypeToCreate(type);

            if (typeToCreate == null)
            {
                // We don't know how to create this type
                return false;
            }

            if (typeToCreate.IsInterface)
            {
                // We couldn't identify that the type could be created as either List<> or Dictionary<,>
                return false;
            }

            if (CanPopulate(configuration, buildChain, typeToCreate, referenceName) == false)
            {
                // There is no point trying to create something that we can't populate
                return false;
            }

            return true;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        protected override bool CanPopulate(IBuildConfiguration configuration,
            IBuildChain buildChain, Type type, string? referenceName)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (IsReadOnlyType(type))
            {
                // We can't populate read-only types here
                // We can however let DefaultTypeCreator handle it because the type should have a constructor parameter that it can support
                return false;
            }

            if (IsUnsupportedType(type))
            {
                return false;
            }

            var genericParameterType = FindEnumerableTypeArgument(type);

            if (genericParameterType == null)
            {
                // The type does not implement IEnumerable<T>
                return false;
            }

            var genericTypeDefinition = typeof(ICollection<>);
            var genericType = genericTypeDefinition.MakeGenericType(genericParameterType);

            if (genericType.IsAssignableFrom(type))
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
        protected virtual object? CreateChildItem(Type type, IExecuteStrategy executeStrategy, object? previousItem)
        {
            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            return executeStrategy.Create(type);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        protected override object? Create(IExecuteStrategy executeStrategy, Type type, string? referenceName,
            params object?[]? args)
        {
            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.IsInterface == false)
            {
                return CreateInstance(executeStrategy, type, referenceName, args);
            }

            var typeToCreate = DetermineTypeToCreate(type);

            if (typeToCreate == null)
            {
                var format = "Unable to create type {0} using {1} because it is not compatible with {2}";
                var message = string.Format(CultureInfo.CurrentCulture, format, type.FullName,
                    nameof(EnumerableTypeCreator), "IEnumerable<T>");
                var context = executeStrategy.BuildChain.Last;
                var buildLog = executeStrategy.Log.Output;

                throw new BuildException(message, type, referenceName, context, buildLog);
            }

            return CreateInstance(executeStrategy, typeToCreate, referenceName, args);

        }

        /// <inheritdoc />
        protected override object? CreateInstance(IExecuteStrategy executeStrategy,
            Type type,
            string? referenceName,
            params object?[]? args)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return Activator.CreateInstance(type, args);
        }

        /// <inheritdoc />
        [SuppressMessage(
            "Microsoft.Design",
            "CA1062:Validate arguments of public methods",
            MessageId = "0",
            Justification = "Instance is validated by the base class")]
        protected override object PopulateInstance(IExecuteStrategy executeStrategy, object instance)
        {
            var type = instance.GetType();

            var internalType = FindEnumerableTypeArgument(type);

            if (internalType == null)
            {
                var format = "Unable to populate type {0} using {1} because it is not compatible with {2}";
                var message = string.Format(CultureInfo.CurrentCulture, format, type.FullName,
                    nameof(EnumerableTypeCreator), "IEnumerable<T>");
                var buildLog = executeStrategy.Log.Output;

                throw new BuildException(message, type, null, instance, buildLog);
            }

            var collectionGenericTypeDefinition = typeof(ICollection<>);
            var collectionType = collectionGenericTypeDefinition.MakeGenericType(internalType);

            // Get the Add method
            var addMethod = collectionType.GetMethod("Add");

            object? previousItem = null;

            var count = Generator.NextValue(MinCount, MaxCount);

            for (var index = 0; index < count; index++)
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

        private static Type? DetermineTypeToCreate(Type type)
        {
            if (type.IsInterface == false)
            {
                return type;
            }

            // We need to check if the type is compatible with either List<T> or Dictionary<K,V> so we can use those types to create the instance
            var genericParameterType = FindEnumerableTypeArgument(type);

            if (genericParameterType == null)
            {
                // The type does not implement IEnumerable<T>
                return null;
            }

            // Check if the generic parameter type is a KeyValuePair<K,V>
            if (genericParameterType.IsGenericType)
            {
                // The T in IEnumerable<T> is also a generic type itself
                // We need to try to match this against KeyValuePair<K,V>
                var nestedGenericTypeDefinition = genericParameterType.GetGenericTypeDefinition();

                if (nestedGenericTypeDefinition == typeof(KeyValuePair<,>))
                {
                    // Looks like this may be a dictionary
                    var typeArguments = genericParameterType.GenericTypeArguments;

                    var dictionaryType = GetSupportedGenericType(type, typeof(Dictionary<,>), typeArguments);

                    if (dictionaryType != null)
                    {
                        // This is a Dictionary<K,V> type
                        return dictionaryType;
                    }
                }
            }

            // At this point we have a type that we can check to see if it will be supported by List<T>
            var listType = GetSupportedGenericType(type, typeof(List<>), genericParameterType);

            if (listType != null)
            {
                // This is an interface that can't be satisfied by either List or Dictionary
                return listType;
            }

            return type;
        }

        private static Type? FindEnumerableTypeArgument(Type type)
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

        private static Type? GetEnumerableTypeArgument(Type type)
        {
            if (type.IsGenericType == false)
            {
                return null;
            }

            var genericInternalType = type.GetGenericTypeDefinition();

            var enumerableType = typeof(IEnumerable<>);

            if (genericInternalType != enumerableType)
            {
                // We don't have a match on IEnumerable
                return null;
            }

            return type.GetGenericArguments()[0];
        }

        private static Type? GetSupportedGenericType(Type type, Type genericTypeDefinition,
            params Type[] genericParameterTypes)
        {
            var potentialType = genericTypeDefinition.MakeGenericType(genericParameterTypes);

            if (type.IsAssignableFrom(potentialType))
            {
                // The instance is assignable from the generic type and therefore can be both created and populated by this type creator
                return potentialType;
            }

            return null;
        }

        private static bool IsReadOnlyType(Type type)
        {
            if (type.IsGenericType == false)
            {
                // The readonly types we are looking for are generic types
                return false;
            }

            var definition = type.GetGenericTypeDefinition();

            if (definition == typeof(ReadOnlyCollection<>))
            {
                return true;
            }

            if (definition == typeof(ReadOnlyDictionary<,>))
            {
                return true;
            }

            return false;
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

        /// <inheritdoc />
        public override bool AutoDetectConstructor => false;

        /// <inheritdoc />
        public override bool AutoPopulate => false;

        /// <summary>
        ///     Gets or sets the maximum count generated by this instance.
        /// </summary>
        public int MaxCount { get; set; } = 30;

        /// <summary>
        ///     Gets or sets the minimum count generated by this instance.
        /// </summary>
        public int MinCount { get; set; } = 10;

        /// <inheritdoc />
        public override int Priority => 100;
    }
}