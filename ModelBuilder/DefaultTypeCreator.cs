namespace ModelBuilder
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    /// <summary>
    ///     The <see cref="DefaultTypeCreator" />
    ///     class is used to create an instance of a type using the constructors that match any arguments supplied.
    /// </summary>
    public class DefaultTypeCreator : TypeCreatorBase
    {
        /// <inheritdoc />
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0",
             Justification = "Type is validated by the base class")]
        protected override object CreateInstance(
            Type type,
            string referenceName,
            IExecuteStrategy executeStrategy,
            params object[] args)
        {
            Debug.Assert(type != null, "type != null");

            if (args == null)
            {
                return Activator.CreateInstance(type);
            }

            if (args.Length == 0)
            {
                return Activator.CreateInstance(type);
            }

            var constructor = executeStrategy?.Configuration?.ConstructorResolver?.Resolve(type, args);

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