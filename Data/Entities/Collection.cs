using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class Collection
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public bool IsFavorite { get; set; }
        public string ShareLink { get; set; }
        public ICollection<Workspace> Workspaces { get; set; }
        public ICollection<Folder> Folders { get; set; }
    }
}
