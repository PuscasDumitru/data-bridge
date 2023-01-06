using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Enums;

namespace Data.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public string UserName { get; set; }
        public Role? Role { get; set; }
        public Guid? WorkspaceId { get; set; }
        public Workspace Workspace { get; set; }

    }
}
