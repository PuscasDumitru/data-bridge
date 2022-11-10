using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Data.Entities
{
    [JsonObject(IsReference = true)]
    public class Workspace
    {
        public Workspace()
        {
            this.Collections = new HashSet<Collection>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string DbConnectionString { get; set; }
        public string DefaultConfigsForQueries { get; set; }
        public string EnvVariables { get; set; }
        public string Documentation { get; set; }
        public int UsersLimit { get; set; }
        public string InviteLink { get; set; }
        public virtual ICollection<Collection> Collections { get; set; }

        public Guid UserId { get; set; }
        public string Users { get; set; } = "1;2;3;";
        public ICollection<string> UsersList { get { return Users.Split(';'); } }
    }
}
