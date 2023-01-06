using Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class History
    {
        //[ForeignKey("Query")]
        
        public Guid? Id { get; set; }
        //public int UserId { get; set; }
        //public int QueryId { get; set; }
        //public Action Action { get; set; }
    }
}
