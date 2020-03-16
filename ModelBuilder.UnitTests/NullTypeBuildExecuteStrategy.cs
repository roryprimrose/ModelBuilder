namespace ModelBuilder.UnitTests
{
    using System;

    public class NullTypeBuildExecuteStrategy : DefaultExecuteStrategy
    {
        protected override object Build(Type type, params object[] args)
        {
            return base.Build((Type)null, args);
        }
    }
}