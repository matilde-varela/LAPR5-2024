using MDBackoffice.Domain.Emails;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace MDBackoffice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailTestController : ControllerBase
    {
        private readonly EmailService _service;

        public EmailTestController(EmailService service)
        {
            _service = service;
        }

        [HttpPost("send-email-google-gmail")]
        public async Task<IActionResult> SendTestEmail(EmailMessageDto messageDto)
        {
            await _service.SendAccountDeletionEmail(messageDto);
            return Ok();
        }
    }
}
