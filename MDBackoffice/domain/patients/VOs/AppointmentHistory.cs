using System;
using MDBackoffice.Domain.Patients;
using MDBackoffice.Domain.Shared;

public class AppointmentHistory : Entity<AppointmentHistoryId> 
{
    public string ObjectId { get; set; }
    public AppointmentHistoryStatus Status { get; set; }
    public AppointmentHistoryType Type { get; set; }
    public Date CreatedAt { get; set; }
    public MedicalRecordNumber PatientId { get; set; }

    public AppointmentHistory()
    {
    
    }

    public AppointmentHistory(string id, int status, int type, MedicalRecordNumber patientId)
    { 
        this.Id = new AppointmentHistoryId(RandomSequenceGenerator.GenerateUniqueRandomSequence(5).ToString(), true);
        ObjectId = id;
        Status = (AppointmentHistoryStatus)status;
        Type = (AppointmentHistoryType)type;
        CreatedAt = new Date(DateTime.Now.ToString());
        PatientId = patientId;
    }

    public void UpdateStatus(AppointmentHistoryStatus newStatus)
    {
        Status = newStatus;
    }

    public void UpdateType(AppointmentHistoryType newType)
    {
        Type = newType;
    }
}
