using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MDBackoffice.Domain.Shared;
using MDBackoffice.Domain.StaffProfiles;

namespace MDBackoffice.Domain.AppointmentStaffs
{

    public interface IAppointmentStaffRepository : IRepository<AppointmentStaff, AppointmentStaffId>
    {
        public Task<bool> IsStaffAvailableAsync(StaffId staffId, string startTime, string endTime, Guid? excludedAppointmentId = null);
        public Task<List<AppointmentStaff>> GetAppointmentByAptId(string id);
    }
}