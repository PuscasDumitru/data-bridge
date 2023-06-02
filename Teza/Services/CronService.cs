using Data.Entities;
using Data.Repositories.Implementation;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Text;
using System;
using Teza.Models;
using Microsoft.Extensions.Configuration;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Data;

namespace Teza.Services
{
     public class CronService : ICronService
     {
          private readonly HttpClient _httpClient;
          IConfiguration Configuration { get; }

          public CronService(IConfiguration configuration, HttpClient httpClient)
          {
               _httpClient = httpClient;
               Configuration = configuration;
          }

          public string SetUpCron(string connectionString, int dbType, string emailList, string cronExpresion, Guid queryId)
          {
               try
               {
                    var link = Configuration["CronService"] + $"SetupCron?ConnectionString={connectionString}&DbType={dbType}&EmailList={emailList}" +
                         $"&CronExpresion={cronExpresion}&QueryId={queryId}";

                    var payload = new StringContent("", Encoding.UTF8, "application/json");
                    var response = _httpClient.PostAsync(link, payload).Result;
                    var json = response.Content.ReadAsStringAsync().Result;

                    var cronResult = JsonConvert.DeserializeObject<SuccessModel>(json);

                    if (cronResult.data != null )
                    {
                         return cronResult.data.ToString();
                    }

                    return string.Empty;
               }
               catch (Exception e)
               {
                    return string.Empty;
               }
          }

          public bool StopCron(Guid cronId)
          {
               try
               {
                    var link = Configuration["CronService"] + $"StopCron/{cronId}";

                    var payload = new StringContent("", Encoding.UTF8, "application/json");
                    var response = _httpClient.PostAsync(link, payload).Result;
                    var json = response.Content.ReadAsStringAsync().Result;

                    var cronResult = JsonConvert.DeserializeObject<SuccessModel>(json);

                    if (cronResult.success)
                    {
                         return true;
                    }

                    return false;
               }
               catch (Exception e)
               {
                    return false;
               }
          }
     }
}
