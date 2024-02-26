namespace ModelBuilder.TypeCreators
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
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
        protected override bool CanCreate(IBuildConfiguration configuration,
            IBuildChain buildChain, Type type, string? referenceName)
        {
            type = type ?? throw new ArgumentNullException(nameof(type));

            if (type.IsAssignableFrom(typeof(IEnumerable)))
            {
                // This is not enumerable so not supported by this TypeCreator
                return false;
            }

            var typeToCreate = ResolveBuildType(configuration, type);
            
            if (typeToCreate.IsClass == false)
            {
                // This is a struct so we can't create it here
                return false;
            }

            if (typeToCreate.IsAbstract)
            {
                // This is an abstract class so we can't create it
                return false;
            }

            if (typeToCreate.IsInterface)
            {
                // We couldn't identify the concrete type to create
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
            type = type ?? throw new ArgumentNullException(nameof(type));

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

            // See if there is an add method
            var addMethod = GetAddMethod(type);

            if (addMethod != null)
            {
                return true;
            }

            return false;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        protected override object? Create(IExecuteStrategy executeStrategy, Type type, string? referenceName,
            params object?[]? args)
        {
            executeStrategy = executeStrategy ?? throw new ArgumentNullException(nameof(executeStrategy));

            type = type ?? throw new ArgumentNullException(nameof(type));

            var targetType = ResolveBuildType(executeStrategy.Configuration, type);

            if (targetType == null)
            {
                var format = "Unable to create type {0} using {1} because it is not compatible with {2}";
                var message = string.Format(CultureInfo.CurrentCulture, format, type,
                    typeof(EnumerableTypeCreator), typeof(IEnumerable));
                var context = executeStrategy.BuildChain.Last;
                var buildLog = executeStrategy.Log.Output;

                throw new BuildException(message, type, referenceName, context, buildLog);
            }
            
            return CreateInstance(executeStrategy, targetType, referenceName, args);
        }

        /// <summary>
        ///     Creates a child item given the context of a possible previous values being created.
        /// </summary>
        /// <param name="type">The type being populated.</param>
        /// <param name="addMember">The add member used to insert new items into the type.</param>
        /// <param name="executeStrategy">The execute strategy.</param>
        /// <param name="previousValues">The previous values generated, or <c>null</c>.</param>
        /// <returns>The new values generated.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        protected virtual object?[]? CreateChildItem(Type type, MethodInfo addMember, IExecuteStrategy executeStrategy,
            object?[]? previousValues)
        {
            executeStrategy = executeStrategy ?? throw new ArgumentNullException(nameof(executeStrategy));

            return executeStrategy.CreateParameters(addMember);
        }

        /// <inheritdoc />
        protected override object? CreateInstance(IExecuteStrategy executeStrategy,
            Type type,
            string? referenceName,
            params object?[]? args)
        {
            type = type ?? throw new ArgumentNullException(nameof(type));

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

            // Get the Add method
            var addMethod = GetAddMethod(type);

            if (addMethod == null)
            {
                throw new BuildException(
                    $"The type {type.FullName} does not have an 'Add' member to populate with new values.");
            }

            object?[]? previousValues = null;

            var count = Generator.NextValue(MinCount, MaxCount);

            for (var index = 0; index < count; index++)
            {
                var values = CreateChildItem(type, addMethod, executeStrategy, previousValues);

                addMethod.Invoke(
                    instance,
                    values);

                previousValues = values;
            }

            return instance;
        }

        private static MethodInfo? GetAddMethod(Type type)
        {
            return type.GetMethod("Add", BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public);
        }

        private static bool IsReadOnlyType(Type type)
        {
            const string readOnlyName = "ReadOnly";

            if (type.IsGenericType == false)
            {
                if (type.Name.Contains(readOnlyName))
                {
                    return true;
                }

                return false;
            }

            var definition = type.GetGenericTypeDefinition();

            if (definition.Name.Contains(readOnlyName))
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
        public override bool AutoPopulate => false;

        /// <summary>
        ///     Gets or sets the maximum count generated by this instance.
        /// </summary>
        public int MaxCount { get; set; } = 15;

        /// <summary>
        ///     Gets or sets the minimum count generated by this instance.
        /// </summary>
        public int MinCount { get; set; } = 5;

        /// <inheritdoc />
        public override int Priority => 100;
    }
}