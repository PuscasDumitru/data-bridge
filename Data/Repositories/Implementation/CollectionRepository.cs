using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.Implementation
{
    public class CollectionRepository : GenericRepository<Collection>
    {
        public CollectionRepository(RepositoryDbContext repositoryContext) : base(repositoryContext) { }

        public async Task<Collection> GetCollectionByIdAsync(Guid collectionId)
        {
            return await GetByCondition(coll => coll.Id.Equals(collectionId))
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Collection>> GetAllCollectionsAsync()
        {
            return await GetAll()
                .ToListAsync();

        }

    }
}
