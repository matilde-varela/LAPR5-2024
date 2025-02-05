using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using MimeKit;

namespace MDBackoffice.Infrastructure.Users
{
    public interface ILoginAdapter
    {
        Task<AuthenticateResult> GetAuthenticationInfo();
        Task<AuthenticationProperties> GetRedirectionInfo();
    }
}

