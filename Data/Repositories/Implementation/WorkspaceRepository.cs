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
                .Include(c => c.Collections)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Workspace>> GetWorkspacesByUserIdAsync(Guid userId)
        {
            return await GetByCondition(workspace => workspace.UserId.Equals(userId))
                .Include(c => c.Collections)
                .ToListAsync();

        }
    }
}
