namespace ModelBuilder.TypeCreators
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    ///     The <see cref="DefaultTypeCreator" />
    ///     class is used to create an instance of a type using the constructors that match any arguments supplied.
    /// </summary>
    public class DefaultTypeCreator : TypeCreatorBase
    {
        /// <inheritdoc />
        protected override bool CanCreate(IBuildConfiguration configuration, IBuildChain buildChain, Type type,
            string? referenceName)
        {
            var baseValue = base.CanCreate(configuration, buildChain, type, referenceName);

            if (baseValue == false)
            {
                return baseValue;
            }

            // Resolve the type being created
            var typeToCreate = ResolveBuildType(configuration, type);

            // Use constructor detection to figure out how to create this instance
            var constructorResolver = configuration.ConstructorResolver;

            // Try to find a constructor to use
            var constructor = constructorResolver.Resolve(typeToCreate, null);

            if (constructor == null)
            {
                // This type does not have an available constructor
                return false;
            }

            return true;
        }

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
            // Resolve the type being created
            var typeToCreate = ResolveBuildType(executeStrategy.Configuration, type);

            if (args?.Length > 0)
            {
                // We have arguments supplied so we will assume that they may the resolve type
                return Activator.CreateInstance(typeToCreate, args);
            }

            // Use constructor detection to figure out how to create this instance
            var constructorResolver = executeStrategy.Configuration.ConstructorResolver;

            // We aren't provided with arguments so we need to resolve the most appropriate constructor
            // The constructor must exist because the base class has already validated that it could be resolved
            var constructor = constructorResolver.Resolve(typeToCreate)!;

            // Create the arguments for the constructor we have found
            var builtArgs = executeStrategy.CreateParameters(constructor);

            return constructor.Invoke(builtArgs);
        }

        /// <inheritdoc />
        protected override object PopulateInstance(IExecuteStrategy executeStrategy, object instance)
        {
            // There is no out of the box population and this is left up to the execution strategy
            return instance;
        }
    }
}