using System.Threading.Tasks;
using MimeKit;
using MimeKit.Text;
using MDBackoffice.Domain.Tokens;
using MDBackoffice.Domain.Shared;
using MDBackoffice.Infrastructure.Emails;

namespace MDBackoffice.Domain.Emails
{
    public class EmailService
    {
        private readonly TokenService _service;
        private readonly IEmailAdapter _adapter;

        public EmailService(TokenService service, IEmailAdapter adapter)
        {
            this._service = service;
            this._adapter = adapter;
        }

        public async Task SendConfirmationEmail(EmailMessageDto emailDto) {

            //to confirm email formatting
            var senderEmail = new Email(emailDto.SenderEmail);
            var recipientEmail = new Email(emailDto.RecipientEmail);

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("HealthCare Clinic", senderEmail.EmailAddress));
            email.To.Add(MailboxAddress.Parse(recipientEmail.EmailAddress));
            email.Subject = emailDto.EmailSubject;

            // can be TextFormat Plain but changing it to Html allow us to better structure the email content 
            email.Body = new TextPart(TextFormat.Html) { Text = emailDto.EmailBody };

            await _adapter.SendEmail(email);
        }

        public async Task SendAccountDeletionEmail(EmailMessageDto emailDto) {

            TokenDto token = await _service.CreateAccountDeletionToken(emailDto.RecipientEmail);

            //to confirm email formatting
            var senderEmail = new Email(emailDto.SenderEmail);
            var recipientEmail = new Email(emailDto.RecipientEmail);

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("HealthCare Clinic", senderEmail.EmailAddress));
            email.To.Add(MailboxAddress.Parse(recipientEmail.EmailAddress));
            email.Subject = emailDto.EmailSubject;

            // can be TextFormat Plain but changing it to Html allow us to better structure the email content 
            email.Body = new TextPart(TextFormat.Html) { Text = emailDto.EmailBody};

            await _adapter.SendEmail(email);
        }

        public async Task SendProfileEditEmail(EmailMessageDto emailDto)
        {
            //to confirm email formatting
            var senderEmail = new Email(emailDto.SenderEmail);
            var recipientEmail = new Email(emailDto.RecipientEmail);

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("HealthCare Clinic", senderEmail.EmailAddress));
            email.To.Add(MailboxAddress.Parse(recipientEmail.EmailAddress));
            email.Subject = emailDto.EmailSubject;

            // can be TextFormat Plain but changing it to Html allow us to better structure the email content 
            email.Body = new TextPart(TextFormat.Html) { Text = emailDto.EmailBody };

            await _adapter.SendEmail(email);
        }
        

    }

}