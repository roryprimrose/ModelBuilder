namespace ModelBuilder.TypeCreators
{
    using System;

    /// <summary>
    ///     The <see cref="StructTypeCreator" />
    ///     class is used to create an instance of a struct.
    /// </summary>
    public class StructTypeCreator : TypeCreatorBase
    {
        /// <inheritdoc />
        protected override bool CanCreate(IBuildConfiguration configuration, IBuildChain buildChain, Type type,
            string? referenceName)
        {
            type = type ?? throw new ArgumentNullException(nameof(type));

            if (type.IsValueType == false)
            {
                return false;
            }

            if (type.IsEnum)
            {
                return false;
            }

            return true;
        }

        /// <inheritdoc />
        protected override object? CreateInstance(IExecuteStrategy executeStrategy, Type type, string? referenceName,
            params object?[]? args)
        {
            if (args?.Length > 0)
            {
                // We have arguments supplied so we will assume that they may the resolve type
                return Activator.CreateInstance(type, args);
            }

            // Use constructor detection to figure out how to create this instance
            var constructorResolver = executeStrategy.Configuration.ConstructorResolver;

            // We aren't provided with arguments so we need to resolve the most appropriate constructor
            var constructor = constructorResolver.Resolve(type);

            if (constructor == null)
            {
                // Structs return null for a default constructor
                return Activator.CreateInstance(type);
            }

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

        /// <inheritdoc />
        public override int Priority => 1000;
    }
}