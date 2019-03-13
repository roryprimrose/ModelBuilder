namespace ModelBuilder.UnitTests
{
    using System.IO;

    public class DummyTypeMappingRule : TypeMappingRule
    {
        public DummyTypeMappingRule()
            : base(typeof(Stream), typeof(MemoryStream))
        {
        }
    }
}