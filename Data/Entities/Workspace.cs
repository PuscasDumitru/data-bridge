using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class Workspace
    {
        public Workspace()
        {
            this.Collections = new HashSet<Collection>();
        }

        public Guid Id { get; set; }
        //public int UserId { get; set; }      
        public string DbConnectionString { get; set; }
        public string DefaultConfigsForQueries { get; set; }
        public string EnvVariables { get; set; }
        public string Documentation { get; set; }
        public int UsersLimit { get; set; }
        public string InviteLink { get; set; }
        public virtual ICollection<Collection> Collections { get; set; }
        //public ICollection<User> Users { get; set; }
    }
}
