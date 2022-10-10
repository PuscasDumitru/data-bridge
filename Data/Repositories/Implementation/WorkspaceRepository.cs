using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Data.Entities;
using Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories.Implementation
{
    internal sealed class WorkspaceRepository : GenericRepository<Workspace>, IWorkspaceRepository
    {
        public WorkspaceRepository(RepositoryDbContext repositoryContext)
            : base(repositoryContext)
        {
        }
        public async Task<IEnumerable<Workspace>> GetAllWorkspacesAsync(CancellationToken cancellationToken = default)
        {
            return await FindAll()
                .OrderBy(workspace => workspace.Id)
                .ToListAsync(cancellationToken);
        }
        public async Task<Workspace> GetWorkspaceByIdAsync(int workspaceId, CancellationToken cancellationToken = default)
        {
            return await FindByCondition(workspace => workspace.Id.Equals(workspaceId))
                .FirstOrDefaultAsync(cancellationToken);
        }
        public override void Create(Workspace workspace)
        {
            base.Create(workspace);
        }
        public override void Update(Workspace workspace)
        {
            base.Update(workspace);
        }
        public override void Delete(Workspace workspace)
        {
            base.Delete(workspace);
        }
    }
}
