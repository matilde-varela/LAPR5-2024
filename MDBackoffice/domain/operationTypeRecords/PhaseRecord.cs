using System;
using MDBackoffice.Domain.OperationTypes.ValueObjects.Phase;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.OperationTypesRecords
{
    public class PhaseRecord : IValueObject
    {
        public OperationTypeRecordId OperationTypeRecordId { get; set; }

        public string PhaseId {get; set;}
        
        public PhaseDescription Description { get;  private set; }

        public PhaseDuration Duration { get;  private set; }


        public PhaseRecord(){
            // for ORM
        }


        public PhaseRecord(string description, int durationMinutes)
        {
            this.PhaseId = RandomSequenceGenerator.GenerateUniqueRandomSequence(5);
            this.Description = new PhaseDescription(description);
            this.Duration = new PhaseDuration(durationMinutes);
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