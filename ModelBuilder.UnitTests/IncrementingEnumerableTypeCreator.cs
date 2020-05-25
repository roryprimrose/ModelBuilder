namespace ModelBuilder.UnitTests
{
    using System;
    using System.Globalization;
    using ModelBuilder.TypeCreators;

    public class IncrementingEnumerableTypeCreator : EnumerableTypeCreator
    {
        protected override bool CanCreate(IBuildConfiguration configuration,
            IBuildChain buildChain, Type type, string? referenceName)
        {
            if (base.CanCreate(configuration, buildChain, type, referenceName) == false)
            {
                return false;
            }

            var baseType = type.GenericTypeArguments[0];

            if (baseType.IsNullable())
            {
                return false;
            }

            var generator = new RandomGenerator();

            return generator.IsSupported(baseType);
        }

        protected override bool CanPopulate(IBuildConfiguration configuration,
            IBuildChain buildChain, Type type, string? referenceName)
        {
            if (base.CanPopulate(configuration, buildChain, type, referenceName) == false)
            {
                return false;
            }

            var baseType = type.GenericTypeArguments[0];

            if (baseType.IsNullable())
            {
                return false;
            }

            var generator = new RandomGenerator();

            return generator.IsSupported(baseType);
        }

        protected override object? CreateChildItem(Type type, IExecuteStrategy executeStrategy, object? previousItem)
        {
            if (previousItem == null!)
            {
                return base.CreateChildItem(type, executeStrategy, null!);
            }

            // Use a double as the base type then convert later
            var value = Convert.ToDouble(previousItem, CultureInfo.InvariantCulture);

            value++;

            var converted = Convert.ChangeType(value, type, CultureInfo.InvariantCulture);

            return converted;
        }
    }
}