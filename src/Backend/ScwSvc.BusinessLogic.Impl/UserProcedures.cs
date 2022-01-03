using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScwSvc.Procedures.Impl;

public class UserProcedures : IUserProcedures
{
    private readonly IUserOperations _user;
    private readonly ITableOperations _table;

    public int MaxDataSetsPerUser { get; set; } = 20;

    public int MaxSheetsPerUser { get; set; } = 20;

    public UserProcedures(IUserOperations user, ITableOperations table)
    {
        _user = user;
        _table = table;
    }

    public async Task ChangeUserName(User user, string name)
        => await _user.ModifyUser(user.UserId, name, null, null);

    public async Task ChangeUserPassword(User user, string password)
        => await _user.ModifyUser(user.UserId, null, password, null);

    public Task<Table> GetDataSet(User user, Guid tableId)
    {
        var table = user.OwnTables.FirstOrDefault(t => t.TableId == tableId);

        if (table is null)
            throw new TableNotFoundException("The table was not found in the user's tables.") { TableId = tableId };

        if (table.TableType != TableType.DataSet)
            throw new TableMismatchException("Table was not a data set.");

        return Task.FromResult(table);
    }

    public Task<Table> GetSheet(User user, Guid tableId)
    {
        var table = user.OwnTables.FirstOrDefault(t => t.TableId == tableId);

        if (table is null)
            throw new TableNotFoundException("The table was not found in the user's tables.") { TableId = tableId };

        if (table.TableType != TableType.Sheet)
            throw new TableMismatchException("Table was not a sheet.");

        return Task.FromResult(table);
    }

    public Table PrepareDataSet(User owner, Table table)
        => _PrepareDataSet(owner, table);

    public Table PrepareSheet(User owner, Table table)
        => _PrepareSheet(owner, table);

    public async Task CreateDataSet(User owner, Table table)
    {
        if (table.TableType != TableType.DataSet)
            throw new TableMismatchException("Incorrect table type.");

        if (owner.OwnTables.Count(t => t.TableType == TableType.DataSet) >= MaxDataSetsPerUser)
            throw new TableLimitExceededException("User has too many data sets.");

        await _table.AddTable(table);
    }

    public async Task CreateSheet(User owner, Table table)
    {
        if (table.TableType != TableType.Sheet)
            throw new TableMismatchException("Incorrect table type.");

        if (owner.OwnTables.Count(t => t.TableType == TableType.Sheet) >= MaxSheetsPerUser)
            throw new TableLimitExceededException("User has too many sheets.");

        await _table.AddTable(table);
    }

    public async Task DeleteDataSet(User owner, Guid tableId)
    {
        _ = await GetDataSet(owner, tableId);

        await _table.DeleteTable(tableId);
    }

    public async Task DeleteSheet(User owner, Guid tableId)
    {
        _ = await GetSheet(owner, tableId);

        await _table.DeleteTable(tableId);
    }

    public async Task AddDataSetColumn(User owner, Guid tableId, DataSetColumn column)
    {
        var table = await GetDataSet(owner, tableId);

        var maxPos = table.Columns.Max(c => c.Position);
        var col = new DataSetColumn
        {
            TableId = tableId,
            Position = (byte)(maxPos + 1),
            Type = column.Type,
            Name = column.Name,
            Nullable = column.Nullable
        };

        await _table.AddColumn(table.TableId, col);
    }

    public async Task RemoveDataSetColumn(User owner, Guid tableId, string columnName)
    {
        _ = await GetDataSet(owner, tableId);

        await _table.RemoveColumn(tableId, columnName);
    }

    public Task<ICollection<User>> GetCollaborators(User user, Guid tableId)
    {
        var table = user.OwnTables.FirstOrDefault(t => t.TableId == tableId);

        if (table is null)
            throw new TableNotFoundException("The table was not found in the user's tables.");

        return Task.FromResult(table.Collaborators);
    }

    public async Task AddCollaborator(User owner, Guid tableId, Guid userId)
    {
        var table = owner.OwnTables.FirstOrDefault(t => t.TableId == tableId);

        if (table is null)
            throw new TableNotFoundException("The table was not found in the user's tables.");

        var collaborator = await _user.GetUserById(userId);

        if (collaborator is null)
            throw new UserNotFoundException("Collaborator was not found.");

        await _table.AddCollaborator(table, collaborator);
        // ToDo: logging
    }

    public async Task RemoveCollaborator(User owner, Guid tableId, Guid userId)
    {
        var table = owner.OwnTables.FirstOrDefault(t => t.TableId == tableId);

        if (table is null)
            throw new TableNotFoundException("The table was not found in the user's tables.");

        var collaborator = await _user.GetUserById(userId);

        if (collaborator is null)
            throw new UserNotFoundException("Collaborator was not found.");

        await _table.RemoveCollaborator(table, collaborator);
        // ToDo: logging
    }
}
