namespace ModelBuilder
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    ///     The <see cref="DefaultPropertyResolver" />
    ///     class is used to provide default logic for resolving property information.
    /// </summary>
    public class DefaultPropertyResolver : IPropertyResolver
    {
        private static readonly ConcurrentDictionary<Type, object?> _defaultValues =
            new ConcurrentDictionary<Type, object?>();

        private static readonly ConcurrentDictionary<Type, IList<PropertyInfo>> _globalCache =
            new ConcurrentDictionary<Type, IList<PropertyInfo>>();

        private readonly ConcurrentDictionary<Type, IList<PropertyInfo>> _perInstanceCache =
            new ConcurrentDictionary<Type, IList<PropertyInfo>>();

        /// <summary>
        ///     Initializes a new instance of the <see cref="DefaultPropertyResolver" /> class.
        /// </summary>
        /// <param name="cacheLevel">The cache level to use for resolved properties.</param>
        public DefaultPropertyResolver(CacheLevel cacheLevel)
        {
            CacheLevel = cacheLevel;
        }

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="targetType" /> parameter is <c>null</c>.</exception>
        public IEnumerable<PropertyInfo> GetOrderedProperties(IBuildConfiguration configuration, Type targetType)
        {
            configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            targetType = targetType ?? throw new ArgumentNullException(nameof(targetType));

            if (CacheLevel == CacheLevel.Global)
            {
                return _globalCache.GetOrAdd(targetType,
                    x => CalculateOrderedProperties(configuration, targetType).ToList());
            }

            if (CacheLevel == CacheLevel.PerInstance)
            {
                return _perInstanceCache.GetOrAdd(targetType,
                    x => CalculateOrderedProperties(configuration, targetType).ToList());
            }

            return CalculateOrderedProperties(configuration, targetType);
        }

        /// <inheritdoc />
        public virtual bool IsIgnored(
            IBuildConfiguration configuration,
            object instance,
            PropertyInfo propertyInfo,
            object?[]? args)
        {
            configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            instance = instance ?? throw new ArgumentNullException(nameof(instance));

            propertyInfo = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));

            var type = instance.GetType();

            // Check if there is a matching ignore rule
            var ignoreRule = configuration.IgnoreRules?.FirstOrDefault(
                x => x.IsMatch(propertyInfo));

            if (ignoreRule != null)
            {
                // We need to ignore this property
                return true;
            }

            if (propertyInfo.GetIndexParameters().Any())
            {
                // We can't do anything with index parameters
                return true;
            }

            var propertyValue = propertyInfo.GetValue(instance, null);

            if (AreEqual(instance, propertyValue))
            {
                // This property is a self reference to the owning instance
                // For example, Hashtable.SyncRoot is a reference to Hashtable itself
                return true;
            }
            
            var defaultValue = GetDefaultValue(propertyInfo.PropertyType);

            if (AreEqual(propertyValue, defaultValue))
            {
                // The property matches the default value of its type
                // A constructor parameter could have assigned the default type value or no constructor parameter
                // was assigned to the property
                // In either case we want to build a value for this property
                return false;
            }

            if (args == null)
            {
                // No constructor arguments
                // Assume that constructor has not defined a value for this property
                return false;
            }

            if (args.Length == 0)
            {
                // No constructor arguments
                // Assume that constructor has not defined a value for this property
                return false;
            }

            var matchingParameters =
                args.Where(x => x != null && propertyInfo.PropertyType.IsInstanceOfType(x)).ToList();

            if (matchingParameters.Count == 0)
            {
                // There are no constructor types that match the property type
                // Assume that no constructor parameter has defined this value
                return false;
            }

            // Check for instance types (ignoring strings)
            if (propertyInfo.PropertyType.IsValueType == false
                && propertyInfo.PropertyType != typeof(string))
            {
                // This is an interface or class type
                // Look for a matching instance
                if (matchingParameters.Any(x => ReferenceEquals(x, propertyValue)))
                {
                    // This is a direct link between the property value and a constructor parameter
                    return true;
                }

                // There is no instance match between this property value and a constructor parameter
                return false;
            }

            // Get the constructor matching the arguments so that we can try to match constructor parameter names against the property name
            var constructor = configuration.ConstructorResolver.Resolve(type, args);

            if (constructor == null)
            {
                var types = args.Select(x => x?.GetType().FullName ?? "<unknown>").ToArray();
                var parameterTypes = types.Select(x => x).Aggregate((current, next) => current + ", " + next);
                var message = string.Format(
                    CultureInfo.CurrentCulture,
                    "No constructor found matching type {0} with parameters[{1}].",
                    type.FullName,
                    parameterTypes);

                throw new MissingMemberException(message);
            }

            var parameters = constructor!.GetParameters();
            var maxLength = Math.Min(parameters.Length, args.Length);

            for (var index = 0; index < maxLength; index++)
            {
                var parameter = parameters[index];

                if (parameter.ParameterType.IsInstanceOfType(propertyValue) == false)
                {
                    // The constructor parameter type does not match the property value, keep looking
                    continue;
                }

                var parameterValue = args[index];

                if (AreEqual(propertyValue, parameterValue) == false)
                {
                    // This constructor parameter does not match property value, keep looking
                    continue;
                }

                if (string.Equals(propertyInfo.Name, parameter.Name, StringComparison.OrdinalIgnoreCase))
                {
                    // We have found that the property type, name and value are equivalent
                    // This is good enough to assume that the property value came from the constructor and we should not overwrite it
                    return true;
                }
            }

            return false;
        }

        private static bool AreEqual(object? first, object? second)
        {
            if (first == null
                && second == null)
            {
                return true;
            }

            if (first == null
                || second == null)
            {
                // Only one of the values is null
                return false;
            }

            if (ReferenceEquals(first, second))
            {
                // These reference the same instance in memory
                return true;
            }

            if (first.Equals(second))
            {
                return true;
            }

            return false;
        }

        private static IEnumerable<PropertyInfo> CalculateOrderedProperties(IBuildConfiguration configuration,
            Type targetType)
        {
            return from x in targetType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                where CanPopulate(x)
                orderby GetMaximumOrderPriority(configuration, x) descending
                select x;
        }

        /// <summary>
        ///     Determines whether the specified property can be populated.
        /// </summary>
        /// <param name="propertyInfo">The property to evaluate.</param>
        /// <returns><c>true</c> if the property can be populated; otherwise <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="propertyInfo" /> parameter is <c>null</c>.</exception>
        private static bool CanPopulate(PropertyInfo propertyInfo)
        {
            var setMethod = propertyInfo.GetSetMethod();

            if (setMethod != null)
            {
                // This is a publicly settable instance property
                // Against this we can assign both value and reference types
                return true;
            }

            // There is no set method. This is a read-only property
            var getMethod = propertyInfo.GetGetMethod()!;

            if (getMethod.ReturnType.IsValueType)
            {
                // We can't populate a value type via a get method
                return false;
            }

            if (getMethod.ReturnType == typeof(string))
            {
                // We can't populate a string type via a get method because strings are immutable
                return false;
            }

            return true;
        }

        [SuppressMessage(
            "Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Any failure to create the value will default to null for value comparisons.")]
        private static object? GetDefaultValue(Type type)
        {
            try
            {
                // Special case: System.String
                // Attempting to use Activator.CreateInstance on string throws an exception which is poor for performance
                if (type == typeof(string))
                {
                    return _defaultValues.GetOrAdd(type, x => null);
                }

                if (type.IsValueType == false)
                {
                    // This is a reference type which should also default to null
                    return _defaultValues.GetOrAdd(type, x => null);
                }

                return _defaultValues.GetOrAdd(type, Activator.CreateInstance);
            }
            catch
            {
                return null;
            }
        }

        private static int GetMaximumOrderPriority(IBuildConfiguration configuration, PropertyInfo property)
        {
            if (configuration.ExecuteOrderRules == null)
            {
                return 0;
            }

            var matchingRules = from x in configuration.ExecuteOrderRules
                where x.IsMatch(property)
                orderby x.Priority descending
                select x;

            var matchingRule = matchingRules.FirstOrDefault();

            if (matchingRule == null)
            {
                return 0;
            }

            return matchingRule.Priority;
        }

        /// <summary>
        ///     Gets or sets whether properties identified by <see cref="GetOrderedProperties" /> are cached.
        /// </summary>
        /// <returns>Returns the cache level to apply to properties.</returns>
        public CacheLevel CacheLevel { get; set; }
    }
}