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

        public async Task<AuthServiceModel> GetUser(string email)
        {
            var apiUrl = $"{email}";
            var response = _httpClient.GetAsync(apiUrl).Result;
            var json = response.Content.ReadAsStringAsync().Result;

            var userResult = JsonConvert.DeserializeObject<AuthServiceModel>(json);
            
            return userResult;
        }
    }
}