using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Data.Entities;
using Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories.Implementation
{
    internal sealed class HistoryRepository : GenericRepository<History>, IHistoryRepository
    {
        public HistoryRepository(RepositoryDbContext repositoryContext)
            : base(repositoryContext)
        {
        }
        public async Task<IEnumerable<History>> GetAllHistoriesAsync(CancellationToken cancellationToken = default)
        {
            return await FindAll()
                .OrderBy(history => history.Id)
                .ToListAsync(cancellationToken);
        }
        public async Task<History> GetHistoryByIdAsync(int historyId, CancellationToken cancellationToken = default)
        {
            return await FindByCondition(history => history.Id.Equals(historyId))
                .FirstOrDefaultAsync(cancellationToken);
        }
        public override void Create(History history)
        {
            base.Create(history);
        }
        public override void Update(History history)
        {
            base.Update(history);
        }
        public override void Delete(History history)
        {
            base.Delete(history);
        }
    }
}
