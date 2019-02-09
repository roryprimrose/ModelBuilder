namespace ModelBuilder.UnitTests
{
    using System;

    public class NullTypeBuildExecuteStrategy : DefaultExecuteStrategy
    {
        protected override object Build(Type type, string referenceName, object context, params object[] args)
        {
            return base.Build(null, referenceName, context, args);
        }
    }
}