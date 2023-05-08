using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Data.Entities;
using Data.Repositories.Implementation;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Teza.Models;

namespace Teza.Services
{
    public class MailingService : IMailingService
    {
        private const int TOKEN_EXPIRY_TIME_IN_HOURS = 2;
        private readonly HttpClient _httpClient;
        IConfiguration Configuration { get; }

        public MailingService(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            Configuration = configuration;
        }

        public bool SendMail(string email, string workspaceName, Guid workspaceId, UserEmailConfirmationRepository userEmailConfirmationRepository)
        {
            try
            {
                var link = Configuration["MainApi"];

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Expires = DateTime.UtcNow.AddHours(TOKEN_EXPIRY_TIME_IN_HOURS)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var stringToken = tokenHandler.WriteToken(token);

                var mailToSend = new MailingServiceModel
                {
                    ToEmail = email,
                    Subject = "Invitation to join a workspace!",
                    Body =
                        $"Hello, you have been invited to join the workspace <b>{workspaceName}</b>. " +
                        $"Click the following link to accept the invitation: <a href=\"{link}/{workspaceId}/{email}/{stringToken}\"> Join {workspaceName} </a>"
                };
                var serializedMail = JsonConvert.SerializeObject(mailToSend);
                var payload = new StringContent(serializedMail, Encoding.UTF8, "application/json");
                var response = _httpClient.PostAsync("", payload).Result;
                var json = response.Content.ReadAsStringAsync().Result;

                var inviteResult = JsonConvert.DeserializeObject<SuccessModel>(json);

                var userEmailConfirmation = new UserEmailConfirmation
                {
                    Email = email,
                    EmailConfirmationToken = stringToken,
                    ValidTo = token.ValidTo
                };

                userEmailConfirmationRepository.Create(userEmailConfirmation);

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
