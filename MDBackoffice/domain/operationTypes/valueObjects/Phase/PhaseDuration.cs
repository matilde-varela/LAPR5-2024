using System;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.OperationTypes.ValueObjects.Phase
{
    public class PhaseDuration : IValueObject
    {

        public int DurationMinutes { get;  private set; }

        public PhaseDuration(){
            // for ORM
        }

        public PhaseDuration(int durationMinutes)
        {
            if (durationMinutes <= 0)
            {
                throw new BusinessRuleValidationException("The duration in minutes of an operation type's phase cannot be negative.");
            }
            this.DurationMinutes = durationMinutes;
        }

         public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (PhaseDuration)obj;
            return DurationMinutes == other.DurationMinutes;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(DurationMinutes);
        }
    }
}