using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class Folder
    {
        public Folder()
        {
            this.Queries = new HashSet<Query>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Documentation { get; set; }
        public Guid CollectionId { get; set; }
        public virtual Collection Collection { get; set; }
        public virtual ICollection<Query> Queries { get; set; }
    }
}
