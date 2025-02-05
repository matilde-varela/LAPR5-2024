

using System;
using System.Collections.Generic;

namespace MDBackoffice.Domain.Appointments
{
    public class CreatingAppointmentDto
    {

        public string OperationRequestId { get; set; } 

        public string RoomNumber { get; set; } 

        public string StartTime { get; set; } 

        public string EndTime { get; set; } 
        
        public string StartDate { get; set; } 

        public string EndDate { get; set; } 

        public List<string> StaffList { get; set; }


        public CreatingAppointmentDto(string operationRequestId, string roomNumber, string startTime, string endTime, string startDate, string endDate, List<string> staffList)
        {
            OperationRequestId = operationRequestId;
            RoomNumber = roomNumber;
            StartTime = startTime;
            EndTime = endTime;
            StartDate = startDate;
            EndDate = endDate;
            StaffList = staffList;
        }
        
    }
}