using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
     public class QueryVersion
     {
          public Guid Id { get; set; }
          public string Version { get; set; }
          public string RawSql { get; set; }
          public Guid QueryId { get; set; }
          public Query Query { get; set; }
     }
}
