using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class Collection
    {
        public Collection()
        {
            this.Folders = new HashSet<Folder>();
        }

        public Guid? Id { get; set; }
        public string Name { get; set; }
        public string Documentation { get; set; }
        public bool? IsFavorite { get; set; }
        public string ShareLink { get; set; }
        public Guid? WorkspaceId { get; set; }
        public Workspace Workspace { get; set; }
        public ICollection<Folder> Folders { get; set; }
    }
}
