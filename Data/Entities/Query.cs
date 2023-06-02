using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
     public class Query
     {
          public Query() 
          { 
               this.QueryVersions = new HashSet<QueryVersion>();
          }
          public Guid? Id { get; set; }
          public string Name { get; set; }
          public string RawSql { get; set; }
          public int? Count { get; set; }
          public int? Size { get; set; }

          [Column(TypeName = "jsonb")]
          public string Snapshot { get; set; }
          //public string DefaultResponseWithLimit { get; set; }
          public string Documentation { get; set; }
          //public double? LastExecuteTime { get; set; }

          public Guid? CollectionId { get; set; }
          public Guid? FolderId { get; set; }
          public Folder Folder { get; set; }

          public virtual CronJob CronJob { get; set; }

          public ICollection<QueryVersion> QueryVersions { get; set; }
     }
}
