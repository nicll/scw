using ScwSvc.Models;

namespace ScwSvc.Operations.Interfaces;

public interface ITableOperations
{
    int MaxNumberOfColumns { get; }

    Task<TableRef?> GetTable(Guid tableId);

    Task<ICollection<TableRef>> GetTables();

    Task<ICollection<TableRef>> GetTables(Guid userId, TableQuery query);

    Task AddTable(TableRef table);

    Task DeleteTable(Guid tableId);

    Task AddColumn(Guid tableId, DataSetColumn column);

    Task DeleteColumn(Guid tableId, string columnName);
}
