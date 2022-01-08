using ScwSvc.Exceptions;
using ScwSvc.Models;

namespace ScwSvc.DataAccess.Interfaces;

public interface ISysDbRepository
{
    /// <summary>
    /// Whether to automatically save changes when calling any mutating methods.
    /// If this is set to <see langword="true"/> the called method may throw
    /// any exceptions thrown by <see cref="SaveChanges"/>.
    /// </summary>
    bool AutoSave { get; set; }

    /// <summary>
    /// Get a collection of all users.
    /// </summary>
    /// <returns>Collection of all users.</returns>
    Task<ICollection<User>> GetAllUsers();

    /// <summary>
    /// Create a customizable user query.
    /// </summary>
    /// <returns><see cref="IQueryable{T}"/> of all users.</returns>
    IQueryable<User> CreateUsersQuery();

    /// <summary>
    /// Execute the customized user query.
    /// </summary>
    /// <param name="query">The user query.</param>
    /// <returns>Collection of resulting users.</returns>
    Task<ICollection<User>> ExecuteUsersQuery(IQueryable<User> query);

    /// <summary>
    /// Get a user by ID.
    /// Alternatively return <see langword="null"/> if not found.
    /// </summary>
    /// <param name="userId">The user's ID.</param>
    /// <returns>The user object or <see langword="null"/>.</returns>
    Task<User?> GetUserById(Guid userId);

    /// <summary>
    /// Get a user by name.
    /// Alternatively return <see langword="null"/> if not found.
    /// </summary>
    /// <param name="name">The user's name.</param>
    /// <returns>The user object or <see langword="null"/>.</returns>
    Task<User?> GetUserByName(string name);

    /// <summary>
    /// Gets a collection of all users with a specific role.
    /// </summary>
    /// <param name="role">The role to search for.</param>
    /// <returns>Collection of resulting users.</returns>
    Task<ICollection<User>> GetUsersByRole(UserRole role);

    /// <summary>
    /// Check whether the user's name is already assigned.
    /// </summary>
    /// <param name="name">The name to check.</param>
    /// <returns><see langword="true"/> if the name already exists, otherwise <see langword="false"/>.</returns>
    Task<bool> IsUserNameAssigned(string name);

    /// <summary>
    /// Add a user to the SYS database.
    /// </summary>
    /// <param name="user">The user to add.</param>
    Task AddUser(User user);

    /// <summary>
    /// Remove a user from the SYS database.
    /// </summary>
    /// <param name="user">The user to remove.</param>
    Task RemoveUser(User user);

    /// <summary>
    /// Save at least the given user object to the SYS database.
    /// </summary>
    /// <param name="user">The user object to save.</param>
    Task ModifyUser(User user);

    /// <summary>
    /// Get a collection of all tables.
    /// </summary>
    /// <returns>Collection of all tables.</returns>
    Task<ICollection<Table>> GetAllTables();

    /// <summary>
    /// Create a customizable table query.
    /// </summary>
    /// <returns><see cref="IQueryable{T}"/> of all tables.</returns>
    IQueryable<Table> CreateTablesQuery();

    /// <summary>
    /// Execute the customized table query.
    /// </summary>
    /// <param name="query">The table query.</param>
    /// <returns>Collection of resulting tables.</returns>
    Task<ICollection<Table>> ExecuteTablesQuery(IQueryable<Table> query);

    /// <summary>
    /// Get a table by ID.
    /// Alternatively returns <see langword="null"/> if not found.
    /// </summary>
    /// <param name="tableId">The user's ID.</param>
    /// <returns>The table object or <see langword="null"/>.</returns>
    Task<Table?> GetTableById(Guid tableId);

    /// <summary>
    /// Add a table to the SYS database.
    /// </summary>
    /// <param name="table">The table to add.</param>
    Task AddTable(Table table);

    /// <summary>
    /// Remove a table from the SYS database.
    /// </summary>
    /// <param name="table">The table to remove.</param>
    Task RemoveTable(Table table);

    /// <summary>
    /// Save at least the given table object to the SYS database.
    /// </summary>
    /// <param name="table">The table object to save.</param>
    Task ModifyTable(Table table);

    /// <summary>
    /// Save an event to the log.
    /// </summary>
    /// <param name="logEvent">Event to log.</param>
    Task CreateLogEvent(LogEvent logEvent);

    /// <summary>
    /// Fetch an event from the log.
    /// </summary>
    /// <param name="logEventId">ID of the event to fetch.</param>
    /// <returns>Logged event or <see langword="null"/>.</returns>
    Task<LogEvent?> GetLogEvent(Guid logEventId);

    /// <summary>
    /// Create a customizable log query.
    /// </summary>
    /// <returns><see cref="IQueryable{T}"/> of all log.</returns>
    IQueryable<LogEvent> CreateLogQuery();

    /// <summary>
    /// Execute the customized log query.
    /// </summary>
    /// <param name="query">The log query.</param>
    /// <returns>Collection of resulting log entries.</returns>
    Task<ICollection<LogEvent>> ExecuteLogQuery(IQueryable<LogEvent> query);

    /// <summary>
    /// Convert the log query to an async enumerable.
    /// </summary>
    /// <param name="query">The log query.</param>
    /// <returns>The converted async enumerable.</returns>
    IAsyncEnumerable<LogEvent> AsAsyncLogQuery(IQueryable<LogEvent> query);

    /// <summary>
    /// Save all changes.
    /// </summary>
    /// <exception cref="DatabaseException">Thrown if a any database error occurs.</exception>
    Task SaveChanges();
}
