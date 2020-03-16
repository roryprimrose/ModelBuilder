namespace ModelBuilder.UnitTests
{
    public class TestConfigurationModule : IConfigurationModule
    {
        public void Configure(IBuildConfiguration configuration)
        {
            configuration.Add(new DummyExecuteOrderRule());
            configuration.Add(new DummyIgnoreRule());
            configuration.Add(new DummyPostBuildAction());
            configuration.Add(new DummyCreationRule());
            configuration.Add(new DummyTypeCreator());
            configuration.Add(new DummyValueGenerator());
        }
    }
}