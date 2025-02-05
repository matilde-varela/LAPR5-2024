using MDBackoffice.Domain.Patients;
using MDBackoffice.Domain.StaffProfiles;
using MDBackoffice.Domain.OperationTypes;

namespace MDBackoffice.Domain.OperationRequests
{
    public class CreatingOperationRequestDto
    {
        public string DeadLineDate { get; set; }

        public string Priority { get; set; }

        public string DateOfRequest { get; set; }

        public string Status { get; set; }

        public string StaffId { get; set; } 
        public string Description { get; set; } 

        public string PatientId { get; set; } 

        public string OperationTypeId { get; set; } 


        public CreatingOperationRequestDto(string deadLineDate, string priority, string dateOfRequest, string status, string staffId, string description, string patientId, string operationTypeId)
        {
            this.DeadLineDate = deadLineDate;
            this.Priority = priority;
            this.DateOfRequest = dateOfRequest;
            this.Status = status;
            this.StaffId = staffId;
            this.Description = description;
            this.PatientId = patientId;
            this.OperationTypeId = operationTypeId;  
        }
    }
}