namespace ModelBuilder.Generator
{
    using System;

    /// <summary>
    ///     The <see cref="ConstructorModel" /> struct
    ///     describes a single public constructor of a buildable type: its parameters and the rendered
    ///     documentation lines copied from the constructor's xmldoc.
    /// </summary>
    internal readonly struct ConstructorModel : IEquatable<ConstructorModel>
    {
        public ConstructorModel(EquatableArray<MemberModel> parameters, EquatableArray<string> docLines)
        {
            Parameters = parameters;
            DocLines = docLines;
        }

        public bool Equals(ConstructorModel other)
        {
            return Parameters.Equals(other.Parameters)
                   && DocLines.Equals(other.DocLines);
        }

        public override bool Equals(object? obj)
        {
            return obj is ConstructorModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return Parameters.GetHashCode() * 397 ^ DocLines.GetHashCode();
            }
        }

        public EquatableArray<string> DocLines { get; }

        public EquatableArray<MemberModel> Parameters { get; }
    }
}
