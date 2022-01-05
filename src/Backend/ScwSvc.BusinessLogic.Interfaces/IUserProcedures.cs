using ScwSvc.Exceptions;
using ScwSvc.Models;

namespace ScwSvc.Procedures.Interfaces;

public interface IUserProcedures
{
    /// <summary>
    /// The maximum number of data sets a single user may have.
    /// This is only enforced when creating tables using
    /// <see cref="CreateDataSet(User, Table)"/>.
    /// Has an automatically set default value.
    /// </summary>
    int MaxDataSetsPerUser { get; set; }

    /// <summary>
    /// The maximum number of sheets a single user may have.
    /// This is only enforced when creating tables using
    /// <see cref="CreateSheet(User, Table)"/>.
    /// Has an automatically set default value.
    /// </summary>
    int MaxSheetsPerUser { get; set; }

    /// <summary>
    /// Get the name of the user.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns>Name of the user.</returns>
    Task<string> GetUserName(User user);

    /// <summary>
    /// Change the name of a user.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="name">The new name.</param>
    /// <exception cref="UserNotFoundException">Thrown if the user was not found.</exception>
    /// <exception cref="UserAlreadyExistsException">Thrown if a user with the same name as the new one already exists.</exception>
    /// <exception cref="UserModificationException">Thrown if the new name was invalid.</exception>
    /// <exception cref="DatabaseException">Thrown if a general database error occurs.</exception>
    Task ChangeUserName(User user, string name);

    /// <summary>
    /// Change the password of a user.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="password">The new password.</param>
    /// <exception cref="UserNotFoundException">Thrown if the user was not found.</exception>
    /// <exception cref="UserModificationException">Thrown if the new password was invalid.</exception>
    /// <exception cref="DatabaseException">Thrown if a general database error occurs.</exception>
    Task ChangeUserPassword(User user, string password);

    /// <summary>
    /// Get the data sets of a user.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns>Collection of user's data sets.</returns>
    Task<ICollection<Table>> GetDataSets(User user);

    /// <summary>
    /// Get the number of data sets of a user.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns>Number of the user's data sets.</returns>
    Task<int> GetDataSetCount(User user);

    /// <summary>
    /// Get the own data sets of a user.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns>Collection of user's own data sets.</returns>
    Task<ICollection<Table>> GetOwnDataSets(User user);

    /// <summary>
    /// Get the collaboration data sets of a user.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns>Collection of user's collaboration data sets.</returns>
    Task<ICollection<Table>> GetCollaborationDataSets(User user);

    /// <summary>
    /// Get the sheets of a user.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns>Collection of user's sheets.</returns>
    Task<ICollection<Table>> GetSheets(User user);

    /// <summary>
    /// Get the number of sheets of a user.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns>Number of the user's sheets.</returns>
    Task<int> GetSheetCount(User user);

    /// <summary>
    /// Get the own sheets of a user.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns>Collection of user's own sheets.</returns>
    Task<ICollection<Table>> GetOwnSheets(User user);

    /// <summary>
    /// Get the collaboration sheets of a user.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns>Collection of user's collaboration sheets.</returns>
    Task<ICollection<Table>> GetCollaborationSheets(User user);

    /// <summary>
    /// Get the tables of a user.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns>Collection of user's tables.</returns>
    Task<ICollection<Table>> GetTables(User user);

    /// <summary>
    /// Get the own tables of a user.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns>Collection of user's own tables.</returns>
    Task<ICollection<Table>> GetOwnTables(User user);

    /// <summary>
    /// Get the collaboration tables of a user.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <returns>Collection of user's collaboration tables.</returns>
    Task<ICollection<Table>> GetCollaborationTables(User user);

    /// <summary>
    /// Get the data set of a specific user with the corresponding ID.
    /// </summary>
    /// <param name="user">The owner.</param>
    /// <param name="tableId">The ID of the table.</param>
    /// <returns>Data set of the user.</returns>
    /// <exception cref="TableNotFoundException">Thrown if no table of the user with the ID was found.</exception>
    /// <exception cref="TableMismatchException">Thrown if the table type was mismatched.</exception>
    Task<Table> GetDataSet(User user, Guid tableId);

    /// <summary>
    /// Get the sheet of a specific user with the corresponding ID.
    /// </summary>
    /// <param name="user">The owner.</param>
    /// <param name="tableId">The ID of the table.</param>
    /// <returns>Data set of the user.</returns>
    /// <exception cref="TableNotFoundException">Thrown if no table of the user with the ID was found.</exception>
    /// <exception cref="TableMismatchException">Thrown if the table type was mismatched.</exception>
    Task<Table> GetSheet(User user, Guid tableId);

    /// <summary>
    /// Prepare a user-defined table object before creating it in the database.
    /// This method does not check for invalid data. All validation occurs in
    /// <see cref="CreateDataSet(User, Table)"/>.
    /// </summary>
    /// <param name="owner">The owner of the table.</param>
    /// <param name="table">The table.</param>
    /// <returns>The modified table.</returns>
    Table PrepareDataSet(User owner, Table table);

    /// <summary>
    /// Prepare a user-defined table object before creating it in the database.
    /// This method does not check for invalid data. All validation occurs in
    /// <see cref="CreateSheet(User, Table)"/>.
    /// </summary>
    /// <param name="owner">The owner of the table.</param>
    /// <param name="table">The table.</param>
    /// <returns>The modified table.</returns>
    Table PrepareSheet(User owner, Table table);

    /// <summary>
    /// Create a data set.
    /// </summary>
    /// <param name="owner">The owner of the data set.</param>
    /// <param name="table">The data set specification.</param>
    /// <exception cref="TableMismatchException">Thrown if the table type was not <see cref="TableType.DataSet"/>.</exception>
    /// <exception cref="TableLimitExceededException">Thrown if a user owns too many data sets.</exception>
    /// <exception cref="TableDeclarationException">Thrown if a non-column-related part of the data set was invalid.</exception>
    /// <exception cref="TableColumnException">Thrown if a column-related part of the data set was invalid.</exception>
    /// <exception cref="TableAlreadyExistsException">Thrown if a table with this <see cref="Table.TableId"/> already exists.</exception>
    /// <exception cref="DatabaseException">Thrown if a general database error occurs.</exception>
    Task CreateDataSet(User owner, Table table);

    /// <summary>
    /// Create a sheet.
    /// </summary>
    /// <param name="owner">The owner of the sheet.</param>
    /// <param name="table">The sheet specification.</param>
    /// <exception cref="TableMismatchException">Thrown if the table type was not <see cref="TableType.Sheet"/>.</exception>
    /// <exception cref="TableLimitExceededException">Thrown if a user owns too many sheets.</exception>
    /// <exception cref="TableDeclarationException">Thrown if a non-column-related part of the sheet was invalid.</exception>
    /// <exception cref="TableColumnException">Thrown if a column-related part of the sheet was invalid.</exception>
    /// <exception cref="TableAlreadyExistsException">Thrown if a table with this <see cref="Table.TableId"/> already exists.</exception>
    /// <exception cref="DatabaseException">Thrown if a general database error occurs.</exception>
    Task CreateSheet(User owner, Table table);

    /// <summary>
    /// Delete a data set.
    /// </summary>
    /// <param name="owner">The owner of the data set.</param>
    /// <param name="tableId">The ID of the data set.</param>
    /// <exception cref="TableNotFoundException">Thrown if no data set of the user with the ID was found.</exception>
    /// <exception cref="TableMismatchException">Thrown if the table type was not <see cref="TableType.DataSet"/>.</exception>
    /// <exception cref="DatabaseException">Thrown if a general database error occurs.</exception>
    Task DeleteDataSet(User owner, Guid tableId);

    /// <summary>
    /// Delete a sheet.
    /// </summary>
    /// <param name="owner">The owner of the sheet.</param>
    /// <param name="tableId">The ID of the sheet.</param>
    /// <exception cref="TableNotFoundException">Thrown if no sheet of the user with the ID was found.</exception>
    /// <exception cref="TableMismatchException">Thrown if the table type was not <see cref="TableType.Sheet"/>.</exception>
    /// <exception cref="DatabaseException">Thrown if a general database error occurs.</exception>
    Task DeleteSheet(User owner, Guid tableId);

    /// <summary>
    /// Add a column to a data set.
    /// </summary>
    /// <param name="owner">The owner of the data set.</param>
    /// <param name="tableId">The ID of the data set.</param>
    /// <param name="column">The column to add.</param>
    /// <exception cref="TableNotFoundException">Thrown if no data set of the user with the ID was found.</exception>
    /// <exception cref="TableMismatchException">Thrown if the table type was not <see cref="TableType.DataSet"/>.</exception>
    /// <exception cref="TableColumnException">Thrown if the new column is invalid.</exception>
    /// <exception cref="DatabaseException">Thrown if a general database error occurs.</exception>
    Task AddDataSetColumn(User owner, Guid tableId, DataSetColumn column);

    /// <summary>
    /// Remove a column from a data set.
    /// </summary>
    /// <param name="owner">The owner of the data set.</param>
    /// <param name="tableId">The ID of the data set.</param>
    /// <param name="columnName">The name of the column to remove.</param>
    /// <exception cref="TableNotFoundException">Thrown if no data set of the user with the ID was found.</exception>
    /// <exception cref="TableMismatchException">Thrown if the table type was not <see cref="TableType.DataSet"/>.</exception>
    /// <exception cref="TableColumnException">Thrown if the column was not found.</exception>
    /// <exception cref="DatabaseException">Thrown if a general database error occurs.</exception>
    Task RemoveDataSetColumn(User owner, Guid tableId, string columnName);

    /// <summary>
    /// Get a collection of all collaborators of a table.
    /// </summary>
    /// <param name="user">The owner of the table.</param>
    /// <param name="tableId">The ID of the table.</param>
    /// <returns>Collection of the collaborators.</returns>
    /// <exception cref="TableNotFoundException">Thrown if the table was not found in the owner's tables.</exception>
    Task<ICollection<User>> GetCollaborators(User user, Guid tableId);

    /// <summary>
    /// Add a collaborator to a table.
    /// </summary>
    /// <param name="owner">The owner of the table.</param>
    /// <param name="tableId">The ID of the table.</param>
    /// <param name="userId">The ID of the collaborator to add.</param>
    /// <exception cref="TableNotFoundException">Thrown if the table was not found in the owner's tables.</exception>
    /// <exception cref="UserNotFoundException">Thrown if the collaborator was not found.</exception>
    /// <exception cref="TableCollaboratorException">Thrown if the collaborator was invalid.</exception>
    /// <exception cref="DatabaseException">Thrown if a general database error occurs.</exception>
    Task AddCollaborator(User owner, Guid tableId, Guid userId);

    /// <summary>
    /// Remove a collaborator from a table.
    /// </summary>
    /// <param name="owner">The owner of the table.</param>
    /// <param name="tableId">The ID of the table.</param>
    /// <param name="userId">The ID of the collaborator to remove.</param>
    /// <exception cref="TableNotFoundException">Thrown if the table was not found in the owner's tables.</exception>
    /// <exception cref="UserNotFoundException">Thrown if the collaborator was not found.</exception>
    /// <exception cref="TableCollaboratorException">Thrown if the collaborator was invalid.</exception>
    /// <exception cref="DatabaseException">Thrown if a general database error occurs.</exception>
    Task RemoveCollaborator(User owner, Guid tableId, Guid userId);
}
