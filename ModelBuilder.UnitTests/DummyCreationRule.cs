namespace ModelBuilder.UnitTests
{
    public class DummyCreationRule : CreationRule
    {
        public DummyCreationRule() : base(typeof(string), "Stuff", 10, (object)null)
        {
        }
    }
}