using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MDBackoffice.Domain.Appointments;
using MDBackoffice.Domain.Rooms;
using MDBackoffice.Domain.Shared;
using MDBackoffice.Infrastructure.Shared;
using Microsoft.EntityFrameworkCore;

namespace MDBackoffice.Infrastructure.Rooms
{
    public class RoomRepository : BaseRepository<Room, RoomNumber>, IRoomRepository
    {
        private readonly MDBackofficeDbContext _context;
        public RoomRepository(MDBackofficeDbContext context) : base(context.Rooms)
        {
            _context = context;
        }

        public async Task<List<Room>> GetAllRoomsAsync()
        {
            return await _context.Rooms
                .ToListAsync();
        }

        public async Task<bool> IsRoomAvailableAsync(RoomNumber roomNumber, string startTime, string endTime, Guid? excludedAppointmentId = null)
        {
            if (!TimeSpan.TryParse(startTime, out var requestedStartTime) ||
                !TimeSpan.TryParse(endTime, out var requestedEndTime))
            {
                throw new ArgumentException("Horário inválido. Certifique-se de usar o formato correto de tempo (HH:mm).");
            }

            var appointments = await _context.Appointments
                .Where(a => a.RoomNumber == roomNumber && a.Status.Description == AppointmentStatus.Scheduled.Description)
                .ToListAsync();

            foreach (var appointment in appointments)
            {
                if (excludedAppointmentId.HasValue && appointment.Id.AsGuid() == excludedAppointmentId.Value)
                    continue;

                var operationRequest = await _context.OperationRequests
                    .Where(or => or.Id == appointment.OperationRequestId)
                    .FirstOrDefaultAsync();

                if (operationRequest == null)
                    continue;

                var estimatedDuration = await _context.OperationTypes
                    .Where(ot => ot.Id == operationRequest.OperationTypeId)
                    .Select(ot => ot.EstimatedDuration.TotalDurationMinutes)
                    .FirstOrDefaultAsync();

                var appointmentStart = appointment.Slot.TimeInterval.Start;
                var appointmentEnd = appointment.Slot.TimeInterval.End;

                if (requestedStartTime < appointmentEnd && appointmentStart < requestedEndTime)
                {
                    return false;
                }
            }

            return true;
        }
    }
}