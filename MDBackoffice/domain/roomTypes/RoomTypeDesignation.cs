using System;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.RoomTypes
{
    public class RoomTypeDesignation : IValueObject
    {
        public string Designation { get; }
        private static int MAX_LENG_DENOMINATION = 100;
        public RoomTypeDesignation(string designation)
        {
            if (string.IsNullOrEmpty(designation))
            {
                throw new BusinessRuleValidationException("Room Type designation cannot be null or empty.");
            }
            if (designation.Length > MAX_LENG_DENOMINATION)
            {
                throw new BusinessRuleValidationException("Room Type designation cannot have more than 100 characters");
            }
            this.Designation = designation;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (RoomTypeDesignation)obj;
            return Designation == other.Designation;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Designation);
        }

        public override string ToString()
        {
            return Designation;
        }
        public bool Contains(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(value));
            }

            return Designation.Contains(value, StringComparison.OrdinalIgnoreCase);
        }
    }
}