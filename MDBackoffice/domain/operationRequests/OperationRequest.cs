using System;
using MDBackoffice.Domain.Shared;
using MDBackoffice.Domain.StaffProfiles;
using MDBackoffice.Domain.Patients;
using MDBackoffice.Domain.OperationTypes;

namespace MDBackoffice.Domain.OperationRequests
{
    public class OperationRequest : Entity<OperationRequestId>, IAggregateRoot
    {

        public Date DeadLineDate { get; private set; }

        public Priority Priority { get; private set; }

        public Date DateOfRequest { get; private set; }

        public OperationRequestStatus Status { get; private set; }

        public StaffId StaffId { get; private set; }

        public OperationRequestDescription Description { get; private set; }

        public MedicalRecordNumber PatientId { get; private set; }

        public OperationTypeId OperationTypeId { get; private set; }

        public OperationRequest()
        {

        }

        public OperationRequest(string code, string deadLineDate, string priority, string dateOfRequest, string staffId, string description, string patientId, string operationTypeId)
        {
            this.Id = new OperationRequestId(code);
            if (!DateTime.TryParse(deadLineDate, out DateTime deadLineDateVar) || !DateTime.TryParse(dateOfRequest, out DateTime dateOfRequestVar) || dateOfRequestVar > deadLineDateVar)
            {
                throw new BusinessRuleValidationException("The date of request can't be after deadline date.");
            }
            this.DeadLineDate = new Date(deadLineDate);
            this.DateOfRequest = new Date(dateOfRequest);
            this.Priority = Priority.GetPriorityByName(priority);
            this.Status = OperationRequestStatus.Requested;
            this.StaffId = new StaffId(staffId);
            this.Description = new OperationRequestDescription(description);
            this.PatientId = new MedicalRecordNumber(patientId);
            this.OperationTypeId = new OperationTypeId(operationTypeId);
        }

        public OperationRequest(Date deadLineDate, Priority priority, Date dateOfRequest, StaffId staffId, string description, MedicalRecordNumber patientId, OperationTypeId operationTypeId)
        {
            this.Id = new OperationRequestId(Guid.NewGuid());
            if (dateOfRequest.Start > deadLineDate.Start)
            {
                throw new BusinessRuleValidationException("The date of request can't be after deadline date.");
            }
            this.DeadLineDate = deadLineDate;
            this.DateOfRequest = dateOfRequest;
            this.Priority = priority;
            this.Status = OperationRequestStatus.Requested;
            this.StaffId = staffId;
            this.Description = new OperationRequestDescription(description);
            this.PatientId = patientId;
            this.OperationTypeId = operationTypeId;
        }

        public void ChangeDeadLineDate(string deadLineDate)
        {
            this.DeadLineDate = new Date(deadLineDate);
        }

        public void ChangePriority(string priority)
        {
            this.Priority = Priority.GetPriorityByName(priority);
        }

        public void ChangeDateOfRequest(string dateOfRequest)
        {
            this.DateOfRequest = new Date(dateOfRequest);
        }

        public void ChangeStatus(string status)
        {
            this.Status = OperationRequestStatus.GetStatusByDescription(status);
        }

        public void ChangeDescription(string text)
        {
            this.Description = new OperationRequestDescription(text);
        }
    }
}