using System.Collections.Generic;
using System.Threading.Tasks;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.Appointments
{
    public interface IAppointmentRepository : IRepository<Appointment, AppointmentId>
    {
        public Task<Appointment> GetAppointmentByIdWithStaff(string appointmentId);
        public Task<List<Appointment>> GetAppointmentsWithStaff();
    }
}