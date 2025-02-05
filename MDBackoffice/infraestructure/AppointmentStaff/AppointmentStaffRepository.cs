using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MDBackoffice.Domain.Appointments;
using MDBackoffice.Domain.AppointmentStaffs;
using MDBackoffice.Domain.StaffProfiles;
using MDBackoffice.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;


namespace MDBackoffice.Infrastructure.AppointmentStaffs
{
    public class AppointmentStaffRepository : BaseRepository<AppointmentStaff, AppointmentStaffId>, IAppointmentStaffRepository
    {
        private readonly MDBackofficeDbContext _context;
        public AppointmentStaffRepository(MDBackofficeDbContext context) : base(context.AppointmentStaffs)
        {
            _context = context;
        }

        public async Task<List<AppointmentStaff>> GetAppointmentByAptId(string id)
        {
            List<AppointmentStaff> list = await _context.AppointmentStaffs
                .Where(aptStaff => aptStaff.Appointment.Id.Value.Equals(id))
                .ToListAsync();

            return list;
        }

        public async Task<bool> IsStaffAvailableAsync(StaffId staffId, string startTime, string endTime, Guid? excludedAppointmentId = null)
        {
            if (!TimeSpan.TryParse(startTime, out var requestedStartTime) ||
                !TimeSpan.TryParse(endTime, out var requestedEndTime))
            {
                throw new ArgumentException("Horário inválido. Certifique-se de usar o formato correto de tempo (HH:mm).");
            }

            var staff = await _context.StaffProfiles
                .Include(s => s.Slots)
                .FirstOrDefaultAsync(s => s.Id == staffId);

            if (staff == null)
            {
                throw new ArgumentException($"Staff with ID {staffId} not found.");
            }

            var isAvailableInSlot = staff.Slots.Any(slot =>
                slot.TimeInterval.Start <= requestedStartTime && slot.TimeInterval.End >= requestedEndTime);

            if (!isAvailableInSlot)
            {
                return false;
            }

            var appointmentStaffs = await _context.AppointmentStaffs
                .Include(a => a.Appointment)
                .Where(a => a.Staff.Id == staffId &&
                            a.Appointment != null &&
                            a.Appointment.Status.Description != AppointmentStatus.Canceled.Description &&
                            a.Appointment.Status.Description != AppointmentStatus.Completed.Description)
                .ToListAsync();

            if (appointmentStaffs == null || !appointmentStaffs.Any())
            {
                return true;
            }

            var operationRequestIds = appointmentStaffs
                .Select(a => a.Appointment.OperationRequestId)
                .Distinct()
                .ToList();


            var operationRequests = await _context.OperationRequests
                .Where(or => operationRequestIds.Contains(or.Id))
                .ToDictionaryAsync(or => or.Id, or => or.OperationTypeId);


            var operationTypeIds = operationRequests.Values.Distinct().ToList();

            var operationTypes = await _context.OperationTypes
                .Where(ot => operationTypeIds.Contains(ot.Id))
                .ToDictionaryAsync(ot => ot.Id, ot => ot.EstimatedDuration);

            foreach (var appointmentStaff in appointmentStaffs)
            {
                var appointment = appointmentStaff.Appointment;

                if (excludedAppointmentId.HasValue && appointment.Id.AsGuid() == excludedAppointmentId.Value)
                {
                    continue;
                }

                var operationRequestId = appointment.OperationRequestId;

                if (!operationRequests.ContainsKey(operationRequestId))
                {
                    throw new NullReferenceException($"OperationRequest with ID {operationRequestId} not found.");
                }

                var operationTypeId = operationRequests[operationRequestId];

                if (!operationTypes.ContainsKey(operationTypeId))
                {
                    throw new NullReferenceException($"OperationType with ID {operationTypeId} not found.");
                }

                var duration = operationTypes[operationTypeId];

                var staffAppointmentStartTime = appointment.Slot.TimeInterval.Start;
                var staffAppointmentEndTime = appointment.Slot.TimeInterval.End;

                if (requestedStartTime < staffAppointmentEndTime && requestedEndTime > staffAppointmentStartTime)
                {
                    return false;
                }
            }

            return true;
        }

    }
}
