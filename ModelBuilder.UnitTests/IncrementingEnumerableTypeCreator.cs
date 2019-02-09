namespace ModelBuilder.UnitTests
{
    using System;

    public class IncrementingEnumerableTypeCreator : EnumerableTypeCreator
    {
        public override bool CanCreate(Type type, string referenceName, IBuildChain buildChain)
        {
            if (base.CanCreate(type, referenceName, buildChain) == false)
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

        public override bool CanPopulate(Type type, string referenceName, IBuildChain buildChain)
        {
            if (base.CanPopulate(type, referenceName, buildChain) == false)
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

        protected override object CreateChildItem(Type type, IExecuteStrategy executeStrategy, object previousItem)
        {
            if (previousItem == null)
            {
                return base.CreateChildItem(type, executeStrategy, null);
            }

            // Use a double as the base type then convert later
            var value = Convert.ToDouble(previousItem);

            value++;

            var converted = Convert.ChangeType(value, type);

            return converted;
        }
    }
}