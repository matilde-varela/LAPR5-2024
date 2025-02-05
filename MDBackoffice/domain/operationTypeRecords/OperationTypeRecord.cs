using System;
using System.Collections.Generic;
using MDBackoffice.Domain.OperationTypes;
using MDBackoffice.Domain.OperationTypes.ValueObjects;
using MDBackoffice.Domain.OperationTypes.ValueObjects.Phase;
using MDBackoffice.Domain.OperationTypes.ValueObjects.RequiredStaff;
using MDBackoffice.Domain.Shared;


namespace MDBackoffice.Domain.OperationTypesRecords
{
    public class OperationTypeRecord : Entity<OperationTypeRecordId>, IAggregateRoot
    {

        public OperationTypeRecordVersion Version { get;}
        public Date EffectiveDate { get;}
        public OperationTypeParentId OperationTypeId { get;}
        public OperationTypeName Name { get;}
        public EstimatedDuration EstimatedDuration { get;}
        public OperationTypeStatus Status { get;}
        public List<RequiredStaffRecord> RequiredStaffRecords { get;}
        public List<PhaseRecord> Phases { get;}

        private static readonly int NUMBER_OF_OPERATION_PHASES = 3;

        public OperationTypeRecord()
        {
            //for ORM
        }


        public OperationTypeRecord(Date date, int previousVersionNumber, OperationTypeId operationTypeId, OperationTypeName name, EstimatedDuration duration, OperationTypeStatus status, List<RequiredStaff> staff, List<Phase> phases)
        {
            Id = new OperationTypeRecordId(Guid.NewGuid().ToString());
            EffectiveDate = date;
            Version = new OperationTypeRecordVersion(previousVersionNumber + 1);
            OperationTypeId = new OperationTypeParentId(operationTypeId.Value);
            Name = new OperationTypeName(name.OperationName);
            EstimatedDuration = new EstimatedDuration(duration.TotalDurationMinutes);
            Status = new OperationTypeStatus(status.Active);

            RequiredStaffRecords = new List<RequiredStaffRecord>();
            foreach (RequiredStaff requiredStaff in staff){
                RequiredStaffRecords.Add(new RequiredStaffRecord(requiredStaff.StaffQuantity.NumberRequired, requiredStaff.Function.Description, requiredStaff.SpecializationId.Value.ToString()));
            }

            if (phases.Count != NUMBER_OF_OPERATION_PHASES)
            {
                throw new BusinessRuleValidationException("An operation type must have" + NUMBER_OF_OPERATION_PHASES + "phases.");
            }

            Phases = new List<PhaseRecord>();
            foreach (Phase stage in phases)
            {
                Phases.Add(new PhaseRecord(stage.Description.Description, stage.Duration.DurationMinutes));
            }
        }


    }
}