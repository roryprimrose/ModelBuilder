namespace ModelBuilder.UnitTests.Models
{
    using System;

    public class PropertySetters
    {
        internal float _backingField;
        public float BackingFieldMethod() => _backingField;

        public ConsoleColor AutoInit { get; init; }

        public DateTimeOffset AutoInternal { get; internal set; }

        public int AutoPrivate { get; }

        public PropertySetters? AutoPrivateInternal { get; private protected set; }

        public decimal AutoProtected { get; protected set; }

        public Uri? AutoProtectedInternal { get; protected internal set; }
        public Guid AutoPublic { get; set; }

        public string? AutoReadonly { get; } = string.Empty;
        public float BackingField { get => _backingField; set => _backingField = value; }

        public float PrivateBackingField { get => _backingField; private set => _backingField = value; }

        public float PublicBackingField { get => _backingField; set => _backingField = value; }

        public char? Readonly => default;
    }
}