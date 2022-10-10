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
        GenericRepository<Collection> _collectionRepository;
        GenericRepository<Folder> _folderRepository;
        GenericRepository<History> _historyRepository;
        GenericRepository<Query> _queryRepository;
        GenericRepository<Workspace> _workspaceRepository;

        public UnitOfWork(RepositoryDbContext dbContext) => _dbContext = dbContext;

        public GenericRepository<Collection> CollectionRepository
        {
            get
            {
                _collectionRepository ??= new GenericRepository<Collection>(_dbContext);
                return _collectionRepository;
            }
        }

        public GenericRepository<Folder> FolderRepository
        {
            get
            {
                _folderRepository ??= new GenericRepository<Folder>(_dbContext);
                return _folderRepository;
            }
        }

        public GenericRepository<Query> QueryRepository
        {
            get
            {
                _queryRepository ??= new GenericRepository<Query>(_dbContext);
                return _queryRepository;
            }
        }

        public GenericRepository<Workspace> WorkspaceRepository
        {
            get
            {
                _workspaceRepository ??= new GenericRepository<Workspace>(_dbContext);
                return _workspaceRepository;
            }
        }

        public GenericRepository<History> HistoryRepository
        {
            get
            {
                _historyRepository ??= new GenericRepository<History>(_dbContext);
                return _historyRepository;
            }
        }
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
            _dbContext.SaveChangesAsync(cancellationToken);
    }
}
