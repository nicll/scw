using System.Linq;
using System.Threading.Tasks;

namespace ScwSvc.Procedures.Impl;

public class GraphQLTableProcedures : IGraphQLTableProcedures
{
    public Task<Guid> GetDataSetLookupName(User user, Guid tableId)
    {
        var table = user.OwnTables.Concat(user.Collaborations).FirstOrDefault(t => t.TableId == tableId);

        if (table is null)
            throw new TableNotFoundException("Table was not found.") { TableId = tableId };

        if (table.TableType != TableType.DataSet)
            throw new TableMismatchException("Table was not a data set.");

        return Task.FromResult(table.LookupName);
    }

    public Task<Guid> GetSheetLookupName(User user, Guid tableId)
    {
        var table = user.OwnTables.Concat(user.Collaborations).FirstOrDefault(t => t.TableId == tableId);

        if (table is null)
            throw new TableNotFoundException("Table was not found.") { TableId = tableId };

        if (table.TableType != TableType.Sheet)
            throw new TableMismatchException("Table was not a sheet.");

        return Task.FromResult(table.LookupName);
    }
}
