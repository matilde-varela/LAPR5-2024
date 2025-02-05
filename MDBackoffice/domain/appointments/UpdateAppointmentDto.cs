using System;
using System.Collections.Generic;

namespace MDBackoffice.Domain.Appointments
{
    public class UpdateAppointmentDto
    {

        public string AppointmentId { get; set; } 

        public string? NewRoomNumber { get; set; } 

        public string? NewStartTime { get; set; } 

        public string? NewEndTime { get; set; } 
        
        public string? NewStartDate { get; set; } 

        public string? NewEndDate { get; set; } 

        public List<string>? NewStaffList { get; set; }


        public UpdateAppointmentDto(string appointmentId, string? newRoomNumber, string? newStartTime, string? newEndTime, string? newStartDate, string? newEndDate, List<string>? newStaffList)
        {
            AppointmentId = appointmentId;
            NewRoomNumber = newRoomNumber;
            NewStartTime = newStartTime;
            NewEndTime = newEndTime;
            NewStartDate = newStartDate;
            NewEndDate = newEndDate;
            NewStaffList = newStaffList;
        }
        
    }
}