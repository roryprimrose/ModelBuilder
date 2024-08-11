namespace ModelBuilder.UnitTests.Models
{
    internal class SelfReferenceInstance
    {
        private SelfReferenceInstance? _value;

        public SelfReferenceInstance Instance { get => _value ?? this; set => _value = value; }
    }
}