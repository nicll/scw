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
        await _sysDb.Tables
            .Where(t => t.Collaborators.Contains(user))
            .ForEachAsync(t => t.Collaborators.Remove(user))
            .ConfigureAwait(false);
        _sysDb.Tables.RemoveRange(user.OwnTables);
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

    public async Task<ICollection<Table>> GetAllTables()
        => await _sysDb.Tables.ToArrayAsync();

    public IQueryable<Table> CreateTablesQuery()
        => _sysDb.Tables;

    public async Task<ICollection<Table>> ExecuteTablesQuery(IQueryable<Table> query)
        => await query.ToArrayAsync();

    public async Task<Table?> GetTableById(Guid tableId)
        => await _sysDb.Tables.FindAsync(tableId).ConfigureAwait(false);

    public async Task AddTable(Table table)
    {
        await _sysDb.Tables.AddAsync(table).ConfigureAwait(false);

        if (AutoSave)
            await _SaveChanges();
    }

    public async Task RemoveTable(Table table)
    {
        await foreach (var user in _sysDb.Users.AsAsyncEnumerable().ConfigureAwait(false))
            user.Collaborations.Remove(table);

        table.Owner.OwnTables.Remove(table);
        _sysDb.Tables.Remove(table);

        if (AutoSave)
            await _SaveChanges();
    }

    public async Task ModifyTable(Table table)
    {
        //_sysDb.Tables.Update(table);

        if (AutoSave)
            await _SaveChanges();
    }

    public async Task CreateLogEvent(LogEvent logEvent)
    {
        await _sysDb.Log.AddAsync(logEvent).ConfigureAwait(false);

        if (AutoSave)
            await _SaveChanges();
    }

    public async Task<LogEvent?> GetLogEvent(Guid logEventId)
        => await _sysDb.Log.FindAsync(logEventId).ConfigureAwait(false);

    public IQueryable<LogEvent> CreateLogQuery()
        => _sysDb.Log;

    public async Task<ICollection<LogEvent>> ExecuteLogQuery(IQueryable<LogEvent> query)
        => await query.ToArrayAsync().ConfigureAwait(false);

    public IAsyncEnumerable<LogEvent> AsAsyncLogQuery(IQueryable<LogEvent> query)
        => query.AsAsyncEnumerable();

    // only actually saves when AutoSave is disabled
    public async Task SaveChanges()
    {
        if (!AutoSave)
            await _SaveChanges();
    }

    private async Task _SaveChanges()
        => await _sysDb.SaveChangesAsync();
}
