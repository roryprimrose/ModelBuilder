namespace ModelBuilder
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    ///     The <see cref="DefaultPropertyResolver" />
    ///     class is used to provide default logic for resolving property information.
    /// </summary>
    public class DefaultPropertyResolver : IPropertyResolver
    {
        private static readonly ConcurrentDictionary<Type, object> _defaultValues =
            new ConcurrentDictionary<Type, object>();

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="targetType" /> parameter is <c>null</c>.</exception>
        public IEnumerable<PropertyInfo> GetOrderedProperties(IBuildConfiguration configuration, Type targetType)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (targetType == null)
            {
                throw new ArgumentNullException(nameof(targetType));
            }

            return from x in targetType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                where CanPopulate(x)
                orderby GetMaximumOrderPriority(configuration, x) descending
                select x;
        }

        /// <inheritdoc />
        public virtual bool IsIgnored(
            IBuildConfiguration configuration,
            object instance,
            PropertyInfo propertyInfo,
            object[] args)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (instance == null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

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
                // There was no constructor found. This should be a struct with no defined constructor
                // There are no parameters to check
                return false;
            }

            var parameters = constructor.GetParameters();
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

        private static bool AreEqual(object first, object second)
        {
            if (first is IComparable comparer)
            {
                if (comparer.CompareTo(second) == 0)
                {
                    return true;
                }

                // This constructor parameter does not match property value, keep looking
                return false;
            }

            if (first == second)
            {
                return true;
            }

            return false;
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
            var getMethod = propertyInfo.GetGetMethod();

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
        private static object GetDefaultValue(Type type)
        {
            try
            {
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
    }
}