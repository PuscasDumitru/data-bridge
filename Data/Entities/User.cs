using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class User
    {
        // primary key from service
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string HashPassword { get; set; }
        //public virtual ICollection<Workspace> Workspaces { get; set; }
       // public virtual History History { get; set; }
    }
}
