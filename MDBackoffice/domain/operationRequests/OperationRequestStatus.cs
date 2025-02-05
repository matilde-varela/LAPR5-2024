using System;
using System.ComponentModel.DataAnnotations.Schema;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.OperationRequests
{
    public class OperationRequestStatus : IValueObject
    {

        public string Description { get; }
        [NotMapped]
        public static OperationRequestStatus Planned { get; } = new OperationRequestStatus("Planned");
        [NotMapped]
        public static OperationRequestStatus Requested { get; } = new OperationRequestStatus("Requested");

        private OperationRequestStatus()
        {
        }

        private OperationRequestStatus(string status)
        {
            this.Description = status;
        }
        public static OperationRequestStatus? GetStatusByDescription(string description)
        {
            if (Planned.Description.Equals(description))
            {
                return new OperationRequestStatus(Planned.Description);
            }
            else if (Requested.Description.Equals(description))
            {
                return new OperationRequestStatus(Requested.Description);
            }
            return null;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (OperationRequestStatus)obj;
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

    }
}