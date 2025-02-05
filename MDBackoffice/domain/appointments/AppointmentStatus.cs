using System;
using System.ComponentModel.DataAnnotations.Schema;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.Appointments
{
    public class AppointmentStatus : IValueObject
    {
        public string Description { get; }
        [NotMapped]
        public static AppointmentStatus Scheduled { get; } = new AppointmentStatus("Scheduled");
        [NotMapped]
        public static AppointmentStatus Completed { get; } = new AppointmentStatus("Completed");
        [NotMapped]
        public static AppointmentStatus Canceled { get; } = new AppointmentStatus("Canceled");

        public AppointmentStatus()
        {

        }

        public AppointmentStatus(string status)
        {
            this.Description = status;
        }

        public static AppointmentStatus? GetStatusByDescription(string description)
        {
            if (Scheduled.Description.Equals(description))
            {
                return new AppointmentStatus(Scheduled.Description);
            }
            else if (Completed.Description.Equals(description))
            {
                return new AppointmentStatus(Completed.Description);
            }
            else if (Canceled.Description.Equals(description))
            {
                return new AppointmentStatus(Canceled.Description);
            }
            return null;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (AppointmentStatus)obj;
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