namespace MDBackoffice.Domain.Logs
{
public class LogDto(string id, string objectClass, string objectReference, string typeOfChange, string changeDescription, string dateOfChange)
    {
        public string Id { get; set; } = id;
        public string TypeChange { get; set; } = typeOfChange;
        public string ChangeOfDescription { get; set; } = changeDescription;
        public string ObjectReference { get; set; } = objectReference;
        public string ObjectClass { get; set; } = objectClass;
        public string DateOfChange { get; set; } = dateOfChange;

    }

}