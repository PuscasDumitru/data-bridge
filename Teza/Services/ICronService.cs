using System;
using System.Threading.Tasks;
using Teza.Models;

namespace Teza.Services
{
     public interface ICronService
     {
          string SetUpCron(string connectionString, int dbType, string emailList, string cronExpresion, Guid queryId);
          bool StopCron(Guid cronId);
     }
}
