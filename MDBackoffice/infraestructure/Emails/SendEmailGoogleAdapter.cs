using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Newtonsoft.Json.Linq;

namespace MDBackoffice.Infrastructure.Emails
{
    public class SendEmailGoogleAdapter : IEmailAdapter
    {

        private readonly IConfiguration _configuration;
        private readonly string CLIENT_ID;
        private readonly string CLIENT_SECRET;
        private readonly string REFRESH_TOKEN;
        private readonly string TOKEN_END_POINT;
        private readonly string TEST_USER_EMAIL;

        public SendEmailGoogleAdapter(){ }

        public SendEmailGoogleAdapter(IConfiguration configuration)
        {  
            _configuration = configuration;
            CLIENT_ID = _configuration.GetSection("GoogleKeys").GetSection("ClientId").Value;
            CLIENT_SECRET = _configuration.GetSection("GoogleKeys").GetSection("ClientSecret").Value;
            REFRESH_TOKEN = _configuration.GetSection("GoogleKeys").GetSection("RefreshToken").Value;
            TEST_USER_EMAIL = _configuration.GetSection("App").GetSection("Email").Value;
            TOKEN_END_POINT = _configuration.GetSection("GoogleKeys").GetSection("TokenEndPoint").Value;
        }

        public async Task SendEmail(MimeMessage message)
        {
            string accessToken = await GetExternalEmailServiceAccessToken();

            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            var oauth2 = new SaslMechanismOAuth2(TEST_USER_EMAIL, accessToken);
            smtp.Authenticate(oauth2);
            smtp.Send(message);
            smtp.Disconnect(true);
        }



         private async Task<string> GetExternalEmailServiceAccessToken()
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, TOKEN_END_POINT);

            var body = new Dictionary<string, string>
            {
                { "client_id", CLIENT_ID },
                { "client_secret", CLIENT_SECRET },
                { "refresh_token", REFRESH_TOKEN },
                { "grant_type", "refresh_token" }
            };

            request.Content = new FormUrlEncodedContent(body);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await client.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new System.Exception($"Failed to refresh token: {response.StatusCode}, Details: {errorContent}");
            }

            var content = await response.Content.ReadAsStringAsync();
            var tokenData = JObject.Parse(content);

            return tokenData["access_token"].ToString();
        }


    }


}