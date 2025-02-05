using System;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.OperationTypes.ValueObjects.Phase
{
    public class Phase : IValueObject
    {
        public OperationTypeId OperationTypeId { get; set; }

        public string PhaseId {get; set;}

        public PhaseDescription Description { get;  private set; }

        public PhaseDuration Duration { get;  private set; }


        public Phase(){
            // for ORM
        }


        public Phase(string description, int durationMinutes)
        {
            this.Description = new PhaseDescription(description);
            this.Duration = new PhaseDuration(durationMinutes);
            this.PhaseId = RandomSequenceGenerator.GenerateUniqueRandomSequence(5);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (Phase)obj;
            return Description == other.Description && Duration == other.Duration;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Description, Duration);
        }
    }
}