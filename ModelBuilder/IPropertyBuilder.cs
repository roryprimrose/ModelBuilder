namespace ModelBuilder
{
    using System.Reflection;

    public interface IPropertyBuilder
    {
        object BuildValue(PropertyInfo propertyInfo, IExecuteStrategy executeStrategy);
    }

    public class PropertyBuilder : IPropertyBuilder
    {
        public object BuildValue(PropertyInfo propertyInfo, IExecuteStrategy executeStrategy)
        {
            
        }
    }
}