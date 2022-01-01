using ScwSvc.Models;

namespace ScwSvc.DataAccess.Interfaces;

public interface IDynDbRepository
{
    Task CreateTable(TableRef table);

    Task RemoveTable(TableRef table);

    Task AddDataSetColumn(TableRef table, DataSetColumn column);

    Task RemoveDataSetColumn(TableRef table, string columnName);
}
