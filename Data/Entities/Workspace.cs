using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Data.Entities
{
    [JsonObject(IsReference = false)]
    public class Workspace
    {
        public Workspace()
        {
            this.Collections = new HashSet<Collection>();
            this.Collaborators = new HashSet<User>();
        }

        public Guid? Id { get; set; }
        
        public string Name { get; set; }
        public string DbConnectionString { get; set; }
        public string EnvVariables { get; set; }
        //public string Documentation { get; set; }
        //public string DefaultConfigsForQueries { get; set; }
        
        //public int? UsersLimit { get; set; }
        //public string InviteLink { get; set; }
        public ICollection<Collection> Collections { get; set; }

        public Guid? UserId { get; set; }
        public ICollection<User> Collaborators { get; set; }
    }
}
