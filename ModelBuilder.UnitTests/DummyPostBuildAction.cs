namespace ModelBuilder.UnitTests
{
    using System;
    using System.Reflection;
    using ModelBuilder.UnitTests.Models;

    public class DummyPostBuildAction : IPostBuildAction
    {
        public void Execute(object instance, Type type, IBuildChain buildChain)
        {
        }

        public void Execute(object instance, ParameterInfo parameterInfo, IBuildChain buildChain)
        {
        }

        public void Execute(object instance, PropertyInfo propertyInfo, IBuildChain buildChain)
        {
        }

        public bool IsMatch(Type type, IBuildChain buildChain)
        {
            if (type == typeof(Company))
            {
                return true;
            }

            return false;
        }

        public bool IsMatch(ParameterInfo parameterInfo, IBuildChain buildChain)
        {
            return IsMatch(parameterInfo.ParameterType, buildChain);
        }

        public bool IsMatch(PropertyInfo propertyInfo, IBuildChain buildChain)
        {
            return IsMatch(propertyInfo.PropertyType, buildChain);
        }

        public int Priority { get; }
    }
}