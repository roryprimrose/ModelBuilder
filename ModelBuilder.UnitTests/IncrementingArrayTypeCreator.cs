namespace ModelBuilder.UnitTests
{
    using System;

    public class IncrementingArrayTypeCreator : ArrayTypeCreator
    {
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