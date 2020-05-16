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
        protected override object? CreateInstance(IExecuteStrategy executeStrategy, Type type, string? referenceName,
            params object?[]? args)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return Activator.CreateInstance(type, args);
        }

        /// <inheritdoc />
        protected override bool CanCreate(IBuildConfiguration configuration, IBuildChain buildChain, Type type, string? referenceName)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

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
        protected override object PopulateInstance(IExecuteStrategy executeStrategy, object instance)
        {
            // There is no out of the box population and this is left up to the execution strategy
            return instance;
        }

        /// <inheritdoc />
        public override int Priority => 1000;
    }
}