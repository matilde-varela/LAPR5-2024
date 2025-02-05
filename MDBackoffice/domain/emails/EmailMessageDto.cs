namespace MDBackoffice.Domain.Emails
{
public class EmailMessageDto(string sender, string recipient, string title, string body)
    {
        public string SenderEmail { get; set; } = sender;
        public string RecipientEmail { get; set; } = recipient;
        public string EmailSubject { get; set; } = title;
        public string EmailBody { get; set; } = body;
    }

}