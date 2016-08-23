namespace ModelBuilder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    ///     The <see cref="DefaultTypeCreator" />
    ///     class is used to create an instance of a type using the constructors that match any arguments supplied.
    /// </summary>
    public class DefaultTypeCreator : TypeCreatorBase
    {
        /// <inheritdoc />
        protected override object CreateInstance(Type type, string referenceName, LinkedList<object> buildChain,
            params object[] args)
        {
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

        /// <inheritdoc />
        protected override object PopulateInstance(object instance, IExecuteStrategy executeStrategy)
        {
            // There is no out of the box population and this is left up to the execution strategy
            return instance;
        }
    }
}