namespace ModelBuilder.UnitTests.Models
{
    using System;

    public class PropertySetters
    {
        public Guid AutoPublic { get; set; }

        public string? AutoReadonly { get; } = string.Empty;

        public int AutoPrivate { get; private set; }

        public decimal AutoProtected { get; protected set; }

        public Uri? AutoProtectedInternal { get; protected internal set; }

        public DateTimeOffset AutoInternal { get; internal set; }

        public PropertySetters? AutoPrivateInternal { get; private protected set; }

        public ConsoleColor AutoInit { get; init; }

        internal float _backingField;
        public float PublicBackingField { get => _backingField; set => _backingField = value; }

        public float PrivateBackingField { get => _backingField; private set => _backingField = value; }

        public char? Readonly => default;
    }
}