using System;
using MDBackoffice.Domain.Shared;
using MDBackoffice.Domain.Patients;
using MDBackoffice.Domain.StaffProfiles;
using MDBackoffice.Domain.OperationTypes;
using System.Collections.Generic;

namespace MDBackoffice.Domain.OperationRequests
{
    public class OperationRequestScheduleInfoDto
    {
        public List<string> RoomID { get; set; }
        public List<StaffForRequestEntry> SchedulingData { get; set; }

        public string Algorithm { get; set; }

        public string Date { get; set; }

        public OperationRequestScheduleInfoDto() { }
        public OperationRequestScheduleInfoDto(
            List<StaffForRequestEntry> staffForRequest,
            List<string> selectedRoomId,
            string algorithm,
            string day)
        {
            this.SchedulingData = staffForRequest;
            this.RoomID = selectedRoomId;
            this.Algorithm = algorithm;
            this.Date = day;
        }
    }

    public class StaffForRequestEntry
    {
        public List<StaffDto> Staff { get; set; }
        public string OperationRequestID { get; set; }

        public StaffForRequestEntry() { }
        public StaffForRequestEntry(List<StaffDto> staffDtos, string op)
        {
            Staff = staffDtos;
            OperationRequestID = op;
        }
    }

}
