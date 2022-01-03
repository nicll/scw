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
}
