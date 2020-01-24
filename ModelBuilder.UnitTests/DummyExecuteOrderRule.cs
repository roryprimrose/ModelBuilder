namespace ModelBuilder.UnitTests
{
    using System.Reflection;
    using ModelBuilder.ExecuteOrderRules;

    public class DummyExecuteOrderRule : IExecuteOrderRule
    {
        public bool IsMatch(PropertyInfo property)
        {
            return false;
        }

        public int Priority { get; } = 100;
    }
}