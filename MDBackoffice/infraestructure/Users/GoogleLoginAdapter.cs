using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using MailKit.Security;
using MDBackoffice.Domain.Users;
using MDBackoffice.Infrastructure.Users;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using MimeKit;
using Newtonsoft.Json.Linq;

namespace MDBackoffice.Infrastructure.Users
{
    public class GoogleLoginAdapter : ILoginAdapter
    {
        private readonly SignInManager<User> _signinManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUrlHelperFactory _urlHelperFactory;

        public GoogleLoginAdapter(IHttpContextAccessor httpContextAccessor, IUrlHelperFactory urlHelperFactory, SignInManager<User> signInManager)
        {
            _signinManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
            _urlHelperFactory = urlHelperFactory;
        }

        public async Task<AuthenticateResult> GetAuthenticationInfo()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null)
                return null;

            // Create the IUrlHelper
            var urlHelper = _urlHelperFactory.GetUrlHelper(new ActionContext
            {
                HttpContext = httpContext,
                RouteData = httpContext.GetRouteData(),
                ActionDescriptor = new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()
            });

            // Authenticate the user with Google
            AuthenticateResult result = await httpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (result.Succeeded && result.Principal != null)
            {
                return result;
            }

            return null;
        }

        public async Task<AuthenticationProperties> GetRedirectionInfo()
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null)
                return null;

            // Create the IUrlHelper
            var urlHelper = _urlHelperFactory.GetUrlHelper(new ActionContext
            {
                HttpContext = httpContext,
                RouteData = httpContext.GetRouteData(),
                ActionDescriptor = new Microsoft.AspNetCore.Mvc.Abstractions.ActionDescriptor()
            });
            var redirectUrl = "https://localhost:5001/api/login-externalEnd";
            var properties = _signinManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);

            return properties;
        }

        private string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                // Use the first IPv4 address found
                if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No IPv4 address found for this machine.");
        }
    }
}
