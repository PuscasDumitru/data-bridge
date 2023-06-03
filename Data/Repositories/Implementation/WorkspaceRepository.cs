using Data.DTOs;
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
                   .Include(hist => hist.ActivityHistories)
                   .Include(c => c.Collections)
                   .ThenInclude(f => f.Folders)
                   .ThenInclude(q => q.Queries)
                   .ThenInclude(vrs => vrs.QueryVersions)
                   
                  .Include(c => c.Collections)
                  .ThenInclude(f => f.Folders)
                  .ThenInclude(q => q.Queries)
                  .ThenInclude(x => x.CronJob)
                   //.ThenInclude(x => x.CronJob)
                   // add only for versions
                   .FirstOrDefaultAsync();
          }


          public async Task<IEnumerable<object>> GetWorkspacesByEmailAsync(string email)
          {

               var workspaces = await GetByCondition(workspace => workspace.Collaborators.Any(user => user.Email == email))
                         .ToListAsync();

               
               var ret = from workspace in workspaces
                         join user in RepositoryContext.User on workspace.Id equals user.WorkspaceId
                         join collection in RepositoryContext.Collection on workspace.Id equals collection.WorkspaceId
                         join folder in RepositoryContext.Folder on collection.Id equals folder.CollectionId
                         //join query in RepositoryContext.Query on folder.Id equals query.FolderId

                              select new
                              {
                                   workspace,

                                   folder.Queries
                                   //Collections = workspace.Collections,
                                   //QueryVersion = queryVersion.Version
                              };

               //     await GetByCondition(workspace => workspace.Collaborators.Any(user => user.Email == email))
               //.Include(col => col.Collaborators)
               //.Include(hist => hist.ActivityHistories)
               //.Include(c => c.Collections)
               //.ThenInclude(f => f.Folders)
               //.ThenInclude(q => q.Queries)
               //.ThenInclude(vrs => vrs.QueryVersions)
               //add only for versions

               //.Include(c => c.Collections)
               //.ThenInclude(f => f.Folders)
               //.ThenInclude(q => q.Queries)
               //.ThenInclude(x => x.CronJob)

               //.ToListAsync();

          }

     }
}
