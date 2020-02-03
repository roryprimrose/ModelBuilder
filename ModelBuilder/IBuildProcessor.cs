namespace ModelBuilder
{
    using System;
    using System.Reflection;

    public interface IBuildProcessor
    {
        object Build(Type type, IExecuteStrategy executeStrategy);
        object Build(ParameterInfo parameterInfo, IExecuteStrategy executeStrategy);
        object Build(PropertyInfo propertyInfo, IExecuteStrategy executeStrategy);
    }
}