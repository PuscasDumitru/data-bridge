using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.Implementation
{
    public class HistoryRepository : GenericRepository<History>
    {
        public HistoryRepository(RepositoryDbContext repositoryContext) : base(repositoryContext) { }

        public async Task<History> GetHistoryByIdAsync(Guid historyId)
        {
            return await GetByCondition(history => history.Id.Equals(historyId))
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<History>> GetAllHistoriesAsync()
        {
            return await GetAll()
                .ToListAsync();

        }
    }
}
