using System;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.OperationTypesRecords
{
    public class StaffSpecialization : IValueObject
    {

        public string SpeciId { get;  private set; }

        public StaffSpecialization(){
            // for ORM
        }

        public StaffSpecialization(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("Required Staff Record Specialization Id cannot be null or empty.");
            }
            this.SpeciId  = id;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (StaffSpecialization)obj;
            return SpeciId == other.SpeciId;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SpeciId);
        }

    }
}