using ScwSvc.Exceptions;
using ScwSvc.Models;

namespace ScwSvc.Procedures.Interfaces;

public interface IAdminProcedures
{
    /// <summary>
    /// Get a collection of all users.
    /// </summary>
    /// <returns>Collection of all users.</returns>
    Task<ICollection<User>> GetAllUsers();

    /// <summary>
    /// Get a user by ID.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>The user object or <see langword="null"/>.</returns>
    Task<User?> GetUser(Guid userId);

    /// <summary>
    /// Add a user.
    /// </summary>
    /// <param name="name">The name of the user.</param>
    /// <param name="password">The password of the user.</param>
    /// <exception cref="UserAlreadyExistsException">Thrown if a user with the same name already exists.</exception>
    /// <exception cref="UserCredentialsInvalidException">Thrown if the supplied credentials are invalid.</exception>
    /// <exception cref="DatabaseException">Thrown if a any database error occurs.</exception>
    Task AddUser(string name, string password);

    /// <summary>
    /// Remove a user.
    /// </summary>
    /// <param name="userId">The ID of the user to remove.</param>
    /// <exception cref="UserNotFoundException">Thrown if the user was not found.</exception>
    /// <exception cref="DatabaseException">Thrown if a any database error occurs.</exception>
    Task DeleteUser(Guid userId);

    /// <summary>
    /// Change the name of a user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="name">The new name.</param>
    /// <exception cref="UserNotFoundException">Thrown if the user was not found.</exception>
    /// <exception cref="UserAlreadyExistsException">Thrown if a user with the same name as the new one already exists.</exception>
    /// <exception cref="UserModificationException">Thrown if an invalid change was made.</exception>
    /// <exception cref="DatabaseException">Thrown if a any database error occurs.</exception>
    Task ChangeUserName(Guid userId, string name);

    /// <summary>
    /// Change the password of a user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="password">The new password.</param>
    /// <exception cref="UserNotFoundException">Thrown if the user was not found.</exception>
    /// <exception cref="UserModificationException">Thrown if an invalid change was made.</exception>
    /// <exception cref="DatabaseException">Thrown if a any database error occurs.</exception>
    Task ChangeUserPassword(Guid userId, string password);

    /// <summary>
    /// Change the role of a user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="role">The new role.</param>
    /// <exception cref="UserNotFoundException">Thrown if the user was not found.</exception>
    /// <exception cref="UserModificationException">Thrown if an invalid change was made.</exception>
    /// <exception cref="DatabaseException">Thrown if a any database error occurs.</exception>
    Task ChangeUserRole(Guid userId, UserRole role);

    /// <summary>
    /// Get all tables of a user.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>Collection of the user's tables.</returns>
    /// <exception cref="UserNotFoundException">Thrown if the user was not found.</exception>
    Task<ICollection<Table>> GetUserTables(Guid userId);

    /// <summary>
    /// Get all tables a user owns.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>Own tables of the user.</returns>
    /// <exception cref="UserNotFoundException">Thrown if the user was not found.</exception>
    Task<ICollection<Table>> GetUserTablesOwn(Guid userId);

    /// <summary>
    /// Get all tables a user may collaborate with.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>Collection of collaboration tables of the user.</returns>
    /// <exception cref="UserNotFoundException">Thrown if the user was not found.</exception>
    Task<ICollection<Table>> GetUserTablesCollaboration(Guid userId);

    /// <summary>
    /// Get all tables.
    /// </summary>
    /// <returns>Collection of all tables.</returns>
    Task<ICollection<Table>> GetAllTables();

    /// <summary>
    /// Get all data sets.
    /// </summary>
    /// <returns>Collection of all data sets.</returns>
    Task<ICollection<Table>> GetAllDataSets();

    /// <summary>
    /// Get all sheets.
    /// </summary>
    /// <returns>Collection of all sheets.</returns>
    Task<ICollection<Table>> GetAllSheets();

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
    /// <exception cref="TableDeclarationException">Thrown if a non-column-related part of the sheet was invalid.</exception>
    /// <exception cref="TableColumnException">Thrown if a column-related part of the sheet was invalid.</exception>
    /// <exception cref="TableAlreadyExistsException">Thrown if a table with this <see cref="Table.TableId"/> already exists.</exception>
    /// <exception cref="DatabaseException">Thrown if a general database error occurs.</exception>
    Task CreateSheet(User owner, Table table);

    /// <summary>
    /// Delete a data set.
    /// </summary>
    /// <param name="tableId">The ID of the data set.</param>
    /// <exception cref="TableNotFoundException">Thrown if no data set with the ID was found.</exception>
    /// <exception cref="TableMismatchException">Thrown if the table type was not <see cref="TableType.DataSet"/>.</exception>
    /// <exception cref="DatabaseException">Thrown if a general database error occurs.</exception>
    Task DeleteDataSet(Guid tableId);

    /// <summary>
    /// Delete a sheet.
    /// </summary>
    /// <param name="tableId">The ID of the sheet.</param>
    /// <exception cref="TableNotFoundException">Thrown if no sheet with the ID was found.</exception>
    /// <exception cref="TableMismatchException">Thrown if the table type was not <see cref="TableType.Sheet"/>.</exception>
    /// <exception cref="DatabaseException">Thrown if a general database error occurs.</exception>
    Task DeleteSheet(Guid tableId);

    /// <summary>
    /// Try to get a single logged event.
    /// </summary>
    /// <param name="logEventId">ID of the logged event.</param>
    /// <returns>Logged event or <see langword="null"/>.</returns>
    Task<LogEvent?> GetLogEvent(Guid logEventId);

    /// <summary>
    /// Get logged events restricted to the given filters.
    /// </summary>
    /// <param name="typeFilter">If defined, restricts types of events to the input.</param>
    /// <param name="dateFilter">If defined, restricts the date range of events.</param>
    /// <returns>Collection of matching events.</returns>
    Task<ICollection<LogEvent>> GetLogEvents(LogEventType? typeFilter, (DateTime start, DateTime end)? dateFilter);
}
