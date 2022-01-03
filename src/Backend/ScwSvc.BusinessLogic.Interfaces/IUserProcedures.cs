using ScwSvc.Exceptions;
using ScwSvc.Models;

namespace ScwSvc.Procedures.Interfaces;

public interface IUserProcedures
{
    /// <summary>
    /// The maximum number of data sets a single user may have.
    /// This is only enforced when creating tables using
    /// <see cref="CreateDataSet(Table)"/> and <see cref="CreateSheet(Table)"/>.
    /// Has an automatically set default value.
    /// </summary>
    int MaxDataSetsPerUser { get; set; }

    /// <summary>
    /// The maximum number of sheets a single user may have.
    /// This is only enforced when creating tables using
    /// <see cref="CreateDataSet(Table)"/> and <see cref="CreateSheet(Table)"/>.
    /// Has an automatically set default value.
    /// </summary>
    int MaxSheetsPerUser { get; set; }

    /// <summary>
    /// Change the name of a user.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="name">The new name.</param>
    /// <exception cref="UserNotFoundException">Thrown if the user was not found.</exception>
    /// <exception cref="UserAlreadyExistsException">Thrown if a user with the same name as the new one already exists.</exception>
    /// <exception cref="UserChangeException">Thrown if the new name was invalid.</exception>
    /// <exception cref="DatabaseException">Thrown if a general database error occurs.</exception>
    Task ChangeUserName(User user, string name);

    /// <summary>
    /// Change the password of a user.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="password">The new password.</param>
    /// <exception cref="UserNotFoundException">Thrown if the user was not found.</exception>
    /// <exception cref="UserChangeException">Thrown if the new password was invalid.</exception>
    /// <exception cref="DatabaseException">Thrown if a general database error occurs.</exception>
    Task ChangeUserPassword(User user, string password);

    //Task ChangeUserRole(User user, UserRole role);

    //Task<IEnumerable<TableRef>> GetTables();

    //Task<IEnumerable<TableRef>> GetUserTables(User user, TableQuery query);

    //Task<TableRef> GetTable(User user, Guid tableId);

    //Task CreateDataSet();

    //Task DeleteDataSet();

    //Task CreateSheet();

    //Task DeleteSheet();

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
    /// <see cref="CreateDataSet(Table)"/> and <see cref="CreateSheet(Table)"/>.
    /// </summary>
    /// <param name="owner">The owner of the table.</param>
    /// <param name="table">The table.</param>
    /// <returns>The modified table.</returns>
    Table PrepareDataSet(User owner, Table table);

    /// <summary>
    /// Prepare a user-defined table object before creating it in the database.
    /// This method does not check for invalid data. All validation occurs in
    /// <see cref="CreateDataSet(Table)"/> and <see cref="CreateSheet(Table)"/>.
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
    /// <exception cref="TableMismatchException">Thrown if the table type was mismatched.</exception>
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
    /// <exception cref="TableMismatchException">Thrown if the table type was mismatched.</exception>
    /// <exception cref="TableLimitExceededException">Thrown if a user owns too many sheet.</exception>
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
