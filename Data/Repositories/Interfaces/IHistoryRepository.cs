using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Data.Entities;

namespace Data.Repositories.Interfaces
{
    public interface IHistoryRepository : IGenericRepository<History>
    {
        Task<IEnumerable<History>> GetAllHistoriesAsync(CancellationToken cancellationToken = default);
        Task<History> GetHistoryByIdAsync(int historyId, CancellationToken cancellationToken = default);

    }

}
