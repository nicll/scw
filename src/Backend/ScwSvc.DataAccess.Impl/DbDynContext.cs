using Microsoft.EntityFrameworkCore;

namespace ScwSvc.DataAccess.Impl
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
