using ScwSvc.Exceptions;
using ScwSvc.Models;

namespace ScwSvc.DataAccess.Interfaces;

public interface IDynDbRepository
{
    /// <summary>
    /// Create a new table in the DYN database.
    /// </summary>
    /// <param name="table">The table to create.</param>
    /// <exception cref="InvalidTableException">Thrown if the specified table is invalid.</exception>
    /// <exception cref="TableColumnException">Thrown if the table contains an invalid column.</exception>
    /// <exception cref="DatabaseException">Thrown if a general database error occurs.</exception>
    Task CreateTable(Table table);

    /// <summary>
    /// Remove a table from the DYN database.
    /// </summary>
    /// <param name="table">The table to remove.</param>
    /// <exception cref="DatabaseException">Thrown if a general database error occurs.</exception>
    Task RemoveTable(Table table);

    /// <summary>
    /// Add a column to a data set in the DYN database.
    /// </summary>
    /// <param name="table">The table to add the column to.</param>
    /// <param name="column">The column to add.</param>
    /// <exception cref="TableColumnException">Thrown if the new column is invalid.</exception>
    /// <exception cref="DatabaseException">Thrown if a general database error occurs.</exception>
    Task AddDataSetColumn(Table table, DataSetColumn column);

    /// <summary>
    /// Remove a column from a data set in the DYN database.
    /// </summary>
    /// <param name="table">The table to remove the column from.</param>
    /// <param name="columnName">The name of the column to remove.</param>
    /// <exception cref="TableColumnException">Thrown if an invalid column was to be removed.</exception>
    /// <exception cref="DatabaseException">Thrown if a general database error occurs.</exception>
    Task RemoveDataSetColumn(Table table, string columnName);
}
