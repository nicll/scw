using Microsoft.EntityFrameworkCore;
using ScwSvc.Models;

namespace ScwSvc.SvcModels
{
    public class DbSysContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<TableRef> TableRefs { get; set; }

        public DbSysContext(DbContextOptions<DbSysContext> options) : base(options)
        {
            if (Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
                Database.Migrate();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("scw1_sys");
            modelBuilder.Entity<DataSetColumn>().HasKey(c => new { c.TableRefId, c.Position });

            modelBuilder.Entity<User>().HasIndex(u => u.Name).IsUnique();
            modelBuilder.Entity<DataSetColumn>().HasAlternateKey(d => new { d.TableRefId, d.Name });

            modelBuilder.Entity<User>().HasMany(u => u.OwnTables).WithOne(t => t.Owner);
            modelBuilder.Entity<User>().HasMany(u => u.Collaborations).WithMany(t => t.Collaborators);

            modelBuilder.Entity<TableRef>().HasOne(t => t.Owner).WithMany(u => u.OwnTables);
            modelBuilder.Entity<TableRef>().HasMany(t => t.Collaborators).WithMany(u => u.Collaborations);
            modelBuilder.Entity<TableRef>().HasMany(t => t.Columns).WithOne(c => c.TableRef);

            modelBuilder.Entity<User>().Property(u => u.PasswordHash).HasMaxLength(32).IsFixedLength();
            modelBuilder.Entity<TableRef>().Property(t => t.LookupName).HasMaxLength(24).IsFixedLength();
        }
    }
}
