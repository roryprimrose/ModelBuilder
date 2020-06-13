namespace ModelBuilder.TypeCreators
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    ///     The <see cref="FactoryTypeCreator" />
    ///     class is used to create a value using a static factory method found on the type.
    /// </summary>
    public class FactoryTypeCreator : TypeCreatorBase
    {
        private static readonly ConcurrentDictionary<Type, MethodInfo?> _globalCache =
            new ConcurrentDictionary<Type, MethodInfo?>();

        private readonly ConcurrentDictionary<Type, MethodInfo?> _perInstanceCache =
            new ConcurrentDictionary<Type, MethodInfo?>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="FactoryTypeCreator" /> class.
        /// </summary>
        /// <param name="cacheLevel">The cache level to use for resolved methods.</param>
        public FactoryTypeCreator(CacheLevel cacheLevel)
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
                // There is a valid constructor to use so we don't need to search for a factory method
                return false;
            }

            var method = GetFactoryMethod(buildType);

            if (method == null)
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

            // The base class has already validated CanCreate which ensures that the factory method is availabe
            var method = GetFactoryMethod(buildType)!;

            if (args?.Length > 0)
            {
                return method.Invoke(null, args);
            }

            // Build any parameters that the method defines
            var parameterArguments = executeStrategy.CreateParameters(method);

            return method.Invoke(null, parameterArguments);
        }

        /// <inheritdoc />
        protected override object PopulateInstance(IExecuteStrategy executeStrategy, object instance)
        {
            return instance;
        }

        private static MethodInfo? CalculateFactoryMethod(Type type)
        {
            var bindingFlags = BindingFlags.Static
                               | BindingFlags.FlattenHierarchy
                               | BindingFlags.Public
                               | BindingFlags.InvokeMethod;

            // Get all the public static methods that return the return type but do not have the return type as a parameter
            // order by the the methods with the least amount of parameters
            var methods = from x in type.GetMethods(bindingFlags)
                orderby x.GetParameters().Length
                where type.IsAssignableFrom(x.ReturnType)
                      && x.GetParameters().Any(y => y.ParameterType == type) == false
                select x;

            return methods.FirstOrDefault();
        }

        private MethodInfo? GetFactoryMethod(Type type)
        {
            if (CacheLevel == CacheLevel.Global)
            {
                return _globalCache.GetOrAdd(type,
                    x => CalculateFactoryMethod(type));
            }

            if (CacheLevel == CacheLevel.PerInstance)
            {
                return _perInstanceCache.GetOrAdd(type,
                    x => CalculateFactoryMethod(type));
            }

            return CalculateFactoryMethod(type);
        }

        /// <summary>
        ///     Gets or sets whether resolved factory methods are cached.
        /// </summary>
        /// <returns>Returns the cache level to apply to methods.</returns>
        public CacheLevel CacheLevel { get; set; }

        /// <inheritdoc />
        public override int Priority { get; } = 200;
    }
}