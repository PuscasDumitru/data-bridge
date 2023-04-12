using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories.Implementation
{
    public class UserEmailConfirmationRepository : GenericRepository<UserEmailConfirmation>
    {
        public UserEmailConfirmationRepository(RepositoryDbContext repositoryContext) : base(repositoryContext) { }

        public async Task<UserEmailConfirmation> GetEmailConfirmationByTokenAsync(string token)
        {
            return await GetByCondition(conf => conf.EmailConfirmationToken == token)
                .FirstOrDefaultAsync();
        }
    }
}
