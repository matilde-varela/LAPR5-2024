using System;
using System.ComponentModel.DataAnnotations.Schema;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.Rooms
{
    public class CurrentStatus : IValueObject
    {
        public string Description { get; }
        [NotMapped]
        public static CurrentStatus Available { get; } = new CurrentStatus("Available");
        [NotMapped]
        public static CurrentStatus Occupied { get; } = new CurrentStatus("Occupied");
        [NotMapped]
        public static CurrentStatus UnderMaintenance { get; } = new CurrentStatus("Under Maintenance");

        private CurrentStatus()
        {

        }

        public CurrentStatus(string status)
        {
            this.Description = status;
        }

        public static CurrentStatus? GetStatusByDescription(string description)
        {
            if(Available.Description.Equals(description))
            {
                return new CurrentStatus(Available.Description);
            } 
            else if(Occupied.Description.Equals(description))
            {
                return new CurrentStatus(Occupied.Description);
            } 
            else if(UnderMaintenance.Description.Equals(description))
            {
                return new CurrentStatus(UnderMaintenance.Description);
            } 
            return null;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (CurrentStatus) obj;
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