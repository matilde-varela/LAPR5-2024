using System;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.OperationTypes.ValueObjects.RequiredStaff
{
    public class NumberStaff : IValueObject
    {

        public int NumberRequired { get;  private set; }

        public NumberStaff(){
            // for ORM
        }

        public NumberStaff(int staffRequired)
        {
            if (staffRequired == null)
            {
                throw new BusinessRuleValidationException("The number of required staff of the operation type cannot be null.");
            }
            if (staffRequired <= 0)
            {
                throw new BusinessRuleValidationException("The number of required staff of the operation type cannot be negative or 0.");
            }
            this.NumberRequired = staffRequired;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (NumberStaff)obj;
            return NumberRequired == other.NumberRequired;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(NumberRequired);
        }


    }
}