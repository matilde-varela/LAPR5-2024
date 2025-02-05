using System;
using System.Collections.Generic;

namespace MDBackoffice.Domain.Appointments
{
    public class AppointmentDto
    {
        public Guid Id { get; set; }

        public string Status { get; set; }

        public string OperationRequestId { get; set; } 

        public string RoomNumber { get; set; } 

        public string StartTime { get; set; } 

        public string EndTime { get; set; } 
        
        public string StartDate { get; set; } 

        public string EndDate { get; set; } 

        public List<string> Staffs { get; set; }


        public AppointmentDto(Guid id, string status, string operationRequestId, string roomNumber, string startTime, string endTime, string startDate, string endDate, List<string> staffList)
        {
            Id = id;
            Status = status;
            OperationRequestId = operationRequestId;
            RoomNumber = roomNumber;
            StartTime = startTime;
            EndTime = endTime;
            StartDate = startDate;
            EndDate = endDate;
            Staffs = staffList;
        }
        
    }
}