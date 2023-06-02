using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
     public class CronJob
     {
          public Guid Id { get; set; }
          public string EmailList { get; set; }
          public string CronExpresion { get; set; }
          public Guid QueryId { get; set; }
          public virtual Query QueryEx { get; set; }

     }
}
