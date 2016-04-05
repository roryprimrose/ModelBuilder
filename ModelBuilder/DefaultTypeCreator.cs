using System;
using System.Linq;

namespace ModelBuilder
{
    /// <summary>
    /// The <see cref="DefaultTypeCreator"/>
    /// class is used to create an instance of a type using the constructors that match any arguments supplied.
    /// </summary>
    public class DefaultTypeCreator : TypeCreatorBase
    {
        /// <inheritdoc />
        public override object Create(Type type, string referenceName, object context, params object[] args)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            VerifyCreateRequest(type, referenceName, context);

            if (args?.Length == 0)
            {
                return Activator.CreateInstance(type);
            }

            var types = args.Select(x => x.GetType()).ToArray();

            var constructor = type.GetConstructor(types);

            if (constructor == null)
            {
                throw new MissingMemberException("No constructor found matching type.");
            }

            return constructor.Invoke(args);
        }
    }
}