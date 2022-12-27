using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class Query
    {
        public Guid? Id { get; set; }
        //public int CollectionId { get; set; }
        public string Name { get; set; }
        public string RawSql { get; set; }
        public string DefaultResponseWithLimit { get; set; }
        public string Documentation { get; set; }
        public double? LastExecuteTime { get; set; }
        public int? Count { get; set; }
        public int? Size { get; set; }
        public Guid? FolderId { get; set; }
        public Folder Folder { get; set; }
        //public virtual History History { get; set; }
    }
}
