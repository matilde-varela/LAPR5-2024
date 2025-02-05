using System;
using System.Text.Json.Serialization;
using MDBackoffice.Domain.Appointments;
using MDBackoffice.Domain.Shared;
using MDBackoffice.Domain.StaffProfiles;

namespace MDBackoffice.Domain.AppointmentStaffs
{
     public class AppointmentStaffId : EntityId {

    public AppointmentId AppointmentId { get; private set; }

    public StaffId StaffId { get; private set; }

    public AppointmentStaffId(string id): base(id) {
        var parts = id.Split('-');
        if (parts.Length == 2) {
            AppointmentId = new AppointmentId(parts[0]);
            StaffId = new StaffId(parts[1]);
        }
    }

    [JsonConstructor]
    public AppointmentStaffId(AppointmentId appointmentId, StaffId staffId) : base($"{appointmentId.AsGuid()}-{staffId.Value}") {
        AppointmentId = appointmentId;
        StaffId = staffId;
    }
    public override string AsString() {
        return $"{AppointmentId.AsGuid()}-{StaffId.Value}";
    }

    protected override object createFromString(String text) {
        return text;
    }

  }
}