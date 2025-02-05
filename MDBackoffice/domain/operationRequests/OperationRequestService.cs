using System.Threading.Tasks;
using System.Collections.Generic;
using MDBackoffice.Domain.Shared;
using MDBackoffice.Domain.StaffProfiles;
using MDBackoffice.Domain.Patients;
using MDBackoffice.Domain.OperationTypes;
using System;
using System.Linq;
using Microsoft.AspNetCore.Http.HttpResults;
using MDBackoffice.Domain.Users;
using Microsoft.AspNetCore.Identity;
using MDBackoffice.Domain.Logs;
using Microsoft.AspNetCore.Mvc;
using MDBackoffice.Infrastructure.OperationRequests;
using MDBackoffice.Domain.Rooms;
using MDBackoffice.Domain.Appointments;
using System.Text;
using MDBackoffice.Domain.Specializations;

namespace MDBackoffice.Domain.OperationRequests
{
    public class OperationRequestService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOperationRequestRepository _repo;
        private readonly IStaffRepository _repoSta;
        private readonly LogService _logService;
        private readonly PatientService _patientService;
        private readonly IPatientRepository _repoPat;
        private readonly IOperationTypeRepository _repoOpTy;
        private readonly UserService _userService;
        private readonly IOperationSchedulerAdapter _planningAdapter;
        private readonly RoomService _roomService;
        private readonly AppointmentService _appointService;
        private readonly SpecializationService _specService;

        public OperationRequestService(IUnitOfWork unitOfWork, IOperationRequestRepository repo, IStaffRepository repoSta, LogService logService, PatientService patientService, IPatientRepository repoPat, IOperationTypeRepository repoOpTy, UserService userService,
                                            IOperationSchedulerAdapter adapter, RoomService room, AppointmentService appointmentService,SpecializationService specializationService )
        {
            this._unitOfWork = unitOfWork;
            this._repo = repo;
            this._repoSta = repoSta;
            this._repoPat = repoPat;
            this._repoOpTy = repoOpTy;
            this._logService = logService;
            this._patientService = patientService;
            this._userService = userService;
            this._planningAdapter = adapter;
            this._roomService = room;
            this._appointService = appointmentService;
            _specService = specializationService;
        }

        public async Task<List<OperationRequestDto>> GetAllAsysnc()
        {
            var list = await _repo.GetAllAsync();

            List<OperationRequestDto> listDto = list.ConvertAll(opRequest =>
                new OperationRequestDto(opRequest.Id.AsGuid(), opRequest.DeadLineDate.ToString(), opRequest.Priority.ToString(),
                 opRequest.DateOfRequest.ToString(), opRequest.Status.ToString(), opRequest.StaffId.AsString(), opRequest.Description.DescriptionText, opRequest.PatientId.AsString(), opRequest.OperationTypeId.AsString()));

            return listDto;
        }

        public async Task<OperationRequestDto> GetByIdAsync(OperationRequestId id)
        {
            var opRequest = await this._repo.GetByIdAsync(id);

            if (opRequest == null)
                return null;

            return new OperationRequestDto(opRequest.Id.AsGuid(), opRequest.DeadLineDate.ToString(), opRequest.Priority.ToString(),
                 opRequest.DateOfRequest.ToString(), opRequest.Status.ToString(), opRequest.StaffId.AsString(), opRequest.Description.DescriptionText, opRequest.PatientId.AsString(), opRequest.OperationTypeId.AsString());
        }

        public async Task<OperationRequestDto> AddAsync(CreatingOperationRequestDto dto)
        {
            Staff staff = await this._repoSta.GetByIdAsync(new StaffId(dto.StaffId)) ??
                throw new BusinessRuleValidationException("Staff is invalid.");

            Patient patient = await this._repoPat.GetByIdAsync(new MedicalRecordNumber(dto.PatientId)) ??
                throw new BusinessRuleValidationException("Patient is invalid.");

            OperationType opType = await this._repoOpTy.GetByNameAsync(dto.OperationTypeId) ??
                throw new BusinessRuleValidationException("Operation Type is invalid.");

            CheckStaffFunctionAndSpecialization(opType, staff);

            var opRequest = new OperationRequest(new Date(dto.DeadLineDate), Priority.GetPriorityByName(dto.Priority), new Date(dto.DateOfRequest), staff.Id, dto.Description, patient.Id, opType.Id);

            await this._repo.AddAsync(opRequest);

            await this._unitOfWork.CommitAsync();

            return new OperationRequestDto(opRequest.Id.AsGuid(), opRequest.DeadLineDate.ToString(), opRequest.Priority.ToString(),
                 opRequest.DateOfRequest.ToString(), opRequest.Status.ToString(), opRequest.StaffId.AsString(), opRequest.Description.DescriptionText, opRequest.PatientId.AsString(), opRequest.OperationTypeId.AsString());
        }

        private void CheckStaffFunctionAndSpecialization(OperationType opType, Staff staff)
        {
            bool specializationMatches = false;
            foreach (var requiredStaff in opType.RequiredStaff)
            {
                if (requiredStaff.Function.Equals(staff.Function) && requiredStaff.SpecializationId.AsString() == staff.SpecializationId.AsString())
                {
                    specializationMatches = true;
                    break;
                }
            }

            if (!specializationMatches)
            {
                throw new BusinessRuleValidationException("The Staff Function or Specialization does not match any of the required staff for the operation.");
            }
        }


        public virtual async Task<OperationRequestDto> UpdateAsync(UpdateOperationRequestDto dto)
        {
            var opRequest = await _repo.GetByIdAsync(new OperationRequestId(dto.Id));

            if (opRequest == null)
                return null;

            if (!opRequest.DeadLineDate.Equals(dto.DeadLineDate))
            {
                opRequest.ChangeDeadLineDate(dto.DeadLineDate);
                await _logService.CreateEditLog(opRequest.Id.ToString(), opRequest.DeadLineDate.GetType().Name, "The operation request deadline date was altered.");
            }

            if (!opRequest.Priority.Equals(dto.Priority))
            {
                opRequest.ChangePriority(dto.Priority);
                await _logService.CreateEditLog(opRequest.Id.ToString(), opRequest.Priority.GetType().Name, "The operation request priority was altered.");
            }

            if (!opRequest.Description.Equals(dto.Description))
            {
                opRequest.ChangeDescription(dto.Description);
                await _logService.CreateEditLog(opRequest.Id.ToString(), opRequest.Description.GetType().Name, "The operation request description was altered.");
            }

            await _unitOfWork.CommitAsync();

            return new OperationRequestDto(opRequest.Id.AsGuid(), opRequest.DeadLineDate.ToString(), opRequest.Priority.ToString(),
                 opRequest.DateOfRequest.ToString(), opRequest.Status.ToString(), opRequest.StaffId.AsString(), opRequest.Description.DescriptionText, opRequest.PatientId.AsString(), opRequest.OperationTypeId.AsString());
        }

        public async Task<OperationRequestDto> DeleteAsync(OperationRequestId id)
        {
            var opRequest = await this._repo.GetByIdAsync(id);

            if (opRequest == null)
                return null;

            this._repo.Remove(opRequest);
            await this._unitOfWork.CommitAsync();

            return new OperationRequestDto(opRequest.Id.AsGuid(), opRequest.DeadLineDate.ToString(), opRequest.Priority.ToString(),
                 opRequest.DateOfRequest.ToString(), opRequest.Status.ToString(), opRequest.StaffId.AsString(), opRequest.Description.DescriptionText, opRequest.PatientId.AsString(), opRequest.OperationTypeId.AsString());
        }

        public async Task<List<ListOperationRequestDto>> GetOperationRequestByFiltersAsync(string email, string? name, string? priority, string? operationType, string? status, string? dateOfRequest, string? deadLineDate)
        {

            var staff = await _repoSta.GetStaffWithEmail(email);

            var opRequests = await _repo.FindAllConditioned(staff.Id, name, priority, operationType, status, dateOfRequest, deadLineDate);

            return await ConvertOperationRequestToDto(opRequests.ToList());
        }

        private async Task<List<ListOperationRequestDto>> ConvertOperationRequestToDto(List<OperationRequest> opRequests)
        {
            var operationRequestDtos = new List<ListOperationRequestDto>();
            foreach (var opRequest in opRequests)
            {
                var patient = await _repoPat.GetByIdAsync(opRequest.PatientId) ?? throw new BusinessRuleValidationException("Couldn't obtain the patient associated with the operation request.");

                var dto = new ListOperationRequestDto(
                    opRequest.Id.Value.ToString(),
                    patient.Name.ToString(),
                    opRequest.OperationTypeId.AsString(),
                    opRequest.Status.ToString()
                );

                operationRequestDtos.Add(dto);
            }
            return operationRequestDtos;
        }

        public async Task<bool> DeleteOperationRequest(string id, string userEmail)
        {
            User user = await _userService.FindByEmailAsync(userEmail);
            if (user == null)
            {
                throw new BusinessRuleValidationException("No user found with this email.");
            }

            Staff staff = await _repoSta.FindStaffWithUserId(user.Id.ToString());
            if (staff == null)
            {
                throw new BusinessRuleValidationException("No staff found with this user.");
            }

            OperationRequest operationRequest = await _repo.GetByIdAsync(new OperationRequestId(id));
            if (operationRequest == null)
            {
                return false;
            }

            if (operationRequest.Status.Description.Equals("Requested") &&
                operationRequest.Id.Value == id &&
                operationRequest.StaffId == staff.Id)
            {
                _repo.Remove(operationRequest);
                await _unitOfWork.CommitAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual async Task<bool> CheckDoctorIsRequestingDoctor(string? userEmail, string id)
        {

            var request = await GetByIdAsync(new OperationRequestId(id));

            var doctor = await _repoSta.GetStaffWithEmail(userEmail);

            if (request.StaffId.Equals(doctor.Id.AsString()))
            {
                return true;
            }

            return false;
        }

        public virtual async Task<bool> DeleteOperationRequestFromPatient(string patientId, string operationRequestId, string email)
        {
            User user = await _userService.FindByEmailAsync(email);
            if (user == null)
            {
                throw new BusinessRuleValidationException("No user found with this email.");
            }

            Staff staff = await _repoSta.FindStaffWithUserId(user.Id.ToString());
            if (staff == null)
            {
                throw new BusinessRuleValidationException("No staff found with this user.");
            }

            Patient patient = await _repoPat.GetByIdWithAppointmentHistoryAsync(new MedicalRecordNumber(patientId));
            if (patient == null)
            {
                throw new BusinessRuleValidationException("No patient found with this id.");
            }

            patient.RemoveRequestFromHistory(new OperationRequestId(operationRequestId));
            await _unitOfWork.CommitAsync();
            return true;
        }


        public async Task<bool> AddOperationRequestToPatient(string patientId, string operationRequestId, string email)
        {
            User user = await _userService.FindByEmailAsync(email);
            if (user == null)
            {
                throw new BusinessRuleValidationException("No user found with this email.");
            }

            Staff staff = await _repoSta.FindStaffWithUserId(user.Id.ToString());
            if (staff == null)
            {
                throw new BusinessRuleValidationException("No staff found with this user.");
            }

            Patient patient = await _repoPat.GetByIdWithAppointmentHistoryAsync(new MedicalRecordNumber(patientId));
            if (patient == null)
            {
                throw new BusinessRuleValidationException("No patient found with this id.");
            }

            var appointmentHistory = new AppointmentHistory(operationRequestId, 1, 2, patient.Id);
            await _unitOfWork.CommitAsync();

            patient.AddRequestToHistory(appointmentHistory);
            await _unitOfWork.CommitAsync();
            return true;
        }

        public virtual async Task<ActionResult<IEnumerable<OperationRequestDto>>> GetAllFromDoctorAsysnc(string userEmail)
        {
            User user = await _userService.FindByEmailAsync(userEmail);
            if (user == null)
            {
                throw new BusinessRuleValidationException("No user found with this email.");
            }

            Staff staff = await _repoSta.FindStaffWithUserId(user.Id.ToString());
            if (staff == null)
            {
                throw new BusinessRuleValidationException("No staff found with this user.");
            }

            var list = await this._repo.GetAllFromDoctorAsync(staff.Id.ToString());

            List<OperationRequestDto> listDto = list.ConvertAll<OperationRequestDto>(opRequest =>
                new OperationRequestDto(opRequest.Id.AsGuid(), opRequest.DeadLineDate.ToString(), opRequest.Priority.ToString(),
                 opRequest.DateOfRequest.ToString(), opRequest.Status.ToString(), opRequest.StaffId.AsString(), opRequest.Description.DescriptionText, opRequest.PatientId.AsString(), opRequest.OperationTypeId.AsString()));

            return listDto;
        }

        public async Task<string> Schedule(OperationRequestScheduleInfoDto scheduleInfoDto)
        {

            if (scheduleInfoDto == null)
            {
                throw new ArgumentNullException(nameof(scheduleInfoDto));
            }

            DateTime dayDate;
            if (!DateTime.TryParseExact(scheduleInfoDto.Date, "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out dayDate))
            {
                throw new InvalidOperationException("Invalid day format. Expected format is yyyy-MM-dd.");
            }
            var roomIds = scheduleInfoDto.RoomID;
            var rooms = new List<RoomDto>();
            foreach (var roomId in roomIds)
            {
                var room = await _roomService.GetRoomDtoById(roomId);
                if (room == null)
                {
                    throw new InvalidOperationException($"Room with ID {roomId} not found.");
                }

                room.MaintenanceSlots = room.MaintenanceSlots
                .Where(slot =>
                {
                    DateTime startDate, endDate;
                    bool isStartDateValid = DateTime.TryParseExact(slot.StartDate, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out startDate);
                    bool isEndDateValid = DateTime.TryParseExact(slot.EndDate, "dd/MM/yyyy HH:mm:ss", null, System.Globalization.DateTimeStyles.None, out endDate);

                    // If either the start date or end date matches the specified day
                    return (isStartDateValid && startDate.Date == dayDate.Date) || (isEndDateValid && endDate.Date == dayDate.Date);
                })
                .ToList();
                rooms.Add(room);
            }

            var operationsMap = new Dictionary<ScheduleOperationRequestDto, List<ScheduleStaffDto>>();

            foreach (StaffForRequestEntry staffForRequestEntry in scheduleInfoDto.SchedulingData)
            {
                var operationRequest = await GetByIdAsync(new OperationRequestId(staffForRequestEntry.OperationRequestID));
                var opType = await _repoOpTy.GetByIdAsync(new OperationTypeId(operationRequest.OperationTypeId));

                ScheduleOperationRequestDto scheduleOperationRequestDto = new ScheduleOperationRequestDto(staffForRequestEntry.OperationRequestID,
                                                                                                        opType.Name.OperationName,
                                                                                                        opType.Phases[0].Duration.DurationMinutes,
                                                                                                        opType.Phases[1].Duration.DurationMinutes,
                                                                                                        opType.Phases[2].Duration.DurationMinutes);

                var listStaff = new List<ScheduleStaffDto>();

                foreach (StaffDto staffDto in staffForRequestEntry.Staff)
                {
                    var staff = await this._repoSta.GetByIdAsync(new StaffId(staffDto.Id));
                    if (staff == null)
                    {
                        throw new BusinessRuleValidationException("No staff exists with that Id.");
                    }
                    List<Slot> freeSlots = staff.Slots;
                    var busySpots = GetBusyIntervals(freeSlots, scheduleInfoDto.Date);
                    SpecializationDto specDto = await _specService.GetByIdAsync(new SpecializationCode(staffDto.SpecializationId));
                    ScheduleStaffDto scheduleStaffDto = new ScheduleStaffDto(staffDto.Id, staffDto.Function, specDto.Denomination, busySpots);
                    listStaff.Add(scheduleStaffDto);
                }

                operationsMap.Add(scheduleOperationRequestDto, listStaff);
            }

            List<SchedulesDto> listSchedulesToDo = await _planningAdapter.ScheduleOperationsAsync(operationsMap, rooms, scheduleInfoDto.Date, scheduleInfoDto.Algorithm);

            await UpdateSchedules(listSchedulesToDo);

            StringBuilder sb = new StringBuilder();
            foreach (SchedulesDto schedules in listSchedulesToDo)
            {
                sb.Append("DAY : ");
                sb.Append(schedules.Date);
                sb.Append("/");
                sb.Append("ROOM ID : ");
                sb.Append(schedules.RoomId);
                sb.Append("/");
                sb.Append("Room Schedule:\n");
                if (schedules.RoomSchedule.ContainsKey(schedules.RoomId))
                {
                    var roomSlots = schedules.RoomSchedule[schedules.RoomId];
                    foreach (var slot in roomSlots)
                    {
                        sb.Append($"- Start: {slot.StartTime}, End: {slot.EndTime}, Date: {slot.StartDate}\n");
                    }
                }
                else
                {
                    sb.Append("No room schedule available.\n");
                }
                sb.Append(" | Staff Schedule:\n");
                if (schedules.StaffSchedule != null && schedules.StaffSchedule.Count > 0)
                {
                    foreach (var staff in schedules.StaffSchedule)
                    {
                        sb.Append($"Staff ID: {staff.Key}\n");
                        var staffSlots = staff.Value;
                        foreach (var slot in staffSlots)
                        {
                            sb.Append($"- Start: {slot.StartTime}, End: {slot.EndTime}, Date: {slot.StartDate}\n");
                        }
                    }
                }
                else
                {
                    sb.Append("No staff schedule available.\n");
                }
            }

            string message = sb.ToString();
            return message;
        }

        public static List<SlotsDto> GetBusyIntervals(List<Slot> freeSlots, string targetDate)
        {
            // Validate the input date
            if (!DateTime.TryParse(targetDate, out DateTime targetDay))
            {
                throw new ArgumentException("Invalid date format. Use a valid date string (e.g., yyyy-MM-dd).");
            }


            // Filter slots for the target day
            var daySlots = freeSlots
                .Where(slot => slot.Date.Start == targetDay.Date)
                .OrderBy(slot => slot.TimeInterval.Start) // Sort by StartTime
                .ToList();

            var busyIntervals = new List<SlotsDto>();

            // Find gaps between consecutive free slots
            for (int i = 0; i < daySlots.Count - 1; i++)
            {
                var currentSlotEnd = daySlots[i].TimeInterval.End;
                var nextSlotStart = daySlots[i + 1].TimeInterval.Start;

                if (currentSlotEnd < nextSlotStart) // A busy interval exists
                {
                    busyIntervals.Add(new SlotsDto(
                        targetDate, targetDate,
                        currentSlotEnd.ToString(),
                        nextSlotStart.ToString(),
                        "Busy Interval"
                    ));
                }
            }

            return busyIntervals;
        }

        public async Task UpdateSchedules(List<SchedulesDto> schedules)
        {
            // Process each schedule in the list
            foreach (var schedule in schedules)
            {
                var roomId = schedule.RoomId;
                // Process each slot in the room schedule to update the operation request
                foreach (var roomEntry in schedule.RoomSchedule)
                {
                    var slots = roomEntry.Value;

                    foreach (var slot in slots)
                    {
                        if (!string.IsNullOrEmpty(slot.Name) && slot.Name != "occupied")
                        {
                            await CreateSurgeryAppointment(slot.Name, roomId, slot, schedule.StaffSchedule.Keys.ToList());
                          //  await UpdateOperationRequest(slot.Name);
                        }
                    }
                }
                // Process staff schedules
                foreach (var staffId in schedule.StaffSchedule.Keys)
                {
                    if (!schedule.StaffSchedule.TryGetValue(staffId, out var busySlots)) continue;

                    // Replace staff's free time slots with updated free slots based on busy schedule
                    var freeSlots = CalculateFreeSlots(busySlots, 400, 1400);
                    // Update the staff's free time slots in the system (assuming a dictionary or similar structure exists)
                    await UpdateStaffSchedule(staffId, freeSlots);
                }

                // Process room schedule (adding maintenance slots)
                
                var roomBusySlots = schedule.RoomSchedule.ContainsKey(roomId)
                    ? schedule.RoomSchedule[roomId]
                    : new List<SlotsDto>();
                // Update the room's schedule in the system (assuming a dictionary or similar structure exists)
                await UpdateRoomSchedule(roomId, roomBusySlots);
            }
            await _unitOfWork.CommitAsync();
        }


        private async Task<List<SlotsDto>> GetRoomMaintenanceSlots(string roomId)
        {
            var room = await _roomService.GetRoomDtoById(roomId);

            return room.MaintenanceSlots;
        }

        private async Task UpdateStaffSchedule(string staffId, List<SlotsDto> freeSlots)
        {
            var staff = await _repoSta.GetStaffWithIdIncludingSlots(staffId);

            staff.ChangeSlots(freeSlots);
        }

        private async Task UpdateRoomSchedule(string roomId, List<SlotsDto> roomSchedule)
        {
            var room = await _roomService.GetRoomById(roomId);

            room.ChangeSlots(roomSchedule);
        }


        private async Task UpdateOperationRequest(string opRequestId)
        {
            var opRequest = await _repo.GetByIdAsync(new OperationRequestId(opRequestId));

            opRequest.ChangeStatus("Planned");
        }

        private async Task CreateSurgeryAppointment(string opRequestId, string roomNumber, SlotsDto slot, List<string> staffList)
        {
            var appointmentDto = new CreatingAppointmentDto(opRequestId, roomNumber, slot.StartTime,
               slot.EndTime, slot.StartDate, slot.EndDate, staffList);

            await _appointService.CreateAppointmentScheduler(appointmentDto);
        }

        private List<SlotsDto> CalculateFreeSlots(List<SlotsDto> busySlots, int startOfDay, int endOfDay)
        {
            // Convert start and end times into `TimeSpan` objects
            var dayStart = TimeSpan.FromMinutes(startOfDay);
            var dayEnd = TimeSpan.FromMinutes(endOfDay);

            // Sort busy slots by start time
            var sortedBusySlots = busySlots
                .OrderBy(slot => TimeSpan.Parse(slot.StartTime))
                .ToList();

            var freeSlots = new List<SlotsDto>();
            var currentFreeStart = dayStart;

            // Identify gaps between busy slots
            foreach (var slot in sortedBusySlots)
            {
                var busyStart = TimeSpan.Parse(slot.StartTime);
                var busyEnd = TimeSpan.Parse(slot.EndTime);

                if (currentFreeStart < busyStart) // Gap exists
                {
                    freeSlots.Add(new SlotsDto(
                        slot.StartDate,
                        slot.StartDate,
                        currentFreeStart.ToString(@"hh\:mm"),
                        busyStart.ToString(@"hh\:mm"),
                        "Free Time"
                    ));
                }

                // Update the free start to the end of the current busy slot
                currentFreeStart = busyEnd > currentFreeStart ? busyEnd : currentFreeStart;
            }

            // Add free slot from the last busy slot to the end of the day
            if (currentFreeStart < dayEnd)
            {
                freeSlots.Add(new SlotsDto(
                    sortedBusySlots.FirstOrDefault()?.StartDate ?? DateTime.Now.ToString("yyyy-MM-dd"),
                    sortedBusySlots.FirstOrDefault()?.StartDate ?? DateTime.Now.ToString("yyyy-MM-dd"),
                    currentFreeStart.ToString(@"hh\:mm"),
                    dayEnd.ToString(@"hh\:mm"),
                    "Free Time"
                ));
            }

            return freeSlots;
        }

    }
}