using System.Threading.Tasks;
using System.Collections.Generic;
using MDBackoffice.Domain.Shared;
using MDBackoffice.Domain.OperationTypes.ValueObjects.RequiredStaff;
using MDBackoffice.Domain.OperationTypes.ValueObjects.Phase;
using System;
using MDBackoffice.Domain.Logs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MDBackoffice.Domain.OperationTypes.ValueObjects;
using System.Linq;
using MDBackoffice.Infrastructure.Specializations;
using MDBackoffice.Domain.Specializations;

namespace MDBackoffice.Domain.OperationTypes
{
    public class OperationTypeService
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IOperationTypeRepository _repo;
        private readonly OperationTypeRecordService _recordService;
        private readonly LogService _logService;
        private readonly ISpecializationRepository _specRepo;

        public OperationTypeService(IUnitOfWork unitOfWork, IOperationTypeRepository repo, LogService logService, OperationTypeRecordService operationTypeRecordService, ISpecializationRepository specializationRepository)
        {
            _unitOfWork = unitOfWork;
            _repo = repo;
            _logService = logService;
            _recordService = operationTypeRecordService;
            _specRepo = specializationRepository;
        }

        public async Task<List<OperationTypeDto>> GetAllAsync()
        {
            var list = await this._repo.GetAllAsync();

            List<OperationTypeDto> listDto = list.ConvertAll<OperationTypeDto>(operationType => ToDto(operationType));

            return listDto;
        }

        

        public async Task<OperationTypeDto> GetByIdAsync(OperationTypeId id)
        {
            var operationType = await this._repo.GetByIdWithStaffAsync(id);

            if (operationType == null)
                return null;

            return ToDto(operationType);
        }

        public virtual async Task<OperationTypeDto> AddAsync(OperationTypeDto dto)
        {
            for(int i = 0; i < dto.RequiredStaff.Count(); i++)
            {
                dto.RequiredStaff.ElementAt(i).Specialization = (await _specRepo.FindByDenomination(dto.RequiredStaff.ElementAt(i).Specialization)).Id.Value;
            } 

            var operationType = new OperationType(
                dto.Name,
                dto.EstimatedDuration,
                dto.Status,
                dto.RequiredStaff,
                dto.Phases
            );

            await _repo.AddAsync(operationType);
            await _unitOfWork.CommitAsync();

            await _logService.CreateCreationLog(operationType.Id.Value, operationType.GetType().Name, "New operation type added: " + operationType.Name.OperationName);

            var operation = await _repo.GetByIdAsync(operationType.Id);

            await _recordService.AddAsync(operation);

            await _unitOfWork.CommitAsync();

            return ToDto(operationType);
        }

        public async Task<OperationTypeDto> UpdateAsync(OperationTypeDto dto)
        {
            var operationType = await _repo.GetByIdAsync(new OperationTypeId(dto.Name));

            if (operationType == null)
                return null;

            operationType.ChangeName(dto.Name);
            operationType.ChangeEstimatedDuration(dto.EstimatedDuration);
            operationType.ChangeStatus(dto.Status);
            operationType.ChangeRequiredStaff(dto.RequiredStaff);
            operationType.ChangePhases(dto.Phases);

            await this._unitOfWork.CommitAsync();

            return ToDto(operationType);
        }

        public async Task<OperationTypeDto> InactivateAsync(string dto)
        {
            var operationType = await this._repo.GetByNameAsync(dto)?? throw new BusinessRuleValidationException("Invalid operation type.");
            operationType.Inactive();
            await this._unitOfWork.CommitAsync();
            return ToDto(operationType);
        }

        public async Task<OperationTypeDto> DeleteAsync(OperationTypeId id)
        {
            var operationType = await this._repo.GetByIdAsync(id);

            if (operationType == null)
                return null;

            if (operationType.Status.Active)
                throw new BusinessRuleValidationException("It is not possible to delete an active OperationType.");

            this._repo.Remove(operationType);
            await this._unitOfWork.CommitAsync();

            return ToDto(operationType);
        }

        public async Task<List<OperationTypeDto>> FilterOperationTypes(OperationTypeQueryParametersDto dto)
        {
            List<OperationType> filteredOperationTypes = await _repo.FilterOperationTypes(dto);
            List<OperationTypeDto> OperationTypeDtoListFiltered = [];

            foreach (OperationType operationType in filteredOperationTypes)
            {
                OperationTypeDtoListFiltered.Add(ToDto(operationType));
            }

            return OperationTypeDtoListFiltered;
        }

        private OperationTypeDto ToDto(OperationType operationType)
        {

            if (operationType.RequiredStaff.Count == 0)
            {
                Console.WriteLine("\n\nNo required staff obtained. \n\n");
                return new OperationTypeDto
                {
                    Name = operationType.Name.OperationName,
                    EstimatedDuration = operationType.EstimatedDuration.TotalDurationMinutes,
                    Status = operationType.Status.Active,
                    RequiredStaff = [],
                    Phases = operationType.Phases.ConvertAll(phase => new PhaseDto
                    {
                        Description = phase.Description.Description,
                        Duration = phase.Duration.DurationMinutes
                    })
                };
            }
            else
            {
                return new OperationTypeDto
                {
                    Name = operationType.Name.OperationName,
                    EstimatedDuration = operationType.EstimatedDuration.TotalDurationMinutes,
                    Status = operationType.Status.Active,
                    RequiredStaff = operationType.RequiredStaff.ConvertAll(staff => new RequiredStaffDto
                    {
                        StaffQuantity = staff.StaffQuantity.NumberRequired,
                        Function = staff.Function.Description,
                        Specialization = staff.SpecializationId.Value
                    }),
                    Phases = operationType.Phases.ConvertAll(phase => new PhaseDto
                    {
                        Description = phase.Description.Description,
                        Duration = phase.Duration.DurationMinutes
                    })
                };
            }

        }

        private EditOpTypeDto ToDtoEdit(OperationType operationType)
        {
            var requiredStaff = operationType.RequiredStaff.Count == 0
                ? new List<RequiredStaffDto>()
                : operationType.RequiredStaff.ConvertAll(staff => new RequiredStaffDto
                {
                    StaffQuantity = staff.StaffQuantity.NumberRequired,
                    Function = staff.Function.Description,
                    Specialization = staff.SpecializationId.Value
                });

            return new EditOpTypeDto
            {
                Id = operationType.Id.Value.ToString(),
                Name = operationType.Name.OperationName,
                EstimatedDuration = operationType.EstimatedDuration.TotalDurationMinutes,
                Status = operationType.Status.Active,
                RequiredStaff = requiredStaff,
                Phases = operationType.Phases.ConvertAll(phase => new PhaseDto
                {
                    Description = phase.Description.Description,
                    Duration = phase.Duration.DurationMinutes
                })
            };
        }


        public async Task EditOperationType(EditOpTypeDto editOpTypeDto)
        {
            var operationType = await _repo.GetByIdWithStaffAsync(new OperationTypeId(editOpTypeDto.Id));
            if (operationType == null)
            {
                throw new BusinessRuleValidationException("No operation type found with this Id.");
            }

            if (editOpTypeDto.EstimatedDuration != null)
            {
                operationType.ChangeEstimatedDuration(editOpTypeDto.EstimatedDuration);
            }

            if (editOpTypeDto.Name != null)
            {
                operationType.ChangeName(editOpTypeDto.Name);
            }

            if (editOpTypeDto.Phases != null)
            {
                operationType.ChangePhases(editOpTypeDto.Phases);
            }

            if (editOpTypeDto.RequiredStaff != null)
            {
                operationType.ChangeRequiredStaff(editOpTypeDto.RequiredStaff);
            }

            var result = await this._unitOfWork.CommitAsync();

            await _logService.CreateEditLog(operationType.Id.Value, operationType.GetType().Name, "Operation type edited: " + operationType.Name.OperationName);

            await _recordService.AddAsync(operationType);

            await _unitOfWork.CommitAsync();

        }

        public async Task<EditOpTypeDto> GetWithName(string name)
        {
            var operationType = await this._repo.GetByNameAsync(name);

            if (operationType == null)
                return null;

            return ToDtoEdit(operationType);
        }
    }
}
