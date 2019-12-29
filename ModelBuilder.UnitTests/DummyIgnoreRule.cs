namespace ModelBuilder.UnitTests
{
    public class DummyIgnoreRule : IgnoreRule
    {
        public DummyIgnoreRule() : base(typeof(string), "SomeRandomPropertyWhichDoesNotExist")
        {
        }
    }
}