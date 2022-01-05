using System.Collections.Generic;
using System.Threading.Tasks;

namespace ScwSvc.Procedures.Impl;

public class AdminProcedures : IAdminProcedures
{
    private readonly IUserOperations _userOp;
    private readonly ITableOperations _tableOp;

    public AdminProcedures(IUserOperations userOp, ITableOperations tableOp)
    {
        _userOp = userOp;
        _tableOp = tableOp;
    }

    public async Task<ICollection<User>> GetAllUsers()
        => await _userOp.GetUsers();

    public async Task<User?> GetUser(Guid userId)
        => await _userOp.GetUserById(userId);

    public async Task AddUser(string name, string password)
        => await _userOp.AddUser(name, password);

    public async Task DeleteUser(Guid userId)
        => await _userOp.DeleteUser(userId);

    public async Task ChangeUserName(Guid userId, string name)
        => await _userOp.ModifyUser(userId, name, null, null);

    public async Task ChangeUserPassword(Guid userId, string password)
        => await _userOp.ModifyUser(userId, null, password, null);

    public async Task ChangeUserRole(Guid userId, UserRole role)
        => await _userOp.ModifyUser(userId, null, null, role);

    public async Task<ICollection<Table>> GetUserTables(Guid userId)
        => await _tableOp.GetTables(userId, TableQuery.Own | TableQuery.Collaborations);

    public async Task<ICollection<Table>> GetUserTablesOwn(Guid userId)
        => await _tableOp.GetTables(userId, TableQuery.Own);

    public async Task<ICollection<Table>> GetUserTablesCollaboration(Guid userId)
        => await _tableOp.GetTables(userId, TableQuery.Collaborations);

    public async Task<ICollection<Table>> GetAllTables()
        => await _tableOp.GetTables();

    public async Task<ICollection<Table>> GetAllDataSets()
        => await _tableOp.GetTables(TableQuery.DataSet);

    public async Task<ICollection<Table>> GetAllSheets()
        => await _tableOp.GetTables(TableQuery.Sheet);

    public Table PrepareDataSet(User owner, Table table)
        => _PrepareDataSet(owner, table);

    public Table PrepareSheet(User owner, Table table)
        => _PrepareSheet(owner, table);

    public async Task CreateDataSet(User owner, Table table)
    {
        if (table.TableType != TableType.DataSet)
            throw new TableMismatchException("Incorrect table type.");

        await _tableOp.AddTable(table);
    }

    public async Task CreateSheet(User owner, Table table)
    {
        if (table.TableType != TableType.Sheet)
            throw new TableMismatchException("Incorrect table type.");

        await _tableOp.AddTable(table);
    }

    public async Task DeleteDataSet(Guid tableId)
    {
        var table = await _tableOp.GetTable(tableId)
            ?? throw new TableNotFoundException("Table was not found.") { TableId = tableId };

        if (table.TableType != TableType.DataSet)
            throw new TableMismatchException("Incorrect table type.");

        await _tableOp.DeleteTable(tableId);
    }

    public async Task DeleteSheet(Guid tableId)
    {
        var table = await _tableOp.GetTable(tableId)
            ?? throw new TableNotFoundException("Table was not found.") { TableId = tableId };

        if (table.TableType != TableType.Sheet)
            throw new TableMismatchException("Incorrect table type.");

        await _tableOp.DeleteTable(tableId);
    }
}
