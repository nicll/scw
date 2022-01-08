using System.Linq;
using System.Threading.Tasks;

namespace ScwSvc.Procedures.Impl;

public class GraphQLTableProcedures : IGraphQLTableProcedures
{
    private readonly IUserOperations _userOp;

    public GraphQLTableProcedures(IUserOperations userOp)
        => _userOp = userOp;

    public async Task<Guid> GetDataSetLookupName(User user, Guid tableId)
    {
        var table = user.OwnTables.Concat(user.Collaborations).FirstOrDefault(t => t.TableId == tableId);

        if (table is null)
            throw new TableNotFoundException("Table was not found.") { TableId = tableId };

        if (table.TableType != TableType.DataSet)
            throw new TableMismatchException("Table was not a data set.");

        await _userOp._LogLookupEvent(user.UserId, tableId);
        return table.LookupName;
    }

    public async Task<Guid> GetSheetLookupName(User user, Guid tableId)
    {
        var table = user.OwnTables.Concat(user.Collaborations).FirstOrDefault(t => t.TableId == tableId);

        if (table is null)
            throw new TableNotFoundException("Table was not found.") { TableId = tableId };

        if (table.TableType != TableType.Sheet)
            throw new TableMismatchException("Table was not a sheet.");

        await _userOp._LogLookupEvent(user.UserId, tableId);
        return table.LookupName;
    }
}
