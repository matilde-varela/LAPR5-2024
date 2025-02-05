namespace MDBackoffice.Domain.OperationRequests
{
    public class ListOperationRequestDto(string id, string name, string operationType, string status)
    {
        public string Id { get; set; } = id;
        public string PatientName { get; set; } = name;

        public string OperationType { get; set; } = operationType;

        public string Status { get; set; } = status;
    }
}