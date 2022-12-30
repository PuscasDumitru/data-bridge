using System;
using System.Net.Http;
using System.Threading.Tasks;
using Data.Entities;
using Newtonsoft.Json;
using Teza.Models;

namespace Teza.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<AuthServiceModel> GetUser(Guid userId)
        {
            var apiUrl = $"{userId}";
            var response = _httpClient.GetAsync(apiUrl).Result;
            var json = response.Content.ReadAsStringAsync().Result;

            var obj2 = JsonConvert.DeserializeObject<AuthServiceModel>(json);
            var obj = JsonConvert.DeserializeObject<SuccessModel>(json);
            
            return obj2;
        }
    }
}