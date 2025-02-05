using System.Threading.Tasks;
using System.Collections.Generic;
using MDBackoffice.Domain.Shared;
using MDBackoffice.Domain.OperationTypes.ValueObjects.RequiredStaff;
using MDBackoffice.Domain.OperationTypes.ValueObjects.Phase;
using System;
using MDBackoffice.Domain.Logs;
using MDBackoffice.Domain.OperationTypesRecords;

namespace MDBackoffice.Domain.OperationTypes
{
    public class OperationTypeRecordService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IOperationTypeRecordRepository _repo;

        private readonly LogService _logService;

        public OperationTypeRecordService(IUnitOfWork unitOfWork, LogService logService, IOperationTypeRecordRepository operationTypeRecordRepository)
        {
            _unitOfWork = unitOfWork;
            _logService = logService;
            _repo = operationTypeRecordRepository;
        }

        public virtual async Task<OperationTypeRecordDto> AddAsync(OperationType operationType)
        {
            var operationRecord1 = await _repo.GetLastFromOpType(operationType.Id);
            var todayDate = new Date(DateTime.Now.ToString("yyyy-MM-dd"));
            OperationTypeRecord operationRecord;
            if (operationRecord1 == null)
            {
                operationRecord = new OperationTypeRecord(new Date(todayDate.Start.ToString(), todayDate.End.ToString()), 0, operationType.Id, operationType.Name, operationType.EstimatedDuration, operationType.Status, operationType.RequiredStaff, operationType.Phases);
            }
            else
            {
                operationRecord = new OperationTypeRecord(new Date(todayDate.Start.ToString(), todayDate.End.ToString()), operationRecord1.Version.Version+1, operationType.Id, operationType.Name, operationType.EstimatedDuration, operationType.Status, operationType.RequiredStaff, operationType.Phases);
            }
            await _repo.AddAsync(operationRecord);
            await _unitOfWork.CommitAsync();
            await _logService.CreateCreationLog(operationRecord.Id.Value, operationRecord.GetType().Name, "New operation type version record created.");

            List<RequiredStaffDto> requiredStaffDtos = [];
            foreach (RequiredStaffRecord requiredStaffRecord in operationRecord.RequiredStaffRecords){
                requiredStaffDtos.Add(new RequiredStaffDto{StaffQuantity = requiredStaffRecord.StaffQuantity.NumberRequired,
                 Function = requiredStaffRecord.Function.Description, Specialization = requiredStaffRecord.SpecializationId.SpeciId});
            }

            List<PhaseDto> phaseDtos = [];
            foreach (PhaseRecord phase in operationRecord.Phases){
                phaseDtos.Add(new PhaseDto{Description = phase.Description.Description, Duration = phase.Duration.DurationMinutes});
            }

           return  new OperationTypeRecordDto(operationRecord.Id.Value.ToString(), operationRecord.Version.Version,
             operationRecord.EffectiveDate.ToString(), operationRecord.OperationTypeId.OpID,
              operationRecord.Name.OperationName, operationRecord.EstimatedDuration.TotalDurationMinutes,
               operationRecord.Status.Active, requiredStaffDtos, phaseDtos);
        }

    }
}
