using System.Threading.Tasks;

namespace ScwSvc.Procedures.Impl;

internal static class Utils
{
    internal static Table _PrepareDataSet(User owner, Table table)
    {
        _PrepareBase(owner, table);
        table.TableType = TableType.DataSet;

        for (int i = 0; i < table.Columns.Count; ++i)
        {
            table.Columns[i].TableId = table.TableId;
            table.Columns[i].Position = (byte)i;
        }

        return table;
    }

    internal static Table _PrepareSheet(User owner, Table table)
    {
        _PrepareBase(owner, table);
        table.TableType = TableType.Sheet;
        table.Columns = Array.Empty<DataSetColumn>();
        return table;
    }

    private static void _PrepareBase(User owner, Table table)
    {
        table.TableId = Guid.NewGuid();
        // table.DisplayName
        // table.TableType
        table.CreationDate = DateTime.UtcNow;
        table.LookupName = Guid.NewGuid();
        table.OwnerUserId = owner.UserId;
        // table.Collaborators
        // table.Columns
    }

    internal static async Task _LogUserEvent(this IUserOperations userOp, Guid userId, UserLogEventType eventType)
        => await userOp.LogUserEvent(new() { Timestamp = DateTime.UtcNow, UserId = userId, UserAction = eventType });

    internal static async Task _LogLookupEvent(this IUserOperations userOp, Guid userId, Guid tableId)
        => await userOp.LogLookupEvent(new() { Timestamp = DateTime.UtcNow, UserId = userId, TableId = tableId });

    internal static async Task _LogTableDefEvent(this ITableOperations tableOp, Guid tableId, TableType tableType, TableDefinitionLogEventType eventType)
        => await tableOp.LogTableEvent(new TableDefinitionLogEvent { Timestamp = DateTime.UtcNow, TableId = tableId, TableType = tableType, TableAction = eventType });

    internal static async Task _LogTableCollabEvent(this ITableOperations tableOp, Guid tableId, Guid userId, TableCollaboratorLogEventType eventType)
        => await tableOp.LogTableEvent(new TableCollaboratorLogEvent { Timestamp = DateTime.UtcNow, TableId = tableId, UserId = userId, CollaboratorAction = eventType });
}
