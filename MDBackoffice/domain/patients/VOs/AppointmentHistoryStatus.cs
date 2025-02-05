using System.ComponentModel;

public enum AppointmentHistoryStatus
{
    [Description("Requested")]
    Requested = 1,
    [Description("Scheduled")]    
    Scheduled = 2,    
    [Description("Completed")]
    Completed = 3,   

}
