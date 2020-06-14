namespace ModelBuilder.TypeCreators
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    ///     The <see cref="SingletonTypeCreator" />
    ///     class is used to create a value using a Singleton property found on the type.
    /// </summary>
    public class SingletonTypeCreator : TypeCreatorBase
    {
        private static readonly ConcurrentDictionary<Type, PropertyInfo?> _globalCache =
            new ConcurrentDictionary<Type, PropertyInfo?>();

        private readonly ConcurrentDictionary<Type, PropertyInfo?> _perInstanceCache =
            new ConcurrentDictionary<Type, PropertyInfo?>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="SingletonTypeCreator" /> class.
        /// </summary>
        /// <param name="cacheLevel">The cache level to use for resolved methods.</param>
        public SingletonTypeCreator(CacheLevel cacheLevel)
        {
            CacheLevel = cacheLevel;
        }

        /// <inheritdoc />
        protected override bool CanCreate(IBuildConfiguration configuration, IBuildChain buildChain, Type type,
            string? referenceName)
        {
            var buildType = ResolveBuildType(configuration, type);

            var baseValue = base.CanCreate(configuration, buildChain, buildType, referenceName);

            if (baseValue == false)
            {
                return false;
            }

            // Check if there is no constructor to use
            var constructor = configuration.ConstructorResolver.Resolve(buildType);

            if (constructor != null)
            {
                // There is a valid constructor to use so we don't need to search for a singleton property
                return false;
            }

            var propertyInfo = GetSingletonProperty(buildType);

            if (propertyInfo == null)
            {
                // There is no factory method that can be used
                return false;
            }

            return true;
        }

        /// <inheritdoc />
        protected override object? CreateInstance(IExecuteStrategy executeStrategy, Type type, string? referenceName,
            params object?[]? args)
        {
            var buildType = ResolveBuildType(executeStrategy.Configuration, type);

            // The base class has already validated CanCreate which ensures that the singleton property is available
            var propertyInfo = GetSingletonProperty(buildType)!;

            return propertyInfo.GetValue(null, args);
        }

        /// <inheritdoc />
        protected override object PopulateInstance(IExecuteStrategy executeStrategy, object instance)
        {
            return instance;
        }

        private static PropertyInfo? CalculateSingletonProperty(Type type)
        {
            const BindingFlags bindingFlags = BindingFlags.Static
                                              | BindingFlags.FlattenHierarchy
                                              | BindingFlags.Public
                                              | BindingFlags.GetProperty;

            // Get all the public static readonly properties that return the return type
            var methods = from x in type.GetProperties(bindingFlags)
                where x.GetIndexParameters().Length == 0
                      && type.IsAssignableFrom(x.PropertyType)
                select x;

            return methods.FirstOrDefault();
        }

        private PropertyInfo? GetSingletonProperty(Type type)
        {
            if (CacheLevel == CacheLevel.Global)
            {
                return _globalCache.GetOrAdd(type,
                    x => CalculateSingletonProperty(type));
            }

            if (CacheLevel == CacheLevel.PerInstance)
            {
                return _perInstanceCache.GetOrAdd(type,
                    x => CalculateSingletonProperty(type));
            }

            return CalculateSingletonProperty(type);
        }

        /// <summary>
        ///     Gets or sets whether resolved singleton properties are cached.
        /// </summary>
        /// <returns>Returns the cache level to apply to properties.</returns>
        public CacheLevel CacheLevel { get; set; }

        /// <inheritdoc />
        public override int Priority { get; } = 200;
    }
}