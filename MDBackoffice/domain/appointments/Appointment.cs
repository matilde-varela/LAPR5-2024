using System;
using System.Collections.Generic;
using System.Linq;
using MDBackoffice.Domain.AppointmentStaffs;
using MDBackoffice.Domain.OperationRequests;
using MDBackoffice.Domain.OperationTypes.ValueObjects.RequiredStaff;
using MDBackoffice.Domain.Rooms;
using MDBackoffice.Domain.Shared;
using MDBackoffice.Domain.StaffProfiles;
using Org.BouncyCastle.Asn1.Cms;

namespace MDBackoffice.Domain.Appointments
{
    public class Appointment : Entity<AppointmentId>, IAggregateRoot
    {
        public AppointmentStatus Status { get; set; }

        public OperationRequestId OperationRequestId { get; private set; }

        public RoomNumber RoomNumber { get; private set; }

        public Slot Slot { get; private set; }
        public ICollection<AppointmentStaff> AppointmentStaffs {get; private set;} = new List<AppointmentStaff>();

        public Appointment()
        {

        }

        public Appointment(OperationRequestId opRequestId, RoomNumber roomNumber, Slot slot)
        {
            this.Id = new AppointmentId(Guid.NewGuid());
            this.Status = AppointmentStatus.Scheduled;
            this.OperationRequestId = opRequestId;
            this.RoomNumber = roomNumber;
            this.Slot = slot;
        }

        public Appointment(OperationRequestId opRequestId, string roomNumber, string startTime, string endTime, string startDate, string endDate = null)
        {
            this.Id = new AppointmentId(Guid.NewGuid());
            this.Status = AppointmentStatus.GetStatusByDescription("Scheduled");
            this.OperationRequestId = opRequestId;
            this.RoomNumber = new RoomNumber(roomNumber);
            this.Slot = new Slot(startTime, endTime, startDate, endDate);
        }

        public void ChangeStatus(string status)
        {
            this.Status = AppointmentStatus.GetStatusByDescription(status);
        }

        public void ChangeRoom(string roomNumber)
        {
            this.RoomNumber = new RoomNumber(roomNumber);
        }

        public void ChangeSlot(string newStartTime, string newEndTime, string newStartDate, string newEndDate)
        {
            this.Slot = new Slot(newStartTime, newEndTime, newStartDate, newEndDate);
        }

        public void ChangeStaff(List<Staff> newStaffList)
        {
            // Add the new staff
            foreach (var newStaff in newStaffList)
            {
                this.AppointmentStaffs.Add(new AppointmentStaff(this, newStaff));
            }
        }

        public void ClearStaff()
        {
            foreach (var staff in AppointmentStaffs.ToList())
            {
                // Explicitly mark as deleted
                AppointmentStaffs.Remove(staff);
            }
        }

    }
}