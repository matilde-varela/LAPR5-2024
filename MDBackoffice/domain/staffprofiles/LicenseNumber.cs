using System;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.StaffProfiles
{
    public class LicenseNumber : IValueObject
    {
        public string Number { get; }
        public LicenseNumber(string number)
        {
            if (string.IsNullOrEmpty(number))
            {
                throw new BusinessRuleValidationException("License number cannot be null or empty");
            }
            Number = number;
        }
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (LicenseNumber)obj;
            return Number == other.Number;
        }

        public override int GetHashCode()
        {
            return  Number.GetHashCode();
        }

    }
}