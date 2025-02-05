using System;
using MDBackoffice.Domain.Shared;
using MDBackoffice.Domain.Patients;
using MDBackoffice.Domain.StaffProfiles;
using MDBackoffice.Domain.OperationTypes;

namespace MDBackoffice.Domain.OperationRequests
{
    public class ScheduleOperationRequestDto
    {
        public string Id { get; set; }

        public string OperationTypeId { get; set; }

        public int PrepTime { get; set; }

        public int SurgTime { get; set; }

        public int CleanTime { get; set; }


        public ScheduleOperationRequestDto(string Id, string operationType, int prepTime, int surgTime, int cleanTime)
        {
            this.Id = Id;
            this.OperationTypeId = operationType;
            this.PrepTime = prepTime;
            this.SurgTime = surgTime;
            this.CleanTime = cleanTime;
        }
    }
}