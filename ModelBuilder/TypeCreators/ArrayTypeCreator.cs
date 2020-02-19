namespace ModelBuilder.TypeCreators
{
    using System;
    using System.Diagnostics;

    /// <summary>
    ///     The <see cref="ArrayTypeCreator" />
    ///     class is used to create an instance from array types.
    /// </summary>
    public class ArrayTypeCreator : DefaultTypeCreator
    {
        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        public override bool CanCreate(Type type, string referenceName, IBuildConfiguration configuration,
            IBuildChain buildChain)
        {
            // Creating using this creator has the same rules for populate as it does for create
            return CanPopulate(type, referenceName, buildChain);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        public override bool CanPopulate(Type type, string referenceName, IBuildChain buildChain)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (type.IsArray)
            {
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
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        protected override object CreateInstance(Type type, string referenceName, IExecuteStrategy executeStrategy,
            params object[] args)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            var count = Generator.NextValue(1, MaxCount);

            var parameters = new object[]
            {
                count
            };

            // Array has a dark-magic constructor that takes an int to define the size of the array
            var parameterTypes = new[]
            {
                typeof(int)
            };
            var constructor = type.GetConstructor(parameterTypes);

            Debug.Assert(constructor != null, "No constructor was found on the array");

            return constructor.Invoke(parameters);
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="instance" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="executeStrategy" /> parameter is <c>null</c>.</exception>
        protected override object PopulateInstance(object instance, IExecuteStrategy executeStrategy)
        {
            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (executeStrategy == null)
            {
                throw new ArgumentNullException(nameof(executeStrategy));
            }

            var instanceType = instance.GetType();

            var target = (Array) instance;

            if (target.Length == 0)
            {
                return instance;
            }

            Type itemType;

            // The array has entries for which we may be able to get a type
            var firstInstance = target.GetValue(0);

            if (firstInstance != null)
            {
                itemType = firstInstance.GetType();
            }
            else
            {
                // The type of item in the array has a default value of null so we need to attempt to parse the name from the name of the array type itself
#if NETSTANDARD2_0
                var typeName = instanceType.AssemblyQualifiedName?.Replace("[]", string.Empty);
#else
                var typeName = instanceType.AssemblyQualifiedName?.Replace(
                    "[]",
                    string.Empty,
                    StringComparison.OrdinalIgnoreCase);
#endif

                itemType = Type.GetType(typeName);
            }

            object previousItem = null;

            for (var index = 0; index < target.Length; index++)
            {
                var childInstance = CreateChildItem(itemType, executeStrategy, previousItem);

                target.SetValue(childInstance, index);

                previousItem = childInstance;
            }

            return instance;
        }

        /// <summary>
        ///     Gets or sets the default maximum count that can be generated.
        /// </summary>
        public static int DefaultMaxCount { get; set; } = 30;

        /// <inheritdoc />
        public override bool AutoDetectConstructor => false;

        /// <inheritdoc />
        public override bool AutoPopulate => false;

        /// <summary>
        ///     Gets or sets the maximum count generated by this instance.
        /// </summary>
        public int MaxCount { get; set; } = DefaultMaxCount;

        /// <inheritdoc />
        public override int Priority { get; } = 100;
    }
}