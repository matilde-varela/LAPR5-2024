using System;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.RoomTypes
{
    public class RoomTypeDescription : IValueObject
    {
        public string Description { get; }
        private static int MAX_LENG_DESCRIPTION = 2048;
        public RoomTypeDescription(string description)
        {
            if (string.IsNullOrEmpty(description))
            {
                throw new BusinessRuleValidationException("Room Type description cannot be null or empty.");
            }
            if (description.Length > MAX_LENG_DESCRIPTION)
            {
                throw new BusinessRuleValidationException("Room Type description cannot have more than 2048 characters");
            }
            this.Description = description;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (RoomTypeDescription)obj;
            return Description == other.Description;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Description);
        }

        public override string ToString()
        {
            return Description;
        }
        public bool Contains(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(value));
            }

            return Description.Contains(value, StringComparison.OrdinalIgnoreCase);
        }
    }
}