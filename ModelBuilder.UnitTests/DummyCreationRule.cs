namespace ModelBuilder.UnitTests
{
    public class DummyCreationRule : CreationRule
    {
        public DummyCreationRule() : base(typeof(string), "SomeRandomPropertyWhichDoesNotExist", 10, (object)null)
        {
        }
    }
}