using System;
using System.Collections.Generic;

namespace MDBackoffice.Domain.Appointments
{
    public class AppointmentRoomInfoDto
    {

        public string RoomNumber { get; set; } 

        public string StartTime { get; set; } 

        public string EndTime { get; set; } 
        
        public string StartDate { get; set; } 

        public string EndDate { get; set; } 



        public AppointmentRoomInfoDto(string roomNumber, string startTime, string endTime, string startDate, string endDate)
        {
            RoomNumber = roomNumber;
            StartTime = startTime;
            EndTime = endTime;
            StartDate = startDate;
            EndDate = endDate;
        }
        
    }
}