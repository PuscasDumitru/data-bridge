using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Data.Repositories.Implementation;

namespace Data.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        GenericRepository<Collection> CollectionRepository { get; }
        GenericRepository<Folder> FolderRepository { get; }
        GenericRepository<History> HistoryRepository { get; }
        GenericRepository<Query> QueryRepository { get; }
        GenericRepository<Workspace> WorkspaceRepository { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
