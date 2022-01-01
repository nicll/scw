using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ScwSvc.DataAccess.Interfaces;
using ScwSvc.Models;

namespace ScwSvc.DataAccess.Impl;

public class SysDbRepository : ISysDbRepository
{
    private readonly DbSysContext _sysDb;

    public bool AutoSave { get; set; } = true;

    public SysDbRepository(DbSysContext sysDb)
        => _sysDb = sysDb;

    public async Task<ICollection<User>> GetAllUsers()
        => await _sysDb.Users.ToArrayAsync();

    public IQueryable<User> CreateUsersQuery()
        => _sysDb.Users;

    public async Task<ICollection<User>> ExecuteUsersQuery(IQueryable<User> query)
        => await query.ToArrayAsync();

    public async Task<User?> GetUserById(Guid userId)
        => await _sysDb.Users.FindAsync(userId).ConfigureAwait(false);

    public async Task<User?> GetUserByName(string name)
        => await _sysDb.Users.FirstOrDefaultAsync(u => u.Name == name).ConfigureAwait(false);

    public async Task<ICollection<User>> GetUsersByRole(UserRole role)
        => await _sysDb.Users.Where(u => u.Role == role).ToArrayAsync().ConfigureAwait(false);

    public async Task<bool> IsUserNameAssigned(string name)
        => await _sysDb.Users.AnyAsync(u => u.Name == name).ConfigureAwait(false);

    public async Task AddUser(User user)
    {
        await _sysDb.Users.AddAsync(user).ConfigureAwait(false);

        if (AutoSave)
            await _SaveChanges();
    }

    public async Task RemoveUser(User user)
    {
        await _sysDb.TableRefs
            .Where(t => t.Collaborators.Contains(user))
            .ForEachAsync(t => t.Collaborators.Remove(user))
            .ConfigureAwait(false);
        _sysDb.TableRefs.RemoveRange(user.OwnTables);
        _sysDb.Users.Remove(user);

        if (AutoSave)
            await _SaveChanges();
    }

    public async Task ModifyUser(User user)
    {
        //_sysDb.Users.Update(user);

        if (AutoSave)
            await _SaveChanges();
    }

    public async Task<ICollection<TableRef>> GetAllTables()
        => await _sysDb.TableRefs.ToArrayAsync();

    public IQueryable<TableRef> CreateTablesQuery()
        => _sysDb.TableRefs;

    public async Task<ICollection<TableRef>> ExecuteTablesQuery(IQueryable<TableRef> query)
        => await query.ToArrayAsync();

    public async Task<TableRef?> GetTableById(Guid tableId)
        => await _sysDb.TableRefs.FindAsync(tableId).ConfigureAwait(false);

    public async Task AddTable(TableRef table)
    {
        await _sysDb.TableRefs.AddAsync(table).ConfigureAwait(false);

        if (AutoSave)
            await _SaveChanges();
    }

    public async Task RemoveTable(TableRef table)
    {
        await foreach (var user in _sysDb.Users.AsAsyncEnumerable().ConfigureAwait(false))
            user.Collaborations.Remove(table);

        table.Owner.OwnTables.Remove(table);
        _sysDb.TableRefs.Remove(table);

        if (AutoSave)
            await _SaveChanges();
    }

    public async Task ModifyTable(TableRef table)
    {
        //_sysDb.TableRefs.Update(table);

        if (AutoSave)
            await _SaveChanges();
    }

    // only actually saves when AutoSave is disabled
    public async Task SaveChanges()
    {
        if (!AutoSave)
            await _SaveChanges();
    }

    private async Task _SaveChanges()
        => await _sysDb.SaveChangesAsync();
}
