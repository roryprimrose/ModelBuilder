using System;

namespace ModelBuilder.UnitTests
{
    public class NullTypeBuildExecuteStrategy<T> : DefaultExecuteStrategy<T>
    {
        protected override object Build(Type type, string referenceName, object context, params object[] args)
        {
            return base.Build(null, referenceName, context, args);
        }
    }
}