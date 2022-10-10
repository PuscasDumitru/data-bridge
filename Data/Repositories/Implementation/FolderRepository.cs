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
    internal sealed class FolderRepository : GenericRepository<Folder>, IFolderRepository
    {
        public FolderRepository(RepositoryDbContext repositoryContext)
            : base(repositoryContext)
        {
        }
        public async Task<IEnumerable<Folder>> GetAllFoldersAsync(CancellationToken cancellationToken = default)
        {
            return await FindAll()
                .OrderBy(folder => folder.Id)
                .ToListAsync(cancellationToken);
        }
        public async Task<Folder> GetFolderByIdAsync(int folderId, CancellationToken cancellationToken = default)
        {
            return await FindByCondition(folder => folder.Id.Equals(folderId))
                .FirstOrDefaultAsync(cancellationToken);
        }
        public override void Create(Folder folder)
        {
            base.Create(folder);
        }
        public override void Update(Folder folder)
        {
            base.Update(folder);
        }
        public override void Delete(Folder folder)
        {
            base.Delete(folder);
        }
    }
}
