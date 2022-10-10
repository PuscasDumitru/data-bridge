using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Data.Entities;

namespace Data.Repositories.Interfaces
{
    public interface IQueryRepository : IGenericRepository<Query>
    {
        Task<IEnumerable<Query>> GetAllQueriesAsync(CancellationToken cancellationToken = default);
        Task<Query> GetQueryByIdAsync(int queryId, CancellationToken cancellationToken = default);
  }

}
