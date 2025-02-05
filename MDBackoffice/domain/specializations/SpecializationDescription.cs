using System;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.Specializations
{
    public class SpecializationDescription : IValueObject
    {
        public string Description { get; }
        private static int MAX_LENG_DESCRIPTION = 2048;
        public SpecializationDescription(string description)
        {
            if (string.IsNullOrEmpty(description))
            {
                throw new BusinessRuleValidationException("Specialization description cannot be null or empty.");
            }
            if (description.Length > MAX_LENG_DESCRIPTION)
            {
                throw new BusinessRuleValidationException("Specialization description cannot have more than 2048 characters");
            }
            this.Description = description;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (SpecializationDescription)obj;
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