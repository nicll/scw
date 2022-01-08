using System.Linq;

namespace ScwSvc.Operations.Impl;

public class TableOperations : ITableOperations
{
    private const int _MaxNumberOfColumns = 200, _MaxNumberOfCollaborators = 64;
    private readonly ISysDbRepository _sysDb;
    private readonly IDynDbRepository _dynDb;

    public TableOperations(ISysDbRepository sysDb, IDynDbRepository dynDb)
    {
        _sysDb = sysDb;
        _dynDb = dynDb;
    }

    public int MaxNumberOfColumns => _MaxNumberOfColumns;

    public int MaxNumberOfCollaborators => _MaxNumberOfCollaborators;

    public async Task<Table?> GetTable(Guid tableId)
        => await _sysDb.GetTableById(tableId);

    public async Task<ICollection<Table>> GetTables()
        => await _sysDb.GetAllTables();

    public async Task<ICollection<Table>> GetTables(TableQuery query)
    {
        if ((query & TableQuery.TableRelationshipMask) != 0)
            throw new ArgumentException("Query may not contain relationship restrictions.", nameof(query));

        var tables = _sysDb.CreateTablesQuery();

        if (query.HasFlag(TableQuery.DataSet))
            tables = tables.Where(t => t.TableType == TableType.DataSet);

        if (query.HasFlag(TableQuery.Sheet))
            tables = tables.Where(t => t.TableType == TableType.Sheet);

        return await _sysDb.ExecuteTablesQuery(tables);
    }

    public async Task<ICollection<Table>> GetTables(Guid userId, TableQuery query)
    {
        var user = await _sysDb.GetUserById(userId)
            ?? throw new UserNotFoundException("User with this ID was not found.") { UserId = userId };

        var userTables = (query & TableQuery.TableTypeMask) switch
        {
            TableQuery.Own | TableQuery.Collaborations
                => _sysDb.CreateTablesQuery()
                    .Where(table => table.Owner.UserId == userId || table.Collaborators.Contains(user)),
            TableQuery.Own
                => _sysDb.CreateTablesQuery()
                    .Where(table => table.Owner.UserId == userId),
            TableQuery.Collaborations
                => _sysDb.CreateTablesQuery()
                    .Where(table => table.Collaborators.Contains(user)),
            _ => throw new ArgumentException("Unknown kind of table query: " + query, nameof(query))
        };

        if (query.HasFlag(TableQuery.DataSet))
            userTables = userTables.Where(t => t.TableType == TableType.DataSet);

        if (query.HasFlag(TableQuery.Sheet))
            userTables = userTables.Where(t => t.TableType == TableType.Sheet);

        return await _sysDb.ExecuteTablesQuery(userTables);
    }

    public async Task AddTable(Table table)
    {
        // is name set?
        if (String.IsNullOrEmpty(table.DisplayName))
            throw new TableDeclarationException("Name of table is not set.");

        // is type known?
        if (!Enum.IsDefined(table.TableType))
            throw new TableMismatchException("Unknown type of table: " + table.TableType);

        // is the table ID set?
        if (table.TableId == default)
            throw new TableDeclarationException("Table ID is not set.");

        // is the lookup ID set?
        if (table.LookupName == default)
            throw new TableDeclarationException("Table lookup ID is not set.");

        // is the owner ID set?
        if (table.OwnerUserId == default)
            throw new TableDeclarationException("Table owner ID it not set.");

        // is it a data set with columns?
        if (table.TableType != TableType.DataSet && table.Columns.Any())
            throw new TableColumnException("Only data sets can specify columns.");

        // only data sets have columns set
        if (table.TableType == TableType.DataSet)
        {
            // does it have any columns?
            if (table.Columns.Count < 1)
                throw new TableColumnException("Table contains no columns.");

            // does it have too many columns?
            if (table.Columns.Count > _MaxNumberOfColumns)
                throw new TableColumnException("Table contains too many columns.");

            // are all column types well defined?
            if (table.Columns.Any(c => !Enum.IsDefined(c.Type)))
                throw new TableColumnException("Table contains unknown column type.");

            // check all column positions for uniqueness
            var uniquePos = table.Columns.Select(c => c.Position).Distinct().ToArray();
            var minPos = uniquePos.Min();
            var maxPos = uniquePos.Max();

            // are all column positions unique?
            if (uniquePos.Length != table.Columns.Count)
                throw new TableColumnException("Column positions are not unique.");

            if (minPos != 0)
                throw new TableColumnException("Column position is too low: " + minPos);

            // are there too many columns in total?
            if (maxPos > _MaxNumberOfColumns)
                throw new TableColumnException("Column position is too high: " + maxPos);

            // are all column names unique?
            if (table.Columns.Select(c => c.Name).Distinct().Count() != uniquePos.Length)
                throw new TableColumnException("Column names are not unique.");

            // are the column names too long?
            if (table.Columns.Max(c => c.Name.Length) > DataSetColumn.MaxColumnNameLength)
                throw new TableColumnException("Column name(s) too long.");
        }

        // does a table with this ID already exist?
        if (await _sysDb.GetTableById(table.TableId) is not null)
            throw new TableAlreadyExistsException("A table with this ID already exists.") { TableId = table.TableId };

        await _dynDb.CreateTable(table);
        await _sysDb.AddTable(table);
        await _sysDb.SaveChanges();
    }

    public async Task DeleteTable(Guid tableId)
    {
        var table = await GetTable(tableId)
            ?? throw new TableNotFoundException("Table to delete could not be found.") { TableId = tableId };

        await _sysDb.RemoveTable(table);
        await _sysDb.SaveChanges();
    }

    public async Task AddColumn(Guid tableId, DataSetColumn column)
    {
        var table = await GetTable(tableId)
            ?? throw new TableNotFoundException("Table to delete column from could not be found.") { TableId = tableId };

        if (table.TableType != TableType.DataSet)
            throw new TableMismatchException("Can only add columns on data sets.");

        if (table.Columns.Count >= _MaxNumberOfColumns)
            throw new TableColumnException("Too many columns in table.");

        if (!Enum.IsDefined(column.Type))
            throw new TableColumnException("Table contains unknown column type.");

        if (table.Columns.Any(c => c.Name == column.Name))
            throw new TableColumnException("Column with this name already exists.");

        // ToDo: add a check for max column count in total (DB limit)

        table.Columns.Add(column);
        await _dynDb.AddDataSetColumn(table, column);
        await _sysDb.ModifyTable(table);
        await _sysDb.SaveChanges();
    }

    public async Task RemoveColumn(Guid tableId, string columnName)
    {
        var table = await GetTable(tableId)
            ?? throw new TableNotFoundException("Table to delete column from could not be found.") { TableId = tableId };

        if (table.TableType != TableType.DataSet)
            throw new TableMismatchException("Can only remove columns on data sets.");

        var column = table.Columns.FirstOrDefault(c => c.Name == columnName);
        if (column is null)
            throw new TableColumnException("Column to delete could not be found.");

        table.Columns.Remove(column);
        await _dynDb.RemoveDataSetColumn(table, columnName);
        await _sysDb.ModifyTable(table);
        await _sysDb.SaveChanges();
    }

    public async Task AddCollaborator(Table table, User user)
    {
        if (table.Owner.UserId == user.UserId)
            throw new TableCollaboratorException("Owner of a table cannot be added as a collaborator.");

        if (table.Collaborators.Any(c => c.UserId == user.UserId))
            throw new TableCollaboratorException("Collaborated has already been added.");

        if (table.Collaborators.Count >= _MaxNumberOfCollaborators)
            throw new TableCollaboratorException("Maximum number of collaborators has been reached.");

        table.Collaborators.Add(user);
        await _sysDb.ModifyTable(table);
        await _sysDb.SaveChanges();
    }

    public async Task RemoveCollaborator(Table table, User user)
    {
        if (table.Owner.UserId == user.UserId)
            throw new TableCollaboratorException("Owner of a table cannot be removed as a collaborator.");

        var userRef = table.Collaborators.FirstOrDefault(c => c.UserId == user.UserId);

        if (userRef is null)
            throw new TableCollaboratorException("Collaborator was not found.");

        table.Collaborators.Remove(userRef);
        await _sysDb.ModifyTable(table);
        await _sysDb.SaveChanges();
    }

    public async Task LogTableEvent(TableLogEvent logEvent)
    {
        await _sysDb.CreateLogEvent(logEvent);
        await _sysDb.SaveChanges();
    }
}
