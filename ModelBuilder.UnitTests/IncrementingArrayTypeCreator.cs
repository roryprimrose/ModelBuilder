namespace ModelBuilder.UnitTests
{
    using System;
    using System.Globalization;
    using ModelBuilder.TypeCreators;

    public class IncrementingArrayTypeCreator : ArrayTypeCreator
    {
        protected override object CreateChildItem(Type type, IExecuteStrategy executeStrategy, object previousItem)
        {
            if (previousItem == null)
            {
                return base.CreateChildItem(type, executeStrategy, null);
            }

            // Use a double as the base type then convert later
            var value = Convert.ToDouble(previousItem, CultureInfo.InvariantCulture);

            value++;

            var converted = Convert.ChangeType(value, type, CultureInfo.InvariantCulture);

            return converted;
        }
    }
}