namespace MDBackoffice.Domain.OperationRequests
{
    public class UpdateOperationRequestDto
    {
        public string Id { get; set; }

        public string DeadLineDate { get; set; }

        public string Priority { get; set; }

        public string Description { get; set; }


        public UpdateOperationRequestDto(string id, string deadLineDate, string priority, string description)
        {
            this.Id = id;
            this.DeadLineDate = deadLineDate;
            this.Priority = priority;
            this.Description = description;
        }
    }
}