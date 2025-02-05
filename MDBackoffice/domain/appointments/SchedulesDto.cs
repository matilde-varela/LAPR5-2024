using System.Collections.Generic;
using MDBackoffice.Domain.Rooms;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.Appointments
{
    public class SchedulesDto
    {
        public string Date { get; set; }

        public string RoomId { get; set; }

        public Dictionary<string, List<SlotsDto>> RoomSchedule { get; set; }

        public Dictionary<string, List<SlotsDto>> StaffSchedule { get; set; }


        public SchedulesDto(string date, string room, Dictionary<string, List<SlotsDto>> roomSche, Dictionary<string, List<SlotsDto>> staffSche)
        {
            Date = date;
            RoomId = room;
            RoomSchedule = roomSche;
            StaffSchedule = staffSche;

        }
    }
}