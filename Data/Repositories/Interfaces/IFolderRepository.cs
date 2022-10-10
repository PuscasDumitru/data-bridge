using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Data.Entities;

namespace Data.Repositories.Interfaces
{
    public interface IFolderRepository : IGenericRepository<Folder>
    {
        Task<IEnumerable<Folder>> GetAllFoldersAsync(CancellationToken cancellationToken = default);
        Task<Folder> GetFolderByIdAsync(int folderId, CancellationToken cancellationToken = default);
    }

}
