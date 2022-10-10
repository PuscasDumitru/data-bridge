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
        [ForeignKey("Query")]
        
        public int Id { get; set; }
        //public int UserId { get; set; }
        //public int QueryId { get; set; }
        public Action Action { get; set; }
        public virtual Query Query { get; set; }
        //public virtual User User { get; set; }
    }
}
