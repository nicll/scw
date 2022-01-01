using ScwSvc.Models;

namespace ScwSvc.DataAccess.Interfaces;

public interface ISysDbRepository
{
    bool AutoSave { get; set; }

    Task<ICollection<User>> GetAllUsers();

    IQueryable<User> CreateUsersQuery();

    Task<ICollection<User>> ExecuteUsersQuery(IQueryable<User> query);

    Task<User?> GetUserById(Guid userId);

    Task<User?> GetUserByName(string name);

    Task<IEnumerable<User>> GetUsersByRole(UserRole role);

    Task<bool> IsUserNameAssigned(string name);

    Task AddUser(User user);

    Task RemoveUser(User user);

    Task ModifyUser(User user);

    Task<ICollection<TableRef>> GetAllTables();

    IQueryable<TableRef> CreateTablesQuery();

    Task<ICollection<TableRef>> ExecuteTablesQuery(IQueryable<TableRef> query);

    Task<TableRef?> GetTableById(Guid tableId);

    Task AddTable(TableRef table);

    Task RemoveTable(TableRef table);

    Task ModifyTable(TableRef table);

    Task SaveChanges();
}
