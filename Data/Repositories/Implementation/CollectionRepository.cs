using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories.Implementation
{
    internal sealed class CollectionRepository : GenericRepository<Collection>, ICollectionRepository
    {
        public CollectionRepository(RepositoryDbContext repositoryContext)
            : base(repositoryContext)
        {
        }
        public async Task<IEnumerable<Collection>> GetAllCollectionsAsync(CancellationToken cancellationToken = default)
        {
            return await FindAll()
                .OrderBy(collection => collection.Id)
                .ToListAsync(cancellationToken);
        }
        public async Task<Collection> GetCollectionByIdAsync(int collectionId, CancellationToken cancellationToken = default)
        {
            return await FindByCondition(collection => collection.Id.Equals(collectionId))
                .FirstOrDefaultAsync(cancellationToken);
        }
        public override void Create(Collection collection)
        {
            base.Create(collection);
        }
        public override void Update(Collection collection)
        {
            base.Update(collection);
        }
        public override void Delete(Collection collection)
        {
            base.Delete(collection);
        }
    }
}
