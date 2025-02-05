using System;
using MDBackoffice.Domain.Appointments;
using MDBackoffice.Domain.Shared;
using MDBackoffice.Domain.StaffProfiles;

namespace MDBackoffice.Domain.AppointmentStaffs
{

     public class AppointmentStaff : Entity<AppointmentStaffId>, IAggregateRoot
    {
        public Appointment Appointment { get; private set; }
        public Staff Staff { get; private set; }

        private AppointmentStaff() { }

        public AppointmentStaff(Appointment appointment, Staff staff)
        {
            this.Id = new AppointmentStaffId(appointment.Id, staff.Id);
            this.Appointment = appointment;

            this.Staff = staff;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var other = (AppointmentStaff)obj;
            return Id.Equals(other.Id);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}