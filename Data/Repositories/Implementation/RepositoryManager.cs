using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Repositories.Interfaces;

namespace Data.Repositories.Implementation
{
    public sealed class RepositoryManager : IRepositoryManager
    {
        readonly Lazy<ICollectionRepository> _lazyCollectionRepository;
        readonly Lazy<IQueryRepository> _lazyQueryRepository;
        readonly Lazy<IWorkspaceRepository> _lazyWorkspaceRepository;
        readonly Lazy<IFolderRepository> _lazyFolderRepository;
        readonly Lazy<IHistoryRepository> _lazyHistoryRepository;
        readonly Lazy<IUnitOfWork> _lazyUnitOfWork;

        public RepositoryManager(RepositoryDbContext dbContext)
        {
            _lazyCollectionRepository = new Lazy<ICollectionRepository>(() => new CollectionRepository(dbContext));
            _lazyQueryRepository = new Lazy<IQueryRepository>(() => new QueryRepository(dbContext));
            _lazyWorkspaceRepository = new Lazy<IWorkspaceRepository>(() => new WorkspaceRepository(dbContext));
            _lazyFolderRepository = new Lazy<IFolderRepository>(() => new FolderRepository(dbContext));
            _lazyHistoryRepository = new Lazy<IHistoryRepository>(() => new HistoryRepository(dbContext));
            _lazyUnitOfWork = new Lazy<IUnitOfWork>(() => new UnitOfWork(dbContext));
        }

        public ICollectionRepository CollectionRepository => _lazyCollectionRepository.Value;
        public IQueryRepository QueryRepository => _lazyQueryRepository.Value;
        public IWorkspaceRepository WorkspaceRepository => _lazyWorkspaceRepository.Value;
        public IFolderRepository FolderRepository => _lazyFolderRepository.Value;
        public IHistoryRepository HistoryRepository => _lazyHistoryRepository.Value;
        public IUnitOfWork UnitOfWork => _lazyUnitOfWork.Value;
    }
}
