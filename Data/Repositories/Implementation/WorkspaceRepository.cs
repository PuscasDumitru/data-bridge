using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.Implementation
{
    public class WorkspaceRepository : GenericRepository<Workspace>
    {
        public WorkspaceRepository(RepositoryDbContext repositoryContext) : base(repositoryContext) { }

        public async Task<Workspace> GetWorkspaceByIdAsync(Guid workspaceId)
        {
            return await GetByCondition(workspace => workspace.Id.Equals(workspaceId))
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Workspace>> GetAllWorkspacesAsync()
        {
            return await GetAll()
                .ToListAsync();

        }
    }
}
