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
          GenericRepository<CronJob> _cronRepository;
          GenericRepository<ActivityHistory> _activityHistoryRepository;
          QueryRepository _queryRepository;
          UserRepository _userRepository;
          WorkspaceRepository _workspaceRepository;
          UserEmailConfirmationRepository _userEmailConfirmationRepository;

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

          public IGenericRepository<CronJob> CronRepository
          {
               get
               {
                    _cronRepository ??= new GenericRepository<CronJob>(_dbContext);
                    return _cronRepository;
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

          public UserEmailConfirmationRepository UserEmailConfirmationRepository
          {
               get
               {
                    _userEmailConfirmationRepository ??= new UserEmailConfirmationRepository(_dbContext);
                    return _userEmailConfirmationRepository;
               }
          }

          public IGenericRepository<ActivityHistory> ActivityHistoryRepository
          {
               get
               {
                    _activityHistoryRepository ??= new GenericRepository<ActivityHistory>(_dbContext);
                    return _activityHistoryRepository;
               }
          }
          public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
              _dbContext.SaveChangesAsync(cancellationToken);
     }
}
