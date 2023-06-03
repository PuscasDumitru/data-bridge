using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DTOs
{
     public class CronParamsModel
     {
        public string ConnectionString { get; set; }
        public int DbType { get; set; }
        public string EmailList { get; set; }
        public string CronExpression { get; set; }
        public Guid QueryId { get; set; }

     }
}
