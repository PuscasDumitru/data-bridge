using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.DTOs
{
     public class WorkspaceClone
     {
          public Guid? Id { get; set; }

          public string Name { get; set; }
          public string DbConnectionString { get; set; }
          public string EnvVariables { get; set; }
          
          public int DataBaseType { get; set; }
          public ICollection<Collection> Collections { get; set; }

          public Guid? UserId { get; set; }
          public ICollection<User> Collaborators { get; set; }
          public ICollection<ActivityHistory> ActivityHistories { get; set; }
     }
}
