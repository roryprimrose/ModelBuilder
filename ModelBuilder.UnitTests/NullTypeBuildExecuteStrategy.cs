namespace ModelBuilder.UnitTests
{
    using System;

    public class NullTypeBuildExecuteStrategy : DefaultExecuteStrategy
    {
        protected override object Build(Type type, params object[] arguments)
        {
            return base.Build((Type)null, arguments);
        }
    }
}