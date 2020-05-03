namespace ModelBuilder.UnitTests
{
    using System;
    using System.Reflection;
    using ModelBuilder.CreationRules;

    public class DummyCreationRule : ICreationRule
    {
        public object Create(IExecuteStrategy executeStrategy, Type type)
        {
            throw new NotImplementedException();
        }

        public object Create(IExecuteStrategy executeStrategy, PropertyInfo propertyInfo)
        {
            throw new NotImplementedException();
        }

        public object Create(IExecuteStrategy executeStrategy, ParameterInfo parameterInfo)
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

        public Guid Value { get; set; }
    }
}