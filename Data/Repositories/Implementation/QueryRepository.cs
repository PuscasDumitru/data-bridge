using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.Implementation
{
     public class QueryRepository : GenericRepository<Query>
     {
          public QueryRepository(RepositoryDbContext repositoryContext) : base(repositoryContext) { }

          public async Task<Query> GetQueryByIdAsync(Guid queryId)
          {
               return await GetByCondition(qry => qry.Id.Equals(queryId))
                       .Include(x => x.CronJob)
                   .FirstOrDefaultAsync();
          }

          public async Task<IEnumerable<Query>> GetAllQueriesAsync()
          {
               return await GetAll()
                   .ToListAsync();

          }
     }
}
