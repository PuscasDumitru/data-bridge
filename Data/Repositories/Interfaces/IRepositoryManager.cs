using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.Interfaces
{
    public interface IRepositoryManager
    {
        ICollectionRepository CollectionRepository { get; }
        IQueryRepository QueryRepository { get; }
        IWorkspaceRepository WorkspaceRepository { get; }
        IHistoryRepository HistoryRepository { get; }
        IFolderRepository FolderRepository { get; }
        IUnitOfWork UnitOfWork { get; }
    }
}
