namespace ModelBuilder.UnitTests
{
    using System;
    using System.Reflection;
    using ModelBuilder.UnitTests.Models;

    public class DummyPostBuildAction : IPostBuildAction
    {
        public void Execute(IBuildChain buildChain, object instance, Type type)
        {
        }

        public void Execute(IBuildChain buildChain, object instance, ParameterInfo parameterInfo)
        {
        }

        public void Execute(IBuildChain buildChain, object instance, PropertyInfo propertyInfo)
        {
        }

        public bool IsMatch(IBuildChain buildChain, Type type)
        {
            if (type == typeof(Company))
            {
                return true;
            }

            return false;
        }

        public bool IsMatch(IBuildChain buildChain, ParameterInfo parameterInfo)
        {
            return IsMatch(buildChain, parameterInfo.ParameterType);
        }

        public bool IsMatch(IBuildChain buildChain, PropertyInfo propertyInfo)
        {
            return IsMatch(buildChain, propertyInfo.PropertyType);
        }

        public int Priority { get; }
    }
}