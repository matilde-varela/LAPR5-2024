using System;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.Specializations
{
    public class SpecializationDenomination : IValueObject
    {
        public string Denomination { get; }
        private static int MAX_LENG_DENOMINATION = 100;
        public SpecializationDenomination(string denomination)
        {
            if (string.IsNullOrEmpty(denomination))
            {
                throw new BusinessRuleValidationException("Specialization denomination cannot be null or empty.");
            }
            if (denomination.Length > MAX_LENG_DENOMINATION)
            {
                throw new BusinessRuleValidationException("Specialization denomination cannot have more than 100 characters");
            }
            this.Denomination = denomination;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (SpecializationDenomination)obj;
            return Denomination == other.Denomination;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Denomination);
        }

        public override string ToString()
        {
            return Denomination;
        }
        public bool Contains(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new ArgumentException("Value cannot be null or empty.", nameof(value));
            }

            return Denomination.Contains(value, StringComparison.OrdinalIgnoreCase);
        }
    }
}