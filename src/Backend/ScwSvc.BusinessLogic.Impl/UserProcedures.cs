using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScwSvc.Procedures.Impl;

public class UserProcedures : IUserProcedures
{
    private readonly IUserOperations _userOp;
    private readonly ITableOperations _tableOp;

    public int MaxDataSetsPerUser { get; set; } = 20;

    public int MaxSheetsPerUser { get; set; } = 20;

    public UserProcedures(IUserOperations userOp, ITableOperations tableOp)
    {
        _userOp = userOp;
        _tableOp = tableOp;
    }

    public Task<string> GetUserName(User user)
        => Task.FromResult(user.Name);

    public async Task ChangeUserName(User user, string name)
    {
        await _userOp.ModifyUser(user.UserId, name, null, null);
        await _userOp._LogUserEvent(user.UserId, UserLogEventType.ChangeName);
    }

    public async Task ChangeUserPassword(User user, string password)
    {
        await _userOp.ModifyUser(user.UserId, null, password, null);
        await _userOp._LogUserEvent(user.UserId, UserLogEventType.ChangePassword);
    }

    public Task<ICollection<Table>> GetDataSets(User user)
        => Task.FromResult((ICollection<Table>)user.OwnTables.Concat(user.Collaborations).Where(t => t.TableType == TableType.DataSet).ToArray());

    public Task<int> GetDataSetCount(User user)
        => Task.FromResult(user.OwnTables.Count(t => t.TableType == TableType.DataSet));

    public Task<ICollection<Table>> GetOwnDataSets(User user)
        => Task.FromResult((ICollection<Table>)user.OwnTables.Where(t => t.TableType == TableType.DataSet));

    public Task<ICollection<Table>> GetCollaborationDataSets(User user)
        => Task.FromResult((ICollection<Table>)user.Collaborations.Where(t => t.TableType == TableType.DataSet));

    public Task<ICollection<Table>> GetSheets(User user)
        => Task.FromResult((ICollection<Table>)user.OwnTables.Concat(user.Collaborations).Where(t => t.TableType == TableType.Sheet).ToArray());

    public Task<int> GetSheetCount(User user)
        => Task.FromResult(user.OwnTables.Count(t => t.TableType == TableType.Sheet));

    public Task<ICollection<Table>> GetOwnSheets(User user)
        => Task.FromResult((ICollection<Table>)user.OwnTables.Where(t => t.TableType == TableType.Sheet));

    public Task<ICollection<Table>> GetCollaborationSheets(User user)
        => Task.FromResult((ICollection<Table>)user.Collaborations.Where(t => t.TableType == TableType.Sheet));

    public Task<ICollection<Table>> GetTables(User user)
        => Task.FromResult((ICollection<Table>)user.OwnTables.Concat(user.Collaborations).ToArray());

    public Task<ICollection<Table>> GetOwnTables(User user)
        => Task.FromResult(user.OwnTables);

    public Task<ICollection<Table>> GetCollaborationTables(User user)
        => Task.FromResult(user.Collaborations);

    public Task<Table> GetDataSet(User user, Guid tableId)
    {
        var table = user.OwnTables.Concat(user.Collaborations).FirstOrDefault(t => t.TableId == tableId);

        if (table is null)
            throw new TableNotFoundException("The table was not found in the user's tables or collaborations.") { TableId = tableId };

        if (table.TableType != TableType.DataSet)
            throw new TableMismatchException("Table was not a data set.");

        return Task.FromResult(table);
    }

    public Task<Table> GetSheet(User user, Guid tableId)
    {
        var table = user.OwnTables.Concat(user.Collaborations).FirstOrDefault(t => t.TableId == tableId);

        if (table is null)
            throw new TableNotFoundException("The table was not found in the user's tables or collaborations.") { TableId = tableId };

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

        await _tableOp.AddTable(table);
        await _tableOp._LogTableDefEvent(table.TableId, TableType.DataSet, TableDefinitionLogEventType.CreateTable);
    }

    public async Task CreateSheet(User owner, Table table)
    {
        if (table.TableType != TableType.Sheet)
            throw new TableMismatchException("Incorrect table type.");

        if (owner.OwnTables.Count(t => t.TableType == TableType.Sheet) >= MaxSheetsPerUser)
            throw new TableLimitExceededException("User has too many sheets.");

        await _tableOp.AddTable(table);
        await _tableOp._LogTableDefEvent(table.TableId, TableType.Sheet, TableDefinitionLogEventType.CreateTable);
    }

    public async Task DeleteDataSet(User owner, Guid tableId)
    {
        _ = await GetDataSet(owner, tableId);

        await _tableOp.DeleteTable(tableId);
        await _tableOp._LogTableDefEvent(tableId, TableType.DataSet, TableDefinitionLogEventType.DeleteTable);
    }

    public async Task DeleteSheet(User owner, Guid tableId)
    {
        _ = await GetSheet(owner, tableId);

        await _tableOp.DeleteTable(tableId);
        await _tableOp._LogTableDefEvent(tableId, TableType.Sheet, TableDefinitionLogEventType.DeleteTable);
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

        await _tableOp.AddColumn(table.TableId, col);
        await _tableOp._LogTableDefEvent(tableId, TableType.DataSet, TableDefinitionLogEventType.AddColumn);
    }

    public async Task RemoveDataSetColumn(User owner, Guid tableId, string columnName)
    {
        _ = await GetDataSet(owner, tableId);

        await _tableOp.RemoveColumn(tableId, columnName);
        await _tableOp._LogTableDefEvent(tableId, TableType.DataSet, TableDefinitionLogEventType.RemoveColumn);
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

        var collaborator = await _userOp.GetUserById(userId);

        if (collaborator is null)
            throw new UserNotFoundException("Collaborator was not found.");

        await _tableOp.AddCollaborator(table, collaborator);
        await _tableOp._LogTableCollabEvent(tableId, userId, TableCollaboratorLogEventType.AddCollaborator);
    }

    public async Task RemoveCollaborator(User owner, Guid tableId, Guid userId)
    {
        var table = owner.OwnTables.FirstOrDefault(t => t.TableId == tableId);

        if (table is null)
            throw new TableNotFoundException("The table was not found in the user's tables.");

        var collaborator = await _userOp.GetUserById(userId);

        if (collaborator is null)
            throw new UserNotFoundException("Collaborator was not found.");

        await _tableOp.RemoveCollaborator(table, collaborator);
        await _tableOp._LogTableCollabEvent(tableId, userId, TableCollaboratorLogEventType.RemoveCollaborator);
    }
}
