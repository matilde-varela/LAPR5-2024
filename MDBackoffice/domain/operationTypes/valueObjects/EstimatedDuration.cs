using System;
using MDBackoffice.Domain.Shared;
using Newtonsoft.Json.Linq;

namespace MDBackoffice.Domain.OperationTypes.ValueObjects
{
    public class EstimatedDuration : IValueObject
    {

        public int TotalDurationMinutes { get;  private set; }

        public EstimatedDuration(){
            //for ORM
        }

        public EstimatedDuration(int minutes)
        {
            if (minutes == null || minutes == 0)
            {
                throw new BusinessRuleValidationException("Estimated duration cannot be null or 0.");
            }
            if (minutes < 0)
            {
                throw new BusinessRuleValidationException("The estimated duration of the operation type cannot be negative.");
            }
            this.TotalDurationMinutes = minutes;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (EstimatedDuration)obj;
            return TotalDurationMinutes == other.TotalDurationMinutes;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(TotalDurationMinutes);
        }

    }
}