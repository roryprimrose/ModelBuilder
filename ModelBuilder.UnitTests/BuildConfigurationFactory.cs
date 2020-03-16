namespace ModelBuilder.UnitTests
{
    public static class BuildConfigurationFactory
    {
        public static IBuildConfiguration CreateEmpty()
        {
            return new BuildConfiguration
            {
                ConstructorResolver = new DefaultConstructorResolver(),
                PropertyResolver = new DefaultPropertyResolver()
            };
        }
    }
}