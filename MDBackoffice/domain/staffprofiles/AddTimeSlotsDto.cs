namespace MDBackoffice.Domain.StaffProfiles
{
    public class AddTimeSlotsDto(string slot, string date)
    {
        public string Slot { get; set; } = slot;
        public string Date { get; set; } = date;   
    }
}