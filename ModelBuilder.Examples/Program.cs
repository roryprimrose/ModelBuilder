namespace ModelBuilder.Examples
{
    using ModelBuilder.Examples.Examples;

    /// <summary>
    ///     The <see cref="Program" /> class
    ///     runs every example in turn so the output of each API scenario can be seen end to end. Each
    ///     example is a self-contained static class under the <c>Examples</c> folder.
    /// </summary>
    internal static class Program
    {
        private static void Main()
        {
            CreatingModelsExample.Run();
            TypedConstructionExample.Run();
            IgnoringMembersExample.Run();
            MappingTypesExample.Run();
            CustomValueSourcesExample.Run();
            PopulatingModelsExample.Run();
            ChangingAfterCreationExample.Run();
            LoggingBuildsExample.Run();
            HandlingFailuresExample.Run();
            ConfigurationModulesExample.Run();
            TuningTheBuildExample.Run();
            BuiltInDataExample.Run();
        }
    }
}
