namespace ModelBuilder.TypeCreators
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using ModelBuilder.Properties;

    /// <summary>
    ///     The <see cref="DefaultTypeCreator" />
    ///     class is used to create an instance of a type using the constructors that match any arguments supplied.
    /// </summary>
    public class DefaultTypeCreator : TypeCreatorBase
    {
        /// <inheritdoc />
        [SuppressMessage(
            "Microsoft.Design",
            "CA1062:Validate arguments of public methods",
            MessageId = "0",
            Justification = "Type is validated by the base class")]
        protected override object? CreateInstance(IExecuteStrategy executeStrategy,
            Type type,
            string? referenceName,
            params object?[]? args)
        {
            if (args == null)
            {
                return Activator.CreateInstance(type);
            }

            if (args.Length == 0)
            {
                return Activator.CreateInstance(type);
            }

            var constructor = executeStrategy.Configuration?.ConstructorResolver?.Resolve(type, args);

            if (constructor == null)
            {
                throw new MissingMemberException(Resources.DefaultTypeCreator_NoMatchingConstructor);
            }

            return constructor.Invoke(args);
        }

        /// <inheritdoc />
        protected override object PopulateInstance(IExecuteStrategy executeStrategy, object instance)
        {
            // There is no out of the box population and this is left up to the execution strategy
            return instance;
        }
    }
}