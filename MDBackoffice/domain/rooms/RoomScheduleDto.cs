using System.Collections.Generic;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.Rooms
{
    public class RoomScheduleDto
    {
        public string RoomNumber { get; set; }
        public List<SlotsDto> Schedule { get; set; }

        public RoomScheduleDto(string roomID, List<SlotsDto> scheduledOperations)
        {
            RoomNumber = roomID;
            Schedule = scheduledOperations ?? [];
        }
    }

}
