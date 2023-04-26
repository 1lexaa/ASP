using ASP.DATA.Entity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ASP.DATA
{
    public class DataContext : DbContext
    {
        public DbSet<Entity.User> Users { get; set; }

        public DataContext(DbContextOptions options) : base(options)
        {
        }
    }
}
