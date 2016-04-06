using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using ModelBuilder.Properties;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="DefaultConstructorResolver"/>
    /// class is used to resolve a constructor for a type.
    /// </summary>
    public class DefaultConstructorResolver : IConstructorResolver
    {
        /// <inheritdoc />
        public ConstructorInfo Resolve(Type type, params object[] args)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (args?.Length == 0)
            {
                var constructors = from x in type.GetConstructors()
                    orderby x.GetParameters().Length
                    select x;

                var bestConstructor = constructors.FirstOrDefault();

                if (bestConstructor == null)
                {
                    var message = string.Format(CultureInfo.CurrentCulture,
                        Resources.ConstructorResolver_NoPublicConstructorFound, type.FullName);

                    throw new MissingMemberException(message);
                }

                return bestConstructor;
            }

            // Search for a matching constructor
            var types = args.Select(x => x.GetType()).ToArray();

            var constructor = type.GetConstructor(types);

            if (constructor == null)
            {
                var parameterTypes = types.Select(x => x.FullName).Aggregate((current, next) => current + ", " + next);
                var message = string.Format(CultureInfo.CurrentCulture,
                    "No constructor found matching type {0} with parameters[{1}].", type.FullName, parameterTypes);

                throw new MissingMemberException(message);
            }

            return constructor;
        }
    }
}