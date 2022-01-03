using ScwSvc.Exceptions;
using ScwSvc.Models;

namespace ScwSvc.Operations.Interfaces;

public interface ITableOperations
{
    /// <summary>
    /// Specifies the maximum number of columns (<see cref="Table.Columns"/>) a single table may have.
    /// </summary>
    int MaxNumberOfColumns { get; }

    /// <summary>
    /// Specifies the maximum number of collaborators (<see cref="Table.Collaborators"/>) a single table may have.
    /// </summary>
    int MaxNumberOfCollaborators { get; }

    /// <summary>
    /// Get a table by its ID.
    /// </summary>
    /// <param name="tableId">The ID of the table.</param>
    /// <returns>The table or <see langword="null"/>.</returns>
    Task<Table?> GetTable(Guid tableId);

    /// <summary>
    /// Get a collection of all tables.
    /// </summary>
    /// <returns>Collection of all tables.</returns>
    Task<ICollection<Table>> GetTables();

    /// <summary>
    /// Get a collection of the all tables restricted by a filter.
    /// Only table type filters may be applied.
    /// </summary>
    /// <param name="query">The filter as a bitmask.</param>
    /// <returns>Collection of resulting tables.</returns>
    Task<ICollection<Table>> GetTables(TableQuery query);

    /// <summary>
    /// Get a collection of the user's tables restricted by a filter.
    /// </summary>
    /// <param name="userId">Which user's tables to fetch.</param>
    /// <param name="query">The filter as a bitmask.</param>
    /// <returns>Collection of resulting tables.</returns>
    /// <exception cref="UserNotFoundException">Thrown if the user was not found.</exception>
    Task<ICollection<Table>> GetTables(Guid userId, TableQuery query);

    /// <summary>
    /// Add a table.
    /// </summary>
    /// <param name="table">The table to add.</param>
    /// <exception cref="TableMismatchException">Thrown if the table was of an unknown type.</exception>
    /// <exception cref="TableDeclarationException">Thrown if a non-column-related part of the table was invalid.</exception>
    /// <exception cref="TableColumnException">Thrown if a column-related part of the table was invalid.</exception>
    /// <exception cref="TableAlreadyExistsException">Thrown if a table with this <see cref="Table.TableId"/> already exists.</exception>
    /// <exception cref="DatabaseException">Thrown if a general database error occurs.</exception>
    Task AddTable(Table table);

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
    Task RemoveColumn(Guid tableId, string columnName);

    /// <summary>
    /// Add a collaborator to a table.
    /// </summary>
    /// <param name="table">The table to add to.</param>
    /// <param name="user">The user to add.</param>
    /// <exception cref="TableNotFoundException">Thrown if the table was not found.</exception>
    /// <exception cref="TableCollaboratorException">Thrown if the collaborator was invalid.</exception>
    /// <exception cref="DatabaseException">Thrown if a general database error occurs.</exception>
    Task AddCollaborator(Table table, User user);

    /// <summary>
    /// Remove a collaborator from a table.
    /// </summary>
    /// <param name="table">The table to remove from.</param>
    /// <param name="user">The user to remove.</param>
    /// <exception cref="TableNotFoundException">Thrown if the table was not found.</exception>
    /// <exception cref="TableCollaboratorException">Thrown if the collaborator was invalid.</exception>
    /// <exception cref="DatabaseException">Thrown if a general database error occurs.</exception>
    Task RemoveCollaborator(Table table, User user);
}
