using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScwSvc.Models;

namespace ScwSvc.DataAccess.Interfaces;

public interface ISysDbRepository
{
    IQueryable<User> GetUsers();

    Task<User> GetUserById(Guid userId);

    Task<User> GetUserByName(string name);

    Task<IEnumerable<User>> GetUsersByRole(UserRole role);

    Task<bool> IsUserNameAssigned(string name);

    Task AddUser(User user);

    Task RemoveUser(User user);

    Task ModifyUser(User user);

    IQueryable<TableRef> GetTables();

    Task<TableRef> GetTableById(Guid tableId);

    Task AddTable(TableRef table);

    Task RemoveTable(TableRef table);

    Task ModifyTable(TableRef table);

    Task SaveChanges();
}
