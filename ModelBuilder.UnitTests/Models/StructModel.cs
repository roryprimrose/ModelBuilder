namespace ModelBuilder.UnitTests.Models
{
    using System;

    public struct StructModel : IEquatable<StructModel>
    {
        public bool Equals(StructModel other)
        {
            return Id.Equals(other.Id) && FirstName == other.FirstName && LastName == other.LastName
                   && Email == other.Email;
        }

        public override bool Equals(object? obj)
        {
            return obj is StructModel other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id.GetHashCode();
                hashCode = (hashCode * 397)
                           ^ (FirstName != null ? FirstName.GetHashCode(StringComparison.CurrentCulture) : 0);
                hashCode = (hashCode * 397)
                           ^ (LastName != null ? LastName.GetHashCode(StringComparison.CurrentCulture) : 0);
                hashCode = (hashCode * 397) ^ (Email != null ? Email.GetHashCode(StringComparison.CurrentCulture) : 0);
                return hashCode;
            }
        }

        public static bool operator ==(StructModel left, StructModel right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(StructModel left, StructModel right)
        {
            return !left.Equals(right);
        }

        public Guid Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? Email { get; set; }
    }
}