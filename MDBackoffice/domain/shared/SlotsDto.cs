namespace MDBackoffice.Domain.Shared
{
    public class SlotsDto
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string? Name { get; set; }

        // Default Constructor
        public SlotsDto()
        {
        }

        public SlotsDto(string startDate, string endDate, string startTime, string endTime)
        {
            StartDate = startDate;
            EndDate = endDate;
            StartTime = startTime;
            EndTime = endTime;
        }
        public SlotsDto(string startDate, string endDate, string startTime, string endTime, string name)
        {
            StartDate = startDate;
            EndDate = endDate;
            StartTime = startTime;
            EndTime = endTime;
            Name = name;
        }
    }
}