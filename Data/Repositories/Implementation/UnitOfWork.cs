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
        QueryRepository _queryRepository;
        UserRepository _userRepository;
        WorkspaceRepository _workspaceRepository;

        public UnitOfWork(RepositoryDbContext dbContext) => _dbContext = dbContext;

        public IGenericRepository<Collection> CollectionRepository
        {
            get
            {
                _collectionRepository ??= new GenericRepository<Collection>(_dbContext);
                return _collectionRepository;
            }
        }

        public IGenericRepository<Folder> FolderRepository
        {
            get
            {
                _folderRepository ??= new GenericRepository<Folder>(_dbContext);
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

        public UserRepository UserRepository
        {
            get
            {
                _userRepository ??= new UserRepository(_dbContext);
                return _userRepository;
            }
        }

        public IGenericRepository<History> HistoryRepository
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
