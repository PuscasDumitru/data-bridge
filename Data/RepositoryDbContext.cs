using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class RepositoryDbContext : DbContext
    {
        public RepositoryDbContext() {}
        public RepositoryDbContext(DbContextOptions options) : base(options) {}

        public DbSet<Collection> Collection { get; set; }
        public DbSet<Folder> Folder { get; set; }
        public DbSet<Query> Query { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Workspace> Workspace { get; set; }
        public DbSet<History> History { get; set; }
    }
}
