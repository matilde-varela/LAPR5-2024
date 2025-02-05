using System;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.Rooms
{
    public class Capacity : IValueObject
    {
        public int CapcityNumber { get; private set; }

        public Capacity()
        {

        }

        public Capacity(int limit)
        {
            if (limit == null || limit == 0)
            {
                throw new BusinessRuleValidationException("The Room Capacity cannot be null or 0");
            }
            if (limit < 0)
            {
                throw new BusinessRuleValidationException("The Room Capacity cannot be negative");
            }
            this.CapcityNumber = limit;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (Capacity)obj;
            return CapcityNumber == other.CapcityNumber;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(CapcityNumber);
        }




    }
}