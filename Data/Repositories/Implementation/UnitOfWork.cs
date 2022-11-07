using Data.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Data.Entities;

namespace Data.Repositories.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        readonly RepositoryDbContext _dbContext;
        CollectionRepository _collectionRepository;
        FolderRepository _folderRepository;
        HistoryRepository _historyRepository;
        QueryRepository _queryRepository;
        WorkspaceRepository _workspaceRepository;

        public UnitOfWork(RepositoryDbContext dbContext) => _dbContext = dbContext;

        public CollectionRepository CollectionRepository
        {
            get
            {
                _collectionRepository ??= new CollectionRepository(_dbContext);
                return _collectionRepository;
            }
        }

        public FolderRepository FolderRepository
        {
            get
            {
                _folderRepository ??= new FolderRepository(_dbContext);
                return _folderRepository;
            }
        }

        public QueryRepository QueryRepository
        {
            get
            {
                _queryRepository ??= new QueryRepository(_dbContext);
                return _queryRepository;
            }
        }

        public WorkspaceRepository WorkspaceRepository
        {
            get
            {
                _workspaceRepository ??= new WorkspaceRepository(_dbContext);
                return _workspaceRepository;
            }
        }

        public HistoryRepository HistoryRepository
        {
            get
            {
                _historyRepository ??= new HistoryRepository(_dbContext);
                return _historyRepository;
            }
        }
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
            _dbContext.SaveChangesAsync(cancellationToken);
    }
}
