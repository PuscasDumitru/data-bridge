using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.Repositories.Implementation
{
    public class UserRepository : GenericRepository<User>
    {
        public UserRepository(RepositoryDbContext repositoryContext) : base(repositoryContext) { }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await GetByCondition(user => user.Email == email)
                .FirstOrDefaultAsync();
        }
    }
}
