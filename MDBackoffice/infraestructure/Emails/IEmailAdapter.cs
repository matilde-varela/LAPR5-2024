using System.Threading.Tasks;
using MimeKit;

namespace MDBackoffice.Infrastructure.Emails
{
    public interface IEmailAdapter
    {
        
        Task SendEmail(MimeMessage message);

    }
}