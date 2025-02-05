using System;
using System.Collections.Generic;
using MDBackoffice.Domain.OperationTypes.ValueObjects;
using MDBackoffice.Domain.OperationTypes.ValueObjects.Phase;
using MDBackoffice.Domain.OperationTypes.ValueObjects.RequiredStaff;
using MDBackoffice.Domain.OperationTypesRecords;
using MDBackoffice.Domain.Shared;

namespace MDBackoffice.Domain.OperationTypes
{
    public class OperationType : Entity<OperationTypeId>, IAggregateRoot
    {
        public EstimatedDuration EstimatedDuration { get; private set; }
        public OperationTypeStatus Status { get; private set; }
        public OperationTypeName Name { get; private set; }
        public List<RequiredStaff> RequiredStaff { get; private set; }

        private static readonly int NUMBER_OF_OPERATION_PHASES = 3;

        public List<Phase> Phases { get; private set; }

        public OperationType()
        {
            //for ORM

            RequiredStaff = new List<RequiredStaff>();
            Phases = new List<Phase>();
        }


        public OperationType(string name, int duration, bool status, List<RequiredStaffDto> staff, List<PhaseDto> phases)
        {
            this.Id = new OperationTypeId(name);
            this.EstimatedDuration = new EstimatedDuration(duration);
            this.Status = new OperationTypeStatus(status);
            this.Name = new OperationTypeName(name);

            RequiredStaff = new List<RequiredStaff>();

            foreach (RequiredStaffDto person in staff)
            {
                RequiredStaff.Add(new RequiredStaff(person.StaffQuantity, person.Function, person.Specialization));
            }

            if (phases.Count != NUMBER_OF_OPERATION_PHASES)
            {
                throw new BusinessRuleValidationException("An operation type must have" + NUMBER_OF_OPERATION_PHASES + "phases.");
            }

            Phases = new List<Phase>();
            foreach (PhaseDto stage in phases)
            {
                Phases.Add(new Phase(stage.Description, stage.Duration));
            }
        }

        public virtual void ChangeName(string name)
        {
            if (!this.Status.Active)
                throw new BusinessRuleValidationException("It is not possible to change the name to an inactive Operation Type.");
            this.Name = new OperationTypeName(name);
        }

        public virtual void ChangeEstimatedDuration(int duration)
        {
            if (!this.Status.Active)
                throw new BusinessRuleValidationException("It is not possible to change the estimated duration to an inactive Operation Type.");
            this.EstimatedDuration = new EstimatedDuration(duration);
        }

        public void ChangeStatus(bool status)
        {
            this.Status = new OperationTypeStatus(status);
        }

        public void Inactive()
        {
            this.Status = new OperationTypeStatus(false);
        }
        public void ChangeRequiredStaff(List<RequiredStaffDto> newStaff)
        {
            if (!this.Status.Active)
                throw new BusinessRuleValidationException("It is not possible to change the required staff for an inactive Operation Type.");

            // Add new staff with the correct OperationTypeId
            foreach (RequiredStaffDto staffDto in newStaff)
            {
                var existingStaff = this.RequiredStaff.Find(rs => rs.SpecializationId.Value == staffDto.Specialization);

                if (existingStaff != null)
                {
                    existingStaff.ChangeStaffQuantity(staffDto.StaffQuantity);
                    existingStaff.ChangeFunction(staffDto.Function);
                    existingStaff.ChangeSpecialization(staffDto.Specialization);
                }
                else
                {
                    RequiredStaff.Add(new RequiredStaff(staffDto.StaffQuantity, staffDto.Function, staffDto.Specialization));
                }
            }
        }

        /* public void ChangeRequiredStaff(List<RequiredStaffDto> newStaff)
        {
            if (!this.Status.Active)
                throw new BusinessRuleValidationException("It is not possible to change the required staff for an inactive Operation Type.");

            // Clear the current collection
            this.RequiredStaff.Clear();

            foreach (RequiredStaffDto staffDto in newStaff)
            {
                var newStaff1 = new RequiredStaff(staffDto.StaffQuantity, staffDto.Function, staffDto.Specialization);
                newStaff1.OperationTypeId = this.Id;
                this.RequiredStaff.Add(newStaff1);
            }
        } */


        public void ChangePhases(List<PhaseDto> newPhases)
        {
            if (!this.Status.Active)
                throw new BusinessRuleValidationException("It is not possible to change the phases for an inactive Operation Type.");

            if (newPhases.Count != NUMBER_OF_OPERATION_PHASES)
            {
                throw new BusinessRuleValidationException("An operation type must have " + NUMBER_OF_OPERATION_PHASES + " phases.");
            }

            Phases = new List<Phase>();

            foreach (PhaseDto phaseDto in newPhases)
            {
                this.Phases.Add(new Phase(phaseDto.Description, phaseDto.Duration));
            }
        }


    }
}