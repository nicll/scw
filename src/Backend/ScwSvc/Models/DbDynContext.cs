using Microsoft.EntityFrameworkCore;
using System;

namespace ScwSvc.Models
{
    public class DbDynContext : DbContext
    {
        public DbDynContext(DbContextOptions<DbDynContext> options) : base(options)
        {
            Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("scw1_dyn");
        }
    }
}
