namespace ModelBuilder
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    ///     The <see cref="DefaultTypeResolver" />
    ///     class provides the default implementation for resolving types to build.
    /// </summary>
    public class DefaultTypeResolver : ITypeResolver
    {
        /// <inheritdoc />
        public Type GetBuildType(IBuildConfiguration configuration, Type requestedType)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (requestedType == null)
            {
                throw new ArgumentNullException(nameof(requestedType));
            }

            var typeMappingRule = configuration.TypeMappingRules?.Where(x => x.SourceType == requestedType)
                .FirstOrDefault();

            if (typeMappingRule != null)
            {
                return typeMappingRule.TargetType;
            }

            // There is no type mapping for this type
            if (requestedType.IsInterface
                || requestedType.IsAbstract)
            {
                return AutoResolveBuildType(requestedType);
            }

            return requestedType;
        }

        private static Type AutoResolveBuildType(Type requestedType)
        {
            // Automatically resolve a derived type within the same assembly
            var assemblyTypes = requestedType.GetTypeInfo().Assembly.GetTypes();
            var possibleTypes = from x in assemblyTypes
                                where x.IsPublic && x.IsInterface == false && x.IsAbstract == false
                                select x;

            var matchingTypes = new List<Type>();

            foreach (var possibleType in possibleTypes)
            {
                // We need to check if the type is generic
                if (requestedType.IsGenericType)
                {
                    // They are both generic type definitions
                    var requestedParameters = requestedType.GetGenericArguments();
                    var evaluateParameters = possibleType.GetGenericArguments();

                    if (requestedParameters.Length != evaluateParameters.Length)
                    {
                        // The generic parameter list is different
                        continue;
                    }

                    try
                    {
                        // Check that we can make a generic type of the evaluateType using the generic parameters of the requested type
                        var closedType = possibleType.MakeGenericType(requestedParameters);

                        // If we got this far, we could create a generic type with the original generic parameters
                        // Now we need to check if it is a matching type
                        if (requestedType.IsAssignableFrom(closedType))
                        {
                            matchingTypes.Add(closedType);
                        }
                    }
                    catch (ArgumentException)
                    {
                        // We couldn't create a type with the generic parameters
                        // This is not a matching type
                    }
                }
                else
                {
                    if (possibleType.IsGenericType)
                    {
                        // We don't want to mix generic and non-generic types
                        continue;
                    }

                    if (requestedType.IsAssignableFrom(possibleType))
                    {
                        matchingTypes.Add(possibleType);
                    }
                }
            }

            if (requestedType.IsInterface)
            {
                // Check to see if there is a type that has the same name as the interface (minus the I)
                var interfaceNameMatchingType = matchingTypes.FirstOrDefault(x => "I" + x.Name == requestedType.Name);

                if (interfaceNameMatchingType != null)
                {
                    return interfaceNameMatchingType;
                }

                if (requestedType.IsGenericType == false
                    && requestedType.IsAssignableFrom(typeof(ArrayList)))
                {
                    // This is an enumerable type that supports ArrayList
                    // At this point we will just take ArrayList
                    // The reason for checking compatibility for ArrayList is that there are many IEnumerable types other than ArrayList which are
                    // a little to specific in their purpose whereas ArrayList is generic
                    return typeof(ArrayList);
                }
            }

            // Next give priority to a derived class name that has the same name as the requested type (without "Base" suffix for abstract classes)
            if (requestedType.IsAbstract)
            {
                var abstractNameToMatch = requestedType.Name;

                abstractNameToMatch = abstractNameToMatch.Replace("Base", string.Empty);

                var abstractNameMatchingType = matchingTypes.FirstOrDefault(x => x.Name == abstractNameToMatch);

                if (abstractNameMatchingType != null)
                {
                    return abstractNameMatchingType;
                }
            }

            var matchingType = matchingTypes.FirstOrDefault();

            if (matchingType == null)
            {
                return requestedType;
            }

            return matchingType;
        }
    }
}