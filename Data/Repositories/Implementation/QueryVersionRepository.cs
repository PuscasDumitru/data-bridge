using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories.Implementation
{
     public class QueryVersionRepository : GenericRepository<QueryVersion>
     {
          public QueryVersionRepository(RepositoryDbContext repositoryContext) : base(repositoryContext) { }

          public async Task<QueryVersion> GetQueryVersionByVersionAsync(string queryVersion, Guid queryId)
          {
               return await GetByCondition(qry => (qry.Version == queryVersion) && (qry.QueryId == queryId))
                    .FirstOrDefaultAsync();
          }

          public string GetVersionForNewQueryVersion(Guid queryId)
          {
               var result = Math.Round(GetByCondition(qryVrsn => qryVrsn.QueryId.Equals(queryId))
                    .Select(qryVrsn => Convert.ToDouble(qryVrsn.Version)).Max() + 0.1, 2).ToString();

               if (!result.Contains("."))
               {
                    result += ".0";
               }
               return result;
          }

          public bool ExistsQueryVersionsForQueryId(Guid queryId)
          {
               return GetAll().Any(qryVrsn => qryVrsn.QueryId.Equals(queryId));
          }

     }
}
