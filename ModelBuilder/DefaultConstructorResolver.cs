namespace ModelBuilder
{
    using System;
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
        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="configuration" /> parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentNullException">The <paramref name="constructor" /> parameter is <c>null</c>.</exception>
        public IEnumerable<ParameterInfo> GetOrderedParameters(IBuildConfiguration configuration,
            ConstructorInfo constructor)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (constructor == null)
            {
                throw new ArgumentNullException(nameof(constructor));
            }

            return from x in constructor.GetParameters()
                orderby GetMaximumOrderPriority(configuration, x) descending
                select x;
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
        public ConstructorInfo Resolve(Type type, params object[] args)
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

            return FindConstructorMatchingTypes(type, args);
        }

        private static ConstructorInfo FindConstructorMatchingArguments(Type type, IList<object> args)
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

        private static ConstructorInfo FindSmallestConstructor(Type type)
        {
            var availableConstructors = type.GetConstructors().ToList();

            // Ignore any constructors that have a parameter with the type being created (a copy constructor)
            var validConstructors = availableConstructors
                .Where(x => x.GetParameters().Any(y => y.ParameterType == type) == false)
                .OrderBy(x => x.GetParameters().Length).ToList();

            var bestConstructor = validConstructors.FirstOrDefault();

            if (bestConstructor != null)
            {
                return bestConstructor;
            }

            string message;

            if (availableConstructors.Count > validConstructors.Count)
            {
                message = string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.ConstructorResolver_NoValidConstructorFound,
                    type.FullName);
            }
            else
            {
                message = string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.ConstructorResolver_NoPublicConstructorFound,
                    type.FullName);
            }

            throw new MissingMemberException(message);
        }

        private static int GetMaximumOrderPriority(IBuildConfiguration configuration, ParameterInfo parameter)
        {
            if (configuration.ExecuteOrderRules == null)
            {
                return 0;
            }

            var matchingRules = from x in configuration.ExecuteOrderRules
                where x.IsMatch(parameter)
                orderby x.Priority descending
                select x;

            var matchingRule = matchingRules.FirstOrDefault();

            if (matchingRule == null)
            {
                return 0;
            }

            return matchingRule.Priority;
        }

        private static bool ParametersMatchArguments(IList<ParameterInfo> parameters, IList<object> args)
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
    }
}