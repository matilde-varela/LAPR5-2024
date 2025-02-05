using System.Collections.Generic;
using MDBackoffice.Domain.OperationTypes.ValueObjects.Phase;
using MDBackoffice.Domain.OperationTypes.ValueObjects.RequiredStaff;

namespace MDBackoffice.Domain.OperationTypesRecords
{
    public class OperationTypeRecordDto
    {
        public string Id { get; set; }
        public int VersionNumber { get; set; }
        public string EffectiveDate { get; set; }
        public string OperationTypeId { get; set; }
        public string Name { get; set; }
        public int EstimatedDuration { get; set; }
        public bool Status { get; set; }
        public List<RequiredStaffDto> RequiredStaffRecords { get; set; }
        public List<PhaseDto> Phases { get; set; }

        public OperationTypeRecordDto(string id, int versionNumber, string effectiveDate, string operationTypeId,
            string name, int estimatedDuration, bool status,
            List<RequiredStaffDto> requiredStaffRecords, List<PhaseDto> phases)
        {
            Id = id;
            VersionNumber = versionNumber;
            EffectiveDate = effectiveDate;
            OperationTypeId = operationTypeId;
            Name = name;
            EstimatedDuration = estimatedDuration;
            Status = status;
            RequiredStaffRecords = requiredStaffRecords;
            Phases = phases;
        }

    }
}
