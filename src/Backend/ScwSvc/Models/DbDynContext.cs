using Microsoft.EntityFrameworkCore;
using System;

namespace ScwSvc.Models
{
    public class DbDynContext : DbContext
    {
        public DbDynContext(DbContextOptions<DbDynContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("scw1_dyn");
        }
    }
}
