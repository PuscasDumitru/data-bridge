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
    internal sealed class QueryRepository : GenericRepository<Query>, IQueryRepository
    {
        public QueryRepository(RepositoryDbContext repositoryContext)
            : base(repositoryContext)
        {
        }
        public async Task<IEnumerable<Query>> GetAllQueriesAsync(CancellationToken cancellationToken = default)
        {
            return await FindAll()
                .OrderBy(query => query.Id)
                .ToListAsync(cancellationToken);
        }
        public async Task<Query> GetQueryByIdAsync(int queryId, CancellationToken cancellationToken = default)
        {
            return await FindByCondition(query => query.Id.Equals(queryId))
                .FirstOrDefaultAsync(cancellationToken);
        }
        public override void Create(Query query)        
        {
            base.Create(query);
        }
        public override void Update(Query query)
        {
            base.Update(query);
        }
        public override void Delete(Query query)
        {
            base.Delete(query);
        }
    }
}
