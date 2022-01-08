using Microsoft.EntityFrameworkCore;
using ScwSvc.Models;

namespace ScwSvc.DataAccess.Impl;

public class DbSysContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public DbSet<Table> Tables { get; set; }

    public DbSet<LogEvent> Log { get; set; }

    public DbSysContext(DbContextOptions<DbSysContext> options) : base(options)
    {
        if (Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
            Database.Migrate();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("scw1_sys");
        modelBuilder.Entity<DataSetColumn>().HasKey(c => new { c.TableId, c.Position });

        modelBuilder.Entity<UserLogEvent>();
        modelBuilder.Entity<LookupLogEvent>();
        modelBuilder.Entity<TableLogEvent>();
        modelBuilder.Entity<TableDefinitionLogEvent>();
        modelBuilder.Entity<TableCollaboratorLogEvent>();

        modelBuilder.Entity<User>().HasIndex(u => u.Name).IsUnique();
        modelBuilder.Entity<DataSetColumn>().HasAlternateKey(d => new { d.TableId, d.Name });

        modelBuilder.Entity<User>().HasMany(u => u.OwnTables).WithOne(t => t.Owner);
        modelBuilder.Entity<User>().HasMany(u => u.Collaborations).WithMany(t => t.Collaborators);

        modelBuilder.Entity<Table>().HasOne(t => t.Owner).WithMany(u => u.OwnTables);
        modelBuilder.Entity<Table>().HasMany(t => t.Collaborators).WithMany(u => u.Collaborations);
        modelBuilder.Entity<Table>().HasMany(t => t.Columns).WithOne(c => c.Table);

        modelBuilder.Entity<User>().Property(u => u.PasswordHash).HasMaxLength(32).IsFixedLength();
        modelBuilder.Entity<Table>().Property(t => t.LookupName).HasMaxLength(24).IsFixedLength();
    }
}
