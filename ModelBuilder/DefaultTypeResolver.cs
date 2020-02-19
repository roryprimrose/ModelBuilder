namespace ModelBuilder
{
    using System;
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
                // Automatically resolve a derived type within the same assembly
                var assemblyTypes = requestedType.GetTypeInfo().Assembly.GetTypes();
                var possibleTypes = from x in assemblyTypes
                    where x.IsPublic && x.IsInterface == false && x.IsAbstract == false
                          && requestedType.IsAssignableFrom(x)
                    select x;

                var matchingType = possibleTypes.FirstOrDefault(requestedType.IsAssignableFrom);

                if (matchingType == null)
                {
                    return requestedType;
                }

                return matchingType;
            }

            return requestedType;
        }
    }
}