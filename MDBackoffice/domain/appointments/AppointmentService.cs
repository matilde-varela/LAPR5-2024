using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MDBackoffice.Domain.AppointmentStaffs;
using MDBackoffice.Domain.OperationRequests;
using MDBackoffice.Domain.OperationTypes;
using MDBackoffice.Domain.OperationTypes.ValueObjects.RequiredStaff;
using MDBackoffice.Domain.Rooms;
using MDBackoffice.Domain.Shared;
using MDBackoffice.Domain.Specializations;
using MDBackoffice.Domain.StaffProfiles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace MDBackoffice.Domain.Appointments
{
    public class AppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppointmentRepository _repo;
        private readonly IOperationRequestRepository _repoOpReq;
        private readonly IRoomRepository _repoRoom;
        private readonly IOperationTypeRepository _repoOpType;
        private readonly IStaffRepository _repoSta;
        private readonly IRequiredStaffRepository _repoReqSta;
        private readonly IAppointmentStaffRepository _appointmentStaffRepo;

        public AppointmentService(IUnitOfWork unitOfWork, IAppointmentRepository repo, IOperationRequestRepository repoOpReq, IRoomRepository repoRoom, IOperationTypeRepository repoOpType,
                                IStaffRepository repoSta, IRequiredStaffRepository repoReqSta, IAppointmentStaffRepository appointmentStaffRepo)
        {
            _unitOfWork = unitOfWork;
            _repo = repo;
            _repoOpReq = repoOpReq;
            _repoRoom = repoRoom;
            _repoOpType = repoOpType;
            _repoSta = repoSta;
            _repoReqSta = repoReqSta;
            _appointmentStaffRepo = appointmentStaffRepo;
        }
        public async Task<AppointmentDto> CreateAppointmentScheduler(CreatingAppointmentDto dto){
            OperationRequest operationRequest = await this._repoOpReq.GetByIdAsync(new OperationRequestId(dto.OperationRequestId)) ??
                throw new BusinessRuleValidationException("Operation Request is invalid.");

            OperationType operationType = await this._repoOpType.GetByIdAsync(new OperationTypeId(operationRequest.OperationTypeId.AsString())) ??
                throw new BusinessRuleValidationException("Operation Type is invalid.");

            List<RequiredStaff> requiredStaffList = await this._repoOpType.GetRequiredStaffByOperationTypeIdAsync(operationType.Id) ??
                throw new BusinessRuleValidationException("There are no Required Staff.");

            List<Staff> staffs = new List<Staff>();
            var isStaffAvailable = true;
            var usedStaffIds = new List<string>();
            
            foreach(RequiredStaff requiredStaff in requiredStaffList){
                Function function = requiredStaff.Function;
                SpecializationCode specialization = requiredStaff.SpecializationId;
                NumberStaff numberStaff = requiredStaff.StaffQuantity;

                int staffAddedForThisRequirement = 0;
        
                foreach(string sta in dto.StaffList){

                    if (usedStaffIds.Contains(sta)) {
                        continue;
                    }

                   Staff staff = await this._repoSta.GetByIdAsync(new StaffId(sta)) ??
                        throw new BusinessRuleValidationException("Staff is invalid.");

                    isStaffAvailable &= await _appointmentStaffRepo.IsStaffAvailableAsync(staff.Id, dto.StartTime, dto.EndTime);
                    
                    if(staff.Function.Equals(function) && staff.SpecializationId.Equals(specialization))
                    {                      
                        if (staffAddedForThisRequirement <  numberStaff.NumberRequired){
                            staffs.Add(staff);
                            staffAddedForThisRequirement++;
                            usedStaffIds.Add(sta);
                        }
                    } 
                    if (staffAddedForThisRequirement >=  numberStaff.NumberRequired){
                        break;
                    }
                }
                if (staffAddedForThisRequirement <  numberStaff.NumberRequired){
                    throw new BusinessRuleValidationException("Not enough staff available for this surgery.");
                }
            }

            if (usedStaffIds.Count != dto.StaffList.Count)
            {
                throw new BusinessRuleValidationException("The are more staff than the ones required for this surgery.");
            }

            if (!isStaffAvailable)
            {
                throw new BusinessRuleValidationException("One or more staff members are not available for the selected time.");
            }

            Room room = await this._repoRoom.GetByIdAsync(new RoomNumber(dto.RoomNumber)) ??
                throw new BusinessRuleValidationException("Room is invalid");

            var isRoomAvailable = await _repoRoom.IsRoomAvailableAsync(room.Id, dto.StartTime, dto.EndTime);

            if (!isRoomAvailable)
            {
                throw new BusinessRuleValidationException("The surgery room is not available for the selected time.");
            }

            var appointment = new Appointment(
                operationRequest.Id,
                room.Id.AsString(),
                dto.StartTime,
                dto.EndTime,
                dto.StartDate,
                dto.EndDate
            );

            await _repo.AddAsync(appointment);

            List<string> staffIds = new List<string>();
            foreach (Staff staff in staffs)
            {
                var appointmentStaff = new AppointmentStaff(appointment, staff);
                await _appointmentStaffRepo.AddAsync(appointmentStaff);
                staffIds.Add(staff.Id.AsString());
            }

            operationRequest.ChangeStatus("Planned");

            return new AppointmentDto(appointment.Id.AsGuid(), appointment.Status.Description.ToString(), appointment.OperationRequestId.Value, appointment.RoomNumber.AsString(),
                appointment.Slot.TimeInterval.Start.ToString(), appointment.Slot.TimeInterval.End.ToString(), appointment.Slot.Date.Start.ToString(), appointment.Slot.Date.End.ToString(), staffIds
            );
        }
        
        public async Task<AppointmentDto> CreateAppointment(CreatingAppointmentDto dto){
            OperationRequest operationRequest = await this._repoOpReq.GetByIdAsync(new OperationRequestId(dto.OperationRequestId)) ??
                throw new BusinessRuleValidationException("Operation Request is invalid.");

            OperationType operationType = await this._repoOpType.GetByIdAsync(new OperationTypeId(operationRequest.OperationTypeId.AsString())) ??
                throw new BusinessRuleValidationException("Operation Type is invalid.");

            List<RequiredStaff> requiredStaffList = await this._repoOpType.GetRequiredStaffByOperationTypeIdAsync(operationType.Id) ??
                throw new BusinessRuleValidationException("There are no Required Staff.");

            List<Staff> staffs = new List<Staff>();
            var isStaffAvailable = true;
            var usedStaffIds = new List<string>();
            
            foreach(RequiredStaff requiredStaff in requiredStaffList){
                Function function = requiredStaff.Function;
                SpecializationCode specialization = requiredStaff.SpecializationId;
                NumberStaff numberStaff = requiredStaff.StaffQuantity;

                int staffAddedForThisRequirement = 0;
        
                foreach(string sta in dto.StaffList){

                    if (usedStaffIds.Contains(sta)) {
                        continue;
                    }

                   Staff staff = await this._repoSta.GetByIdAsync(new StaffId(sta)) ??
                        throw new BusinessRuleValidationException("Staff is invalid.");

                    isStaffAvailable &= await _appointmentStaffRepo.IsStaffAvailableAsync(staff.Id, dto.StartTime, dto.EndTime);
                    
                    if(staff.Function.Equals(function) && staff.SpecializationId.Equals(specialization))
                    {                      
                        if (staffAddedForThisRequirement <  numberStaff.NumberRequired){
                            staffs.Add(staff);
                            staffAddedForThisRequirement++;
                            usedStaffIds.Add(sta);
                        }
                    } 
                    if (staffAddedForThisRequirement >=  numberStaff.NumberRequired){
                        break;
                    }
                }
                if (staffAddedForThisRequirement <  numberStaff.NumberRequired){
                    throw new BusinessRuleValidationException("Not enough staff available for this surgery.");
                }
            }

            if (usedStaffIds.Count != dto.StaffList.Count)
            {
                throw new BusinessRuleValidationException("The are more staff than the ones required for this surgery.");
            }

            if (!isStaffAvailable)
            {
                throw new BusinessRuleValidationException("One or more staff members are not available for the selected time.");
            }

            Room room = await this._repoRoom.GetByIdAsync(new RoomNumber(dto.RoomNumber)) ??
                throw new BusinessRuleValidationException("Room is invalid");

            var isRoomAvailable = await _repoRoom.IsRoomAvailableAsync(room.Id, dto.StartTime, dto.EndTime);

            if (!isRoomAvailable)
            {
                throw new BusinessRuleValidationException("The surgery room is not available for the selected time.");
            }

            var appointment = new Appointment(
                operationRequest.Id,
                room.Id.AsString(),
                dto.StartTime,
                dto.EndTime,
                dto.StartDate,
                dto.EndDate
            );

            await _repo.AddAsync(appointment);

            List<string> staffIds = new List<string>();
            foreach (Staff staff in staffs)
            {
                var appointmentStaff = new AppointmentStaff(appointment, staff);
                await _appointmentStaffRepo.AddAsync(appointmentStaff);
                staffIds.Add(staff.Id.AsString());
                appointment.AppointmentStaffs.Add(appointmentStaff);
            }

            operationRequest.ChangeStatus("Planned");

            room.AddSlot(dto.StartTime, dto.EndTime, dto.StartDate, dto.EndDate);

            return new AppointmentDto(appointment.Id.AsGuid(), appointment.Status.Description.ToString(), appointment.OperationRequestId.Value, appointment.RoomNumber.AsString(),
                appointment.Slot.TimeInterval.Start.ToString(), appointment.Slot.TimeInterval.End.ToString(), appointment.Slot.Date.Start.ToString(), appointment.Slot.Date.End.ToString(), staffIds
            );
        }
        public virtual async Task<AppointmentDto> AddAsync(CreatingAppointmentDto dto)
        {
            var apt = await CreateAppointment(dto);
            await _unitOfWork.CommitAsync();
            return apt;
        }

        public async Task<List<AppointmentDto>> GetAllAsync()
        {
            var list = await _repo.GetAppointmentsWithStaff();

            var listDto = new List<AppointmentDto>();

            for(int i = 0; i < list.Count(); i++)
            {
                var dto = new AppointmentDto
                (
                    list.ElementAt(i).Id.AsGuid(),
                    list.ElementAt(i).Status.Description,
                    list.ElementAt(i).OperationRequestId.ToString(),
                    list.ElementAt(i).RoomNumber.Value,
                    list.ElementAt(i).Slot.TimeInterval.Start.ToString(),
                    list.ElementAt(i).Slot.TimeInterval.End.ToString(),
                    list.ElementAt(i).Slot.Date.Start.ToString(),
                    list.ElementAt(i).Slot.Date.End.ToString(),
                    list.ElementAt(i).AppointmentStaffs.Select(appointStaff => appointStaff.Id.Value.Split("-").ToList().Last()).ToList()
                );
                listDto.Add(dto);
            }

            return listDto; // Convert the array to a List<AppointmentDto>
        }



        public virtual async Task<AppointmentDto> UpdateAsync(UpdateAppointmentDto dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var appointment = await _repo.GetAppointmentByIdWithStaff(dto.AppointmentId);
            if (appointment == null) return null;

            bool roomChange = !dto.NewRoomNumber.Equals("");
            bool startTimeChange = !dto.NewStartTime.Equals("");
            bool endTimeChange = !dto.NewEndTime.Equals("");
            bool startDateChange = !dto.NewStartDate.Equals("");
            bool endDateChange = !dto.NewEndDate.Equals("");
            bool staffChange = !(dto.NewStaffList.Count() == 0);

            OperationRequest operationRequest = await this._repoOpReq.GetByIdAsync(new OperationRequestId(appointment.OperationRequestId.Value)) ??
                throw new BusinessRuleValidationException("Operation Request is invalid.");

            OperationType operationType = await this._repoOpType.GetByIdAsync(new OperationTypeId(operationRequest.OperationTypeId.AsString())) ??
                throw new BusinessRuleValidationException("Operation Type is invalid.");

            List<RequiredStaff> requiredStaffList = await this._repoOpType.GetRequiredStaffByOperationTypeIdAsync(operationType.Id) ??
                throw new BusinessRuleValidationException("There are no Required Staff.");
            
            List<Staff> staffs = new List<Staff>();

            List<string> staffsToTested = new List<string>();
            var isStaffAvailable = true;

            if (startTimeChange || endTimeChange || startDateChange || endDateChange)
            {
                var newStartTime = startTimeChange ? dto.NewStartTime : appointment.Slot.TimeInterval.Start.ToString();
                var newEndTime = endTimeChange ? dto.NewEndTime : appointment.Slot.TimeInterval.End.ToString();
                var newStartDate = startDateChange ? dto.NewStartDate : appointment.Slot.Date.Start.ToString();
                var newEndDate = endDateChange ? dto.NewEndDate : appointment.Slot.Date.End.ToString();

                appointment.ChangeSlot(newStartTime, newEndTime, newStartDate, newEndDate);
            }

            if (roomChange) appointment.ChangeRoom(dto.NewRoomNumber);

            if(staffChange){
                staffsToTested = dto.NewStaffList;
            }else{
                foreach(AppointmentStaff appointmentStaff in appointment.AppointmentStaffs){
                    staffsToTested.Add(appointmentStaff.Staff.Id.Value);
                }
            }

            foreach(RequiredStaff requiredStaff in requiredStaffList){
                Function function = requiredStaff.Function;
                SpecializationCode specialization = requiredStaff.SpecializationId;
                NumberStaff numberStaff = requiredStaff.StaffQuantity;

                int total = 0;
                
                foreach(string sta in staffsToTested){
                   Staff staff = await this._repoSta.GetByIdAsync(new StaffId(sta)) ??
                        throw new BusinessRuleValidationException("Staff is invalid."); //

                        isStaffAvailable &= await _appointmentStaffRepo.IsStaffAvailableAsync(staff.Id, appointment.Slot.TimeInterval.Start.ToString(), appointment.Slot.TimeInterval.End.ToString(),null);

                    if(staff.Function.Equals(function) && staff.SpecializationId.Equals(specialization) && total < numberStaff.NumberRequired)
                    {
                        total++;
                        staffs.Add(staff);
                    } else {
                        throw new BusinessRuleValidationException("The Staff is not required for this surgery.");
                    }
                }
            }

            if (!isStaffAvailable)
            {
                throw new BusinessRuleValidationException("One or more staff members are not available for the selected time.");
            }

            Room room = await this._repoRoom.GetByIdAsync(new RoomNumber(appointment.RoomNumber.Value)) ??
                throw new BusinessRuleValidationException("Room is invalid");

            var isRoomAvailable = await _repoRoom.IsRoomAvailableAsync(room.Id, appointment.Slot.TimeInterval.Start.ToString(), appointment.Slot.TimeInterval.End.ToString(), appointment.Id.AsGuid());

            if (!isRoomAvailable)
            {
                throw new BusinessRuleValidationException("The surgery room is not available for the selected time.");
            }

            if (staffChange)
            {
                var staffList = new List<Staff>();
                foreach (var staffId in dto.NewStaffList)
                {
                    var staff = await _repoSta.GetByIdAsync(new StaffId(staffId));
                    if (staff != null) staffList.Add(staff);
                }

                appointment.ClearStaff();

                foreach (var appointStaff in appointment.AppointmentStaffs)
                {
                    _appointmentStaffRepo.Remove(appointStaff);
                }

                await _unitOfWork.CommitAsync();

                foreach (var appointStaff in appointment.AppointmentStaffs)
                {
                    await _appointmentStaffRepo.AddAsync(appointStaff);
                }

                appointment.ChangeStaff(staffList);
            }

            await _unitOfWork.CommitAsync();
            return new AppointmentDto(appointment.Id.AsGuid(),
                                        appointment.Status.Description, 
                                        appointment.OperationRequestId.Value, 
                                        appointment.RoomNumber.Value, 
                                        appointment.Slot.TimeInterval.Start.ToString(), 
                                        appointment.Slot.TimeInterval.End.ToString(), 
                                        appointment.Slot.Date.Start.ToString(), 
                                        appointment.Slot.Date.End.ToString(), 
                                        appointment.AppointmentStaffs.Select(appointStaff => new string(appointStaff.Staff.Id.Value)).ToList());
        }


        public async Task<ActionResult<AppointmentDto>> GetByIdAsync(string id)
        {
           var appointment = await _repo.GetByIdAsync(new AppointmentId(id));
           return new AppointmentDto(appointment.Id.AsGuid(),
                                        appointment.Status.Description, 
                                        appointment.OperationRequestId.Value, 
                                        appointment.RoomNumber.Value,
                                       $"{appointment.Slot.TimeInterval.Start.Hours:D2}:{appointment.Slot.TimeInterval.Start.Minutes:D2}", // Format TimeSpan
                                        $"{appointment.Slot.TimeInterval.End.Hours:D2}:{appointment.Slot.TimeInterval.End.Minutes:D2}",   // Format TimeSpan
                                        appointment.Slot.Date.Start.ToString("yyyy-MM-dd"), 
                                        appointment.Slot.Date.End.ToString("yyyy-MM-dd"), 
                                        appointment.AppointmentStaffs.Select(appointStaff => new string(appointStaff.Staff.Id.Value)).ToList());
        }

        public async Task<ActionResult<AppointmentWithoutStaffDto>> GetAppointmentForSimulation(AppointmentRoomInfoDto roomInfoDto)
        {
            List<Appointment> appointments = await _repo.GetAllAsync();

            Appointment appointment = null;

            DateTime startDate = DateTime.Parse(roomInfoDto.StartDate);
            DateTime endDate = DateTime.Parse(roomInfoDto.EndDate);
            TimeSpan startTime = TimeSpan.Parse(roomInfoDto.StartTime);
            TimeSpan endTime = TimeSpan.Parse(roomInfoDto.EndTime);

            foreach (Appointment app in appointments)
            {
                if (app.RoomNumber.Value == roomInfoDto.RoomNumber)
                {
                    DateTime appointmentStartDate = app.Slot.Date.Start.Date;
                    DateTime appointmentEndDate = app.Slot.Date.End.Date;

                    TimeSpan appointmentStartTime = app.Slot.TimeInterval.Start;
                    TimeSpan appointmentEndTime = app.Slot.TimeInterval.End;

                    bool isStartDateMatch = appointmentStartDate.Date >= startDate.Date && appointmentStartDate.Date <= endDate.Date;
                    bool isEndDateMatch = appointmentEndDate.Date >= startDate.Date && appointmentEndDate.Date <= endDate.Date;

                    bool isStartTimeMatch = appointmentStartTime >= startTime && appointmentStartTime <= endTime;
                    bool isEndTimeMatch = appointmentEndTime >= startTime && appointmentEndTime <= endTime;

                    if (isStartDateMatch && isEndDateMatch && isStartTimeMatch && isEndTimeMatch)
                    {
                        appointment = app;
                        break;
                    }
                }
            }

            if (appointment == null)
            {
                throw new BusinessRuleValidationException("Appointment Not Found.");
            }

            return new AppointmentWithoutStaffDto(
                appointment.Id.AsGuid(),
                appointment.Status.Description,
                appointment.OperationRequestId.Value,
                appointment.RoomNumber.Value,
                $"{appointment.Slot.TimeInterval.Start.Hours:D2}:{appointment.Slot.TimeInterval.Start.Minutes:D2}", 
                $"{appointment.Slot.TimeInterval.End.Hours:D2}:{appointment.Slot.TimeInterval.End.Minutes:D2}",
                appointment.Slot.Date.Start.ToString("yyyy-MM-dd"),
                appointment.Slot.Date.End.ToString("yyyy-MM-dd"));;
        }
    }
}