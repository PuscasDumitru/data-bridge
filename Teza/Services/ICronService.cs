using Data.DTOs;
using System;
using System.Threading.Tasks;
using Teza.Models;

namespace Teza.Services
{
     public interface ICronService
     {
          string SetUpCron(CronParamsModel cronParams);
          bool StopCron(Guid cronId);
     }
}
