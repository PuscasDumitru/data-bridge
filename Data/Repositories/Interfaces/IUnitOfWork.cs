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
        CollectionRepository CollectionRepository { get; }
        FolderRepository FolderRepository { get; }
        HistoryRepository HistoryRepository { get; }
        QueryRepository QueryRepository { get; }
        WorkspaceRepository WorkspaceRepository { get; }
        UserRepository UserRepository { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
