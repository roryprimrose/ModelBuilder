namespace ModelBuilder.UnitTests
{
    using Xunit.Abstractions;

    public class OutputBuildLog : DefaultBuildLog
    {
        private readonly ITestOutputHelper _output;

        public OutputBuildLog(ITestOutputHelper output)
        {
            _output = output;
        }

        protected override void WriteMessage(string message)
        {
            _output.WriteLine(message);
        }
    }
}