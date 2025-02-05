using System;
using System.Collections.Generic;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.Rooms
{
    public class RoomDto
    {
        public string RoomNumber { get; set; }
        public string Type { get; set; }
        public int Capacity { get; set; }
        public List<string> AvailableEquipment { get; set; }
        public string CurrentStatus { get; set; }
        public List<SlotsDto> MaintenanceSlots { get; set; }

        public RoomDto(string roomNumber, string type, int capacity, List<string> availableEquipment, string currentStatus, List<SlotsDto> maintenanceSlots)
        {
            RoomNumber = roomNumber;
            Type = type;
            Capacity = capacity;
            AvailableEquipment = availableEquipment ?? new List<string>();
            CurrentStatus = currentStatus;
            MaintenanceSlots = maintenanceSlots ?? new List<SlotsDto>();
        }
    }
}
