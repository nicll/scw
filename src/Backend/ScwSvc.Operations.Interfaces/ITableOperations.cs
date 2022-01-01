using ScwSvc.Exceptions;
using ScwSvc.Models;

namespace ScwSvc.Operations.Interfaces;

public interface ITableOperations
{
    /// <summary>
    /// Specifies the maximum number of columns (<see cref="TableRef.Columns"/>) a single table may have.
    /// </summary>
    int MaxNumberOfColumns { get; }

    /// <summary>
    /// Get a table by its ID.
    /// </summary>
    /// <param name="tableId">The ID of the table.</param>
    /// <returns>The table or <see langword="null"/>.</returns>
    Task<TableRef?> GetTable(Guid tableId);

    /// <summary>
    /// Get a collection of all tables.
    /// </summary>
    /// <returns>Collection of all tables.</returns>
    Task<ICollection<TableRef>> GetTables();

    /// <summary>
    /// Get a collection of the user's tables restricted by a filter.
    /// </summary>
    /// <param name="userId">Which user's tables to fetch.</param>
    /// <param name="query">The filter as a bitmask.</param>
    /// <returns>Collection of resulting tables.</returns>
    /// <exception cref="UserNotFoundException">Thrown if the user was not found.</exception>
    Task<ICollection<TableRef>> GetTables(Guid userId, TableQuery query);

    /// <summary>
    /// Add a table.
    /// </summary>
    /// <param name="table">The table to add.</param>
    /// <exception cref="TableDeclarationException">Thrown if a non-column-related part of the table was invalid.</exception>
    /// <exception cref="TableColumnException">Thrown if a column-related part of the table was invalid.</exception>
    /// <exception cref="TableAlreadyExistsException">Thrown if a table with this <see cref="TableRef.TableId"/> already exists.</exception>
    /// <exception cref="DatabaseException">Thrown if a any database error occurs.</exception>
    Task AddTable(TableRef table);

    /// <summary>
    /// Delete a table.
    /// </summary>
    /// <param name="tableId">The ID of the table to delete.</param>
    /// <exception cref="TableNotFoundException">Thrown if the table was not found.</exception>
    /// <exception cref="DatabaseException">Thrown if a any database error occurs.</exception>
    Task DeleteTable(Guid tableId);

    /// <summary>
    /// Add a column to a data set.
    /// </summary>
    /// <param name="tableId">The ID of the table.</param>
    /// <param name="column">The column to add.</param>
    /// <exception cref="TableNotFoundException">Thrown if the table was not found.</exception>
    /// <exception cref="TableMismatchException">Thrown if the table was not a data set.</exception>
    /// <exception cref="TableColumnException">Thrown if the new column is invalid.</exception>
    /// <exception cref="DatabaseException">Thrown if a general database error occurs.</exception>
    Task AddColumn(Guid tableId, DataSetColumn column);

    /// <summary>
    /// Delete a column from a data set.
    /// </summary>
    /// <param name="tableId">The ID of the table.</param>
    /// <param name="columnName">The name of the column to delete.</param>
    /// <exception cref="TableNotFoundException">Thrown if the table was not found.</exception>
    /// <exception cref="TableMismatchException">Thrown if the table was not a data set.</exception>
    /// <exception cref="TableColumnException">Thrown if the column was not found.</exception>
    /// <exception cref="DatabaseException">Thrown if a general database error occurs.</exception>
    Task DeleteColumn(Guid tableId, string columnName);
}
