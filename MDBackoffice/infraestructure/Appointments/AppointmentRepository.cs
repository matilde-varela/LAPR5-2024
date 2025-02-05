using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MDBackoffice.Domain.Appointments;
using MDBackoffice.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;


namespace MDBackoffice.Infrastructure.Appointments
{
    public class AppointmentRepository : BaseRepository<Appointment, AppointmentId>, IAppointmentRepository
    {
        private readonly MDBackofficeDbContext _context;
        public AppointmentRepository(MDBackofficeDbContext context):base(context.Appointments){
            _context = context;
        }

       public async Task<Appointment> GetAppointmentByIdWithStaff(string appointmentId)
        {
            var appointment = await _context.Appointments
                .Include(p => p.AppointmentStaffs)
                .Where(p => p.Id == new AppointmentId(appointmentId))  // Convert the value to string and compare
                .FirstOrDefaultAsync();

            return appointment;
        }

        public async Task<List<Appointment>> GetAppointmentsWithStaff()
        {
            var appointment =
                _context.Appointments
                .Include(p => p.AppointmentStaffs);

            return appointment.ToList();
        }
    }
}
