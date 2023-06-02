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
        IGenericRepository<Collection> CollectionRepository { get; }
        IGenericRepository<Folder> FolderRepository { get; }
        IGenericRepository<CronJob> CronRepository { get; }
        IGenericRepository<ActivityHistory> ActivityHistoryRepository { get; }
        QueryRepository QueryRepository { get; }
        WorkspaceRepository WorkspaceRepository { get; }
        UserRepository UserRepository { get; }
        UserEmailConfirmationRepository UserEmailConfirmationRepository { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
