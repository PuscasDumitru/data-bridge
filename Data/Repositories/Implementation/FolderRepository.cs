using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.Implementation
{
    public class FolderRepository : GenericRepository<Folder>
    {
        public FolderRepository(RepositoryDbContext repositoryContext) : base(repositoryContext) { }

        public async Task<Folder> GetFolderByIdAsync(Guid folderId)
        {
            return await GetByCondition(folder => folder.Id.Equals(folderId))
                .Include(q => q.Queries)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Folder>> GetAllFoldersAsync()
        {
            return await GetAll()
                .Include(q => q.Queries)
                .ToListAsync();

        }
    }
}
