using System.Collections.Generic;
using System.Threading.Tasks;
using MDBackoffice.Domain.Appointments;
using MDBackoffice.Domain.OperationRequests;
using MDBackoffice.Domain.Rooms;
using MDBackoffice.Domain.StaffProfiles;

namespace MDBackoffice.Domain.OperationRequests
{
    public interface IOperationSchedulerAdapter
    {
         Task<List<SchedulesDto>> ScheduleOperationsAsync(
            Dictionary<ScheduleOperationRequestDto, List<ScheduleStaffDto>> operationsMap,
            List<RoomDto> room,
            string day,
            string algorithm);
    }
}