namespace ModelBuilder.UnitTests
{
    using System;
    using System.Reflection;
    using ModelBuilder.CreationRules;

    public class DummyCreationRule : ICreationRule
    {
        public object Create(Type type, IExecuteStrategy executeStrategy)
        {
            throw new NotImplementedException();
        }

        public object Create(PropertyInfo propertyInfo, IExecuteStrategy executeStrategy)
        {
            throw new NotImplementedException();
        }

        public object Create(ParameterInfo parameterInfo, IExecuteStrategy executeStrategy)
        {
            throw new NotImplementedException();
        }

        public bool IsMatch(Type type)
        {
            return false;
        }

        public bool IsMatch(PropertyInfo propertyInfo)
        {
            return false;
        }

        public bool IsMatch(ParameterInfo parameterInfo)
        {
            return false;
        }

        public int Priority { get; }
    }
}