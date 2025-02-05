using System;
using System.Collections.Generic;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.Rooms
{
    public class CreateRoomDto
    {
        public string RoomNumber { get; set; }
        public string TypeDesignation { get; set; }
        public int Capacity { get; set; }
        public List<string>? AvailableEquipment { get; set; }
        public List<string>? MaintenanceSlots { get; set; }

        public CreateRoomDto(string roomNumber, string type, int capacity, List<string> availableEquipment, List<string> maintenanceSlots)
        {
            RoomNumber = roomNumber;
            TypeDesignation = type;
            Capacity = capacity;
            AvailableEquipment = availableEquipment ?? new List<string>();
            MaintenanceSlots = maintenanceSlots ?? new List<string>();
        }
    }
}
