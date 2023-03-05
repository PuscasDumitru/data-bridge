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
                .Include(col => col.Collaborators)
                .Include(c => c.Collections)
                .ThenInclude(f => f.Folders)
                .ThenInclude(q => q.Queries)    
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Workspace>> GetWorkspacesByEmailAsync(string email)
        {
            return await GetByCondition(workspace => workspace.Collaborators.Any(user => user.Email == email))
                .Include(col => col.Collaborators)
                .Include(c => c.Collections)
                .ThenInclude(f => f.Folders)
                .ThenInclude(q => q.Queries)
                .ToListAsync();

        }
        
    }
}
