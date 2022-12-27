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

        public async Task<IEnumerable<Workspace>> GetWorkspacesByUserIdAsync(Guid userId)
        {
            return await GetByCondition(workspace => workspace.UserId.Equals(userId))
                .Include(col => col.Collaborators)
                .Include(c => c.Collections)
                .ThenInclude(f => f.Folders)
                .ThenInclude(q => q.Queries)
                .ToListAsync();

        }

        //public Workspace UpdateEntity(Workspace originalWorkspace, Workspace updatedWorkspace)
        //{
        //    foreach (var property in updatedWorkspace.GetType().GetProperties())
        //    {
        //        if (property.GetValue(updatedWorkspace, null) == null)
        //        {
        //            property.SetValue(updatedWorkspace, originalWorkspace.GetType().GetProperty(property.Name)
        //            .GetValue(originalWorkspace, null));
        //        }
        //    }
        //    return updatedWorkspace;
        //}

        public void AddUser(User user)
        {
            RepositoryContext.User.Add(user);
        }
    }
}
