using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Data.Entities;

namespace Data.Repositories.Interfaces
{
    public interface ICollectionRepository : IGenericRepository<Collection>
    {
        Task<IEnumerable<Collection>> GetAllCollectionsAsync(CancellationToken cancellationToken = default);
        Task<Collection> GetCollectionByIdAsync(int collectionId, CancellationToken cancellationToken = default);
    }
}
