namespace ModelBuilder.UnitTests
{
    using System;

    public class DummyExecuteStrategy : IExecuteStrategy<string>
    {
        public string Create(params object?[]? args)
        {
            throw new NotImplementedException();
        }

#if NETSTANDARD2_1
        [return: System.Diagnostics.CodeAnalysis.MaybeNull]
#endif
        public object Create(Type type, params object?[]? args)
        {
            throw new NotImplementedException();
        }

        public void Initialize(IBuildConfiguration configuration)
        {
        }

        public string Populate(string instance)
        {
            throw new NotImplementedException();
        }

        public object Populate(object instance)
        {
            throw new NotImplementedException();
        }

        public IBuildChain BuildChain { get; } = new BuildHistory();

        public IBuildConfiguration Configuration { get; } = new BuildConfiguration();

        public IBuildLog Log { get; } = new DefaultBuildLog();
    }
}