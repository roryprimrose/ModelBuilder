namespace ModelBuilder
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using ModelBuilder.Properties;

    /// <summary>
    /// The <see cref="DefaultConstructorResolver"/>
    /// class is used to resolve a constructor for a type.
    /// </summary>
    public class DefaultConstructorResolver : IConstructorResolver
    {
        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">The <paramref name="type"/> parameter is null.</exception>
        /// <exception cref="MissingMemberException">The <paramref name="type"/> parameter does not have a public constructor and no arguments are supplied.</exception>
        /// <exception cref="MissingMemberException">The <paramref name="type"/> parameter does not have a constructor that matches the supplied arguments.</exception>
        public ConstructorInfo Resolve(Type type, params object[] args)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (args == null || args.Length == 0)
            {
                var availableConstructors = type.GetConstructors().ToList();

                // Ignore any constructors that have a parameter with the type being created (a copy constructor)
                var validConstructors =
                    availableConstructors.Where(x => x.GetParameters().Any(y => y.ParameterType == type) == false)
                        .OrderBy(x => x.GetParameters().Length)
                        .ToList();

                var bestConstructor = validConstructors.FirstOrDefault();

                if (bestConstructor == null)
                {
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

                return bestConstructor;
            }

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
    }
}