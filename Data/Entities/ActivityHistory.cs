using Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Entities
{
    public class ActivityHistory
    {
        public Guid? Id { get; set; }
        public string UserName { get; set; }
        public string EntityName { get; set; }
        public EntityType EntityType { get; set; }
        public Enums.Action Action { get; set; }
        public string ActionPerformedTime { get; set; }
        public Guid? WorkspaceId { get; set; }
        public Workspace Workspace { get; set; }
    }
}
