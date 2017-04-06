namespace ModelBuilder.UnitTests
{
    public class TestCompilerModule : ICompilerModule
    {
        public void Configure(IBuildStrategyCompiler compiler)
        {
            compiler.Add(new DummyExecuteOrderRule());
            compiler.Add(new DummyIgnoreRule());
            compiler.Add(new DummyPostBuildAction());
            compiler.Add(new DummyCreationRule());
            compiler.Add(new DummyTypeCreator());
            compiler.Add(new DummyValueGenerator());
        }
    }
}