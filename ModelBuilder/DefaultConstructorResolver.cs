namespace ModelBuilder
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using ModelBuilder.Properties;

    /// <summary>
    ///     The <see cref="DefaultConstructorResolver" />
    ///     class is used to resolve a constructor for a type.
    /// </summary>
    public class DefaultConstructorResolver : IConstructorResolver
    {
        private static readonly ConcurrentDictionary<Type, ConstructorInfo?> _globalConstructorCache =
            new ConcurrentDictionary<Type, ConstructorInfo?>();

        private readonly ConcurrentDictionary<Type, ConstructorInfo?> _perInstanceConstructorCache =
            new ConcurrentDictionary<Type, ConstructorInfo?>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultConstructorResolver" /> class.
        /// </summary>
        /// <param name="cacheLevel">The cache level to use for resolved constructors and parameters.</param>
        public DefaultConstructorResolver(CacheLevel cacheLevel)
        {
            CacheLevel = cacheLevel;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="type" /> parameter is <c>null</c>.</exception>
        /// <exception cref="MissingMemberException">
        ///     The <paramref name="type" /> parameter does not have a public constructor and
        ///     no arguments are supplied.
        /// </exception>
        /// <exception cref="MissingMemberException">
        ///     The <paramref name="type" /> parameter does not have a constructor that
        ///     matches the supplied arguments.
        /// </exception>
        public ConstructorInfo? Resolve(Type type, params object?[]? args)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (args == null)
            {
                return FindSmallestConstructor(type);
            }

            if (args.Length == 0)
            {
                return FindSmallestConstructor(type);
            }

            if (args.Any(x => x == null))
            {
                return FindConstructorMatchingArguments(type, args);
            }

            return FindConstructorMatchingTypes(type, args!);
        }

        private static ConstructorInfo? CalculateSmallestConstructor(Type type)
        {
            var availableConstructors = type.GetConstructors().ToList();

            if (availableConstructors.Count == 0)
            {
                // This could be a struct with no constructor defined
                // or it is a type that does not have public constructors
                // We can't return anything here and must rely on TypeCreators to handle this scenario
                return null;
            }

            // Ignore any constructors that have a parameter with the type being created (a copy constructor)
            var validConstructors = availableConstructors
                .Where(x => x.GetParameters().Any(y => y.ParameterType == type) == false)
                .OrderBy(x => x.GetParameters().Length).ToList();

            var bestConstructor = validConstructors.FirstOrDefault();

            if (bestConstructor != null)
            {
                return bestConstructor;
            }

            // There are public constructors but none of them are available that do not also have the target type as a parameter
            return null;
        }

        private static ConstructorInfo FindConstructorMatchingArguments(Type type, IList<object?> args)
        {
            // Parameters are consulted a lot here so get it into a dictionary first
            var availableConstructors = type.GetConstructors().ToDictionary(x => x, x => x.GetParameters());
            var possibleConstructors = availableConstructors.Where(x => x.Value.Length >= args.Count)
                .OrderBy(x => x.Value.Length);

            foreach (var constructor in possibleConstructors)
            {
                if (ParametersMatchArguments(constructor.Value, args))
                {
                    return constructor.Key;
                }
            }

            var message = string.Format(
                CultureInfo.CurrentCulture,
                Resources.ConstructorResolver_NoValidConstructorFound,
                type.FullName);

            throw new MissingMemberException(message);
        }

        private static ConstructorInfo FindConstructorMatchingTypes(Type type, object[] args)
        {
            // Search for a matching constructor
            var types = args.Select(x => x.GetType()).ToArray();

            var constructor = type.GetConstructor(types);

            if (constructor == null)
            {
                var parameterTypes = types.Select(x => x.FullName).Aggregate((current, next) => current + ", " + next);
                var message = string.Format(
                    CultureInfo.CurrentCulture,
                    "No constructor found matching type {0} with parameters[{1}].",
                    type.FullName,
                    parameterTypes);

                throw new MissingMemberException(message);
            }

            return constructor;
        }

        private static bool ParametersMatchArguments(IList<ParameterInfo> parameters, IList<object?> args)
        {
            Debug.Assert(
                args.Count <= parameters.Count,
                "To many arguments have been provided to match with this constructor, check previous LINQ filter");

            var firstOptionalIndex = parameters.ToList().FindIndex(x => x.IsOptional);

            if (firstOptionalIndex == -1
                && parameters.Count != args.Count)
            {
                // There are no optional parameters on this constructor and a mismatch between the number of parameters <-> arguments
                return false;
            }

            if (firstOptionalIndex > args.Count)
            {
                // There are more required parameters on the constructor than there are arguments provided
                return false;
            }

            for (var index = 0; index < args.Count; index++)
            {
                var parameter = parameters[index];
                var argument = args[index];

                if (argument == null)
                {
                    if (parameter.ParameterType.IsNullable())
                    {
                        // This is a special known case where nullable types can be null but are also value types
                        // This is a valid match of null against this parameter type
                        continue;
                    }

                    if (parameter.ParameterType.IsValueType)
                    {
                        // This is a null argument which is not equivalent to a value type
                        // This is not a matching constructor
                        return false;
                    }

                    // Else case here is that we have to assume that this parameter is a possible match because a null could be provided to a non-value type parameter
                    continue;
                }

                if (parameter.ParameterType.IsInstanceOfType(argument) == false)
                {
                    // This parameter type does not support the argument
                    return false;
                }

                // This parameter matches the argument type, keep checking other parameters
            }

            // All the parameters match the arguments supplied, this looks like a good constructor
            return true;
        }

        private ConstructorInfo? FindSmallestConstructor(Type type)
        {
            if (CacheLevel == CacheLevel.Global)
            {
                return _globalConstructorCache.GetOrAdd(type,
                    x => CalculateSmallestConstructor(type));
            }

            if (CacheLevel == CacheLevel.PerInstance)
            {
                return _perInstanceConstructorCache.GetOrAdd(type,
                    x => CalculateSmallestConstructor(type));
            }

            return CalculateSmallestConstructor(type);
        }

        /// <summary>
        ///     Gets or sets whether constructors identified by <see cref="Resolve" /> are cached.
        /// </summary>
        /// <returns>Returns the cache level to apply to parameters.</returns>
        public CacheLevel CacheLevel { get; set; }
    }
}