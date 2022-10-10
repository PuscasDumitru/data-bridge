using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Data.Entities;

namespace Data.Repositories.Interfaces
{
    public interface IWorkspaceRepository : IGenericRepository<Workspace>
    {
        Task<IEnumerable<Workspace>> GetAllWorkspacesAsync(CancellationToken cancellationToken = default);
        Task<Workspace> GetWorkspaceByIdAsync(int workspaceId, CancellationToken cancellationToken = default);
    }
}
